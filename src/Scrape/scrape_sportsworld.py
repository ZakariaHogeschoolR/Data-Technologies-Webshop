import json
import os
import re
import time
from urllib.parse import urljoin

import requests
from selenium import webdriver
from selenium.common.exceptions import TimeoutException, WebDriverException
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager


BASE_URL = "https://www.sportsworld.nl/"
CATEGORY_URL = "https://www.sportsworld.nl/football-shirts/premier-league-football-shirts?subcategorygroup.nl-NL=Voetbalsokken"
OUTPUT_JSON = "sportsworld_products.json"
IMAGE_DIR = "downloaded_images"

MAX_PAGES = 2

START_INDEX = 0     # 0, 10, 20, 30...
BATCH_SIZE = 50     # hoeveel products toegevoegd per run (max 50 recommended to avoid timeouts)


def safe_filename(value: str) -> str:
    value = re.sub(r"[^\w\-]+", "_", value.strip().lower())
    return value[:80].strip("_")


def load_existing_results():
    if os.path.exists(OUTPUT_JSON):
        try:
            with open(OUTPUT_JSON, "r", encoding="utf-8") as f:
                return json.load(f)
        except Exception:
            return []
    return []


def save_results(results):
    with open(OUTPUT_JSON, "w", encoding="utf-8") as f:
        json.dump(results, f, ensure_ascii=False, indent=2)


def start_driver():
    options = webdriver.ChromeOptions()
    options.add_argument("--start-maximized")
    options.add_argument("--disable-blink-features=AutomationControlled")
    options.add_argument("--disable-dev-shm-usage")
    options.add_argument("--no-sandbox")
    options.add_experimental_option("excludeSwitches", ["enable-automation"])
    options.add_experimental_option("useAutomationExtension", False)

    driver = webdriver.Chrome(
        service=Service(ChromeDriverManager().install()),
        options=options
    )
    driver.set_page_load_timeout(20)
    driver.execute_script(
        "Object.defineProperty(navigator, 'webdriver', {get: () => undefined})"
    )
    return driver


def safe_get(driver, url, retries=2):
    for attempt in range(retries + 1):
        try:
            driver.get(url)
            return True
        except TimeoutException:
            print(f"TIMEOUT -> {url} ({attempt + 1}/{retries + 1})")
            try:
                driver.execute_script("window.stop();")
            except Exception:
                pass
            if attempt == retries:
                return False
        except WebDriverException as ex:
            print(f"WEBDRIVER HATA -> {url} -> {ex}")
            if attempt == retries:
                return False
    return False


def get_page_url(page_number: int) -> str:
    if page_number == 1:
        return CATEGORY_URL
    return f"{CATEGORY_URL}?page={page_number}"


def collect_product_links_from_page(driver, page_number: int):
    url = get_page_url(page_number)

    ok = safe_get(driver, url)
    if not ok:
        print(f"Sayfa acilamadi: {url}")
        return []

    time.sleep(3)

    html = driver.page_source
    pattern = r"https://www\.sportsworld\.nl/[a-zA-Z0-9\-_\/]+-\d{6}"
    matches = re.findall(pattern, html)

    results = []
    seen = set()

    for match in matches:
        clean_url = match.split('"')[0].split("'")[0].strip()

        if clean_url in seen:
            continue
        seen.add(clean_url)

        results.append({"url": clean_url})

    print(f"Sayfa {page_number}: {len(results)} link bulundu")
    return results


def is_shirt_name(name: str) -> bool:
    value = name.lower()
    keywords = [
        "shirt", "jersey", "home", "away", "third", "fourth",
        "goalkeeper", "replica", "authentic", "minikit", "kit"
    ]
    return any(keyword in value for keyword in keywords)


def parse_feature_pairs(driver):
    features = {}

    try:
        body_text = driver.find_element(By.TAG_NAME, "body").text
    except Exception:
        return features

    labels = [
        "Kit Type",
        "Mouwlengte",
        "Seizoen",
        "Team",
        "Kit Class",
        "Toernooi"
    ]

    lines = [line.strip() for line in body_text.splitlines() if line.strip()]

    for i, line in enumerate(lines):
        if line in labels and i + 1 < len(lines):
            features[line] = lines[i + 1]

    return features


def get_best_image_src(driver):
    imgs = driver.find_elements(By.TAG_NAME, "img")

    for img in imgs:
        src = (
            img.get_attribute("src")
            or img.get_attribute("data-src")
            or img.get_attribute("data-lazy-src")
            or ""
        )
        alt = (img.get_attribute("alt") or "").lower()

        if not src:
            continue

        full_src = urljoin(BASE_URL, src)

        bad_words = ["logo", "icon", "payment", "banner", "trust", "review", "flag"]
        if any(word in full_src.lower() for word in bad_words):
            continue

        if any(word in alt for word in ["shirt", "jersey", "football", "kit"]):
            return full_src

    return ""


def download_image(session, image_url, local_path):
    if not image_url:
        return False

    try:
        response = session.get(image_url, timeout=10)
        response.raise_for_status()

        with open(local_path, "wb") as f:
            f.write(response.content)

        return True
    except Exception:
        return False


def create_requests_session_from_driver(driver):
    session = requests.Session()
    session.headers.update({
        "User-Agent": "Mozilla/5.0",
        "Referer": BASE_URL
    })

    for cookie in driver.get_cookies():
        session.cookies.set(cookie["name"], cookie["value"])

    return session


def extract_price_from_text(body_text):
    match = re.search(r"€\s*[0-9]+(?:,[0-9]{2})?", body_text)
    if match:
        return match.group(0).replace(" ", "")
    return ""


def extract_brand(driver):
    possible_xpaths = [
        "//a[contains(@href, '/nike') or contains(@href, '/adidas') or contains(@href, '/puma') or contains(@href, '/umbro') or contains(@href, '/macron')]",
        "//h1/preceding::a[1]",
    ]

    for xpath in possible_xpaths:
        try:
            text = driver.find_element(By.XPATH, xpath).text.strip()
            if text and len(text) < 50:
                return text
        except Exception:
            pass

    return ""


def extract_name(driver):
    possible_xpaths = [
        "//h1",
        "//main//h1",
        "//div//h1",
    ]

    for xpath in possible_xpaths:
        try:
            text = driver.find_element(By.XPATH, xpath).text.strip()
            if text and len(text) > 3:
                return text
        except Exception:
            pass

    return driver.title.replace("| Sports World", "").strip()


def scrape_product(driver, session, url, index):
    ok = safe_get(driver, url)
    if not ok:
        return None

    time.sleep(2)

    try:
        body_text = driver.find_element(By.TAG_NAME, "body").text
    except Exception:
        return None

    name = extract_name(driver)

    if not name or not is_shirt_name(name):
        return None

    brand = extract_brand(driver)
    price = extract_price_from_text(body_text)
    features = parse_feature_pairs(driver)

    image_url = get_best_image_src(driver)
    local_image = ""

    if image_url:
        os.makedirs(IMAGE_DIR, exist_ok=True)
        filename = f"{index:03d}_{safe_filename(name)}.jpg"
        local_path = os.path.join(IMAGE_DIR, filename)

        ok = download_image(session, image_url, local_path)
        if ok:
            local_image = local_path.replace("\\", "/")

    return {
        "name": name,
        "brand": brand,
        "price": price,
        "description": name,
        "team": features.get("Team", ""),
        "season": features.get("Seizoen", ""),
        "kitType": features.get("Kit Type", ""),
        "tournament": features.get("Toernooi", ""),
        "imageUrl": image_url,
        "localImage": local_image,
        "productUrl": url,
        "source": "SportsWorld"
    }


def main():
    driver = start_driver()

    try:
        all_links = []
        seen_links = set()

        for page_number in range(1, MAX_PAGES + 1):
            page_links = collect_product_links_from_page(driver, page_number)

            for item in page_links:
                if item["url"] in seen_links:
                    continue
                seen_links.add(item["url"])
                all_links.append(item)

        print(f"Toplam benzersiz link: {len(all_links)}")

        selected_links = all_links[START_INDEX:START_INDEX + BATCH_SIZE]
        print(f"Bu calistirmada alinacak link sayisi: {len(selected_links)}")
        print(f"Aralik: {START_INDEX} - {START_INDEX + BATCH_SIZE - 1}")

        existing_results = load_existing_results()
        existing_urls = {item.get('productUrl', '') for item in existing_results}

        session = create_requests_session_from_driver(driver)
        results = existing_results[:]

        for item in selected_links:
            if item["url"] in existing_urls:
                print(f"ZATEN VAR -> {item['url']}")
                continue

            try:
                product = scrape_product(driver, session, item["url"], len(results) + 1)

                if product is None:
                    print(f"SKIP -> {item['url']}")
                    continue

                results.append(product)
                existing_urls.add(item["url"])
                save_results(results)
                print(f"EKLENDI -> {product['name']} | Toplam: {len(results)}")

            except Exception as ex:
                print(f"HATA -> {item['url']} -> {ex}")
                continue

        save_results(results)
        print(f"Bitti. Toplam kayit: {len(results)}")

    finally:
        driver.quit()


if __name__ == "__main__":
    main()
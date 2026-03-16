import json
import time
from urllib.parse import urljoin

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager


BASE_URL = "https://12345-67890.x.yupoo.com/"


def start_driver():
    options = webdriver.ChromeOptions()
    options.add_argument("--start-maximized")
    options.add_argument("--disable-blink-features=AutomationControlled")
    options.add_argument("--disable-dev-shm-usage")
    options.add_argument("--no-sandbox")
    options.add_experimental_option("excludeSwitches", ["enable-automation"])
    options.add_experimental_option("useAutomationExtension", False)

    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=options)
    driver.execute_script("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})")
    return driver


def scrape_homepage(driver):
    driver.get(BASE_URL)
    time.sleep(5)

    links = driver.find_elements(By.TAG_NAME, "a")

    products = []
    seen = set()

    for link in links:
        href = link.get_attribute("href")
        text = link.text.strip()

        if not href or "/albums/" not in href:
            continue

        if not text:
            continue

        if href in seen:
            continue

        seen.add(href)
        products.append({
            "name": text,
            "album_url": href
        })

    return products


def scrape_album(driver, album_url):
    driver.get(album_url)
    time.sleep(4)

    title = driver.title.strip()

    images = []
    seen = set()

    img_elements = driver.find_elements(By.TAG_NAME, "img")
    for img in img_elements:
        src = img.get_attribute("src")
        if not src:
            continue

        full_url = urljoin(album_url, src)

        if full_url in seen:
            continue

        seen.add(full_url)
        images.append(full_url)

    return {
        "title": title,
        "images": images
    }


def main():
    driver = start_driver()

    try:
        products = scrape_homepage(driver)
        print(f"Bulunan album sayisi: {len(products)}")

        results = []
        limit = min(20, len(products))

        for i, product in enumerate(products[:limit], start=1):
            try:
                details = scrape_album(driver, product["album_url"])

                item = {
                    "name": product["name"],
                    "title": details["title"],
                    "album_url": product["album_url"],
                    "image_urls": details["images"],
                    "main_image": details["images"][0] if details["images"] else ""
                }

                results.append(item)
                print(f"[{i}/{limit}] OK -> {product['name']}")
            except Exception as ex:
                print(f"[{i}/{limit}] HATA -> {product['album_url']} -> {ex}")

        with open("yupoo_products_selenium.json", "w", encoding="utf-8") as f:
            json.dump(results, f, ensure_ascii=False, indent=2)

        print("Bitti: yupoo_products_selenium.json olusturuldu.")

    finally:
        driver.quit()


if __name__ == "__main__":
    main()
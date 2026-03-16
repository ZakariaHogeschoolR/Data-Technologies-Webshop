import json
import re
import time
from urllib.parse import urljoin

import requests
from bs4 import BeautifulSoup

BASE_URL = "https://12345-67890.x.yupoo.com/"
HEADERS = {
    "User-Agent": "Mozilla/5.0"
}

session = requests.Session()
session.headers.update(HEADERS)


def clean_text(value: str) -> str:
    return re.sub(r"\s+", " ", value).strip()


def get_html(url: str) -> str:
    response = session.get(url, timeout=30)
    response.raise_for_status()
    return response.text


def scrape_homepage():
    html = get_html(BASE_URL)
    soup = BeautifulSoup(html, "html.parser")

    products = []
    seen_urls = set()

    for a in soup.find_all("a", href=True):
        href = a["href"].strip()
        text = clean_text(a.get_text(" ", strip=True))

        if "/albums/" not in href:
            continue

        full_url = urljoin(BASE_URL, href)

        if full_url in seen_urls:
            continue

        if not text:
            continue

        seen_urls.add(full_url)

        products.append({
            "name": text,
            "album_url": full_url
        })

    return products


def extract_image_urls_from_album(album_html: str):
    soup = BeautifulSoup(album_html, "html.parser")
    image_urls = []
    seen = set()

    for img in soup.find_all("img"):
        candidates = [
            img.get("data-src"),
            img.get("data-original"),
            img.get("src"),
            img.get("data-lazy-src")
        ]

        for candidate in candidates:
            if not candidate:
                continue

            candidate = candidate.strip()
            full_url = urljoin(BASE_URL, candidate)

            if "logo" in full_url.lower():
                continue
            if "avatar" in full_url.lower():
                continue

            if full_url not in seen:
                seen.add(full_url)
                image_urls.append(full_url)

    return image_urls


def scrape_album_details(album_url: str):
    html = get_html(album_url)
    soup = BeautifulSoup(html, "html.parser")

    title = None

    h1 = soup.find("h1")
    if h1:
        title = clean_text(h1.get_text(" ", strip=True))

    if not title:
        title_tag = soup.find("title")
        if title_tag:
            title = clean_text(title_tag.get_text(" ", strip=True))

    image_urls = extract_image_urls_from_album(html)

    return {
        "title": title or "",
        "image_urls": image_urls
    }


def main():
    products = scrape_homepage()
    print(f"Homepage'de bulunan album sayisi: {len(products)}")

    enriched = []
    limit = 30

    for index, product in enumerate(products[:limit], start=1):
        try:
            details = scrape_album_details(product["album_url"])

            enriched_product = {
                "name": details["title"] or product["name"],
                "description": "",
                "category": "",
                "album_url": product["album_url"],
                "image_urls": details["image_urls"],
                "main_image": details["image_urls"][0] if details["image_urls"] else "",
                "price": 0,
                "currency": "EUR",
                "source": "Yupoo"
            }

            enriched.append(enriched_product)
            print(f"[{index}/{min(limit, len(products))}] OK -> {enriched_product['name']}")
            time.sleep(1)

        except Exception as ex:
            print(f"[{index}] HATA -> {product['album_url']} -> {ex}")

    with open("yupoo_products.json", "w", encoding="utf-8") as f:
        json.dump(enriched, f, ensure_ascii=False, indent=2)

    print("Bitti. yupoo_products.json olusturuldu.")


if __name__ == "__main__":
    main()
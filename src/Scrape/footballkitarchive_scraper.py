import json
import re
import time
from urllib.parse import urljoin

import requests
from bs4 import BeautifulSoup

BASE_URL = "https://www.footballkitarchive.com/"
START_URL = "https://www.footballkitarchive.com/champions-league-kits/"

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


def extract_cards_from_homepage():
    html = get_html(START_URL)
    soup = BeautifulSoup(html, "html.parser")

    items = []
    seen = set()

    # Sitedeki bütün linkleri geziyoruz
    for a in soup.find_all("a", href=True):
        href = a["href"].strip()
        text = clean_text(a.get_text(" ", strip=True))

        if not href:
            continue

        full_url = urljoin(BASE_URL, href)

        # Boş yazıları alma
        if not text:
            continue

        # Aynı linkleri tekrar alma
        if full_url in seen:
            continue

        # Sadece site içi linkler
        if "footballkitarchive.com" not in full_url:
            continue

        seen.add(full_url)

        # Resim bulmaya çalış
        image_url = ""
        img = a.find("img")
        if img:
            for attr in ["src", "data-src", "data-lazy-src"]:
                candidate = img.get(attr)
                if candidate:
                    image_url = urljoin(BASE_URL, candidate.strip())
                    break

        items.append({
            "name": text,
            "detail_url": full_url,
            "image_url": image_url
        })

    return items


def filter_kit_like_items(items):
    filtered = []

    keywords = [
        "kit", "jersey", "home", "away", "third", "fourth",
        "goalkeeper", "track", "jacket", "training", "travel",
        "anthem", "pre-match", "shirt"
    ]

    for item in items:
        name_lower = item["name"].lower()
        url_lower = item["detail_url"].lower()

        if any(keyword in name_lower for keyword in keywords) or "/kits/" in url_lower:
            filtered.append(item)

    return filtered


def parse_name_fields(name: str):
    """
    Basit ayırma:
    'Brazil 2026 Away' gibi bir şey gelirse
    Team = Brazil
    Season = 2026
    Category = Away
    """
    season = ""
    category = ""
    team = name

    season_match = re.search(r"\b(19|20)\d{2}\b", name)
    if season_match:
        season = season_match.group(0)

    categories = [
        "Home", "Away", "Third", "Fourth", "Goalkeeper",
        "Track", "Jacket", "Training", "Travel", "Anthem",
        "Pre-Match", "Shirt", "Kit", "Jersey"
    ]

    for cat in categories:
        if cat.lower() in name.lower():
            category = cat
            break

    if season:
        team = name.replace(season, "").strip()

    if category:
        team = re.sub(category, "", team, flags=re.IGNORECASE).strip()

    team = re.sub(r"\s{2,}", " ", team).strip(" -")

    return {
        "team": team,
        "season": season,
        "category": category
    }


def main():
    print("Ana sayfa taraniyor...")
    items = extract_cards_from_homepage()
    print(f"Bulunan toplam link: {len(items)}")

    kit_items = filter_kit_like_items(items)
    print(f"Kit benzeri filtrelenmis kayit: {len(kit_items)}")

    # ilk deneme için limit
    limit = min(150, len(kit_items))

    results = []
    seen_urls = set()

    for index, item in enumerate(kit_items[:limit], start=1):
        if item["detail_url"] in seen_urls:
            continue

        seen_urls.add(item["detail_url"])

        parsed = parse_name_fields(item["name"])

        product = {
            "name": item["name"],
            "team": parsed["team"],
            "season": parsed["season"],
            "category": parsed["category"],
            "brand": "",
            "description": item["name"],
            "image_url": item["image_url"],
            "detail_url": item["detail_url"],
            "price": 0,
            "currency": "EUR",
            "source": "FootballKitArchive"
        }

        results.append(product)
        print(f"[{index}/{limit}] OK -> {product['name']}")
        time.sleep(0.1)

    with open("footballkit_products.json", "w", encoding="utf-8") as f:
        json.dump(results, f, ensure_ascii=False, indent=2)

    print("Bitti. footballkit_products.json olusturuldu.")


if __name__ == "__main__":
    main()
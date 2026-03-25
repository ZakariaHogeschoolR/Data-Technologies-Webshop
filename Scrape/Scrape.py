import json
import re
import requests
from urllib.parse import urljoin

ALGOLIA_URL = "https://ergcyoyil1-dsn.algolia.net/1/indexes/*/queries"
ALGOLIA_APP_ID = "ERGCYOYIL1"
ALGOLIA_API_KEY = "7e8e4fd5d835601fbecf1b348f733a26"
ALGOLIA_AGENT = "Algolia for JavaScript (5.34.1); Lite (5.34.1); Browser"

BASE_SITE = "https://www.sportsworld.nl"
OUTPUT_JSON = "national_team_products.json"

MAX_PRODUCTS = 300
HITS_PER_PAGE = 100

NATIONAL_TEAMS = [
    "Argentina",
    "Brazil",
    "Netherlands",
    "England",
    "France",
    "Germany",
    "Spain",
    "Portugal",
    "Belgium",
    "Italy",
    "Croatia",
    "Morocco",
    "Turkey",
    "USA",
    "Mexico",
    "Japan",
    "Jamaica",
    "Scotland",
    "Wales",
    "Ireland",
    "Nigeria",
    "Ghana",
    "Senegal",
    "Poland",
    "Austria",
    "Switzerland",
    "Denmark",
    "Sweden",
    "Norway",
    "Canada",
    "Australia",
    "Korea",
    "South Korea"
]

KIT_TYPES = [
    "Home",
    "Away",
    "Third",
    "Fourth",
    "Goalkeeper",
    "GK"
]

TOURNAMENTS = [
    "World Cup",
    "Euro",
    "UEFA Euro",
    "Copa America",
    "Nations League",
    "FIFA World Cup"
]


def clean_text(value):
    if value is None:
        return ""

    if isinstance(value, list):
        value = " ".join(str(x) for x in value)

    text = str(value).replace("\xa0", " ")
    text = re.sub(r"\s+", " ", text).strip()
    return text


def first_lang_value(value):
    if isinstance(value, dict):
        if "en-GB" in value:
            return clean_text(value["en-GB"])
        if "nl-NL" in value:
            return clean_text(value["nl-NL"])
    return clean_text(value)


def first_list_lang_value(value):
    if isinstance(value, dict):
        if "en-GB" in value and isinstance(value["en-GB"], list) and value["en-GB"]:
            return clean_text(value["en-GB"][0])
        if "nl-NL" in value and isinstance(value["nl-NL"], list) and value["nl-NL"]:
            return clean_text(value["nl-NL"][0])
    return clean_text(value)


def detect_team(text):
    lower_text = text.lower()
    for team in NATIONAL_TEAMS:
        if team.lower() in lower_text:
            return team
    return ""


def detect_kit_type(text):
    lower_text = text.lower()
    for kit in KIT_TYPES:
        if kit.lower() in lower_text:
            if kit == "GK":
                return "Goalkeeper"
            return kit
    return ""


def detect_tournament(text):
    lower_text = text.lower()
    for tournament in TOURNAMENTS:
        if tournament.lower() in lower_text:
            return tournament
    return ""


def detect_season(text):
    match = re.search(r"(\d{4}\s*/\s*\d{2,4}|\d{4}\s+\d{4}|\d{2}/\d{2})", text)
    if match:
        return match.group(1).replace(" ", "")
    return ""


def extract_price(hit):
    prices = hit.get("prices", {})
    eur = prices.get("EUR", {})
    selling_price = eur.get("sellingPrice")

    if selling_price is None:
        return ""

    return f"€{selling_price:.2f}".replace(".", ",")


def build_product_url(hit):
    possible_fields = [
        "url",
        "productUrl",
        "productURL",
        "slug",
        "href",
        "link"
    ]

    for field in possible_fields:
        value = hit.get(field)
        if isinstance(value, str) and value.strip():
            if value.startswith("http"):
                return value
            return urljoin(BASE_SITE, value)

    object_id = clean_text(hit.get("objectID"))
    if object_id:
        return f"{BASE_SITE}/search/{object_id}"

    return ""


def extract_image_url(hit):
    possible_fields = [
        "imageUrl",
        "imageURL",
        "image",
        "defaultImage",
        "defaultImageUrl",
        "primaryImage",
        "productImage",
        "smallImage",
        "largeImage",
        "imageurl",
        "thumbnail",
        "thumbUrl"
    ]

    for field in possible_fields:
        value = hit.get(field)
        if isinstance(value, str) and value.strip():
            if value.startswith("//"):
                return "https:" + value
            if value.startswith("/"):
                return urljoin(BASE_SITE, value)
            return value

    return ""


def is_likely_football_shirt(hit):
    name = first_lang_value(hit.get("name"))
    category = first_list_lang_value(hit.get("category"))
    subcategory = first_list_lang_value(hit.get("subcategory"))
    activity = first_list_lang_value(hit.get("activity"))
    brand = clean_text(hit.get("brand"))

    full_text = " ".join([name, category, subcategory, activity, brand]).lower()

    football_keywords = [
        "shirt",
        "jersey",
        "football shirt",
        "replica",
        "home shirt",
        "away shirt",
        "third shirt",
        "goalkeeper shirt",
        "kit",
        "top"
    ]

    has_football_word = any(word in full_text for word in football_keywords)
    has_team = detect_team(full_text) != ""

    return has_team and has_football_word


def transform_hit(hit):
    name = first_lang_value(hit.get("name"))
    brand = clean_text(hit.get("brand"))
    category = first_list_lang_value(hit.get("category"))
    subcategory = first_list_lang_value(hit.get("subcategory"))
    colour_name = first_lang_value(hit.get("colourName"))
    activity = first_list_lang_value(hit.get("activity"))

    full_text = " ".join([name, brand, category, subcategory, colour_name, activity])

    product = {
        "name": name,
        "brand": brand,
        "price": extract_price(hit),
        "description": name,
        "team": detect_team(full_text),
        "season": detect_season(full_text),
        "kitType": detect_kit_type(full_text),
        "tournament": detect_tournament(full_text),
        "imageUrl": extract_image_url(hit),
        "localImage": "",
        "productUrl": build_product_url(hit),
        "source": "SportsWorld"
    }

    return product


def fetch_page(page_number):
    params = {
        "x-algolia-agent": ALGOLIA_AGENT,
        "x-algolia-api-key": ALGOLIA_API_KEY,
        "x-algolia-application-id": ALGOLIA_APP_ID
    }

    headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
        "Origin": "https://www.sportsworld.nl",
        "Referer": "https://www.sportsworld.nl/",
        "User-Agent": "Mozilla/5.0"
    }

    request_body = {
        "requests": [
            {
                "indexName": "sdnl_production_search",
                "query": "",
                "hitsPerPage": HITS_PER_PAGE,
                "page": page_number,
                "filters": "isHidden:false AND hasInventory:true AND hideFromSearch:false AND hasDefaultPrice:true"
            }
        ]
    }

    response = requests.post(
        ALGOLIA_URL,
        params=params,
        headers=headers,
        json=request_body,
        timeout=30
    )
    response.raise_for_status()

    data = response.json()
    results = data.get("results", [])

    if not results:
        return [], 0

    result = results[0]
    hits = result.get("hits", [])
    nb_pages = result.get("nbPages", 0)

    return hits, nb_pages


def scrape_products():
    collected = []
    seen_ids = set()

    page_number = 0
    total_pages = 1

    while page_number < total_pages and len(collected) < MAX_PRODUCTS:
        print(f"Page {page_number + 1} being fetched...")
        hits, total_pages = fetch_page(page_number)

        if not hits:
            break

        for hit in hits:
            object_id = clean_text(hit.get("objectID"))

            if object_id in seen_ids:
                continue

            seen_ids.add(object_id)

            if not is_likely_football_shirt(hit):
                continue

            product = transform_hit(hit)
            collected.append(product)

            print(f"[{len(collected)}] {product['name']}")

            if len(collected) >= MAX_PRODUCTS:
                break

        page_number += 1

    return collected


def save_json(products):
    with open(OUTPUT_JSON, "w", encoding="utf-8") as f:
        json.dump(products, f, ensure_ascii=False, indent=2)


if __name__ == "__main__":
    products = scrape_products()
    save_json(products)

    print()
    print(f"Total saved products: {len(products)}")
    print(f"File: {OUTPUT_JSON}")
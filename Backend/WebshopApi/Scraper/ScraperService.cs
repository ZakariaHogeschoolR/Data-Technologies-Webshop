using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using DataTransferObject;

namespace Service;

public class ScraperService
{
    private readonly HttpClient _http;
    private readonly ProductRepository _productRepository;

    public ScraperService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    public async Task ImportFromApiAsync()
    {
        var json = File.ReadAllText(
            @"C:\Users\Public\Data-Technologies-Webshop\Backend\WebshopApi\Scraper\data\products_with_teams.json");
        var items = JsonSerializer.Deserialize<List<FakeStoreProduct>>(json);

        foreach (var item in items)
        {
            var fullName = item.Name.Replace("\n", " ").Trim();

            var enriched = EnrichFromName(fullName);

            // 🔥 TEAM
            var teamName = item.Team ?? enriched.Team ?? "Unknown";
            var teamType = item.TeamType ?? "club"; // bv: national / club

            var teamId = await _productRepository.GetOrCreateTeam(teamName, teamType);

            // 🔥 CATEGORY
            var categoryName = item.Category ?? "other";
            var categoryId = await _productRepository.GetOrCreateCategory(categoryName);

            // 🔥 PRODUCT
            var product = new ProductDto(null, item.Image, fullName, enriched.Description, item.Price, teamId);

            var productId = await _productRepository.AddProductScrape(product);

            // 🔗 LINK CATEGORY
            await _productRepository.AddProductCategory(productId, categoryId);
        }
    }

    private (string Description, string Team, string Season, string KitType) EnrichFromName(string name)
    {
        // Season: "2025 2026" or "25/26"
        var seasonMatch = Regex.Match(name, @"(\d{4}[\s\/]\d{2,4})");
        var season = seasonMatch.Success ? seasonMatch.Value.Replace(" ", "/") : "";

        // Kit type
        var kitType = "";
        if (Regex.IsMatch(name, @"\bHome\b", RegexOptions.IgnoreCase))
            kitType = "Home";
        else if (Regex.IsMatch(name, @"\bAway\b", RegexOptions.IgnoreCase))
            kitType = "Away";
        else if (Regex.IsMatch(name, @"\bThird\b", RegexOptions.IgnoreCase))
            kitType = "Third";
        else if (Regex.IsMatch(name, @"\bFourth\b", RegexOptions.IgnoreCase))
            kitType = "Fourth";

        // Known teams
        var teams = new[]
        {
            "Ajax", "Liverpool", "Jamaica", "Brazil", "USA", "Real Madrid",
            "Barcelona", "Valencia", "Manchester City", "Manchester United",
            "Tottenham Hotspur", "Bournemouth", "New York City"
        };
        var team = Array.Find(teams, t =>
            Regex.IsMatch(name, t, RegexOptions.IgnoreCase)) ?? "";

        // Build description from parts
        var parts = new[] { team, kitType, season }.Where(p => !string.IsNullOrEmpty(p));
        var description = string.Join(" ", parts);
        if (string.IsNullOrEmpty(description)) description = name;

        return (description, team, season, kitType);
    }

    public class FlexiblePriceConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDecimal();

            if (reader.TokenType == JsonTokenType.String)
            {
                var raw = reader.GetString()?
                    .Replace("€", "")
                    .Replace(",", ".")
                    .Trim();

                return decimal.TryParse(raw,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var result)
                    ? result
                    : 0;
            }

            throw new JsonException($"Unexpected token type for price: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }

    public class FakeStoreProduct
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("team")] public string Team { get; set; }

        [JsonPropertyName("teamType")] public string TeamType { get; set; }

        [JsonPropertyName("image")] // ✅ was "imageUrl" — wrong for this file
        public string Image { get; set; }

        [JsonPropertyName("link")] // ✅ was "productUrl" — wrong for this file
        public string Detail { get; set; }

        [JsonPropertyName("price")]
        [JsonConverter(typeof(FlexiblePriceConverter))]
        public decimal Price { get; set; }

        [JsonPropertyName("category")] public string Category { get; set; }
    }
}

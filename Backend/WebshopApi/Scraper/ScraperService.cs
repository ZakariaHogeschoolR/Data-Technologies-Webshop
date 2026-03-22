using System.Text.Json;

using System.Text.Json.Serialization;

using DataTransferObject;

namespace Service;

public class ScraperService
{
    private readonly ProductRepository _productRepository;
    private readonly HttpClient _http;

    public ScraperService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

        public ScraperService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        }

        public async Task ImportFromApiAsync()
        {
            var json = File.ReadAllText(@"C:\Users\zahar\Downloads\sportsworld_products.json");
            var items = JsonSerializer.Deserialize<List<FakeStoreProduct>>(json);

        foreach (var item in items)
        {
            var product = new ProductDto
            {
                var fullName = item.Name.Replace("\n", " ").Trim(); // ✅ "adidas Ajax Originals Icons T-Shirt Adults"
                var enriched = EnrichFromName(fullName);

                var product = new ProductDto
                {
                    Name         = fullName,
                    Price        = item.Price,
                    Description  = enriched.Description,
                    ProductImage = item.Image
                };

                await _productRepository.AddProduct(product);
            }
        }
    }

        private (string Description, string Team, string Season, string KitType) EnrichFromName(string name)
        {
            // Season: "2025 2026" or "25/26"
            var seasonMatch = System.Text.RegularExpressions.Regex.Match(name, @"(\d{4}[\s\/]\d{2,4})");
            var season = seasonMatch.Success ? seasonMatch.Value.Replace(" ", "/") : "";

            // Kit type
            var kitType = "";
            if (System.Text.RegularExpressions.Regex.IsMatch(name, @"\bHome\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                kitType = "Home";
            else if (System.Text.RegularExpressions.Regex.IsMatch(name, @"\bAway\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                kitType = "Away";
            else if (System.Text.RegularExpressions.Regex.IsMatch(name, @"\bThird\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                kitType = "Third";
            else if (System.Text.RegularExpressions.Regex.IsMatch(name, @"\bFourth\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                kitType = "Fourth";

            // Known teams
            var teams = new[]
            {
                "Ajax", "Liverpool", "Jamaica", "Brazil", "USA", "Real Madrid",
                "Barcelona", "Valencia", "Manchester City", "Manchester United",
                "Tottenham Hotspur", "Bournemouth", "New York City"
            };
            var team = Array.Find(teams, t =>
                System.Text.RegularExpressions.Regex.IsMatch(name, t, System.Text.RegularExpressions.RegexOptions.IgnoreCase)) ?? "";

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
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var result) ? result : 0;
                }

                throw new JsonException($"Unexpected token type for price: {reader.TokenType}");
            }

            public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
                => writer.WriteNumberValue(value);
        }

        public class FakeStoreProduct
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("brand")]
            public string Brand { get; set; }

            [JsonPropertyName("team")]
            public string Team { get; set; }

            [JsonPropertyName("season")]
            public string Season { get; set; }

            [JsonPropertyName("kitType")]
            public string KitType { get; set; }

            [JsonPropertyName("imageUrl")]      // ✅ was "imageUrl" — wrong for this file
            public string Image { get; set; }

            [JsonPropertyName("productUrl")]     // ✅ was "productUrl" — wrong for this file
            public string Detail { get; set; }

            [JsonPropertyName("price")]
            [JsonConverter(typeof(FlexiblePriceConverter))]
            public decimal Price { get; set; }

            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            [JsonPropertyName("source")]
            public string Source { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }
        }
    }
}

using System.Text.Json;

using System.Text.Json.Serialization;

using DataTransferObject;

        public async Task ImportFromApiAsync()
        {
            var json = File.ReadAllText(@"C:\Users\Public\Data-Technologies-Webshop\Backend\WebshopApi\Scraper\data\products_with_teams.json");
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
                var product = new ProductDto
                {
                    Name         = fullName,
                    Price        = item.Price,
                    Description  = enriched.Description,
                    ProductImage = item.Image,
                    TeamId       = teamId
                };

                var productId = await _productRepository.AddProductScrape(product);

                // 🔗 LINK CATEGORY
                await _productRepository.AddProductCategory(productId, categoryId);
            }
        }

        foreach (var item in items)
        {
            var product = new ProductDto
            {
                Name = item.Title,
                Price = (decimal)item.Price,
                Description = item.Description?.Length > 250
                                ? item.Description[..250]
                                : item.Description,
                ProductImage = item.ProductImage
            };
            await _productRepository.AddProduct(product);
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

            [JsonPropertyName("team")]
            public string Team { get; set; }

            [JsonPropertyName("teamType")]
            public string TeamType { get; set; }

            [JsonPropertyName("image")]      // ✅ was "imageUrl" — wrong for this file
            public string Image { get; set; }

            [JsonPropertyName("link")]     // ✅ was "productUrl" — wrong for this file
            public string Detail { get; set; }

    // DTO voor de API response
    public class FakeStoreProduct
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

            [JsonPropertyName("category")]
            public string Category { get; set; }
        }
    }
}

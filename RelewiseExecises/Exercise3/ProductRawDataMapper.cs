using System.Globalization;
using Relewise.Client.DataTypes;

namespace RelewiseExecises.Exercise3;

public class ProductRawDataMapper : IJob
{
    public async Task<string> Execute()
    {
        try
        {
            string rawUrl = "https://cdn.relewise.com/academy/productdata/raw";
            var httpClient = new HttpClient();
            string rawData = await httpClient.GetStringAsync(rawUrl);

            var lines = rawData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<Product> mappedProducts = new List<Product>();

            for (int i = 2; i < lines.Length; i++)
            {
                var columns = lines[i].Split('|');
                if (columns.Length < 6) continue;

                var productId = columns[1].Trim();
                var productName = columns[2].Trim();
                var salesPrice = columns[4].Trim();
                var listPrice = columns[3].Trim();

                if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(productName))
                {
                    var product = new Product
                    {
                        Id = productId,
                        DisplayName = new Multilingual(new Multilingual.Value("en", productName)),
                        ListPrice = new MultiCurrency(new Money("USD", ParsePrice(listPrice))),
                        SalesPrice = new MultiCurrency(new Money("USD", ParsePrice(salesPrice)))
                    };

                    mappedProducts.Add(product);
                }
            }
            
            return $"Mapped {mappedProducts.Count} products successfully.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return $"An error occurred: {ex.Message}";
        }
    }

    private decimal ParsePrice(string price)
    {
        if (string.IsNullOrEmpty(price))
            return 0;

        var numericString = new string(price.Where(c => char.IsDigit(c) || c == '.').ToArray());

        if (string.IsNullOrEmpty(numericString) || !decimal.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
        {
            return 0;
        }

        return parsedValue;
    }
}
using System.Globalization;
using Relewise.Client.DataTypes;

namespace RelewiseExecises.Exercise3;

public class ProductRawDataMapper : IJob
{
    public async Task<string> Execute(
        JobArguments arguments,
        Func<string, Task> info,
        Func<string, Task> warn,
        CancellationToken token)
    {
        try
        {
            await info("Job started...");

            string rawUrl = "https://cdn.relewise.com/academy/productdata/raw";
            var httpClient = new HttpClient();

            if (token.IsCancellationRequested)
            {
                await warn("Job was cancelled before fetching data.");
                token.ThrowIfCancellationRequested();
            }

            string rawData = await httpClient.GetStringAsync(rawUrl);

            var lines = rawData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<Product> mappedProducts = new List<Product>();
            Language english = new Language("en"); 
            Currency usd = new Currency("USD");

            for (int i = 2; i < lines.Length; i++)
            {
                if (token.IsCancellationRequested)
                {
                    await warn("Job was cancelled during processing.");
                    token.ThrowIfCancellationRequested();
                }

                var columns = lines[i].Split('|');
                if (columns.Length < 6) continue;

                var productId = columns[1].Trim();
                var productName = columns[2].Trim();
                var salesPrice = columns[4].Trim();
                var listPrice = columns[3].Trim();

                if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(productName))
                {
                    var product = new Product(productId);

                    product.DisplayName = new Multilingual(new Multilingual.Value(english, productName));
                    product.ListPrice = new MultiCurrency(new Money(usd, ParsePrice(listPrice)));
                    product.SalesPrice = new MultiCurrency(new Money(usd, ParsePrice(salesPrice)));

                    mappedProducts.Add(product);
                }
            }

            await info($"Successfully mapped {mappedProducts.Count} products.");

            return $"Mapped {mappedProducts.Count} products successfully.";
        }
        catch (Exception ex)
        {
            await warn($"An error occurred: {ex.Message}");
            throw;
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
using System.Globalization;
using Newtonsoft.Json;
using Relewise.Client.DataTypes;

namespace RelewiseExecises.Exercise1;

public class ProductJsonMapper : IJob
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

            string jsonUrl = "https://cdn.relewise.com/academy/productdata/customjsonfeed";
            var httpClient = new HttpClient();

            if (token.IsCancellationRequested)
            {
                await warn("Job was cancelled before fetching data.");
                token.ThrowIfCancellationRequested();
            }

            string jsonData = await httpClient.GetStringAsync(jsonUrl);
            var productJsonArray = JsonConvert.DeserializeObject<ProductJson[]>(jsonData);

            Product[] mappedProducts = new Product[productJsonArray.Length];

            Language english = new Language("en");
            Currency usd = new Currency("USD");

            for (int i = 0; i < productJsonArray.Length; i++)
            {
                if (token.IsCancellationRequested)
                {
                    await warn("Job was cancelled during processing.");
                    token.ThrowIfCancellationRequested();
                }

                var productJson = productJsonArray[i];
                var product = new Product(productJson.productId);

                product.DisplayName = new Multilingual(new Multilingual.Value(english, productJson.productName));
                product.ListPrice = new MultiCurrency(new Money(usd, ParsePrice(productJson.listPrice)));
                product.SalesPrice = new MultiCurrency(new Money(usd, ParsePrice(productJson.salesPrice)));

                mappedProducts[i] = product;
            }

            await info($"Successfully mapped {mappedProducts.Length} products.");

            return $"Mapped {mappedProducts.Length} products successfully.";
        }
        catch (Exception ex)
        {
            await warn($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private decimal ParsePrice(string? price)
    {
        return decimal.Parse(price.Replace("$", ""), NumberStyles.Currency, CultureInfo.InvariantCulture);
    }
}
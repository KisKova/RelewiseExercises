using System.Globalization;
using Newtonsoft.Json;
using Relewise.Client.DataTypes;

namespace RelewiseExecises;

public class ProductJsonMapper : IJob
{
    public async Task<string> Execute()
    {
        // Step 1: Download JSON data
        string jsonUrl = "https://cdn.relewise.com/academy/productdata/customjsonfeed";
        var httpClient = new HttpClient();
        string jsonData = await httpClient.GetStringAsync(jsonUrl);

        // Step 2: Deserialize JSON data into local ProductJson class
        var productJsonArray = JsonConvert.DeserializeObject<List<ProductJson>>(jsonData);

        // Step 3: Map ProductJson to Relewise Product
        List<Product> mappedProducts = new List<Product>();
        Language english = new Language("en"); 
        Currency usd = new Currency("USD");

        foreach (var productJson in productJsonArray)
        {
            var product = new Product
            {
                Id = productJson.productId,
                DisplayName = new Multilingual( new Multilingual.Value( english, productJson.productName)),
                ListPrice = new MultiCurrency(new Money(usd, ParsePrice(productJson.listPrice))),
                SalesPrice = new MultiCurrency(new Money(usd, ParsePrice(productJson.salesPrice)))
            };
            mappedProducts.Add(product);
        }

        // Step 4: Return a message containing the count of products and their names
        return $"Mapped {mappedProducts.Count} products successfully.";
    }

    private decimal ParsePrice(string price)
    {
        // Remove any currency symbols and parse the string as a decimal
        return decimal.Parse(price.Replace("$", ""), NumberStyles.Currency, CultureInfo.InvariantCulture);
    }
}
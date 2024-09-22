using System.Globalization;
using Newtonsoft.Json;
using Relewise.Client.DataTypes;

namespace RelewiseExecises.Exercise1;

public class ProductJsonMapper : IJob
{
    public async Task<string> Execute()
    {
        string jsonUrl = "https://cdn.relewise.com/academy/productdata/customjsonfeed";
        var httpClient = new HttpClient();
        string jsonData = await httpClient.GetStringAsync(jsonUrl);
        
        var productJsonArray = JsonConvert.DeserializeObject<List<ProductJson>>(jsonData);
        
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
        
        return $"Mapped {mappedProducts.Count} products successfully.";
    }

    private decimal ParsePrice(string price)
    {
        return decimal.Parse(price.Replace("$", ""), NumberStyles.Currency, CultureInfo.InvariantCulture);
    }
}
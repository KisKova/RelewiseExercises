using System.Globalization;
using System.Xml.Linq;
using Relewise.Client.DataTypes;

namespace RelewiseExecises.Exercise2;

public class ProductGoogleFeedMapper : IJob
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

            string xmlUrl = "https://cdn.relewise.com/academy/productdata/googleshoppingfeed";
            var httpClient = new HttpClient();

            if (token.IsCancellationRequested)
            {
                await warn("Job was cancelled before fetching data.");
                token.ThrowIfCancellationRequested();
            }

            string xmlData = await httpClient.GetStringAsync(xmlUrl);

            XDocument xmlDoc = XDocument.Parse(xmlData);
            var items = xmlDoc.Descendants("item");

            List<Product> mappedProducts = new List<Product>();
            Language english = new Language("en"); 
            Currency usd = new Currency("USD");

            foreach (var item in items)
            {
                if (token.IsCancellationRequested)
                {
                    await warn("Job was cancelled during processing.");
                    token.ThrowIfCancellationRequested();
                }

                var productId = item.Element(XName.Get("id", "http://base.google.com/ns/1.0"))?.Value;
                var title = item.Element("title")?.Value;
                var price = item.Element(XName.Get("price", "http://base.google.com/ns/1.0"))?.Value;
                var salePrice = item.Element(XName.Get("sale_price", "http://base.google.com/ns/1.0"))?.Value;

                if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(title))
                {
                    var product = new Product(productId);

                    product.DisplayName = new Multilingual(new Multilingual.Value(english, title));
                    product.ListPrice = new MultiCurrency(new Money(usd, ParsePrice(price)));
                    product.SalesPrice = new MultiCurrency(new Money(usd, ParsePrice(salePrice)));
                    
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

    private decimal ParsePrice(string? price)
    {
        if (string.IsNullOrEmpty(price))
            return 0;
        
        var numericString = new string(price.Where(c => char.IsDigit(c) || c == '.').ToArray());
        
        return decimal.Parse(numericString, CultureInfo.InvariantCulture);
    }
}
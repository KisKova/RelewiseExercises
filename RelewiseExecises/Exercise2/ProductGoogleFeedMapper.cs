using System.Globalization;
using System.Xml.Linq;
using Relewise.Client.DataTypes;

namespace RelewiseExecises.Exercise2;

public class ProductGoogleFeedMapper : IJob
{
    public async Task<string> Execute()
    {
        try
        {
            string xmlUrl = "https://cdn.relewise.com/academy/productdata/googleshoppingfeed";
            var httpClient = new HttpClient();
            string xmlData = await httpClient.GetStringAsync(xmlUrl);
            
            XDocument xmlDoc = XDocument.Parse(xmlData);
            var items = xmlDoc.Descendants("item");
            
            List<Product> mappedProducts = new List<Product>();

            foreach (var item in items)
            {
                var productId = item.Element(XName.Get("id", "http://base.google.com/ns/1.0"))?.Value;
                var title = item.Element("title")?.Value;
                var price = item.Element(XName.Get("price", "http://base.google.com/ns/1.0"))?.Value;
                var salePrice = item.Element(XName.Get("sale_price", "http://base.google.com/ns/1.0"))?.Value;

                if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(title))
                {
                    var product = new Product
                    {
                        Id = productId,
                        DisplayName = new Multilingual(new Multilingual.Value("en", title)),
                        ListPrice = new MultiCurrency(new Money("USD", ParsePrice(price))),
                        SalesPrice = new MultiCurrency(new Money("USD", ParsePrice(salePrice)))
                    };

                    mappedProducts.Add(product);
                }
            }
            
            return $"Mapped {mappedProducts.Count} products successfully.";
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }

    private decimal ParsePrice(string price)
    {
        if (string.IsNullOrEmpty(price))
            return 0;
        
        var numericString = new string(price.Where(c => char.IsDigit(c) || c == '.').ToArray());
        
        return decimal.Parse(numericString, CultureInfo.InvariantCulture);
    }
}
using RelewiseExecises.Exercise1;
using RelewiseExecises.Exercise2;

namespace RelewiseExecises;

public class Program
{
    public static async Task Main(string[] args)
    {
        IJob job = new ProductJsonMapper();
        string result = await job.Execute();
        Console.WriteLine(result);
        
        var mapper = new ProductGoogleFeedMapper();
        string result2 = await mapper.Execute();
        Console.WriteLine(result2);
    }
}
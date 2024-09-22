using RelewiseExecises.Exercise1;
using RelewiseExecises.Exercise2;
using RelewiseExecises.Exercise3;

namespace RelewiseExecises;

public class Program
{
    public static async Task Main(string[] args)
    {
        IJob jobOne = new ProductJsonMapper();
        string result = await jobOne.Execute();
        Console.WriteLine(result);
        
        IJob jobTwo = new ProductGoogleFeedMapper();
        string result2 = await jobTwo.Execute();
        Console.WriteLine(result2);
        
        IJob jobThree = new ProductRawDataMapper();
        string result3 = await jobThree.Execute();
        Console.WriteLine(result3);
    }
}
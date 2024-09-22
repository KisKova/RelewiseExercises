using RelewiseExecises.Exercise1;
using RelewiseExecises.Exercise2;
using RelewiseExecises.Exercise3;

namespace RelewiseExecises;

public class Program
{
    public static async Task Main(string[] args)
    {
        var arguments = new JobArguments(
            Guid.NewGuid(),
            "api-key",
            new Dictionary<string, string>());

        Func<string, Task> infoLogger = message => Task.Run(() => Console.WriteLine($"INFO: {message}"));
        Func<string, Task> warnLogger = message => Task.Run(() => Console.WriteLine($"WARN: {message}"));
        CancellationTokenSource cts = new CancellationTokenSource();
        
        IJob jobOne = new ProductJsonMapper();
        string result = await jobOne.Execute(arguments, infoLogger, warnLogger, cts.Token);
        Console.WriteLine(result);
        
        IJob jobTwo = new ProductGoogleFeedMapper();
        string result2 = await jobTwo.Execute(arguments, infoLogger, warnLogger, cts.Token);
        Console.WriteLine(result2);
        
        IJob jobThree = new ProductRawDataMapper();
        string result3 = await jobThree.Execute(arguments, infoLogger, warnLogger, cts.Token);
        Console.WriteLine(result3);
    }
}
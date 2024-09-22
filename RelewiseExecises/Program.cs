namespace RelewiseExecises;

public class Program
{
    public static async Task Main(string[] args)
    {
        IJob job = new ProductJsonMapper();
        string result = await job.Execute();
        Console.WriteLine(result);
    }
}
namespace RelewiseExecises;

public interface IJob
{
    Task<string> Execute(
        JobArguments arguments,
        Func<string, Task> info,
        Func<string, Task> warn,
        CancellationToken token);
}
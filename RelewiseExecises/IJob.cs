namespace RelewiseExecises;

public interface IJob
{
    Task<string> Execute();
}
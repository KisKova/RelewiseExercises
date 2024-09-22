namespace RelewiseExecises;

public class JobArguments
{
    public JobArguments(
        Guid datasetId,
        string apiKey,
        IReadOnlyDictionary<string, string> jobConfiguration)
    {
        DatasetId = datasetId;
        ApiKey = apiKey;
        JobConfiguration = jobConfiguration;
    }
    public Guid DatasetId { get; }
    public string ApiKey { get; }
    public IReadOnlyDictionary<string, string> JobConfiguration { get; }
}
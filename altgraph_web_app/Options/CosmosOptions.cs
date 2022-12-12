namespace altgraph_web_app.Options
{
    public class CosmosOptions
    {
        public const string Cosmos = "Cosmos";
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerId { get; set; } = string.Empty;
        public string DatabaseId { get; set; } = string.Empty;
        public bool PopulateQueryMetrics { get; set; } = false;
        public bool QueryMetricsEnabled { get; set; } = false;
        public int MaxDegreeOfParallelism { get; set; } = 1;
    }
}

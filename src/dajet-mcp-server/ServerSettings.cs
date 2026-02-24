namespace DaJet.Mcp.Server
{
    public sealed class ServerSettings
    {
        public List<DataSourceSettings> DataSources { get; } = new();
    }
    public sealed class DataSourceSettings
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // SqlServer или PostgreSql
        public string ConnectionString { get; set; } = string.Empty;
    }
}
namespace DaJet.Mcp.Model
{
    public sealed class MdObject
    {
        public string Name { get; set; } = string.Empty;

        public string DbName { get; set; } = string.Empty;

        public List<MdProperty> Properties { get; set; } = new();

        public List<MdObject> TableParts { get; set; } = new();
    }
}
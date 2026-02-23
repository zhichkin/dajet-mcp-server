using DaJet.Metadata;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using MetadataCache = DaJet.Metadata.MetadataProvider;

namespace DaJet.Mcp.Server.Tools
{
    [McpServerToolType]
    public sealed class MetadataTool
    {
        [McpServerTool, Description("Получает свойства базы данных")]
        public static string GetDatabaseProperties([Description("Имя базы данных")]string name)
        {
            MetadataProvider provider = MetadataCache.Get(name)
                ?? throw new McpException($"База данных '{name}' не найдена");

            Configuration configuration = provider.GetConfiguration();

            return $"{configuration.Name} {configuration.AppConfigVersion}";
        }

        [McpServerTool, Description("Получает из базы данных список объектов метаданных указанного типа")]
        public static List<string> GetMetadataStructure(
            [Description("Имя базы данных")] string databaseName,
            [Description("Тип объекта метаданных")] string typeName)
        {
            MetadataProvider provider = MetadataCache.Get(databaseName)
                ?? throw new McpException($"База данных '{databaseName}' не найдена");

            if (!(typeName == "Документ" || typeName == "Справочник"))
            {
                throw new McpException($"Объект метаданных '{typeName}' не найден. Используйте 'Документ' или 'Справочник'.");
            }

            return provider.GetMetadataNames(typeName);
        }
    }
}
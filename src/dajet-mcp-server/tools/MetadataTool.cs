using DaJet.Data;
using DaJet.Json;
using DaJet.Mcp.Model;
using DaJet.Metadata;
using DaJet.TypeSystem;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using MetadataCache = DaJet.Metadata.MetadataProvider;

namespace DaJet.Mcp.Server.Tools
{
    [McpServerToolType]
    public sealed class MetadataTool
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        static MetadataTool()
        {
            JsonOptions.Converters.Add(new DataTypeJsonConverter());
        }

        [McpServerTool, Description("Получает список поддерживаемых типов объектов метаданных")]
        public List<string> GetMetadataTypeNames()
        {
            return new List<string>()
            {
                MetadataNames.Constant,
                MetadataNames.Catalog,
                MetadataNames.Document,
                MetadataNames.Enumeration,
                MetadataNames.Characteristic,
                MetadataNames.Publication,
                MetadataNames.BusinessTask,
                MetadataNames.BusinessProcess,
                MetadataNames.InformationRegister,
                MetadataNames.AccumulationRegister,
                MetadataNames.Account,
                MetadataNames.AccountingRegister
            };
        }

        [McpServerTool, Description("Получает список имён доступных баз данных")]
        public List<string> GetDatabaseNames()
        {
            List<MetadataProviderStatus> providers = MetadataCache.ToList();

            List<string> databases = new(providers.Count);

            foreach (MetadataProviderStatus provider in providers)
            {
                if (provider.DataSource == DataSourceType.SqlServer ||
                    provider.DataSource == DataSourceType.PostgreSql)
                {
                    databases.Add(provider.Name);
                }
            }

            return databases;
        }

        [McpServerTool(UseStructuredContent = true), Description("Получает описание базы данных (конфигурации) по её имени")]
        public InfoBase GetDatabaseDescription([Description("Имя базы данных (конфигурации)")]string name)
        {
            MetadataProvider provider = MetadataCache.Get(name)
                ?? throw new McpException($"База данных '{name}' не найдена");

            Configuration configuration = provider.GetConfiguration();

            InfoBase infoBase = new()
            {
                Name = configuration.Name,
                AppVersion = configuration.AppConfigVersion,
                PlatformVersion = configuration.CompatibilityVersion
            };

            return infoBase;
        }

        [McpServerTool(UseStructuredContent = true), Description("Получает описание структуры метаданных базы данных (конфигурации) по её имени")]
        public Dictionary<string, List<string>> GetDatabaseMetadata([Description("Имя базы данных (конфигурации)")] string name)
        {
            MetadataProvider provider = MetadataCache.Get(name)
                ?? throw new McpException($"База данных '{name}' не найдена");

            List<string> types = GetMetadataTypeNames();

            Dictionary<string, List<string>> metadata = new(types.Count);

            foreach (string type in types)
            {
                if (!metadata.ContainsKey(type))
                {
                    List<string> names = provider.GetMetadataNames(type);

                    metadata.Add(type, names);
                }
            }

            return metadata;
        }

        [McpServerTool(UseStructuredContent = true), Description("Получает описание структуры объекта метаданных базы данных (конфигурации) по его типу и имени")]
        public MdObject GetMetadataObject(
            [Description("Имя базы данных (конфигурации)")] string database,
            [Description("Тип объекта метаданных")] string type,
            [Description("Имя объекта метаданных")] string name)
        {
            MetadataProvider provider = MetadataCache.Get(database)
                ?? throw new McpException($"База данных '{database}' не найдена");

            EntityDefinition entity = provider.GetMetadataObject($"{type}.{name}");

            if (entity is null)
            {
                throw new McpException($"Объект метаданных '{type}.{name}' не найден в базе данных '{database}'.");
            }

            MdObject metadata = new()
            {
                Name = entity.Name,
                DbName = entity.DbName
            };

            CopyProperties(entity.Properties, metadata);

            foreach (EntityDefinition table in entity.Entities)
            {
                MdObject target = new();

                CopyProperties(table.Properties, in target);

                metadata.TableParts.Add(target);
            }

            return metadata;
        }
        private static void CopyProperties(in List<PropertyDefinition> properties, in MdObject target)
        {
            foreach (PropertyDefinition property in properties)
            {
                MdProperty copy = new()
                {
                    Name = property.Name,
                    Type = JsonSerializer.Serialize(property.Type, JsonOptions),
                    Purpose = property.Purpose.ToString()
                };

                foreach (ColumnDefinition column in property.Columns)
                {
                    copy.Columns.Add(new MdColumn()
                    {
                        Name = column.Name,
                        Type = JsonSerializer.Serialize(column.Type, JsonOptions),
                        Purpose = column.Purpose.ToString()
                    });
                }

                target.Properties.Add(copy);
            }
        }
    }
}
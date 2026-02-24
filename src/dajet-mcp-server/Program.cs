using DaJet.Data;
using MetadataCache = DaJet.Metadata.MetadataProvider;

namespace DaJet.Mcp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitializeMetadataCache();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSystemd();
            builder.Host.UseWindowsService();

            builder.Services
                .AddMcpServer()
                .WithHttpTransport()
                .WithToolsFromAssembly();

            WebApplication app = builder.Build();

            app.MapMcp();

            app.Run();
        }
        private static void InitializeMetadataCache()
        {
            ServerSettings settings = new();

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("datasources.json", optional: false)
                .Build();

            config.Bind(settings);

            foreach (DataSourceSettings dataSource in settings.DataSources)
            {
                if (string.IsNullOrWhiteSpace(dataSource.Name))
                {
                    FileLogger.Default.Write($"Не указано имя источника данных.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(dataSource.Type))
                {
                    FileLogger.Default.Write($"Не указан тип источника данных для '{dataSource.Name}'.");
                    continue;
                }

                if (!Enum.TryParse(dataSource.Type, out DataSourceType sourceType))
                {
                    FileLogger.Default.Write($"Указан неподдерживаемый тип источника данных '{dataSource.Type}' для '{dataSource.Name}'. Возможные значения: SqlServer или PostgreSql.");
                    continue;
                }

                try
                {
                    MetadataCache.Add(dataSource.Name, sourceType, dataSource.ConnectionString);
                }
                catch (Exception exception)
                {
                    string message = $"Ошибка регистрации источника данных '{dataSource.Name}': {exception.Message}";
                    FileLogger.Default.Write(message);
                    continue;
                }
            }
        }
    }
}
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
            AppSettings settings = new();

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            config.Bind(settings);

            if (!Enum.TryParse(settings.DataSource, out DataSourceType dataSource))
            {
                throw new InvalidOperationException("Неверно указаны настройки источника данных");
            }

            try
            {
                MetadataCache.Add("MySource", dataSource, settings.ConnectionString);
            }
            catch (Exception exception)
            {
                string message = $"Ошибка регистрации источника данных 'MySource': {exception.Message}";

                throw new InvalidOperationException(message);
            }
        }
    }
}
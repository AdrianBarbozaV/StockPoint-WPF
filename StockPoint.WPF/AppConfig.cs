using Microsoft.Extensions.Configuration;

namespace StockPoint.WPF
{
    public static class AppConfig
    {
        private static readonly IConfiguration _config;

        static AppConfig()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        public static string ApiBaseUrl => _config["ApiSettings:BaseUrl"] ?? "https://localhost:7154/";
    }
}

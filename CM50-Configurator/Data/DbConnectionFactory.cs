using System.Configuration;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    internal static class DbConnectionFactory
    {
        private const string ConnectionStringName = "dbfw";

        public static MySqlConnection CreateConnection()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (settings == null)
            {
                throw new ConfigurationErrorsException(
                    $"Stringa di connessione '{ConnectionStringName}' non trovata in App.config.");
            }

            return new MySqlConnection(settings.ConnectionString);
        }
    }
}

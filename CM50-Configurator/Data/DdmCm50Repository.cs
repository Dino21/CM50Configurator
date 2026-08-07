using System.Collections.Generic;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    internal class DdmCm50Repository
    {
        private const string TableName = "cfg.ddmcm50";

        public List<DdmCm50> GetAll()
        {
            const string query = @"
SELECT idDDM, code, fwCode, cassFwFilename, bascFwFilename, `type`, enableFeedbackSorter
FROM cfg.ddmcm50
ORDER BY idDDM;";

            var result = new List<DdmCm50>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DdmCm50
                        {
                            IdDDM = reader.GetInt32OrDefault("idDDM"),
                            Code = reader.GetStringOrNull("code"),
                            FwCode = reader.GetStringOrNull("fwCode"),
                            CassFwFilename = reader.GetStringOrNull("cassFwFilename"),
                            BascFwFilename = reader.GetStringOrNull("bascFwFilename"),
                            Type = reader.GetStringOrNull("type"),
                            EnableFeedbackSorter = reader.GetBooleanOrDefault("enableFeedbackSorter")
                        });
                    }
                }
            }

            return result;
        }

        public static SimpleTableConfig<DdmCm50> CreateWriterConfig() => new SimpleTableConfig<DdmCm50>
        {
            TableName = TableName,
            IdColumn = "idDDM",
            GetId = x => x.IdDDM,
            SetId = (x, id) => x.IdDDM = id,
            Columns = new (string, System.Func<DdmCm50, object>)[]
            {
                ("code", x => x.Code),
                ("fwCode", x => x.FwCode),
                ("cassFwFilename", x => x.CassFwFilename),
                ("bascFwFilename", x => x.BascFwFilename),
                ("`type`", x => x.Type),
                ("enableFeedbackSorter", x => x.EnableFeedbackSorter)
            }
        };
    }
}

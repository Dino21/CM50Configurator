using System.Collections.Generic;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    internal class FeederCm50Repository
    {
        private const string TableName = "cfg.feedercm50";

        public List<FeederCm50> GetAll()
        {
            const string query = @"
SELECT idCfg, code, `TL`, fwRTC, fwPH02, cdfName, debugMode, jumpPhAdj
FROM cfg.feedercm50
ORDER BY idCfg;";

            var result = new List<FeederCm50>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new FeederCm50
                        {
                            IdCfg = reader.GetInt32OrDefault("idCfg"),
                            Code = reader.GetStringOrNull("code"),
                            TL = reader.GetStringOrNull("TL"),
                            FwRTC = reader.GetStringOrNull("fwRTC"),
                            FwPH02 = reader.GetStringOrNull("fwPH02"),
                            CdfName = reader.GetStringOrNull("cdfName"),
                            DebugMode = reader.GetInt32OrDefault("debugMode"),
                            JumpPhAdj = reader.GetInt32OrDefault("jumpPhAdj")
                        });
                    }
                }
            }

            return result;
        }

        public static SimpleTableConfig<FeederCm50> CreateWriterConfig() => new SimpleTableConfig<FeederCm50>
        {
            TableName = TableName,
            IdColumn = "idCfg",
            GetId = x => x.IdCfg,
            SetId = (x, id) => x.IdCfg = id,
            Columns = new (string, System.Func<FeederCm50, object>)[]
            {
                ("code", x => x.Code),
                ("`TL`", x => x.TL),
                ("fwRTC", x => x.FwRTC),
                ("fwPH02", x => x.FwPH02),
                ("cdfName", x => x.CdfName),
                ("debugMode", x => x.DebugMode),
                ("jumpPhAdj", x => x.JumpPhAdj)
            }
        };
    }
}

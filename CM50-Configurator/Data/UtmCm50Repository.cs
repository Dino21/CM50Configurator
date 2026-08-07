using System.Collections.Generic;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    internal class UtmCm50Repository
    {
        private const string TableName = "cfg.utmcm50";

        public List<UtmCm50> GetAll()
        {
            const string query = @"
SELECT idCfg, code, model, bidirectional, snr, uim, highSpeed, fwCode,
    rtcFwFilename, ph01FwFilename, ph02FwFilename, safeFwFilename, uiFwFilename, debugMode
FROM cfg.utmcm50
ORDER BY idCfg;";

            var result = new List<UtmCm50>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new UtmCm50
                        {
                            IdCfg = reader.GetInt32OrDefault("idCfg"),
                            Code = reader.GetStringOrNull("code"),
                            Model = reader.GetStringOrNull("model"),
                            Bidirectional = reader.GetStringOrNull("bidirectional"),
                            Snr = reader.GetStringOrNull("snr"),
                            Uim = reader.GetStringOrNull("uim"),
                            HighSpeed = reader.GetStringOrNull("highSpeed"),
                            FwCode = reader.GetStringOrNull("fwCode"),
                            RtcFwFilename = reader.GetStringOrNull("rtcFwFilename"),
                            Ph01FwFilename = reader.GetStringOrNull("ph01FwFilename"),
                            Ph02FwFilename = reader.GetStringOrNull("ph02FwFilename"),
                            SafeFwFilename = reader.GetStringOrNull("safeFwFilename"),
                            UiFwFilename = reader.GetStringOrNull("uiFwFilename"),
                            DebugMode = reader.GetStringOrNull("debugMode")
                        });
                    }
                }
            }

            return result;
        }

        public static SimpleTableConfig<UtmCm50> CreateWriterConfig() => new SimpleTableConfig<UtmCm50>
        {
            TableName = TableName,
            IdColumn = "idCfg",
            GetId = x => x.IdCfg,
            SetId = (x, id) => x.IdCfg = id,
            Columns = new (string, System.Func<UtmCm50, object>)[]
            {
                ("code", x => x.Code),
                ("model", x => x.Model),
                ("bidirectional", x => x.Bidirectional),
                ("snr", x => x.Snr),
                ("uim", x => x.Uim),
                ("highSpeed", x => x.HighSpeed),
                ("fwCode", x => x.FwCode),
                ("rtcFwFilename", x => x.RtcFwFilename),
                ("ph01FwFilename", x => x.Ph01FwFilename),
                ("ph02FwFilename", x => x.Ph02FwFilename),
                ("safeFwFilename", x => x.SafeFwFilename),
                ("uiFwFilename", x => x.UiFwFilename),
                ("debugMode", x => x.DebugMode)
            }
        };
    }
}

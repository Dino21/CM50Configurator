using System.Collections.Generic;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    internal class LtmCm50Repository
    {
        private const string TableName = "cfg.ltmcm50";

        public List<LtmCm50> GetAll()
        {
            const string query = @"
SELECT idLTM, code, cassetteNumber, cageType, safeFwFileName, alarmFwFileName, fwCode
FROM cfg.ltmcm50
ORDER BY idLTM;";

            var result = new List<LtmCm50>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new LtmCm50
                        {
                            IdLTM = reader.GetInt32OrDefault("idLTM"),
                            Code = reader.GetStringOrNull("code"),
                            CassetteNumber = reader.GetInt32OrDefault("cassetteNumber"),
                            CageType = reader.GetStringOrNull("cageType"),
                            SafeFwFileName = reader.GetStringOrNull("safeFwFileName"),
                            AlarmFwFileName = reader.GetStringOrNull("alarmFwFileName"),
                            FwCode = reader.GetStringOrNull("fwCode")
                        });
                    }
                }
            }

            return result;
        }

        public static SimpleTableConfig<LtmCm50> CreateWriterConfig() => new SimpleTableConfig<LtmCm50>
        {
            TableName = TableName,
            IdColumn = "idLTM",
            GetId = x => x.IdLTM,
            SetId = (x, id) => x.IdLTM = id,
            Columns = new (string, System.Func<LtmCm50, object>)[]
            {
                ("code", x => x.Code),
                ("cassetteNumber", x => x.CassetteNumber),
                ("cageType", x => x.CageType),
                ("safeFwFileName", x => x.SafeFwFileName),
                ("alarmFwFileName", x => x.AlarmFwFileName),
                ("fwCode", x => x.FwCode)
            }
        };
    }
}

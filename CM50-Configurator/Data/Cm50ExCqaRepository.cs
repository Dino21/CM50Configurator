using System.Collections.Generic;
using System.Linq;
using CM50_Configurator.Models;
using CM50_Configurator.Support;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    internal class Cm50ExCqaRepository
    {
        private const string TableName = "cm50excqa";

        private const string SelectAllQuery = @"
SELECT
    e.id, e.handleUnfit, e.bundleQty,
    e.setup1, e.ncicli1, e.depType1,
    e.setup2, e.ncicli2, e.depType2,
    e.setup3, e.ncicli3, e.depType3,
    e.setup4, e.ncicli4, e.depType4,
    e.finalSetup, e.finalDepType,
    e.maxCycleReject, e.maxCycleJam,

    p.idProductCode, p.code AS productCode, p.description AS productDescription,
    p.cassetteNumber, p.machineType, p.cassetteName, p.upperLock, p.lowerLock,
    p.readingStation, p.barcodeReader, p.alarmType AS productAlarmType,

    c.idClient, c.clientName
FROM cm50excqa e
LEFT JOIN cm50prodcode p ON e.idProduct = p.idProductCode
LEFT JOIN client c ON e.idClient = c.idClient
ORDER BY p.code, c.clientName, e.id;";

        // Come in Form1/Cm50Form, "Carica da DB" ricarica anche gli elenchi di Prodotti e Clienti
        // usati dai dropdown della PropertyGrid (ProductReferenceConverter/ClientReferenceConverter).
        public List<Cm50ExCqa> GetAllWithReferences()
        {
            var referenceRepository = new ReferenceDataRepository();
            ReferenceDataCache.Products = referenceRepository.GetProducts().ToArray();
            ReferenceDataCache.Clients = referenceRepository.GetClients().ToArray();

            return GetAll();
        }

        public List<Cm50ExCqa> GetAll()
        {
            var result = new List<Cm50ExCqa>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(SelectAllQuery, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(MapRow(reader));
                    }
                }
            }

            return result;
        }

        private static Cm50ExCqa MapRow(MySqlDataReader reader)
        {
            var item = new Cm50ExCqa
            {
                Id = reader.GetInt32OrDefault("id"),
                HandleUnfit = reader.GetBooleanOrNull("handleUnfit"),
                BundleQty = reader.GetInt32OrNull("bundleQty"),
                Setup1 = reader.GetStringOrNull("setup1"),
                Ncicli1 = reader.GetInt32OrNull("ncicli1"),
                DepType1 = reader.GetStringOrNull("depType1"),
                Setup2 = reader.GetStringOrNull("setup2"),
                Ncicli2 = reader.GetInt32OrNull("ncicli2"),
                DepType2 = reader.GetStringOrNull("depType2"),
                Setup3 = reader.GetStringOrNull("setup3"),
                Ncicli3 = reader.GetInt32OrNull("ncicli3"),
                DepType3 = reader.GetStringOrNull("depType3"),
                Setup4 = reader.GetStringOrNull("setup4"),
                Ncicli4 = reader.GetInt32OrNull("ncicli4"),
                DepType4 = reader.GetStringOrNull("depType4"),
                FinalSetup = reader.GetStringOrNull("finalSetup"),
                FinalDepType = reader.GetStringOrNull("finalDepType"),
                MaxCycleReject = reader.GetInt32OrNull("maxCycleReject"),
                MaxCycleJam = reader.GetInt32OrNull("maxCycleJam")
            };

            if (!reader.IsDBNull(reader.GetOrdinal("idProductCode")))
            {
                item.Product = new Cm50ProdCode
                {
                    IdProductCode = reader.GetInt32OrDefault("idProductCode"),
                    Code = reader.GetStringOrNull("productCode"),
                    Description = reader.GetStringOrNull("productDescription"),
                    CassetteNumber = reader.GetInt32OrDefault("cassetteNumber"),
                    MachineType = reader.GetStringOrNull("machineType"),
                    CassetteName = reader.GetStringOrNull("cassetteName"),
                    UpperLock = reader.GetStringOrNull("upperLock"),
                    LowerLock = reader.GetStringOrNull("lowerLock"),
                    ReadingStation = reader.GetStringOrNull("readingStation"),
                    BarcodeReader = reader.GetStringOrNull("barcodeReader"),
                    AlarmType = reader.GetStringOrNull("productAlarmType")
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("idClient")))
            {
                item.Client = new Models.Client
                {
                    IdClient = reader.GetInt32OrDefault("idClient"),
                    ClientName = reader.GetStringOrNull("clientName")
                };
            }

            return item;
        }

        // La PK della tabella è (id, idProduct), ma id è AUTO_INCREMENT e quindi già univoco da solo:
        // l'upsert generico per id di SimpleTableWriter è sufficiente.
        public static SimpleTableConfig<Cm50ExCqa> CreateWriterConfig() => new SimpleTableConfig<Cm50ExCqa>
        {
            TableName = TableName,
            IdColumn = "id",
            GetId = x => x.Id,
            SetId = (x, id) => x.Id = id,
            Columns = new (string, System.Func<Cm50ExCqa, object>)[]
            {
                ("idProduct", x => x.Product?.IdProductCode),
                ("idClient", x => x.Client?.IdClient),
                ("handleUnfit", x => x.HandleUnfit),
                ("bundleQty", x => x.BundleQty),
                ("setup1", x => x.Setup1),
                ("ncicli1", x => x.Ncicli1),
                ("depType1", x => x.DepType1),
                ("setup2", x => x.Setup2),
                ("ncicli2", x => x.Ncicli2),
                ("depType2", x => x.DepType2),
                ("setup3", x => x.Setup3),
                ("ncicli3", x => x.Ncicli3),
                ("depType3", x => x.DepType3),
                ("setup4", x => x.Setup4),
                ("ncicli4", x => x.Ncicli4),
                ("depType4", x => x.DepType4),
                ("finalSetup", x => x.FinalSetup),
                ("finalDepType", x => x.FinalDepType),
                ("maxCycleReject", x => x.MaxCycleReject),
                ("maxCycleJam", x => x.MaxCycleJam)
            }
        };
    }
}

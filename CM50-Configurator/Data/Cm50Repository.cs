using System.Collections.Generic;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    // Legge la tabella "master" cm50 (configurazione per codice prodotto) con tutte le tabelle
    // collegate: a differenza di cm50usercustomization, qui productCode/DDcode/ASSETcode/CDFcode
    // sono FK per codice (non per id), quindi i JOIN avvengono sul campo Code delle rispettive
    // tabelle di riferimento invece che sull'id.
    internal class Cm50Repository
    {
        private const string SelectAllQuery = @"
SELECT
    c.idProduct, c.DDtransitionCode, c.alarmType,

    p.idProductCode, p.code AS productCode, p.description AS productDescription,
    p.cassetteNumber, p.machineType, p.cassetteName, p.upperLock, p.lowerLock,
    p.readingStation, p.barcodeReader, p.alarmType AS productAlarmType,

    cl.idClient, cl.clientName,

    dd.idDD, dd.Code AS ddCode, dd.internalName AS ddInternalName,
    dd.transitionDDCode, dd.transitionFilename, dd.aSISver,

    a.idAsset, a.code AS assetCode, a.internalName AS assetInternalName,
    a.shortFilename, a.tallFilename, a.version AS assetVersion, a.release AS assetRelease, a.serial,

    cdf.idCdf, cdf.code AS cdfCode, cdf.internalName AS cdfInternalName,
    cdf.currCode0, cdf.currCode1, cdf.currCode2, cdf.currCode3, cdf.currCode4,
    cdf.currCode5, cdf.currCode6, cdf.currCode7, cdf.currCode8, cdf.currCode9,
    cdf.currCode10, cdf.currCode11, cdf.currCode12, cdf.currCode13, cdf.currCode14, cdf.currCode15,

    u.idUnitConfiguration, u.cfg, u.optCfg, u.optOneCfg, u.optTwoCfg, u.optThreeCfg,
    u.description AS unitConfigDescription,

    r.idReaderCfg, r.activeBank, r.slot0id, r.slot1id, r.slot2id, r.slot3id,
    r.bank0Name, r.bank0Ver, r.bank1Name, r.bank1Ver, r.bank2Name, r.bank2Ver, r.bank3Name, r.bank3Ver,
    r.bank4Name, r.bank4Ver, r.bank5Name, r.bank5Ver, r.bank6Name, r.bank6Ver, r.bank7Name, r.bank7Ver,
    r.bank8Name, r.bank8Ver, r.bank9Name, r.bank9Ver, r.bank10Name, r.bank10Ver, r.bank11Name, r.bank11Ver,
    r.bank12Name, r.bank12Ver, r.bank13Name, r.bank13Ver, r.bank14Name, r.bank14Ver, r.bank15Name, r.bank15Ver,

    cs.idCassetteCfg, cs.cassA, cs.cassB, cs.cassC, cs.cassD, cs.cassE, cs.cassF,
    cs.cassG, cs.cassH, cs.cassI, cs.cassJ, cs.cassK, cs.cassL

FROM cm50 c
LEFT JOIN cm50prodcode p ON c.productCode = p.code
LEFT JOIN client cl ON c.idClient = cl.idClient
LEFT JOIN cm50ddcode dd ON c.DDcode = dd.Code
LEFT JOIN cm50assetcode a ON c.ASSETcode = a.code
LEFT JOIN cm50cdfcode cdf ON c.CDFcode = cdf.code
LEFT JOIN cm50unitconfiguration u ON c.idUnitConfiguration = u.idUnitConfiguration
LEFT JOIN cfg.cm50reader r ON c.idReaderCfg = r.idReaderCfg
LEFT JOIN cfg.cm50cassette cs ON c.idCassetteCfg = cs.idCassetteCfg
ORDER BY c.idProduct;";

        public List<Cm50> GetAll()
        {
            var result = new List<Cm50>();

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

        private static Cm50 MapRow(MySqlDataReader reader)
        {
            var item = new Cm50
            {
                IdProduct = reader.GetInt32OrDefault("idProduct"),
                DdTransitionCode = reader.GetStringOrNull("DDtransitionCode"),
                AlarmType = reader.GetStringOrNull("alarmType")
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

            if (!reader.IsDBNull(reader.GetOrdinal("idDD")))
            {
                item.Dd = new Cm50DdCode
                {
                    IdDD = reader.GetInt32OrDefault("idDD"),
                    Code = reader.GetStringOrNull("ddCode"),
                    InternalName = reader.GetStringOrNull("ddInternalName"),
                    TransitionDDCode = reader.GetStringOrNull("transitionDDCode"),
                    TransitionFilename = reader.GetStringOrNull("transitionFilename"),
                    ASISver = reader.GetStringOrNull("aSISver")
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("idAsset")))
            {
                item.Asset = new Cm50AssetCode
                {
                    IdAsset = reader.GetInt32OrDefault("idAsset"),
                    Code = reader.GetStringOrNull("assetCode"),
                    InternalName = reader.GetStringOrNull("assetInternalName"),
                    ShortFilename = reader.GetStringOrNull("shortFilename"),
                    TallFilename = reader.GetStringOrNull("tallFilename"),
                    Version = reader.GetStringOrNull("assetVersion"),
                    Release = reader.GetStringOrNull("assetRelease"),
                    Serial = reader.GetStringOrNull("serial")
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("idCdf")))
            {
                item.Cdf = new Cm50CdfCode
                {
                    IdCdf = reader.GetInt32OrDefault("idCdf"),
                    Code = reader.GetStringOrNull("cdfCode"),
                    InternalName = reader.GetStringOrNull("cdfInternalName"),
                    CurrCodes = new[]
                    {
                        reader.GetStringOrNull("currCode0"), reader.GetStringOrNull("currCode1"),
                        reader.GetStringOrNull("currCode2"), reader.GetStringOrNull("currCode3"),
                        reader.GetStringOrNull("currCode4"), reader.GetStringOrNull("currCode5"),
                        reader.GetStringOrNull("currCode6"), reader.GetStringOrNull("currCode7"),
                        reader.GetStringOrNull("currCode8"), reader.GetStringOrNull("currCode9"),
                        reader.GetStringOrNull("currCode10"), reader.GetStringOrNull("currCode11"),
                        reader.GetStringOrNull("currCode12"), reader.GetStringOrNull("currCode13"),
                        reader.GetStringOrNull("currCode14"), reader.GetStringOrNull("currCode15")
                    }
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("idUnitConfiguration")))
            {
                item.UnitConfiguration = new Cm50UnitConfiguration
                {
                    IdUnitConfiguration = reader.GetInt32OrDefault("idUnitConfiguration"),
                    Cfg = reader.GetStringOrNull("cfg"),
                    OptCfg = reader.GetStringOrNull("optCfg"),
                    OptOneCfg = reader.GetStringOrNull("optOneCfg"),
                    OptTwoCfg = reader.GetStringOrNull("optTwoCfg"),
                    OptThreeCfg = reader.GetStringOrNull("optThreeCfg"),
                    Description = reader.GetStringOrNull("unitConfigDescription")
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("idReaderCfg")))
            {
                item.Reader = ReferenceDataRepository.MapReader(reader);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("idCassetteCfg")))
            {
                item.Cassette = ReferenceDataRepository.MapCassette(reader);
            }

            return item;
        }
    }
}

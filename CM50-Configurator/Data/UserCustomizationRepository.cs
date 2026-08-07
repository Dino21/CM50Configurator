using System.Collections.Generic;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    internal class UserCustomizationRepository
    {
        private const string SelectAllQuery = @"
SELECT
    uc.idUserCustomization,
    uc.activeBanks,
    uc.bankConfig,
    uc.refBank0, uc.refBank1, uc.refBank2, uc.refBank3,
    uc.currency,
    uc.fitnessBank0, uc.fitnessBank1, uc.fitnessBank2, uc.fitnessBank3,
    uc.setCassConfig,
    uc.countryCode,
    uc.displayLanguage,
    uc.systemRate,
    uc.ecoTimeOut,

    p.idProductCode, p.code AS productCode, p.description AS productDescription,
    p.cassetteNumber, p.machineType, p.cassetteName, p.upperLock, p.lowerLock,
    p.readingStation, p.barcodeReader, p.alarmType AS productAlarmType,

    c.idClient, c.clientName,

    dd.idDD, dd.Code AS ddCode, dd.internalName AS ddInternalName,
    dd.transitionDDCode, dd.transitionFilename, dd.aSISver,

    a.idAsset, a.code AS assetCode, a.internalName AS assetInternalName,
    a.shortFilename, a.tallFilename, a.version AS assetVersion, a.release AS assetRelease, a.serial,

    u.idUnitConfiguration, u.cfg, u.optCfg, u.optOneCfg, u.optTwoCfg, u.optThreeCfg,
    u.description AS unitConfigDescription,

    cdf.idCdf, cdf.code AS cdfCode, cdf.internalName AS cdfInternalName,
    cdf.currCode0, cdf.currCode1, cdf.currCode2, cdf.currCode3, cdf.currCode4,
    cdf.currCode5, cdf.currCode6, cdf.currCode7, cdf.currCode8, cdf.currCode9,
    cdf.currCode10, cdf.currCode11, cdf.currCode12, cdf.currCode13, cdf.currCode14, cdf.currCode15

FROM cm50usercustomization uc
LEFT JOIN cm50prodcode p ON uc.idProduct = p.idProductCode
LEFT JOIN client c ON uc.idClient = c.idClient
LEFT JOIN cm50ddcode dd ON uc.idDD = dd.idDD
LEFT JOIN cm50assetcode a ON uc.idAsset = a.idAsset
LEFT JOIN cm50unitconfiguration u ON uc.idUnitConfiguration = u.idUnitConfiguration
LEFT JOIN cm50cdfcode cdf ON uc.idCdf = cdf.idCdf
ORDER BY uc.idUserCustomization;";

        public List<Cm50UserCustomization> GetAll()
        {
            var result = new List<Cm50UserCustomization>();

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

        private static Cm50UserCustomization MapRow(MySqlDataReader reader)
        {
            var item = new Cm50UserCustomization
            {
                IdUserCustomization = reader.GetInt32OrDefault("idUserCustomization"),
                ActiveBanks = reader.GetInt32OrDefault("activeBanks"),
                BankConfig = reader.GetStringOrNull("bankConfig"),
                RefBanks = new[]
                {
                    reader.GetStringOrNull("refBank0"),
                    reader.GetStringOrNull("refBank1"),
                    reader.GetStringOrNull("refBank2"),
                    reader.GetStringOrNull("refBank3")
                },
                Currency = reader.GetStringOrNull("currency"),
                FitnessBanks = new[]
                {
                    reader.GetInt32OrDefault("fitnessBank0"),
                    reader.GetInt32OrDefault("fitnessBank1"),
                    reader.GetInt32OrDefault("fitnessBank2"),
                    reader.GetInt32OrDefault("fitnessBank3")
                },
                SetCassConfig = reader.GetStringOrNull("setCassConfig"),
                CountryCode = reader.GetStringOrNull("countryCode"),
                DisplayLanguage = reader.GetStringOrNull("displayLanguage"),
                SystemRate = reader.GetInt32OrDefault("systemRate"),
                EcoTimeOut = reader.GetInt32OrDefault("ecoTimeOut")
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

            return item;
        }
    }
}

using System.Collections.Generic;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    // Legge gli elenchi completi delle tabelle di riferimento (DD, Asset, CDF, Configurazioni Unità),
    // usati dalla finestra di dettaglio per proporre una selezione e per riconoscere/riallineare un
    // codice digitato a mano rispetto ai record già presenti nel DB.
    internal class ReferenceDataRepository
    {
        public List<Cm50DdCode> GetDdCodes()
        {
            const string query = @"
SELECT idDD, Code, internalName, transitionDDCode, transitionFilename, aSISver
FROM cm50ddcode
ORDER BY Code;";

            var result = new List<Cm50DdCode>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Cm50DdCode
                        {
                            IdDD = reader.GetInt32OrDefault("idDD"),
                            Code = reader.GetStringOrNull("Code"),
                            InternalName = reader.GetStringOrNull("internalName"),
                            TransitionDDCode = reader.GetStringOrNull("transitionDDCode"),
                            TransitionFilename = reader.GetStringOrNull("transitionFilename"),
                            ASISver = reader.GetStringOrNull("aSISver")
                        });
                    }
                }
            }

            return result;
        }

        public List<Cm50AssetCode> GetAssetCodes()
        {
            const string query = @"
SELECT idAsset, code, internalName, shortFilename, tallFilename, `version`, `release`, serial
FROM cm50assetcode
ORDER BY code;";

            var result = new List<Cm50AssetCode>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Cm50AssetCode
                        {
                            IdAsset = reader.GetInt32OrDefault("idAsset"),
                            Code = reader.GetStringOrNull("code"),
                            InternalName = reader.GetStringOrNull("internalName"),
                            ShortFilename = reader.GetStringOrNull("shortFilename"),
                            TallFilename = reader.GetStringOrNull("tallFilename"),
                            Version = reader.GetStringOrNull("version"),
                            Release = reader.GetStringOrNull("release"),
                            Serial = reader.GetStringOrNull("serial")
                        });
                    }
                }
            }

            return result;
        }

        public List<Cm50CdfCode> GetCdfCodes()
        {
            const string query = @"
SELECT idCdf, code, internalName,
    currCode0, currCode1, currCode2, currCode3, currCode4, currCode5, currCode6, currCode7,
    currCode8, currCode9, currCode10, currCode11, currCode12, currCode13, currCode14, currCode15
FROM cm50cdfcode
ORDER BY code;";

            var result = new List<Cm50CdfCode>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Cm50CdfCode
                        {
                            IdCdf = reader.GetInt32OrDefault("idCdf"),
                            Code = reader.GetStringOrNull("code"),
                            InternalName = reader.GetStringOrNull("internalName"),
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
                        });
                    }
                }
            }

            return result;
        }

        public List<Cm50ProdCode> GetProducts()
        {
            const string query = @"
SELECT idProductCode, code, description, cassetteNumber, machineType, cassetteName,
    upperLock, lowerLock, readingStation, barcodeReader, alarmType
FROM cm50prodcode
ORDER BY code;";

            var result = new List<Cm50ProdCode>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Cm50ProdCode
                        {
                            IdProductCode = reader.GetInt32OrDefault("idProductCode"),
                            Code = reader.GetStringOrNull("code"),
                            Description = reader.GetStringOrNull("description"),
                            CassetteNumber = reader.GetInt32OrDefault("cassetteNumber"),
                            MachineType = reader.GetStringOrNull("machineType"),
                            CassetteName = reader.GetStringOrNull("cassetteName"),
                            UpperLock = reader.GetStringOrNull("upperLock"),
                            LowerLock = reader.GetStringOrNull("lowerLock"),
                            ReadingStation = reader.GetStringOrNull("readingStation"),
                            BarcodeReader = reader.GetStringOrNull("barcodeReader"),
                            AlarmType = reader.GetStringOrNull("alarmType")
                        });
                    }
                }
            }

            return result;
        }

        public List<Client> GetClients()
        {
            const string query = "SELECT idClient, clientName FROM client ORDER BY clientName;";

            var result = new List<Client>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Client
                        {
                            IdClient = reader.GetInt32OrDefault("idClient"),
                            ClientName = reader.GetStringOrNull("clientName")
                        });
                    }
                }
            }

            return result;
        }

        public List<Cm50UnitConfiguration> GetUnitConfigurations()
        {
            const string query = @"
SELECT idUnitConfiguration, cfg, optCfg, optOneCfg, optTwoCfg, optThreeCfg, description
FROM cm50unitconfiguration
ORDER BY cfg, optCfg, optOneCfg, optTwoCfg, optThreeCfg;";

            var result = new List<Cm50UnitConfiguration>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Cm50UnitConfiguration
                        {
                            IdUnitConfiguration = reader.GetInt32OrDefault("idUnitConfiguration"),
                            Cfg = reader.GetStringOrNull("cfg"),
                            OptCfg = reader.GetStringOrNull("optCfg"),
                            OptOneCfg = reader.GetStringOrNull("optOneCfg"),
                            OptTwoCfg = reader.GetStringOrNull("optTwoCfg"),
                            OptThreeCfg = reader.GetStringOrNull("optThreeCfg"),
                            Description = reader.GetStringOrNull("description")
                        });
                    }
                }
            }

            return result;
        }

        public List<Cm50Reader> GetReaders()
        {
            const string query = @"
SELECT idReaderCfg, activeBank, slot0id, slot1id, slot2id, slot3id,
    bank0Name, bank0Ver, bank1Name, bank1Ver, bank2Name, bank2Ver, bank3Name, bank3Ver,
    bank4Name, bank4Ver, bank5Name, bank5Ver, bank6Name, bank6Ver, bank7Name, bank7Ver,
    bank8Name, bank8Ver, bank9Name, bank9Ver, bank10Name, bank10Ver, bank11Name, bank11Ver,
    bank12Name, bank12Ver, bank13Name, bank13Ver, bank14Name, bank14Ver, bank15Name, bank15Ver
FROM cfg.cm50reader
ORDER BY idReaderCfg;";

            var result = new List<Cm50Reader>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(MapReader(reader));
                    }
                }
            }

            return result;
        }

        public List<Cm50Cassette> GetCassettes()
        {
            const string query = @"
SELECT idCassetteCfg, cassA, cassB, cassC, cassD, cassE, cassF, cassG, cassH, cassI, cassJ, cassK, cassL
FROM cfg.cm50cassette
ORDER BY idCassetteCfg;";

            var result = new List<Cm50Cassette>();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(MapCassette(reader));
                    }
                }
            }

            return result;
        }

        internal static Cm50Reader MapReader(MySqlDataReader reader)
        {
            var names = new string[16];
            var versions = new string[16];
            for (int i = 0; i < 16; i++)
            {
                names[i] = reader.GetStringOrNull($"bank{i}Name");
                versions[i] = reader.GetStringOrNull($"bank{i}Ver");
            }

            return new Cm50Reader
            {
                IdReaderCfg = reader.GetInt32OrDefault("idReaderCfg"),
                ActiveBank = reader.GetInt32OrDefault("activeBank"),
                SlotIds = new[]
                {
                    reader.GetInt32OrDefault("slot0id"), reader.GetInt32OrDefault("slot1id"),
                    reader.GetInt32OrDefault("slot2id"), reader.GetInt32OrDefault("slot3id")
                },
                BankNames = names,
                BankVersions = versions
            };
        }

        internal static Cm50Cassette MapCassette(MySqlDataReader reader) => new Cm50Cassette
        {
            IdCassetteCfg = reader.GetInt32OrDefault("idCassetteCfg"),
            CassA = reader.GetStringOrNull("cassA"),
            CassB = reader.GetStringOrNull("cassB"),
            CassC = reader.GetStringOrNull("cassC"),
            CassD = reader.GetStringOrNull("cassD"),
            CassE = reader.GetStringOrNull("cassE"),
            CassF = reader.GetStringOrNull("cassF"),
            CassG = reader.GetStringOrNull("cassG"),
            CassH = reader.GetStringOrNull("cassH"),
            CassI = reader.GetStringOrNull("cassI"),
            CassJ = reader.GetStringOrNull("cassJ"),
            CassK = reader.GetStringOrNull("cassK"),
            CassL = reader.GetStringOrNull("cassL")
        };
    }
}

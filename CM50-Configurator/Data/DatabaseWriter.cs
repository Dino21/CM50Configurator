using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CM50_Configurator.Models;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    // Scrive su MySQL le personalizzazioni importate da JSON. Per ogni record, prima di scrivere
    // la riga di cm50usercustomization, verifica/crea le righe collegate (DD, Asset, CDF,
    // Configurazione Unità) che non esistono ancora: un codice non trovato viene interpretato
    // come nuovo, e prima di crearlo viene mostrata una finestra di conferma/completamento
    // (NewReferenceForm) con i campi obbligatori evidenziati. Se l'utente annulla, il record
    // viene saltato e segnalato come errore, senza bloccare gli altri. Prodotto e Cliente non
    // vengono mai creati automaticamente (nella UI sono bloccati, si presume debbano già esistere).
    // Ogni record viene scritto in una propria transazione, così un errore su una riga non blocca le altre.
    public class DatabaseWriter
    {
        public ImportResult ImportAll(List<Cm50UserCustomization> items, IWin32Window owner)
        {
            var result = new ImportResult();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();

                foreach (Cm50UserCustomization item in items)
                {
                    result.Processed++;

                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int idProduct = ResolveProductId(connection, transaction, item.Product);
                            int idClient = ResolveClientId(connection, transaction, item.Client);
                            int idDD = EnsureDd(connection, transaction, item.Dd, result.NewDdCodes, owner);
                            int idAsset = EnsureAsset(connection, transaction, item.Asset, result.NewAssetCodes, owner);
                            int idCdf = EnsureCdf(connection, transaction, item.Cdf, result.NewCdfCodes, owner);
                            int idUnitConfiguration = EnsureUnitConfiguration(connection, transaction, item.UnitConfiguration, result.NewUnitConfigurations, owner);

                            bool inserted = UpsertUserCustomization(connection, transaction, item,
                                idProduct, idClient, idDD, idAsset, idUnitConfiguration, idCdf);

                            transaction.Commit();

                            if (inserted) result.Inserted++;
                            else result.Updated++;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            result.Errors.Add($"Record #{item.IdUserCustomization} ({item.Product?.Code} / {item.Client?.ClientName}): {ex.Message}");
                        }
                    }
                }
            }

            return result;
        }

        // Come ImportAll, ma per la tabella "master" cm50: qui Reader e Cassette (schema cfg) si
        // aggiungono alle tabelle di riferimento già gestite per cm50usercustomization, e il
        // Codice Transizione DD deve corrispondere a un transitionDDCode già esistente tra i DD
        // (non viene mai creato automaticamente, come Prodotto e Cliente).
        public ImportResult ImportAllCm50(List<Cm50> items, IWin32Window owner)
        {
            var result = new ImportResult();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();

                foreach (Cm50 item in items)
                {
                    result.Processed++;

                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Product/Dd/Asset/Cdf sono referenziati per codice (non per id) nella tabella
                            // cm50: le Ensure/Resolve vanno comunque chiamate per il loro effetto (verifica
                            // di esistenza, creazione se assenti), ma l'id restituito non serve alla UPSERT.
                            ResolveProductId(connection, transaction, item.Product);
                            int idClient = ResolveClientId(connection, transaction, item.Client);
                            EnsureDd(connection, transaction, item.Dd, result.NewDdCodes, owner);
                            string transitionCode = ResolveDdTransitionCode(connection, transaction, item.DdTransitionCode);
                            EnsureAsset(connection, transaction, item.Asset, result.NewAssetCodes, owner);
                            EnsureCdf(connection, transaction, item.Cdf, result.NewCdfCodes, owner);
                            int idUnitConfiguration = EnsureUnitConfiguration(connection, transaction, item.UnitConfiguration, result.NewUnitConfigurations, owner);
                            int idReaderCfg = EnsureReader(connection, transaction, item.Reader, result.NewReaderConfigurations, owner);
                            int idCassetteCfg = EnsureCassette(connection, transaction, item.Cassette, result.NewCassetteConfigurations, owner);

                            bool inserted = UpsertCm50(connection, transaction, item, idClient,
                                transitionCode, idUnitConfiguration, idReaderCfg, idCassetteCfg);

                            transaction.Commit();

                            if (inserted) result.Inserted++;
                            else result.Updated++;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            result.Errors.Add($"Record #{item.IdProduct} ({item.Product?.Code} / {item.Client?.ClientName}): {ex.Message}");
                        }
                    }
                }
            }

            return result;
        }

        // Mostra la finestra di conferma/completamento; ritorna false se l'utente ha annullato.
        private static bool ConfirmNewReference(object item, string title, string introMessage, IWin32Window owner)
        {
            using (var form = new NewReferenceForm(item, title, introMessage))
            {
                return form.ShowDialog(owner) == DialogResult.OK;
            }
        }

        private static int ResolveProductId(MySqlConnection connection, MySqlTransaction transaction, Cm50ProdCode product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Code))
            {
                throw new InvalidOperationException("Codice prodotto mancante.");
            }

            int? id = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idProductCode FROM cm50prodcode WHERE code = @code", ("@code", product.Code));

            if (!id.HasValue)
            {
                throw new InvalidOperationException($"Prodotto '{product.Code}' non trovato: non viene creato automaticamente.");
            }

            return id.Value;
        }

        private static int ResolveClientId(MySqlConnection connection, MySqlTransaction transaction, Client client)
        {
            if (client == null || string.IsNullOrWhiteSpace(client.ClientName))
            {
                throw new InvalidOperationException("Cliente mancante.");
            }

            int? id = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idClient FROM client WHERE clientName = @name", ("@name", client.ClientName));

            if (!id.HasValue)
            {
                throw new InvalidOperationException($"Cliente '{client.ClientName}' non trovato: non viene creato automaticamente.");
            }

            return id.Value;
        }

        private static int EnsureDd(MySqlConnection connection, MySqlTransaction transaction, Cm50DdCode dd, List<string> newCodes, IWin32Window owner)
        {
            if (dd == null || string.IsNullOrWhiteSpace(dd.Code))
            {
                return 0;
            }

            int? id = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idDD FROM cm50ddcode WHERE Code = @code", ("@code", dd.Code));
            if (id.HasValue)
            {
                return id.Value;
            }

            if (!ConfirmNewReference(dd, "Nuovo codice DD",
                $"Il codice DD '{dd.Code}' non esiste nel database: completa i campi obbligatori per crearlo.", owner))
            {
                throw new OperationCanceledException($"Creazione del nuovo DD '{dd.Code}' annullata dall'utente.");
            }

            // Se durante la conferma il codice è stato modificato in uno già esistente, va usato quello.
            id = SqlHelper.ExecuteScalarInt(connection, transaction, "SELECT idDD FROM cm50ddcode WHERE Code = @code", ("@code", dd.Code));
            if (id.HasValue)
            {
                return id.Value;
            }

            long newId = SqlHelper.ExecuteInsert(connection, transaction,
                "INSERT INTO cm50ddcode (Code, internalName, transitionDDCode, transitionFilename, aSISver) " +
                "VALUES (@code, @internalName, @transitionDDCode, @transitionFilename, @asisVer)",
                ("@code", dd.Code), ("@internalName", dd.InternalName), ("@transitionDDCode", dd.TransitionDDCode),
                ("@transitionFilename", dd.TransitionFilename), ("@asisVer", dd.ASISver));

            newCodes.Add(dd.Code);
            return (int)newId;
        }

        private static int EnsureAsset(MySqlConnection connection, MySqlTransaction transaction, Cm50AssetCode asset, List<string> newCodes, IWin32Window owner)
        {
            if (asset == null || string.IsNullOrWhiteSpace(asset.Code))
            {
                return 0;
            }

            int? id = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idAsset FROM cm50assetcode WHERE code = @code", ("@code", asset.Code));
            if (id.HasValue)
            {
                return id.Value;
            }

            if (!ConfirmNewReference(asset, "Nuovo codice Asset",
                $"Il codice Asset '{asset.Code}' non esiste nel database: completa i campi per crearlo.", owner))
            {
                throw new OperationCanceledException($"Creazione del nuovo Asset '{asset.Code}' annullata dall'utente.");
            }

            id = SqlHelper.ExecuteScalarInt(connection, transaction, "SELECT idAsset FROM cm50assetcode WHERE code = @code", ("@code", asset.Code));
            if (id.HasValue)
            {
                return id.Value;
            }

            long newId = SqlHelper.ExecuteInsert(connection, transaction,
                "INSERT INTO cm50assetcode (code, internalName, shortFilename, tallFilename, `version`, `release`, serial) " +
                "VALUES (@code, @internalName, @shortFilename, @tallFilename, @version, @release, @serial)",
                ("@code", asset.Code), ("@internalName", asset.InternalName), ("@shortFilename", asset.ShortFilename),
                ("@tallFilename", asset.TallFilename), ("@version", asset.Version), ("@release", asset.Release),
                ("@serial", asset.Serial));

            newCodes.Add(asset.Code);
            return (int)newId;
        }

        private static int EnsureCdf(MySqlConnection connection, MySqlTransaction transaction, Cm50CdfCode cdf, List<string> newCodes, IWin32Window owner)
        {
            if (cdf == null || string.IsNullOrWhiteSpace(cdf.Code))
            {
                return 0;
            }

            int? id = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idCdf FROM cm50cdfcode WHERE code = @code", ("@code", cdf.Code));
            if (id.HasValue)
            {
                return id.Value;
            }

            if (!ConfirmNewReference(cdf, "Nuovo codice CDF",
                $"Il codice CDF '{cdf.Code}' non esiste nel database: completa i campi per crearlo.", owner))
            {
                throw new OperationCanceledException($"Creazione del nuovo CDF '{cdf.Code}' annullata dall'utente.");
            }

            id = SqlHelper.ExecuteScalarInt(connection, transaction, "SELECT idCdf FROM cm50cdfcode WHERE code = @code", ("@code", cdf.Code));
            if (id.HasValue)
            {
                return id.Value;
            }

            string[] curr = cdf.CurrCodes ?? new string[16];
            var parameters = new List<(string, object)> { ("@code", cdf.Code), ("@internalName", cdf.InternalName) };
            for (int i = 0; i < 16; i++)
            {
                parameters.Add(($"@curr{i}", i < curr.Length ? curr[i] : null));
            }

            long newId = SqlHelper.ExecuteInsert(connection, transaction,
                "INSERT INTO cm50cdfcode (code, internalName, currCode0, currCode1, currCode2, currCode3, currCode4, " +
                "currCode5, currCode6, currCode7, currCode8, currCode9, currCode10, currCode11, currCode12, currCode13, currCode14, currCode15) " +
                "VALUES (@code, @internalName, @curr0, @curr1, @curr2, @curr3, @curr4, @curr5, @curr6, @curr7, " +
                "@curr8, @curr9, @curr10, @curr11, @curr12, @curr13, @curr14, @curr15)",
                parameters.ToArray());

            newCodes.Add(cdf.Code);
            return (int)newId;
        }

        private static int EnsureUnitConfiguration(MySqlConnection connection, MySqlTransaction transaction, Cm50UnitConfiguration unitConfiguration, List<string> newConfigurations, IWin32Window owner)
        {
            if (unitConfiguration == null || string.IsNullOrWhiteSpace(unitConfiguration.Cfg))
            {
                return 0;
            }

            int? id = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idUnitConfiguration FROM cm50unitconfiguration " +
                "WHERE cfg = @cfg AND optCfg = @optCfg AND optOneCfg = @o1 AND optTwoCfg = @o2 AND optThreeCfg = @o3",
                ("@cfg", unitConfiguration.Cfg), ("@optCfg", unitConfiguration.OptCfg), ("@o1", unitConfiguration.OptOneCfg),
                ("@o2", unitConfiguration.OptTwoCfg), ("@o3", unitConfiguration.OptThreeCfg));
            if (id.HasValue)
            {
                return id.Value;
            }

            string configCode = string.Join("-", unitConfiguration.Cfg, unitConfiguration.OptCfg,
                unitConfiguration.OptOneCfg, unitConfiguration.OptTwoCfg, unitConfiguration.OptThreeCfg);

            if (!ConfirmNewReference(unitConfiguration, "Nuova Configurazione Unità",
                $"La Configurazione Unità '{configCode}' non esiste nel database: completa i campi per crearla.", owner))
            {
                throw new OperationCanceledException($"Creazione della nuova Configurazione Unità '{configCode}' annullata dall'utente.");
            }

            // Se durante la conferma la combinazione è stata modificata in una già esistente, va usata quella.
            id = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idUnitConfiguration FROM cm50unitconfiguration " +
                "WHERE cfg = @cfg AND optCfg = @optCfg AND optOneCfg = @o1 AND optTwoCfg = @o2 AND optThreeCfg = @o3",
                ("@cfg", unitConfiguration.Cfg), ("@optCfg", unitConfiguration.OptCfg), ("@o1", unitConfiguration.OptOneCfg),
                ("@o2", unitConfiguration.OptTwoCfg), ("@o3", unitConfiguration.OptThreeCfg));
            if (id.HasValue)
            {
                return id.Value;
            }

            long newId = SqlHelper.ExecuteInsert(connection, transaction,
                "INSERT INTO cm50unitconfiguration (cfg, optCfg, optOneCfg, optTwoCfg, optThreeCfg, description) " +
                "VALUES (@cfg, @optCfg, @o1, @o2, @o3, @description)",
                ("@cfg", unitConfiguration.Cfg), ("@optCfg", unitConfiguration.OptCfg), ("@o1", unitConfiguration.OptOneCfg),
                ("@o2", unitConfiguration.OptTwoCfg), ("@o3", unitConfiguration.OptThreeCfg), ("@description", unitConfiguration.Description));

            newConfigurations.Add(string.Join("-", unitConfiguration.Cfg, unitConfiguration.OptCfg,
                unitConfiguration.OptOneCfg, unitConfiguration.OptTwoCfg, unitConfiguration.OptThreeCfg));
            return (int)newId;
        }

        // A differenza di DD/Asset/CDF/Configurazione Unità, un transitionDDCode inesistente non
        // viene mai creato automaticamente: deve corrispondere a un DD già presente nel database
        // (eventualmente quello appena creato da EnsureDd, nella stessa transazione).
        private static string ResolveDdTransitionCode(MySqlConnection connection, MySqlTransaction transaction, string transitionCode)
        {
            if (string.IsNullOrWhiteSpace(transitionCode))
            {
                throw new InvalidOperationException("Codice Transizione DD mancante.");
            }

            int? exists = SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT 1 FROM cm50ddcode WHERE transitionDDCode = @code LIMIT 1", ("@code", transitionCode));

            if (!exists.HasValue)
            {
                throw new InvalidOperationException($"Codice Transizione DD '{transitionCode}' non trovato in nessun DD esistente.");
            }

            return transitionCode;
        }

        private static int EnsureReader(MySqlConnection connection, MySqlTransaction transaction, Cm50Reader readerCfg, List<string> newConfigurations, IWin32Window owner)
        {
            if (readerCfg == null)
            {
                throw new InvalidOperationException("Configurazione Reader mancante.");
            }

            if (readerCfg.IdReaderCfg > 0)
            {
                int? existingId = SqlHelper.ExecuteScalarInt(connection, transaction,
                    "SELECT idReaderCfg FROM cfg.cm50reader WHERE idReaderCfg = @id", ("@id", readerCfg.IdReaderCfg));
                if (existingId.HasValue)
                {
                    return existingId.Value;
                }
            }

            if (!ConfirmNewReference(readerCfg, "Nuova Configurazione Reader",
                "Questa Configurazione Reader non esiste nel database: completa i campi per crearla.", owner))
            {
                throw new OperationCanceledException("Creazione della nuova Configurazione Reader annullata dall'utente.");
            }

            int[] slots = readerCfg.SlotIds ?? new int[4];
            string[] names = readerCfg.BankNames ?? new string[16];
            string[] versions = readerCfg.BankVersions ?? new string[16];

            var parameters = new List<(string, object)>
            {
                ("@activeBank", readerCfg.ActiveBank),
                ("@slot0", slots.Length > 0 ? slots[0] : 0), ("@slot1", slots.Length > 1 ? slots[1] : 0),
                ("@slot2", slots.Length > 2 ? slots[2] : 0), ("@slot3", slots.Length > 3 ? slots[3] : 0)
            };

            var columns = new List<string> { "activeBank", "slot0id", "slot1id", "slot2id", "slot3id" };
            for (int i = 0; i < 16; i++)
            {
                columns.Add($"bank{i}Name");
                columns.Add($"bank{i}Ver");
                parameters.Add(($"@name{i}", i < names.Length ? names[i] : null));
                parameters.Add(($"@ver{i}", i < versions.Length ? versions[i] : null));
            }

            string columnList = string.Join(", ", columns);
            string valueList = string.Join(", ", parameters.ConvertAll(p => p.Item1));

            long newId = SqlHelper.ExecuteInsert(connection, transaction,
                $"INSERT INTO cfg.cm50reader ({columnList}) VALUES ({valueList})", parameters.ToArray());

            newConfigurations.Add(readerCfg.ToString());
            return (int)newId;
        }

        private static int EnsureCassette(MySqlConnection connection, MySqlTransaction transaction, Cm50Cassette cassette, List<string> newConfigurations, IWin32Window owner)
        {
            if (cassette == null)
            {
                throw new InvalidOperationException("Configurazione Cassette mancante.");
            }

            int? id = FindCassetteId(connection, transaction, cassette);
            if (id.HasValue)
            {
                return id.Value;
            }

            string cassetteCode = cassette.ToString();

            if (!ConfirmNewReference(cassette, "Nuova Configurazione Cassette",
                $"La Configurazione Cassette '{cassetteCode}' non esiste nel database: completa i 12 slot per crearla.", owner))
            {
                throw new OperationCanceledException($"Creazione della nuova Configurazione Cassette '{cassetteCode}' annullata dall'utente.");
            }

            // Se durante la conferma la combinazione è stata modificata in una già esistente, va usata quella.
            id = FindCassetteId(connection, transaction, cassette);
            if (id.HasValue)
            {
                return id.Value;
            }

            long newId = SqlHelper.ExecuteInsert(connection, transaction,
                "INSERT INTO cfg.cm50cassette (cassA, cassB, cassC, cassD, cassE, cassF, cassG, cassH, cassI, cassJ, cassK, cassL) " +
                "VALUES (@a, @b, @c, @d, @e, @f, @g, @h, @i, @j, @k, @l)",
                ("@a", cassette.CassA), ("@b", cassette.CassB), ("@c", cassette.CassC), ("@d", cassette.CassD),
                ("@e", cassette.CassE), ("@f", cassette.CassF), ("@g", cassette.CassG), ("@h", cassette.CassH),
                ("@i", cassette.CassI), ("@j", cassette.CassJ), ("@k", cassette.CassK), ("@l", cassette.CassL));

            newConfigurations.Add(cassetteCode);
            return (int)newId;
        }

        private static int? FindCassetteId(MySqlConnection connection, MySqlTransaction transaction, Cm50Cassette cassette)
        {
            return SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idCassetteCfg FROM cfg.cm50cassette WHERE " +
                "cassA=@a AND cassB=@b AND cassC=@c AND cassD=@d AND cassE=@e AND cassF=@f AND " +
                "cassG=@g AND cassH=@h AND cassI=@i AND cassJ=@j AND cassK=@k AND cassL=@l",
                ("@a", cassette.CassA), ("@b", cassette.CassB), ("@c", cassette.CassC), ("@d", cassette.CassD),
                ("@e", cassette.CassE), ("@f", cassette.CassF), ("@g", cassette.CassG), ("@h", cassette.CassH),
                ("@i", cassette.CassI), ("@j", cassette.CassJ), ("@k", cassette.CassK), ("@l", cassette.CassL));
        }

        // Ritorna true se è stata inserita una nuova riga, false se è stata aggiornata una esistente.
        private static bool UpsertCm50(MySqlConnection connection, MySqlTransaction transaction, Cm50 item,
            int idClient, string ddTransitionCode, int idUnitConfiguration, int idReaderCfg, int idCassetteCfg)
        {
            var parameters = new List<(string, object)>
            {
                ("@productCode", item.Product.Code), ("@idClient", idClient),
                ("@ddTransitionCode", ddTransitionCode), ("@ddCode", item.Dd.Code),
                ("@assetCode", item.Asset.Code), ("@cdfCode", item.Cdf.Code),
                ("@idUnitConfiguration", idUnitConfiguration), ("@idReaderCfg", idReaderCfg),
                ("@idCassetteCfg", idCassetteCfg), ("@alarmType", item.AlarmType)
            };

            bool exists = item.IdProduct > 0 && SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idProduct FROM cm50 WHERE idProduct = @id", ("@id", item.IdProduct)).HasValue;

            if (exists)
            {
                parameters.Add(("@id", item.IdProduct));
                SqlHelper.ExecuteNonQuery(connection, transaction,
                    "UPDATE cm50 SET " +
                    "productCode=@productCode, idClient=@idClient, DDtransitionCode=@ddTransitionCode, DDcode=@ddCode, " +
                    "ASSETcode=@assetCode, CDFcode=@cdfCode, idUnitConfiguration=@idUnitConfiguration, " +
                    "idReaderCfg=@idReaderCfg, idCassetteCfg=@idCassetteCfg, alarmType=@alarmType " +
                    "WHERE idProduct=@id",
                    parameters.ToArray());

                return false;
            }

            SqlHelper.ExecuteNonQuery(connection, transaction,
                "INSERT INTO cm50 " +
                "(productCode, idClient, DDtransitionCode, DDcode, ASSETcode, CDFcode, idUnitConfiguration, idReaderCfg, idCassetteCfg, alarmType) " +
                "VALUES " +
                "(@productCode, @idClient, @ddTransitionCode, @ddCode, @assetCode, @cdfCode, @idUnitConfiguration, @idReaderCfg, @idCassetteCfg, @alarmType)",
                parameters.ToArray());

            return true;
        }

        // Ritorna true se è stata inserita una nuova riga, false se è stata aggiornata una esistente.
        private static bool UpsertUserCustomization(MySqlConnection connection, MySqlTransaction transaction, Cm50UserCustomization item,
            int idProduct, int idClient, int idDD, int idAsset, int idUnitConfiguration, int idCdf)
        {
            string[] refBanks = item.RefBanks ?? new string[4];
            int[] fitnessBanks = item.FitnessBanks ?? new int[4];

            var parameters = new List<(string, object)>
            {
                ("@idProduct", idProduct), ("@idClient", idClient), ("@idDD", idDD), ("@idAsset", idAsset),
                ("@idUnitConfiguration", idUnitConfiguration), ("@idCdf", idCdf),
                ("@activeBanks", item.ActiveBanks), ("@bankConfig", item.BankConfig),
                ("@refBank0", refBanks.Length > 0 ? refBanks[0] : null),
                ("@refBank1", refBanks.Length > 1 ? refBanks[1] : null),
                ("@refBank2", refBanks.Length > 2 ? refBanks[2] : null),
                ("@refBank3", refBanks.Length > 3 ? refBanks[3] : null),
                ("@currency", item.Currency),
                ("@fitnessBank0", fitnessBanks.Length > 0 ? fitnessBanks[0] : 0),
                ("@fitnessBank1", fitnessBanks.Length > 1 ? fitnessBanks[1] : 0),
                ("@fitnessBank2", fitnessBanks.Length > 2 ? fitnessBanks[2] : 0),
                ("@fitnessBank3", fitnessBanks.Length > 3 ? fitnessBanks[3] : 0),
                ("@setCassConfig", item.SetCassConfig), ("@countryCode", item.CountryCode),
                ("@displayLanguage", item.DisplayLanguage), ("@systemRate", item.SystemRate), ("@ecoTimeOut", item.EcoTimeOut)
            };

            bool exists = item.IdUserCustomization > 0 && SqlHelper.ExecuteScalarInt(connection, transaction,
                "SELECT idUserCustomization FROM cm50usercustomization WHERE idUserCustomization = @id",
                ("@id", item.IdUserCustomization)).HasValue;

            if (exists)
            {
                parameters.Add(("@id", item.IdUserCustomization));
                SqlHelper.ExecuteNonQuery(connection, transaction,
                    "UPDATE cm50usercustomization SET " +
                    "idProduct=@idProduct, idClient=@idClient, idDD=@idDD, idAsset=@idAsset, idUnitConfiguration=@idUnitConfiguration, idCdf=@idCdf, " +
                    "activeBanks=@activeBanks, bankConfig=@bankConfig, refBank0=@refBank0, refBank1=@refBank1, refBank2=@refBank2, refBank3=@refBank3, " +
                    "currency=@currency, fitnessBank0=@fitnessBank0, fitnessBank1=@fitnessBank1, fitnessBank2=@fitnessBank2, fitnessBank3=@fitnessBank3, " +
                    "setCassConfig=@setCassConfig, countryCode=@countryCode, displayLanguage=@displayLanguage, systemRate=@systemRate, ecoTimeOut=@ecoTimeOut " +
                    "WHERE idUserCustomization=@id",
                    parameters.ToArray());

                return false;
            }

            SqlHelper.ExecuteNonQuery(connection, transaction,
                "INSERT INTO cm50usercustomization " +
                "(idProduct, idClient, idDD, idAsset, idUnitConfiguration, idCdf, activeBanks, bankConfig, " +
                "refBank0, refBank1, refBank2, refBank3, currency, fitnessBank0, fitnessBank1, fitnessBank2, fitnessBank3, " +
                "setCassConfig, countryCode, displayLanguage, systemRate, ecoTimeOut) " +
                "VALUES " +
                "(@idProduct, @idClient, @idDD, @idAsset, @idUnitConfiguration, @idCdf, @activeBanks, @bankConfig, " +
                "@refBank0, @refBank1, @refBank2, @refBank3, @currency, @fitnessBank0, @fitnessBank1, @fitnessBank2, @fitnessBank3, " +
                "@setCassConfig, @countryCode, @displayLanguage, @systemRate, @ecoTimeOut)",
                parameters.ToArray());

            return true;
        }

        // ExecuteScalarInt/ExecuteInsert/ExecuteNonQuery ora sono in SqlHelper (condivise con SimpleTableWriter).
    }
}

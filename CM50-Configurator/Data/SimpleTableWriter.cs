using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    // Descrive come leggere/scrivere una riga di tipo T su una tabella "semplice" (senza chiavi
    // esterne da verificare o creare): nome tabella/colonna id qualificati con lo schema, come
    // ottenere/impostare l'id, e l'elenco delle altre colonne con relativo accesso al valore.
    public class SimpleTableConfig<T>
    {
        public string TableName { get; set; }
        public string IdColumn { get; set; }
        public Func<T, int> GetId { get; set; }
        public Action<T, int> SetId { get; set; }
        public (string Column, Func<T, object> GetValue)[] Columns { get; set; }
    }

    // Upsert generico per le tabelle senza riferimenti (ddmcm50, utmcm50, ltmcm50, feedercm50):
    // niente da verificare/creare, quindi nessuna finestra di conferma per riga, a differenza di
    // DatabaseWriter. Ogni record viene scritto in una propria transazione.
    public class SimpleTableWriter
    {
        public ImportResult ImportAll<T>(List<T> items, SimpleTableConfig<T> config)
        {
            var result = new ImportResult();

            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();

                foreach (T item in items)
                {
                    result.Processed++;

                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            bool inserted = Upsert(connection, transaction, item, config);
                            transaction.Commit();

                            if (inserted) result.Inserted++;
                            else result.Updated++;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            result.Errors.Add($"Record #{config.GetId(item)} ({item}): {ex.Message}");
                        }
                    }
                }
            }

            return result;
        }

        private static bool Upsert<T>(MySqlConnection connection, MySqlTransaction transaction, T item, SimpleTableConfig<T> config)
        {
            int id = config.GetId(item);
            bool exists = id > 0 && SqlHelper.ExecuteScalarInt(connection, transaction,
                $"SELECT {config.IdColumn} FROM {config.TableName} WHERE {config.IdColumn} = @id", ("@id", id)).HasValue;

            var parameters = config.Columns
                .Select((c, i) => ($"@p{i}", c.GetValue(item)))
                .ToArray();

            if (exists)
            {
                string setClause = string.Join(", ", config.Columns.Select((c, i) => $"{c.Column}=@p{i}"));
                var updateParameters = parameters.Concat(new[] { ("@id", (object)id) }).ToArray();

                SqlHelper.ExecuteNonQuery(connection, transaction,
                    $"UPDATE {config.TableName} SET {setClause} WHERE {config.IdColumn}=@id", updateParameters);

                return false;
            }

            string columnList = string.Join(", ", config.Columns.Select(c => c.Column));
            string valueList = string.Join(", ", Enumerable.Range(0, config.Columns.Length).Select(i => $"@p{i}"));

            long newId = SqlHelper.ExecuteInsert(connection, transaction,
                $"INSERT INTO {config.TableName} ({columnList}) VALUES ({valueList})", parameters);

            config.SetId(item, (int)newId);
            return true;
        }
    }
}

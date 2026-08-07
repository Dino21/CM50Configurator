using System;
using MySqlConnector;

namespace CM50_Configurator.Data
{
    // Helper ADO.NET condivisi tra DatabaseWriter (tabelle con riferimenti da verificare/creare) e
    // SimpleTableWriter (tabelle semplici senza chiavi esterne).
    internal static class SqlHelper
    {
        public static int? ExecuteScalarInt(MySqlConnection connection, MySqlTransaction transaction, string sql, params (string Name, object Value)[] parameters)
        {
            using (var command = new MySqlCommand(sql, connection, transaction))
            {
                AddParameters(command, parameters);
                object result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? (int?)null : Convert.ToInt32(result);
            }
        }

        public static long ExecuteInsert(MySqlConnection connection, MySqlTransaction transaction, string sql, params (string Name, object Value)[] parameters)
        {
            using (var command = new MySqlCommand(sql, connection, transaction))
            {
                AddParameters(command, parameters);
                command.ExecuteNonQuery();
                return command.LastInsertedId;
            }
        }

        public static void ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, string sql, params (string Name, object Value)[] parameters)
        {
            using (var command = new MySqlCommand(sql, connection, transaction))
            {
                AddParameters(command, parameters);
                command.ExecuteNonQuery();
            }
        }

        private static void AddParameters(MySqlCommand command, (string Name, object Value)[] parameters)
        {
            foreach ((string name, object value) in parameters)
            {
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }
        }
    }
}

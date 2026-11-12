using System;
using System.Data;

namespace CM50_Configurator.Data
{
    internal static class DataReaderExtensions
    {
        public static string GetStringOrNull(this IDataRecord record, string column)
        {
            int ordinal = record.GetOrdinal(column);
            return record.IsDBNull(ordinal) ? null : record.GetString(ordinal);
        }

        public static int GetInt32OrDefault(this IDataRecord record, string column, int defaultValue = 0)
        {
            int ordinal = record.GetOrdinal(column);
            return record.IsDBNull(ordinal) ? defaultValue : record.GetInt32(ordinal);
        }

        public static bool GetBooleanOrDefault(this IDataRecord record, string column, bool defaultValue = false)
        {
            int ordinal = record.GetOrdinal(column);
            return record.IsDBNull(ordinal) ? defaultValue : Convert.ToBoolean(record.GetValue(ordinal));
        }

        // Varianti nullable: preservano il NULL del DB invece di sostituirlo con un default, così una
        // riga riscritta senza toccare quel campo non trasforma NULL in 0/false.
        public static int? GetInt32OrNull(this IDataRecord record, string column)
        {
            int ordinal = record.GetOrdinal(column);
            return record.IsDBNull(ordinal) ? (int?)null : record.GetInt32(ordinal);
        }

        public static bool? GetBooleanOrNull(this IDataRecord record, string column)
        {
            int ordinal = record.GetOrdinal(column);
            return record.IsDBNull(ordinal) ? (bool?)null : Convert.ToBoolean(record.GetValue(ordinal));
        }
    }
}

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
    }
}

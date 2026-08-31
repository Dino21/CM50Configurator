using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CM50_Configurator.Support
{
    // Filtro testuale generico usato dalle griglie principali (Form1, Cm50Form,
    // SimpleTableListForm<T>): un'unica casella di ricerca confronta il testo digitato con il
    // valore di ogni proprietà pubblica della riga, senza dover mappare colonna per colonna.
    internal static class GridFilter
    {
        public static List<T> Apply<T>(IEnumerable<T> items, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
            {
                return items.ToList();
            }

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return items.Where(item => properties.Any(p => MatchesFilter(p.GetValue(item), filterText))).ToList();
        }

        private static bool MatchesFilter(object value, string filterText)
        {
            return value != null && value.ToString().IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}

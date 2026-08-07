using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CM50_Configurator.Services
{
    // Come UserCustomizationDiff/Cm50Diff, ma generico: valido per qualunque tipo "piatto" senza
    // riferimenti annidati (ddmcm50, utmcm50, ltmcm50, feedercm50). Confronta tutte le proprietà
    // tranne quelle marcate [ReadOnly(true)] (convenzione già usata per le chiavi primarie).
    public static class SimpleEntityDiff
    {
        public class PendingChange<T>
        {
            public T Item { get; }
            public bool IsNew { get; }
            public List<string> ChangedFields { get; }

            public PendingChange(T item, bool isNew, List<string> changedFields)
            {
                Item = item;
                IsNew = isNew;
                ChangedFields = changedFields;
            }

            public string StatusLabel => IsNew ? "Nuovo" : "Modificato";
        }

        public static List<PendingChange<T>> ComputePendingChanges<T>(List<T> current, List<T> baseline, Func<T, int> getId)
        {
            Dictionary<int, T> baselineById = baseline
                .Where(b => getId(b) > 0)
                .ToDictionary(getId);

            var result = new List<PendingChange<T>>();

            foreach (T item in current)
            {
                int id = getId(item);
                if (id <= 0 || !baselineById.TryGetValue(id, out T original))
                {
                    result.Add(new PendingChange<T>(item, true, new List<string>()));
                    continue;
                }

                List<string> changes = GetChangedFields(original, item);
                if (changes.Count > 0)
                {
                    result.Add(new PendingChange<T>(item, false, changes));
                }
            }

            return result;
        }

        private static List<string> GetChangedFields<T>(T original, T current)
        {
            var changes = new List<string>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (property.Attributes[typeof(ReadOnlyAttribute)] is ReadOnlyAttribute readOnly && readOnly.IsReadOnly)
                {
                    continue;
                }

                object a = property.GetValue(original);
                object b = property.GetValue(current);
                if (!Equals(a, b))
                {
                    changes.Add(property.DisplayName);
                }
            }

            return changes;
        }
    }
}

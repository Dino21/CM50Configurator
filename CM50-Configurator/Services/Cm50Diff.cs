using System;
using System.Collections.Generic;
using System.Linq;
using CM50_Configurator.Models;

namespace CM50_Configurator.Services
{
    // Come UserCustomizationDiff, ma per la tabella cm50. Prodotto e Cliente non sono mai
    // modificabili quindi non vengono confrontati.
    public static class Cm50Diff
    {
        public class PendingChange
        {
            public Cm50 Item { get; }
            public bool IsNew { get; }
            public List<string> ChangedFields { get; }

            public PendingChange(Cm50 item, bool isNew, List<string> changedFields)
            {
                Item = item;
                IsNew = isNew;
                ChangedFields = changedFields;
            }

            public string StatusLabel => IsNew ? "Nuovo" : "Modificato";
        }

        public static List<PendingChange> ComputePendingChanges(List<Cm50> current, List<Cm50> baseline)
        {
            Dictionary<int, Cm50> baselineById = baseline
                .Where(b => b.IdProduct > 0)
                .ToDictionary(b => b.IdProduct);

            var result = new List<PendingChange>();

            foreach (Cm50 item in current)
            {
                if (item.IdProduct <= 0 || !baselineById.TryGetValue(item.IdProduct, out Cm50 original))
                {
                    result.Add(new PendingChange(item, true, new List<string>()));
                    continue;
                }

                List<string> changes = GetChangedFields(original, item);
                if (changes.Count > 0)
                {
                    result.Add(new PendingChange(item, false, changes));
                }
            }

            return result;
        }

        private static List<string> GetChangedFields(Cm50 original, Cm50 current)
        {
            var changes = new List<string>();

            if (!TextEquals(original.Dd?.Code, current.Dd?.Code)) changes.Add("DD");
            if (!TextEquals(original.DdTransitionCode, current.DdTransitionCode)) changes.Add("Codice Transizione DD");
            if (!TextEquals(original.Asset?.Code, current.Asset?.Code)) changes.Add("Asset");
            if (!TextEquals(original.Cdf?.Code, current.Cdf?.Code)) changes.Add("CDF");
            if (!TextEquals(original.UnitConfiguration?.ToString(), current.UnitConfiguration?.ToString())) changes.Add("Configurazione Unità");
            if (!TextEquals(original.Reader?.ToString(), current.Reader?.ToString())) changes.Add("Configurazione Reader");
            if (!TextEquals(original.Cassette?.ToString(), current.Cassette?.ToString())) changes.Add("Configurazione Cassette");
            if (!TextEquals(original.AlarmType, current.AlarmType)) changes.Add("Tipo Allarme");

            return changes;
        }

        private static bool TextEquals(string a, string b) => string.Equals(a ?? string.Empty, b ?? string.Empty, StringComparison.Ordinal);
    }
}

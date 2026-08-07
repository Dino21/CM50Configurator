using System;
using System.Collections.Generic;
using System.Linq;
using CM50_Configurator.Models;

namespace CM50_Configurator.Services
{
    // Confronta i dati correnti (in memoria) con l'istantanea presa al momento del caricamento dal
    // DB, per individuare le righe nuove e i campi modificati da mostrare nella finestra di revisione
    // prima della scrittura. Product e Cliente non sono mai modificabili, quindi non vengono confrontati.
    public static class UserCustomizationDiff
    {
        public class PendingChange
        {
            public Cm50UserCustomization Item { get; }
            public bool IsNew { get; }
            public List<string> ChangedFields { get; }

            public PendingChange(Cm50UserCustomization item, bool isNew, List<string> changedFields)
            {
                Item = item;
                IsNew = isNew;
                ChangedFields = changedFields;
            }

            public string StatusLabel => IsNew ? "Nuovo" : "Modificato";
        }

        public static List<PendingChange> ComputePendingChanges(List<Cm50UserCustomization> current, List<Cm50UserCustomization> baseline)
        {
            Dictionary<int, Cm50UserCustomization> baselineById = baseline
                .Where(b => b.IdUserCustomization > 0)
                .ToDictionary(b => b.IdUserCustomization);

            var result = new List<PendingChange>();

            foreach (Cm50UserCustomization item in current)
            {
                if (item.IdUserCustomization <= 0 || !baselineById.TryGetValue(item.IdUserCustomization, out Cm50UserCustomization original))
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

        private static List<string> GetChangedFields(Cm50UserCustomization original, Cm50UserCustomization current)
        {
            var changes = new List<string>();

            if (!TextEquals(original.Dd?.Code, current.Dd?.Code)) changes.Add("DD");
            if (!TextEquals(original.Asset?.Code, current.Asset?.Code)) changes.Add("Asset");
            if (!TextEquals(original.UnitConfiguration?.ToString(), current.UnitConfiguration?.ToString())) changes.Add("Configurazione Unità");
            if (!TextEquals(original.Cdf?.Code, current.Cdf?.Code)) changes.Add("CDF");

            if (original.ActiveBanks != current.ActiveBanks) changes.Add("ActiveBanks");
            if (!TextEquals(original.BankConfig, current.BankConfig)) changes.Add("BankConfig");
            if (!ArrayEquals(original.RefBanks, current.RefBanks)) changes.Add("Ref. Banche");
            if (!TextEquals(original.Currency, current.Currency)) changes.Add("Currency");
            if (!ArrayEquals(original.FitnessBanks, current.FitnessBanks)) changes.Add("Fitness Banche");
            if (!TextEquals(original.SetCassConfig, current.SetCassConfig)) changes.Add("Config. Cassetti");
            if (!TextEquals(original.CountryCode, current.CountryCode)) changes.Add("CountryCode");
            if (!TextEquals(original.DisplayLanguage, current.DisplayLanguage)) changes.Add("Lingua Display");
            if (original.SystemRate != current.SystemRate) changes.Add("SystemRate");
            if (original.EcoTimeOut != current.EcoTimeOut) changes.Add("EcoTimeOut");

            return changes;
        }

        private static bool TextEquals(string a, string b) => string.Equals(a ?? string.Empty, b ?? string.Empty, StringComparison.Ordinal);

        private static bool ArrayEquals<T>(T[] a, T[] b)
        {
            a = a ?? new T[0];
            b = b ?? new T[0];
            return a.SequenceEqual(b);
        }
    }
}

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CM50_Configurator.Models;

namespace CM50_Configurator.Support
{
    // Come UnitConfigurationConverter, ma per l'intero oggetto DD/Asset/CDF: permette di scegliere
    // una voce esistente dal DB (utile quando il campo parte nullo, es. "Nuova Configurazione"),
    // oppure di digitare direttamente un codice, nuovo o esistente. Restano comunque espandibili
    // per modificare i singoli campi (InternalName, ecc.) una volta valorizzati.
    public class DdReferenceConverter : OrderedExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                var dd = new Cm50DdCode { Code = text };
                ReferenceSync.SyncDd(dd);
                return dd;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value?.ToString() ?? string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new StandardValuesCollection(ReferenceDataCache.DdCodes.Select(x => x.Clone()).ToArray());
    }

    public class AssetReferenceConverter : OrderedExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                var asset = new Cm50AssetCode { Code = text };
                ReferenceSync.SyncAsset(asset);
                return asset;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value?.ToString() ?? string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new StandardValuesCollection(ReferenceDataCache.AssetCodes.Select(x => x.Clone()).ToArray());
    }

    public class CdfReferenceConverter : OrderedExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                var cdf = new Cm50CdfCode { Code = text };
                ReferenceSync.SyncCdf(cdf);
                return cdf;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value?.ToString() ?? string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new StandardValuesCollection(ReferenceDataCache.CdfCodes.Select(x => x.Clone()).ToArray());
    }

    // Come UnitConfigurationConverter: la Cassette ha una identità testuale compatta (12 slot uniti
    // da "-"), quindi si può sia scegliere dall'elenco sia digitarla direttamente.
    public class CassetteReferenceConverter : OrderedExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                string[] parts = text.Split('-');
                var cassette = new Cm50Cassette
                {
                    CassA = parts.Length > 0 ? parts[0] : string.Empty,
                    CassB = parts.Length > 1 ? parts[1] : string.Empty,
                    CassC = parts.Length > 2 ? parts[2] : string.Empty,
                    CassD = parts.Length > 3 ? parts[3] : string.Empty,
                    CassE = parts.Length > 4 ? parts[4] : string.Empty,
                    CassF = parts.Length > 5 ? parts[5] : string.Empty,
                    CassG = parts.Length > 6 ? parts[6] : string.Empty,
                    CassH = parts.Length > 7 ? parts[7] : string.Empty,
                    CassI = parts.Length > 8 ? parts[8] : string.Empty,
                    CassJ = parts.Length > 9 ? parts[9] : string.Empty,
                    CassK = parts.Length > 10 ? parts[10] : string.Empty,
                    CassL = parts.Length > 11 ? parts[11] : string.Empty
                };

                ReferenceSync.SyncCassette(cassette);
                return cassette;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value?.ToString() ?? string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new StandardValuesCollection(ReferenceDataCache.Cassettes.Select(x => x.Clone()).ToArray());
    }

    // Il Reader non ha un'identità testuale compatta (37 campi): si può solo scegliere dall'elenco
    // esistente, oppure una voce vuota ("Nuovo Reader") da compilare espandendo il nodo.
    public class ReaderReferenceConverter : OrderedExpandableObjectConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            Cm50Reader[] existing = ReferenceDataCache.Readers.Select(x => x.Clone()).ToArray();
            var blank = new Cm50Reader { SlotIds = new int[4], BankNames = new string[16], BankVersions = new string[16] };
            return new StandardValuesCollection(existing.Concat(new[] { blank }).ToArray());
        }
    }

    // Prodotto e Cliente (es. su cm50excqa) si possono solo scegliere tra quelli esistenti, mai
    // creare: dropdown esclusivo dal ReferenceDataCache. La conversione da stringa (usata anche da
    // BulkCodeReplaceForm) accetta sia il testo mostrato in griglia sia il solo codice/nome, e
    // rifiuta un valore che non corrisponde a nessuna voce caricata dal DB.
    public abstract class SelectOnlyReferenceConverter<T> : OrderedExpandableObjectConverter where T : class
    {
        protected abstract T[] GetItems();

        protected abstract string GetKey(T item);

        protected abstract string EntityLabel { get; }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                // Il testo da confrontare arriva dalla PropertyGrid (anche quando si sceglie dal
                // dropdown: la griglia riconverte il testo mostrato) ma può differire da ToString()
                // per spazi/a capo: es. una descrizione prodotto salvata nel DB con "\r\n" finale, che
                // la griglia restituisce in modo diverso. Si confronta quindi tutto normalizzato.
                string trimmed = Normalize(text);
                T match = GetItems().FirstOrDefault(x =>
                    string.Equals(Normalize(GetKey(x)), trimmed, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(Normalize(x.ToString()), trimmed, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    throw new FormatException(
                        $"{EntityLabel} '{trimmed}' non trovato tra quelli caricati dal database (esegui prima \"Carica da DB\").");
                }

                return match;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value?.ToString() ?? string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        // Spazi, tab e a capo (anche multipli, ovunque nel testo) ridotti a un singolo spazio, e
        // spazi iniziali/finali rimossi.
        private static string Normalize(string value)
            => value == null ? null : Regex.Replace(value, @"\s+", " ").Trim();

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new StandardValuesCollection(GetItems());
    }

    public class ProductReferenceConverter : SelectOnlyReferenceConverter<Cm50ProdCode>
    {
        protected override Cm50ProdCode[] GetItems() => ReferenceDataCache.Products;

        protected override string GetKey(Cm50ProdCode item) => item.Code;

        protected override string EntityLabel => "Prodotto";
    }

    public class ClientReferenceConverter : SelectOnlyReferenceConverter<Client>
    {
        protected override Client[] GetItems() => ReferenceDataCache.Clients;

        protected override string GetKey(Client item) => item.ClientName;

        protected override string EntityLabel => "Cliente";
    }
}

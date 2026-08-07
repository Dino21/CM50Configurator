using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using CM50_Configurator.Models;

namespace CM50_Configurator.Support
{
    // Estende l'ordinamento delle proprietà (per poter comunque espandere e modificare i singoli
    // campi cfg/optCfg/...) aggiungendo la possibilità di scegliere un'intera configurazione tra
    // quelle già presenti nel DB, oppure digitarne una nuova nel formato "0801-0000-0800-...-...".
    public class UnitConfigurationConverter : OrderedExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                string[] parts = text.Split('-');
                var result = new Cm50UnitConfiguration
                {
                    Cfg = parts.Length > 0 ? parts[0] : string.Empty,
                    OptCfg = parts.Length > 1 ? parts[1] : string.Empty,
                    OptOneCfg = parts.Length > 2 ? parts[2] : string.Empty,
                    OptTwoCfg = parts.Length > 3 ? parts[3] : string.Empty,
                    OptThreeCfg = parts.Length > 4 ? parts[4] : string.Empty
                };

                // Se la combinazione digitata corrisponde a una configurazione esistente, recupera
                // Id/Descrizione da quella; altrimenti restano vuoti a indicare una voce nuova.
                ReferenceSync.SyncUnitConfiguration(result);
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

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
        {
            // Cloni, non i riferimenti diretti alla cache: se l'utente modifica un campo dopo la
            // selezione (es. la Descrizione), non deve alterare il record condiviso in cache.
            return new StandardValuesCollection(ReferenceDataCache.UnitConfigurations.Select(x => x.Clone()).ToArray());
        }
    }
}

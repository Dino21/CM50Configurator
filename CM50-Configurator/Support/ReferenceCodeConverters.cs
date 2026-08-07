using System.ComponentModel;
using System.Linq;

namespace CM50_Configurator.Support
{
    // Converter per campi stringa "a codice" (DD, Asset, CDF): propone in dropdown i codici già
    // presenti nella relativa tabella del DB, ma GetStandardValuesExclusive=false lascia comunque
    // scrivere un codice non esistente, interpretato come nuovo in fase di importazione.
    public abstract class StandardValuesStringConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetValues());
        }

        protected abstract string[] GetValues();
    }

    public class DdCodeConverter : StandardValuesStringConverter
    {
        protected override string[] GetValues() => ReferenceDataCache.DdCodes.Select(x => x.Code).ToArray();
    }

    public class AssetCodeConverter : StandardValuesStringConverter
    {
        protected override string[] GetValues() => ReferenceDataCache.AssetCodes.Select(x => x.Code).ToArray();
    }

    public class CdfCodeConverter : StandardValuesStringConverter
    {
        protected override string[] GetValues() => ReferenceDataCache.CdfCodes.Select(x => x.Code).ToArray();
    }

    // Usato da Cm50.DdTransitionCode: propone i valori di transitionDDCode già presenti tra i DD
    // esistenti (non necessariamente lo stesso DD scelto per il campo "DD" della stessa riga).
    public class DdTransitionCodeConverter : StandardValuesStringConverter
    {
        protected override string[] GetValues() => ReferenceDataCache.DdCodes
            .Select(x => x.TransitionDDCode)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToArray();
    }
}

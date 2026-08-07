using System.ComponentModel;

namespace CM50_Configurator.Support
{
    // Cerca il primo campo marcato [RequiredField] non valorizzato su un oggetto qualsiasi.
    // Usato sia da NewReferenceForm (nuovo DD/Asset/CDF/Cassette/Reader/Configurazione Unità)
    // sia dalle finestre di creazione di una nuova riga (es. NewCm50Form).
    public static class RequiredFieldValidator
    {
        public static string FindMissingRequiredField(object item)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(item))
            {
                if (!(property.Attributes[typeof(RequiredFieldAttribute)] is RequiredFieldAttribute required))
                {
                    continue;
                }

                object value = property.GetValue(item);
                if (value == null || (value is string text && string.IsNullOrWhiteSpace(text)))
                {
                    return required.Message ?? $"Il campo '{property.DisplayName}' è obbligatorio.";
                }
            }

            return null;
        }
    }
}

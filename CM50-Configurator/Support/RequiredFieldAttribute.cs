using System;

namespace CM50_Configurator.Support
{
    // Segnala nella finestra "nuovo elemento di riferimento" i campi che il DB richiede
    // obbligatoriamente per poter creare la riga (es. transitionDDCode su cm50ddcode).
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredFieldAttribute : Attribute
    {
        public string Message { get; }

        public RequiredFieldAttribute(string message = null)
        {
            Message = message;
        }
    }
}

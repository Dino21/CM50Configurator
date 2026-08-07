using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50CdfCode
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdCdf { get; set; }

        [PropertyOrder(1)]
        [TypeConverter(typeof(CdfCodeConverter))]
        public string Code { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Nome Interno")]
        public string InternalName { get; set; }

        // currCode0..currCode15 della tabella, mappati per indice (CurrCodes[0] = currCode0, ecc.)
        [PropertyOrder(3)]
        [DisplayName("Codici Valuta")]
        public string[] CurrCodes { get; set; }

        public override string ToString() => Code;

        public Cm50CdfCode Clone() => new Cm50CdfCode
        {
            IdCdf = IdCdf,
            Code = Code,
            InternalName = InternalName,
            CurrCodes = (string[])CurrCodes?.Clone()
        };
    }
}

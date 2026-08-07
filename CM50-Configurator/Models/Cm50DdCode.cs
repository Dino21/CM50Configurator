using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50DdCode
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdDD { get; set; }

        [PropertyOrder(1)]
        [TypeConverter(typeof(DdCodeConverter))]
        public string Code { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Nome Interno")]
        public string InternalName { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Codice Transizione")]
        [RequiredField("Il 'Codice Transizione' è obbligatorio per creare un nuovo DD.")]
        public string TransitionDDCode { get; set; }

        [PropertyOrder(4)]
        [DisplayName("File Transizione")]
        public string TransitionFilename { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Versione ASIS")]
        public string ASISver { get; set; }

        public override string ToString() => Code;

        public Cm50DdCode Clone() => new Cm50DdCode
        {
            IdDD = IdDD,
            Code = Code,
            InternalName = InternalName,
            TransitionDDCode = TransitionDDCode,
            TransitionFilename = TransitionFilename,
            ASISver = ASISver
        };
    }
}

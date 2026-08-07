using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // cfg.ltmcm50: configurazione di collaudo del Trasporto Inferiore (Lower Transport Module).
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class LtmCm50
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdLTM { get; set; }

        [PropertyOrder(1)]
        public string Code { get; set; }

        [PropertyOrder(2)]
        [DisplayName("N. Cassette")]
        public int CassetteNumber { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Tipo Gabbia")]
        public string CageType { get; set; }

        [PropertyOrder(4)]
        [DisplayName("File FW Safe")]
        public string SafeFwFileName { get; set; }

        [PropertyOrder(5)]
        [DisplayName("File FW Allarme")]
        public string AlarmFwFileName { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Codice FW")]
        public string FwCode { get; set; }

        public override string ToString() => Code;

        public LtmCm50 Clone() => new LtmCm50
        {
            IdLTM = IdLTM,
            Code = Code,
            CassetteNumber = CassetteNumber,
            CageType = CageType,
            SafeFwFileName = SafeFwFileName,
            AlarmFwFileName = AlarmFwFileName,
            FwCode = FwCode
        };
    }
}

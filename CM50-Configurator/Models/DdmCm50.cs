using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // cfg.ddmcm50: configurazione di collaudo del Cassetto (Double Drum Module). Nessuna chiave
    // esterna: tutti i campi sono nullable nel DB, quindi liberamente modificabili/vuoti.
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class DdmCm50
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdDDM { get; set; }

        [PropertyOrder(1)]
        public string Code { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Codice FW")]
        public string FwCode { get; set; }

        [PropertyOrder(3)]
        [DisplayName("File FW Cassetto")]
        public string CassFwFilename { get; set; }

        [PropertyOrder(4)]
        [DisplayName("File FW Basc")]
        public string BascFwFilename { get; set; }

        [PropertyOrder(5)]
        public string Type { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Abilita Feedback Sorter")]
        public bool EnableFeedbackSorter { get; set; }

        public override string ToString() => Code;

        public DdmCm50 Clone() => new DdmCm50
        {
            IdDDM = IdDDM,
            Code = Code,
            FwCode = FwCode,
            CassFwFilename = CassFwFilename,
            BascFwFilename = BascFwFilename,
            Type = Type,
            EnableFeedbackSorter = EnableFeedbackSorter
        };
    }
}

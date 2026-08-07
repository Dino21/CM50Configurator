using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // cfg.utmcm50: configurazione di collaudo del Trasporto Superiore (Upper Transport Module).
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class UtmCm50
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdCfg { get; set; }

        [PropertyOrder(1)]
        public string Code { get; set; }

        [PropertyOrder(2)]
        public string Model { get; set; }

        [PropertyOrder(3)]
        public string Bidirectional { get; set; }

        [PropertyOrder(4)]
        [DisplayName("SNR")]
        public string Snr { get; set; }

        [PropertyOrder(5)]
        [DisplayName("UIM")]
        public string Uim { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Alta Velocità")]
        public string HighSpeed { get; set; }

        [PropertyOrder(7)]
        [DisplayName("Codice FW")]
        public string FwCode { get; set; }

        [PropertyOrder(8)]
        [DisplayName("File FW RTC")]
        public string RtcFwFilename { get; set; }

        [PropertyOrder(9)]
        [DisplayName("File FW PH01")]
        public string Ph01FwFilename { get; set; }

        [PropertyOrder(10)]
        [DisplayName("File FW PH02")]
        public string Ph02FwFilename { get; set; }

        [PropertyOrder(11)]
        [DisplayName("File FW Safe")]
        public string SafeFwFilename { get; set; }

        [PropertyOrder(12)]
        [DisplayName("File FW UI")]
        public string UiFwFilename { get; set; }

        [PropertyOrder(13)]
        [DisplayName("Modalità Debug")]
        public string DebugMode { get; set; }

        public override string ToString() => Code;

        public UtmCm50 Clone() => new UtmCm50
        {
            IdCfg = IdCfg,
            Code = Code,
            Model = Model,
            Bidirectional = Bidirectional,
            Snr = Snr,
            Uim = Uim,
            HighSpeed = HighSpeed,
            FwCode = FwCode,
            RtcFwFilename = RtcFwFilename,
            Ph01FwFilename = Ph01FwFilename,
            Ph02FwFilename = Ph02FwFilename,
            SafeFwFilename = SafeFwFilename,
            UiFwFilename = UiFwFilename,
            DebugMode = DebugMode
        };
    }
}

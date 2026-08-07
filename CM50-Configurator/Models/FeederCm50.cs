using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // cfg.feedercm50: configurazione di collaudo del Feeder.
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class FeederCm50
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdCfg { get; set; }

        [PropertyOrder(1)]
        public string Code { get; set; }

        [PropertyOrder(2)]
        [DisplayName("TL")]
        public string TL { get; set; }

        [PropertyOrder(3)]
        [DisplayName("File FW RTC")]
        public string FwRTC { get; set; }

        [PropertyOrder(4)]
        [DisplayName("File FW PH02")]
        public string FwPH02 { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Nome CDF")]
        public string CdfName { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Modalità Debug")]
        public int DebugMode { get; set; }

        [PropertyOrder(7)]
        [DisplayName("Jump PH Adj")]
        public int JumpPhAdj { get; set; }

        public override string ToString() => Code;

        public FeederCm50 Clone() => new FeederCm50
        {
            IdCfg = IdCfg,
            Code = Code,
            TL = TL,
            FwRTC = FwRTC,
            FwPH02 = FwPH02,
            CdfName = CdfName,
            DebugMode = DebugMode,
            JumpPhAdj = JumpPhAdj
        };
    }
}

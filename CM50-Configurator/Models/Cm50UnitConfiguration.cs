using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50UnitConfiguration
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdUnitConfiguration { get; set; }

        [PropertyOrder(1)]
        public string Cfg { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Opt Cfg")]
        public string OptCfg { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Opt1 Cfg")]
        public string OptOneCfg { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Opt2 Cfg")]
        public string OptTwoCfg { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Opt3 Cfg")]
        public string OptThreeCfg { get; set; }

        [PropertyOrder(6)]
        public string Description { get; set; }

        public override string ToString() => string.Join("-", Cfg, OptCfg, OptOneCfg, OptTwoCfg, OptThreeCfg);

        public Cm50UnitConfiguration Clone() => new Cm50UnitConfiguration
        {
            IdUnitConfiguration = IdUnitConfiguration,
            Cfg = Cfg,
            OptCfg = OptCfg,
            OptOneCfg = OptOneCfg,
            OptTwoCfg = OptTwoCfg,
            OptThreeCfg = OptThreeCfg,
            Description = Description
        };
    }
}

using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50AssetCode
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdAsset { get; set; }

        [PropertyOrder(1)]
        [TypeConverter(typeof(AssetCodeConverter))]
        public string Code { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Nome Interno")]
        public string InternalName { get; set; }

        [PropertyOrder(3)]
        [DisplayName("File Short")]
        public string ShortFilename { get; set; }

        [PropertyOrder(4)]
        [DisplayName("File Tall")]
        public string TallFilename { get; set; }

        [PropertyOrder(5)]
        public string Version { get; set; }

        [PropertyOrder(6)]
        public string Release { get; set; }

        [PropertyOrder(7)]
        public string Serial { get; set; }

        public override string ToString() => Code;

        public Cm50AssetCode Clone() => new Cm50AssetCode
        {
            IdAsset = IdAsset,
            Code = Code,
            InternalName = InternalName,
            ShortFilename = ShortFilename,
            TallFilename = TallFilename,
            Version = Version,
            Release = Release,
            Serial = Serial
        };
    }
}

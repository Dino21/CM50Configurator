using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50UserCustomization
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        [Category("5 - Identificativo")]
        public int IdUserCustomization { get; set; }

        [PropertyOrder(1)]
        [Category("1 - Riferimenti")]
        public Cm50ProdCode Product { get; set; }

        [PropertyOrder(2)]
        [Category("1 - Riferimenti")]
        public Client Client { get; set; }

        [PropertyOrder(3)]
        [Category("1 - Riferimenti")]
        [DisplayName("DD")]
        [TypeConverter(typeof(DdReferenceConverter))]
        public Cm50DdCode Dd { get; set; }

        [PropertyOrder(4)]
        [Category("1 - Riferimenti")]
        [TypeConverter(typeof(AssetReferenceConverter))]
        public Cm50AssetCode Asset { get; set; }

        [PropertyOrder(5)]
        [Category("1 - Riferimenti")]
        [DisplayName("Configurazione Unità")]
        [TypeConverter(typeof(UnitConfigurationConverter))]
        public Cm50UnitConfiguration UnitConfiguration { get; set; }

        [PropertyOrder(6)]
        [Category("1 - Riferimenti")]
        [DisplayName("CDF")]
        [TypeConverter(typeof(CdfReferenceConverter))]
        public Cm50CdfCode Cdf { get; set; }

        [PropertyOrder(7)]
        [Category("2 - Banconote")]
        public int ActiveBanks { get; set; }

        [PropertyOrder(8)]
        [Category("2 - Banconote")]
        public string BankConfig { get; set; }

        // refBank0..refBank3
        [PropertyOrder(9)]
        [Category("2 - Banconote")]
        [DisplayName("Ref. Banks")]
        public string[] RefBanks { get; set; }

        [PropertyOrder(10)]
        [Category("2 - Banconote")]
        public string Currency { get; set; }

        // fitnessBank0..fitnessBank3
        [PropertyOrder(11)]
        [Category("2 - Banconote")]
        [DisplayName("Fitness Banks")]
        public int[] FitnessBanks { get; set; }

        [PropertyOrder(12)]
        [Category("2 - Banconote")]
        [DisplayName("Config. Cassetti")]
        public string SetCassConfig { get; set; }

        [PropertyOrder(13)]
        [Category("3 - Localizzazione")]
        public string CountryCode { get; set; }

        [PropertyOrder(14)]
        [Category("3 - Localizzazione")]
        [DisplayName("Lingua Display")]
        public string DisplayLanguage { get; set; }

        [PropertyOrder(15)]
        [Category("4 - Varie")]
        public int SystemRate { get; set; }

        [PropertyOrder(16)]
        [Category("4 - Varie")]
        [DisplayName("Eco TimeOut")]
        public int EcoTimeOut { get; set; }

        // Usato per lo snapshot "come caricato dal DB" con cui confrontare le modifiche in corso
        // (vedi UserCustomizationDiff). Product e Client sono condivisi per riferimento: sono
        // sempre di sola lettura in ogni PropertyGrid dell'app, quindi non serve clonarli.
        public Cm50UserCustomization Clone() => new Cm50UserCustomization
        {
            IdUserCustomization = IdUserCustomization,
            Product = Product,
            Client = Client,
            Dd = Dd?.Clone(),
            Asset = Asset?.Clone(),
            UnitConfiguration = UnitConfiguration?.Clone(),
            Cdf = Cdf?.Clone(),
            ActiveBanks = ActiveBanks,
            BankConfig = BankConfig,
            RefBanks = (string[])RefBanks?.Clone(),
            Currency = Currency,
            FitnessBanks = (int[])FitnessBanks?.Clone(),
            SetCassConfig = SetCassConfig,
            CountryCode = CountryCode,
            DisplayLanguage = DisplayLanguage,
            SystemRate = SystemRate,
            EcoTimeOut = EcoTimeOut
        };
    }
}

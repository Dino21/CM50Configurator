using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // Tabella cm50: configurazione "master" per codice prodotto (a differenza di
    // cm50usercustomization, che è la personalizzazione per singola installazione).
    // A differenza di quella tabella, qui tutte le colonne di riferimento sono NOT NULL nel DB:
    // Prodotto, Cliente, DD, Codice Transizione DD, Asset, CDF, Configurazione Unità, Reader,
    // Cassette e Tipo Allarme sono quindi tutti obbligatori per creare una nuova riga.
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        [Category("6 - Identificativo")]
        public int IdProduct { get; set; }

        [PropertyOrder(1)]
        [Category("1 - Riferimenti")]
        [RequiredField("Il Prodotto è obbligatorio.")]
        public Cm50ProdCode Product { get; set; }

        [PropertyOrder(2)]
        [Category("1 - Riferimenti")]
        [RequiredField("Il Cliente è obbligatorio.")]
        public Client Client { get; set; }

        [PropertyOrder(3)]
        [Category("1 - Riferimenti")]
        [DisplayName("DD")]
        [TypeConverter(typeof(DdReferenceConverter))]
        [RequiredField("Il DD è obbligatorio.")]
        public Cm50DdCode Dd { get; set; }

        [PropertyOrder(4)]
        [Category("1 - Riferimenti")]
        [DisplayName("Codice Transizione DD")]
        [TypeConverter(typeof(DdTransitionCodeConverter))]
        [RequiredField("Il Codice Transizione DD è obbligatorio.")]
        public string DdTransitionCode { get; set; }

        [PropertyOrder(5)]
        [Category("1 - Riferimenti")]
        [TypeConverter(typeof(AssetReferenceConverter))]
        [RequiredField("L'Asset è obbligatorio.")]
        public Cm50AssetCode Asset { get; set; }

        [PropertyOrder(6)]
        [Category("1 - Riferimenti")]
        [DisplayName("CDF")]
        [TypeConverter(typeof(CdfReferenceConverter))]
        [RequiredField("Il CDF è obbligatorio.")]
        public Cm50CdfCode Cdf { get; set; }

        [PropertyOrder(7)]
        [Category("1 - Riferimenti")]
        [DisplayName("Configurazione Unità")]
        [TypeConverter(typeof(UnitConfigurationConverter))]
        [RequiredField("La Configurazione Unità è obbligatoria.")]
        public Cm50UnitConfiguration UnitConfiguration { get; set; }

        [PropertyOrder(8)]
        [Category("1 - Riferimenti")]
        [DisplayName("Configurazione Reader")]
        [TypeConverter(typeof(ReaderReferenceConverter))]
        [RequiredField("La Configurazione Reader è obbligatoria.")]
        public Cm50Reader Reader { get; set; }

        [PropertyOrder(9)]
        [Category("1 - Riferimenti")]
        [DisplayName("Configurazione Cassette")]
        [TypeConverter(typeof(CassetteReferenceConverter))]
        [RequiredField("La Configurazione Cassette è obbligatoria.")]
        public Cm50Cassette Cassette { get; set; }

        [PropertyOrder(10)]
        [Category("5 - Varie")]
        [DisplayName("Tipo Allarme")]
        [RequiredField("Il Tipo Allarme è obbligatorio.")]
        public string AlarmType { get; set; }

        public Cm50 Clone() => new Cm50
        {
            IdProduct = IdProduct,
            Product = Product,
            Client = Client,
            Dd = Dd?.Clone(),
            DdTransitionCode = DdTransitionCode,
            Asset = Asset?.Clone(),
            Cdf = Cdf?.Clone(),
            UnitConfiguration = UnitConfiguration?.Clone(),
            Reader = Reader?.Clone(),
            Cassette = Cassette?.Clone(),
            AlarmType = AlarmType
        };
    }
}

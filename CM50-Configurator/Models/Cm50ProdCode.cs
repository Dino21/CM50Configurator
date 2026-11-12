using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // Il prodotto non è modificabile dalla finestra di dettaglio: tutti i campi sono di sola lettura
    // per evitare che una modifica accidentale generi un'esportazione incoerente.
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50ProdCode
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdProductCode { get; set; }

        [PropertyOrder(1)]
        [ReadOnly(true)]
        public string Code { get; set; }

        [PropertyOrder(2)]
        [ReadOnly(true)]
        public string Description { get; set; }

        [PropertyOrder(3)]
        [ReadOnly(true)]
        [DisplayName("N. Cassette")]
        public int CassetteNumber { get; set; }

        [PropertyOrder(4)]
        [ReadOnly(true)]
        [DisplayName("Tipo Macchina")]
        public string MachineType { get; set; }

        [PropertyOrder(5)]
        [ReadOnly(true)]
        [DisplayName("Nome Cassetta")]
        public string CassetteName { get; set; }

        [PropertyOrder(6)]
        [ReadOnly(true)]
        [DisplayName("Blocco Superiore")]
        public string UpperLock { get; set; }

        [PropertyOrder(7)]
        [ReadOnly(true)]
        [DisplayName("Blocco Inferiore")]
        public string LowerLock { get; set; }

        [PropertyOrder(8)]
        [ReadOnly(true)]
        [DisplayName("Stazione Lettura")]
        public string ReadingStation { get; set; }

        [PropertyOrder(9)]
        [ReadOnly(true)]
        [DisplayName("Lettore Barcode")]
        public string BarcodeReader { get; set; }

        [PropertyOrder(10)]
        [ReadOnly(true)]
        [DisplayName("Tipo Allarme")]
        public string AlarmType { get; set; }

        // Identità per id: serve a SimpleEntityDiff (che confronta con Equals) per non segnalare come
        // modificata una riga solo perché il riferimento è un'istanza diversa con lo stesso id (es.
        // scelto di nuovo dal dropdown, o deserializzato da un JSON importato).
        public override bool Equals(object obj) => obj is Cm50ProdCode other && other.IdProductCode == IdProductCode;

        public override int GetHashCode() => IdProductCode;

        public override string ToString() => $"{Code} - {Description}";
    }
}

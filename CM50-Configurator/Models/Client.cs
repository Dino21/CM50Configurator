using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Client
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdClient { get; set; }

        [PropertyOrder(1)]
        [ReadOnly(true)]
        [DisplayName("Nome Cliente")]
        public string ClientName { get; set; }

        // Identità per id: serve a SimpleEntityDiff (che confronta con Equals) per non segnalare come
        // modificata una riga solo perché il riferimento è un'istanza diversa con lo stesso id (es.
        // scelto di nuovo dal dropdown, o deserializzato da un JSON importato).
        public override bool Equals(object obj) => obj is Client other && other.IdClient == IdClient;

        public override int GetHashCode() => IdClient;

        public override string ToString() => ClientName;
    }
}

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

        public override string ToString() => ClientName;
    }
}

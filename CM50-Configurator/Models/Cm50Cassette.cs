using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // cfg.cm50cassette: i 12 slot (A-L) sono tutti NOT NULL nel DB, quindi obbligatori per crearne
    // uno nuovo. Come Cm50UnitConfiguration, l'identità è la combinazione dei 12 valori: si può
    // selezionare dall'elenco esistente o digitarla nel formato "slotA-slotB-...-slotL".
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50Cassette
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdCassetteCfg { get; set; }

        [PropertyOrder(1)] [RequiredField("Lo slot A è obbligatorio.")] public string CassA { get; set; }
        [PropertyOrder(2)] [RequiredField("Lo slot B è obbligatorio.")] public string CassB { get; set; }
        [PropertyOrder(3)] [RequiredField("Lo slot C è obbligatorio.")] public string CassC { get; set; }
        [PropertyOrder(4)] [RequiredField("Lo slot D è obbligatorio.")] public string CassD { get; set; }
        [PropertyOrder(5)] [RequiredField("Lo slot E è obbligatorio.")] public string CassE { get; set; }
        [PropertyOrder(6)] [RequiredField("Lo slot F è obbligatorio.")] public string CassF { get; set; }
        [PropertyOrder(7)] [RequiredField("Lo slot G è obbligatorio.")] public string CassG { get; set; }
        [PropertyOrder(8)] [RequiredField("Lo slot H è obbligatorio.")] public string CassH { get; set; }
        [PropertyOrder(9)] [RequiredField("Lo slot I è obbligatorio.")] public string CassI { get; set; }
        [PropertyOrder(10)] [RequiredField("Lo slot J è obbligatorio.")] public string CassJ { get; set; }
        [PropertyOrder(11)] [RequiredField("Lo slot K è obbligatorio.")] public string CassK { get; set; }
        [PropertyOrder(12)] [RequiredField("Lo slot L è obbligatorio.")] public string CassL { get; set; }

        private string[] AllSlots => new[] { CassA, CassB, CassC, CassD, CassE, CassF, CassG, CassH, CassI, CassJ, CassK, CassL };

        // "Nuova" indica che la riga non esiste ancora nel DB (IdCassetteCfg non assegnato), non che
        // qualche slot sia vuoto: alcuni cassetti hanno legittimamente meno di 12 slot in uso, con i
        // restanti '' (non NULL, ma vuoti) nel DB.
        public override string ToString() => IdCassetteCfg > 0 ? string.Join("-", AllSlots) : "(Nuova)";

        public Cm50Cassette Clone() => new Cm50Cassette
        {
            IdCassetteCfg = IdCassetteCfg,
            CassA = CassA, CassB = CassB, CassC = CassC, CassD = CassD,
            CassE = CassE, CassF = CassF, CassG = CassG, CassH = CassH,
            CassI = CassI, CassJ = CassJ, CassK = CassK, CassL = CassL
        };
    }
}

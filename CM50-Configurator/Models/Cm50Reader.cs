using System.ComponentModel;
using System.Linq;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // cfg.cm50reader: tutti i campi oltre alla chiave sono nullable nel DB, quindi nessuno è
    // obbligatorio per crearne uno nuovo (a differenza di Cm50Cassette). Non ha un "codice"
    // naturale: la selezione avviene solo dall'elenco esistente (vedi ReaderReferenceConverter).
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50Reader
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        public int IdReaderCfg { get; set; }

        [PropertyOrder(1)]
        [DisplayName("Banca Attiva")]
        public int ActiveBank { get; set; }

        // slot0id..slot3id
        [PropertyOrder(2)]
        [DisplayName("Slot")]
        public int[] SlotIds { get; set; }

        // bank0Name..bank15Name
        [PropertyOrder(3)]
        [DisplayName("Nomi Banche")]
        public string[] BankNames { get; set; }

        // bank0Ver..bank15Ver
        [PropertyOrder(4)]
        [DisplayName("Versioni Banche")]
        public string[] BankVersions { get; set; }

        // Mostra i nomi banca effettivamente valorizzati; se non ce ne sono (es. reader vuoto),
        // ricade sull'id se la riga esiste già nel DB, altrimenti "(Nuovo Reader)" (come Cassette,
        // "nuovo" si basa sull'Id, non sul contenuto: un reader esistente senza nomi banca non è nuovo).
        public override string ToString()
        {
            string[] names = (BankNames ?? new string[0]).Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();
            if (names.Length > 0)
            {
                return string.Join("-", names);
            }

            return IdReaderCfg > 0 ? $"Reader #{IdReaderCfg}" : "(Nuovo Reader)";
        }

        public Cm50Reader Clone() => new Cm50Reader
        {
            IdReaderCfg = IdReaderCfg,
            ActiveBank = ActiveBank,
            SlotIds = (int[])SlotIds?.Clone(),
            BankNames = (string[])BankNames?.Clone(),
            BankVersions = (string[])BankVersions?.Clone()
        };
    }
}

using System.ComponentModel;
using CM50_Configurator.Support;

namespace CM50_Configurator.Models
{
    // cm50excqa: parametri ExCQA per coppia Prodotto/Cliente. A differenza delle altre tabelle
    // "semplici" ha due chiavi esterne (idProduct -> cm50prodcode, idClient -> client), che però si
    // possono solo scegliere tra i valori esistenti (mai creare): bastano quindi i converter a
    // selezione ProductReferenceConverter/ClientReferenceConverter, senza logica Ensure* nel writer.
    // I campi nullable nel DB sono nullable anche qui (int?/bool?), così un NULL letto resta NULL
    // quando la riga viene riscritta. I default dei campi replicano i DEFAULT della tabella, perché
    // SimpleTableWriter scrive sempre tutte le colonne (quelli del DB non verrebbero mai applicati).
    [TypeConverter(typeof(OrderedExpandableObjectConverter))]
    public class Cm50ExCqa
    {
        [PropertyOrder(0)]
        [ReadOnly(true)]
        [Category("9 - Identificativo")]
        public int Id { get; set; }

        [PropertyOrder(1)]
        [Category("1 - Riferimenti")]
        [DisplayName("Prodotto")]
        [TypeConverter(typeof(ProductReferenceConverter))]
        [RequiredField("Il Prodotto è obbligatorio (se l'elenco è vuoto, esegui prima \"Carica da DB\").")]
        public Cm50ProdCode Product { get; set; }

        [PropertyOrder(2)]
        [Category("1 - Riferimenti")]
        [DisplayName("Cliente")]
        [TypeConverter(typeof(ClientReferenceConverter))]
        [RequiredField("Il Cliente è obbligatorio (se l'elenco è vuoto, esegui prima \"Carica da DB\").")]
        public Client Client { get; set; }

        [PropertyOrder(3)]
        [Category("2 - Generale")]
        [DisplayName("Gestione Unfit")]
        public bool? HandleUnfit { get; set; }

        [PropertyOrder(4)]
        [Category("2 - Generale")]
        [DisplayName("Quantità Mazzetta")]
        public int? BundleQty { get; set; } = 1193;

        [PropertyOrder(5)]
        [Category("3 - Setup 1")]
        [DisplayName("Setup 1")]
        public string Setup1 { get; set; }

        [PropertyOrder(6)]
        [Category("3 - Setup 1")]
        [DisplayName("N. Cicli 1")]
        public int? Ncicli1 { get; set; }

        [PropertyOrder(7)]
        [Category("3 - Setup 1")]
        [DisplayName("Tipo Deposito 1")]
        public string DepType1 { get; set; } = DefaultDepType;

        [PropertyOrder(8)]
        [Category("4 - Setup 2")]
        [DisplayName("Setup 2")]
        public string Setup2 { get; set; }

        [PropertyOrder(9)]
        [Category("4 - Setup 2")]
        [DisplayName("N. Cicli 2")]
        public int? Ncicli2 { get; set; }

        [PropertyOrder(10)]
        [Category("4 - Setup 2")]
        [DisplayName("Tipo Deposito 2")]
        public string DepType2 { get; set; } = DefaultDepType;

        [PropertyOrder(11)]
        [Category("5 - Setup 3")]
        [DisplayName("Setup 3")]
        public string Setup3 { get; set; }

        [PropertyOrder(12)]
        [Category("5 - Setup 3")]
        [DisplayName("N. Cicli 3")]
        public int? Ncicli3 { get; set; }

        [PropertyOrder(13)]
        [Category("5 - Setup 3")]
        [DisplayName("Tipo Deposito 3")]
        public string DepType3 { get; set; } = DefaultDepType;

        [PropertyOrder(14)]
        [Category("6 - Setup 4")]
        [DisplayName("Setup 4")]
        public string Setup4 { get; set; }

        [PropertyOrder(15)]
        [Category("6 - Setup 4")]
        [DisplayName("N. Cicli 4")]
        public int? Ncicli4 { get; set; }

        [PropertyOrder(16)]
        [Category("6 - Setup 4")]
        [DisplayName("Tipo Deposito 4")]
        public string DepType4 { get; set; } = DefaultDepType;

        [PropertyOrder(17)]
        [Category("7 - Setup Finale")]
        [DisplayName("Setup Finale")]
        public string FinalSetup { get; set; }

        [PropertyOrder(18)]
        [Category("7 - Setup Finale")]
        [DisplayName("Tipo Deposito Finale")]
        public string FinalDepType { get; set; } = "STANDARD";

        [PropertyOrder(19)]
        [Category("8 - Limiti")]
        [DisplayName("Max Cicli Scarti")]
        public int? MaxCycleReject { get; set; } = 9999;

        [PropertyOrder(20)]
        [Category("8 - Limiti")]
        [DisplayName("Max Cicli Inceppamento")]
        public int? MaxCycleJam { get; set; } = 9999;

        // Valore di default di depType1..4 così come definito nella tabella (grafia inclusa).
        private const string DefaultDepType = "CONTINOUS";

        public override string ToString() => $"{Product?.Code} / {Client?.ClientName}";

        // Product e Client sono condivisi per riferimento (come in Cm50UserCustomization.Clone):
        // i loro campi sono di sola lettura, si può solo sostituire l'intero oggetto.
        public Cm50ExCqa Clone() => new Cm50ExCqa
        {
            Id = Id,
            Product = Product,
            Client = Client,
            HandleUnfit = HandleUnfit,
            BundleQty = BundleQty,
            Setup1 = Setup1,
            Ncicli1 = Ncicli1,
            DepType1 = DepType1,
            Setup2 = Setup2,
            Ncicli2 = Ncicli2,
            DepType2 = DepType2,
            Setup3 = Setup3,
            Ncicli3 = Ncicli3,
            DepType3 = DepType3,
            Setup4 = Setup4,
            Ncicli4 = Ncicli4,
            DepType4 = DepType4,
            FinalSetup = FinalSetup,
            FinalDepType = FinalDepType,
            MaxCycleReject = MaxCycleReject,
            MaxCycleJam = MaxCycleJam
        };
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CM50_Configurator.Data;
using CM50_Configurator.Models;

namespace CM50_Configurator
{
    public partial class StartForm : Form
    {
        // Elenco dei moduli di lavoro disponibili. Un nuovo modulo (es. per un'altra tabella)
        // si aggiunge semplicemente con una nuova voce qui.
        private readonly List<WorkModule> _modules = new List<WorkModule>
        {
            new WorkModule(
                "CM50 - Personalizzazioni Utente",
                "Consultazione, modifica ed esportazione/importazione JSON delle personalizzazioni utente (cm50usercustomization) su MySQL.",
                () => new Form1()),
            new WorkModule(
                "CM50 - Configurazioni Master",
                "Consultazione, modifica ed esportazione/importazione JSON delle configurazioni master per codice prodotto (cm50) su MySQL.",
                () => new Cm50Form()),
            new WorkModule(
                "CM50 - Cassetto (DDM)",
                "Consultazione, modifica ed esportazione/importazione JSON della configurazione di collaudo del Cassetto - Double Drum Module (ddmcm50).",
                () => new SimpleTableListForm<DdmCm50>(
                    "ddmcm50", nameof(DdmCm50.IdDDM),
                    () => new DdmCm50Repository().GetAll(),
                    x => x.IdDDM, x => x.Clone(),
                    DdmCm50Repository.CreateWriterConfig())),
            new WorkModule(
                "CM50 - Trasporto Superiore (UTM)",
                "Consultazione, modifica ed esportazione/importazione JSON della configurazione di collaudo del Trasporto Superiore - Upper Transport Module (utmcm50).",
                () => new SimpleTableListForm<UtmCm50>(
                    "utmcm50", nameof(UtmCm50.IdCfg),
                    () => new UtmCm50Repository().GetAll(),
                    x => x.IdCfg, x => x.Clone(),
                    UtmCm50Repository.CreateWriterConfig())),
            new WorkModule(
                "CM50 - Trasporto Inferiore (LTM)",
                "Consultazione, modifica ed esportazione/importazione JSON della configurazione di collaudo del Trasporto Inferiore - Lower Transport Module (ltmcm50).",
                () => new SimpleTableListForm<LtmCm50>(
                    "ltmcm50", nameof(LtmCm50.IdLTM),
                    () => new LtmCm50Repository().GetAll(),
                    x => x.IdLTM, x => x.Clone(),
                    LtmCm50Repository.CreateWriterConfig())),
            new WorkModule(
                "CM50 - Feeder",
                "Consultazione, modifica ed esportazione/importazione JSON della configurazione di collaudo del Feeder (feedercm50).",
                () => new SimpleTableListForm<FeederCm50>(
                    "feedercm50", nameof(FeederCm50.IdCfg),
                    () => new FeederCm50Repository().GetAll(),
                    x => x.IdCfg, x => x.Clone(),
                    FeederCm50Repository.CreateWriterConfig()))
        };

        public Func<Form> SelectedFormFactory { get; private set; }

        public StartForm()
        {
            InitializeComponent();

            lstModules.DataSource = _modules;
            lstModules.SelectedIndexChanged += (s, e) => UpdateDescription();

            if (_modules.Count > 0)
            {
                lstModules.SelectedIndex = 0;
            }
        }

        private void UpdateDescription()
        {
            var module = lstModules.SelectedItem as WorkModule;
            lblDescription.Text = module?.Description;
            btnOpen.Enabled = module != null;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedModule();
        }

        private void lstModules_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedModule();
        }

        private void OpenSelectedModule()
        {
            var module = lstModules.SelectedItem as WorkModule;
            if (module == null)
            {
                return;
            }

            SelectedFormFactory = module.CreateForm;
            DialogResult = DialogResult.OK;
            Close();
        }

        private class WorkModule
        {
            public string Name { get; }
            public string Description { get; }
            public Func<Form> CreateForm { get; }

            public WorkModule(string name, string description, Func<Form> createForm)
            {
                Name = name;
                Description = description;
                CreateForm = createForm;
            }

            public override string ToString() => Name;
        }
    }
}

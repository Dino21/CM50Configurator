using System;
using System.Windows.Forms;
using CM50_Configurator.Models;
using CM50_Configurator.Support;
using CM50_Configurator.Support.UI;

namespace CM50_Configurator
{
    // Crea una nuova personalizzazione da zero. Prodotto, Cliente e System Rate (default 7) sono
    // obbligatori; tutto il resto (DD, Asset, CDF, Configurazione Unità, banconote, ecc.) è
    // opzionale e modificabile liberamente nella PropertyGrid, come nella finestra di dettaglio.
    public partial class NewUserCustomizationForm : Form
    {
        private readonly Cm50UserCustomization _workingItem;

        public Cm50UserCustomization NewItem { get; private set; }

        public NewUserCustomizationForm() : this(null)
        {
        }

        // template != null: "Duplica Riga" - parte da una copia della riga indicata (nuovo id), con
        // Prodotto e Cliente già selezionati nelle tendine.
        public NewUserCustomizationForm(Cm50UserCustomization template)
        {
            InitializeComponent();
            SesamiTheme.Apply(this);

            if (template != null)
            {
                _workingItem = template.Clone();
                _workingItem.IdUserCustomization = 0;
                Text = "Duplica Configurazione";
            }
            else
            {
                _workingItem = new Cm50UserCustomization
                {
                    SystemRate = 7,
                    RefBanks = new string[4],
                    FitnessBanks = new int[4]
                };
            }

            cboProduct.DataSource = ReferenceDataCache.Products;
            cboProduct.SelectedIndexChanged += (s, e) =>
            {
                _workingItem.Product = cboProduct.SelectedItem as Cm50ProdCode;
                propertyGrid.Refresh();
            };
            cboProduct.SelectedIndex = -1;

            cboClient.DataSource = ReferenceDataCache.Clients;
            cboClient.SelectedIndexChanged += (s, e) =>
            {
                _workingItem.Client = cboClient.SelectedItem as Client;
                propertyGrid.Refresh();
            };
            cboClient.SelectedIndex = -1;

            // Va fatto dopo i SelectedIndex = -1 (che azzerano Product/Client): la tendina trova la
            // voce corrispondente tramite Equals per id (vedi Cm50ProdCode/Client).
            if (template != null)
            {
                cboProduct.SelectedItem = template.Product;
                cboClient.SelectedItem = template.Client;
            }

            propertyGrid.SelectedObject = _workingItem;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (cboProduct.SelectedItem == null || cboClient.SelectedItem == null)
            {
                MessageBox.Show(this, "Prodotto e Cliente sono obbligatori.",
                    "Campi obbligatori mancanti", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NewItem = _workingItem;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

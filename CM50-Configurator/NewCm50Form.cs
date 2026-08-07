using System;
using System.Windows.Forms;
using CM50_Configurator.Models;
using CM50_Configurator.Support;

namespace CM50_Configurator
{
    // Crea una nuova riga cm50 da zero. A differenza di NewUserCustomizationForm, qui quasi tutti i
    // campi sono obbligatori (lo schema del DB non permette FK nulle): Prodotto e Cliente si
    // scelgono dalle tendine in alto, tutto il resto va compilato/selezionato nella PropertyGrid
    // e viene verificato con RequiredFieldValidator prima di consentire la creazione.
    public partial class NewCm50Form : Form
    {
        private readonly Cm50 _workingItem;

        public Cm50 NewItem { get; private set; }

        public NewCm50Form()
        {
            InitializeComponent();

            _workingItem = new Cm50();

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

            string error = RequiredFieldValidator.FindMissingRequiredField(_workingItem);
            if (error != null)
            {
                MessageBox.Show(this, error, "Campo obbligatorio mancante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

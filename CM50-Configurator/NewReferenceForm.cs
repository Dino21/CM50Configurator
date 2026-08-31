using System;
using System.Windows.Forms;
using CM50_Configurator.Support;
using CM50_Configurator.Support.UI;

namespace CM50_Configurator
{
    // Mostrata durante l'importazione quando serve creare un codice (DD/Asset/CDF) o una
    // Configurazione Unità non ancora presente nel DB: permette di completare/verificare i dati
    // prima dell'insert. I campi marcati [RequiredField] devono essere valorizzati per procedere.
    public partial class NewReferenceForm : Form
    {
        private readonly object _item;

        public NewReferenceForm(object item, string title, string introMessage)
        {
            InitializeComponent();
            SesamiTheme.Apply(this);

            _item = item ?? throw new ArgumentNullException(nameof(item));
            Text = title;
            lblIntro.Text = introMessage;
            propertyGrid.SelectedObject = item;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string error = RequiredFieldValidator.FindMissingRequiredField(_item);
            if (error != null)
            {
                MessageBox.Show(this, error, "Campo obbligatorio mancante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

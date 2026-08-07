using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CM50_Configurator.Data;
using CM50_Configurator.Services;

namespace CM50_Configurator
{
    // Mostra tutte le righe nuove o modificate rispetto a quanto caricato dal DB, evidenziandole
    // per colore, prima di scrivere effettivamente sul database. Le righe invariate non vengono
    // mostrate né scritte.
    public partial class ReviewChangesForm : Form
    {
        private static readonly Color NewRowColor = Color.FromArgb(214, 255, 214);
        private static readonly Color ModifiedRowColor = Color.FromArgb(255, 249, 196);

        private readonly List<UserCustomizationDiff.PendingChange> _pending;
        private readonly DatabaseWriter _writer;

        public bool Committed { get; private set; }

        public ReviewChangesForm(List<UserCustomizationDiff.PendingChange> pending, DatabaseWriter writer)
        {
            InitializeComponent();

            _pending = pending;
            _writer = writer;

            BindGrid();
        }

        private void BindGrid()
        {
            var rows = _pending.Select(p => new ChangeRow
            {
                Source = p,
                Stato = p.StatusLabel,
                Prodotto = p.Item.Product?.Code,
                Cliente = p.Item.Client?.ClientName,
                DD = p.Item.Dd?.Code,
                Asset = p.Item.Asset?.Code,
                CDF = p.Item.Cdf?.Code,
                ConfigurazioneUnita = p.Item.UnitConfiguration?.ToString(),
                CampiModificati = p.IsNew ? "(intera riga nuova)" : string.Join(", ", p.ChangedFields)
            }).ToList();

            dgvChanges.DataSource = rows;
            dgvChanges.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            foreach (DataGridViewRow row in dgvChanges.Rows)
            {
                var changeRow = (ChangeRow)row.DataBoundItem;
                row.DefaultCellStyle.BackColor = changeRow.Source.IsNew ? NewRowColor : ModifiedRowColor;
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            List<Models.Cm50UserCustomization> items = _pending.Select(p => p.Item).ToList();

            try
            {
                Cursor = Cursors.WaitCursor;
                ImportResult result = _writer.ImportAll(items, this);
                Committed = true;

                MessageBox.Show(this, result.ToSummaryText(),
                    result.Errors.Count > 0 ? "Scrittura completata con errori" : "Scrittura completata",
                    MessageBoxButtons.OK, result.Errors.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Errore durante la scrittura sul database:\n{ex.Message}",
                    "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private class ChangeRow
        {
            public UserCustomizationDiff.PendingChange Source;

            public string Stato { get; set; }
            public string Prodotto { get; set; }
            public string Cliente { get; set; }
            public string DD { get; set; }
            public string Asset { get; set; }
            public string CDF { get; set; }
            public string ConfigurazioneUnita { get; set; }
            public string CampiModificati { get; set; }
        }
    }
}

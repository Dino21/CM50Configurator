using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CM50_Configurator.Data;
using CM50_Configurator.Models;
using CM50_Configurator.Services;
using CM50_Configurator.Support.UI;

namespace CM50_Configurator
{
    // Come ReviewChangesForm, ma per la tabella cm50.
    public partial class Cm50ReviewChangesForm : Form
    {
        private static readonly Color NewRowColor = SesamiTheme.NewRowColor;
        private static readonly Color ModifiedRowColor = SesamiTheme.ModifiedRowColor;

        private readonly List<Cm50Diff.PendingChange> _pending;
        private readonly DatabaseWriter _writer;

        public bool Committed { get; private set; }

        public Cm50ReviewChangesForm(List<Cm50Diff.PendingChange> pending, DatabaseWriter writer)
        {
            InitializeComponent();
            SesamiTheme.Apply(this);

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
                Reader = p.Item.Reader?.ToString(),
                Cassette = p.Item.Cassette?.ToString(),
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
            List<Cm50> items = _pending.Select(p => p.Item).ToList();

            try
            {
                Cursor = Cursors.WaitCursor;
                ImportResult result = _writer.ImportAllCm50(items, this);
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
            public Cm50Diff.PendingChange Source;

            public string Stato { get; set; }
            public string Prodotto { get; set; }
            public string Cliente { get; set; }
            public string DD { get; set; }
            public string Asset { get; set; }
            public string CDF { get; set; }
            public string ConfigurazioneUnita { get; set; }
            public string Reader { get; set; }
            public string Cassette { get; set; }
            public string CampiModificati { get; set; }
        }
    }
}

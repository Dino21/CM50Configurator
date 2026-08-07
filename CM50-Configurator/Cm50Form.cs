using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CM50_Configurator.Data;
using CM50_Configurator.Models;
using CM50_Configurator.Services;
using CM50_Configurator.Support;

namespace CM50_Configurator
{
    // Come Form1, ma per la tabella "master" cm50 (configurazione per codice prodotto).
    public partial class Cm50Form : Form
    {
        private readonly Cm50Repository _repository = new Cm50Repository();
        private readonly ReferenceDataRepository _referenceRepository = new ReferenceDataRepository();
        private readonly DatabaseWriter _databaseWriter = new DatabaseWriter();
        private List<Cm50> _currentData = new List<Cm50>();

        // Istantanea presa subito dopo ogni "Carica da DB": confrontata con _currentData per capire
        // quali righe sono nuove o modificate al momento di "Scrivi su DB..." (vedi Cm50Diff).
        private List<Cm50> _baseline = new List<Cm50>();

        public Cm50Form()
        {
            InitializeComponent();
        }

        private void btnLoadFromDb_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _currentData = _repository.GetAll();
                _baseline = _currentData.Select(x => x.Clone()).ToList();
                LoadReferenceData();
                BindGrid(_currentData);
                btnExportJson.Enabled = _currentData.Count > 0;
                lblStatus.Text = $"Caricati {_currentData.Count} record da cm50.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Errore durante la lettura dal database:\n{ex.Message}",
                    "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lettura dal database fallita.";
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LoadReferenceData()
        {
            ReferenceDataCache.DdCodes = _referenceRepository.GetDdCodes().ToArray();
            ReferenceDataCache.AssetCodes = _referenceRepository.GetAssetCodes().ToArray();
            ReferenceDataCache.CdfCodes = _referenceRepository.GetCdfCodes().ToArray();
            ReferenceDataCache.UnitConfigurations = _referenceRepository.GetUnitConfigurations().ToArray();
            ReferenceDataCache.Products = _referenceRepository.GetProducts().ToArray();
            ReferenceDataCache.Clients = _referenceRepository.GetClients().ToArray();
            ReferenceDataCache.Readers = _referenceRepository.GetReaders().ToArray();
            ReferenceDataCache.Cassettes = _referenceRepository.GetCassettes().ToArray();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            if (ReferenceDataCache.Products.Length == 0 || ReferenceDataCache.Clients.Length == 0)
            {
                MessageBox.Show(this, "Carica prima i dati dal database (Prodotti/Clienti non ancora disponibili).",
                    "Dati non disponibili", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new NewCm50Form())
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _currentData.Add(form.NewItem);
                BindGrid(_currentData);
                btnExportJson.Enabled = _currentData.Count > 0;
                lblStatus.Text = "Nuova riga cm50 aggiunta (non ancora scritta sul database).";
            }
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            if (_currentData.Count == 0)
            {
                return;
            }

            using (var dialog = new SaveFileDialog
            {
                Filter = "File JSON (*.json)|*.json",
                FileName = $"cm50_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    JsonExportService.ExportItems(_currentData, dialog.FileName);
                    lblStatus.Text = $"Esportati {_currentData.Count} record in {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Errore durante l'esportazione del file JSON:\n{ex.Message}",
                        "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImportJson_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Filter = "File JSON (*.json)|*.json" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                List<Cm50> imported;
                try
                {
                    imported = JsonImportService.ImportItems<Cm50>(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Errore durante la lettura del file JSON:\n{ex.Message}",
                        "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (imported.Count == 0)
                {
                    MessageBox.Show(this, "Il file non contiene record da importare.",
                        "Importazione", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MergeIntoCurrentData(imported);
                BindGrid(_currentData);
                btnExportJson.Enabled = _currentData.Count > 0;
                lblStatus.Text = $"Importati {imported.Count} record dal file (non ancora scritti sul database). " +
                    "Usa \"Scrivi su DB...\" per rivedere e confermare.";
            }
        }

        private void MergeIntoCurrentData(List<Cm50> imported)
        {
            foreach (Cm50 item in imported)
            {
                int index = item.IdProduct > 0
                    ? _currentData.FindIndex(x => x.IdProduct == item.IdProduct)
                    : -1;

                if (index >= 0)
                {
                    _currentData[index] = item;
                }
                else
                {
                    _currentData.Add(item);
                }
            }
        }

        private void btnReviewAndWrite_Click(object sender, EventArgs e)
        {
            List<Cm50Diff.PendingChange> pending = Cm50Diff.ComputePendingChanges(_currentData, _baseline);

            if (pending.Count == 0)
            {
                MessageBox.Show(this, "Non ci sono modifiche da scrivere sul database.",
                    "Nessuna modifica", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool committed;
            using (var form = new Cm50ReviewChangesForm(pending, _databaseWriter))
            {
                form.ShowDialog(this);
                committed = form.Committed;
            }

            if (committed)
            {
                btnLoadFromDb_Click(sender, e);
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            OpenDetail(dgvCm50.CurrentRow);
        }

        private void dgvCm50_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            OpenDetail(dgvCm50.Rows[e.RowIndex]);
        }

        private void OpenDetail(DataGridViewRow gridRow)
        {
            var row = gridRow?.DataBoundItem as GridRow;
            if (row == null)
            {
                return;
            }

            using (var form = new Cm50DetailForm(row.Source))
            {
                form.ShowDialog(this);
            }

            BindGrid(_currentData);
        }

        private void BindGrid(IEnumerable<Cm50> items)
        {
            var rows = items.Select(i => new GridRow
            {
                Source = i,
                IdProduct = i.IdProduct,
                Prodotto = i.Product?.Code,
                Cliente = i.Client?.ClientName,
                DD = i.Dd?.Code,
                DdTransitionCode = i.DdTransitionCode,
                Asset = i.Asset?.Code,
                CDF = i.Cdf?.Code,
                UnitConfig = i.UnitConfiguration?.ToString(),
                Reader = i.Reader?.ToString(),
                Cassette = i.Cassette?.ToString(),
                AlarmType = i.AlarmType
            }).ToList();

            dgvCm50.DataSource = rows;
            dgvCm50.Columns["IdProduct"].Visible = false;
            dgvCm50.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        // Vista "piatta" ad uso esclusivo della griglia; l'export JSON usa invece il modello completo annidato.
        private class GridRow
        {
            public Cm50 Source;

            public int IdProduct { get; set; }
            public string Prodotto { get; set; }
            public string Cliente { get; set; }
            public string DD { get; set; }
            public string DdTransitionCode { get; set; }
            public string Asset { get; set; }
            public string CDF { get; set; }
            public string UnitConfig { get; set; }
            public string Reader { get; set; }
            public string Cassette { get; set; }
            public string AlarmType { get; set; }
        }
    }
}

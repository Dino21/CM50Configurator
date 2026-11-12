using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CM50_Configurator.Data;
using CM50_Configurator.Models;
using CM50_Configurator.Services;
using CM50_Configurator.Support;
using CM50_Configurator.Support.UI;

namespace CM50_Configurator
{
    public partial class Form1 : Form
    {
        private readonly UserCustomizationRepository _repository = new UserCustomizationRepository();
        private readonly ReferenceDataRepository _referenceRepository = new ReferenceDataRepository();
        private readonly DatabaseWriter _databaseWriter = new DatabaseWriter();
        private List<Cm50UserCustomization> _currentData = new List<Cm50UserCustomization>();

        // Righe attualmente visualizzate in griglia (già filtrate e ordinate per codice prodotto),
        // usate anche per l'export delle "righe filtrate".
        private List<GridRow> _displayedRows = new List<GridRow>();

        // Istantanea presa subito dopo ogni "Carica da DB": confrontata con _currentData per capire
        // quali righe sono nuove o modificate al momento di "Scrivi su DB..." (vedi UserCustomizationDiff).
        private List<Cm50UserCustomization> _baseline = new List<Cm50UserCustomization>();

        public Form1()
        {
            InitializeComponent();
            SesamiTheme.Apply(this);
            SesamiTheme.AddHeaderBanner(this, "CM50 - Personalizzazioni Utente");
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
                lblStatus.Text = $"Caricati {_currentData.Count} record da cm50usercustomization.";
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
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            if (ReferenceDataCache.Products.Length == 0 || ReferenceDataCache.Clients.Length == 0)
            {
                MessageBox.Show(this, "Carica prima i dati dal database (Prodotti/Clienti non ancora disponibili).",
                    "Dati non disponibili", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new NewUserCustomizationForm())
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _currentData.Add(form.NewItem);
                BindGrid(_currentData);
                btnExportJson.Enabled = _currentData.Count > 0;
                lblStatus.Text = "Nuova configurazione aggiunta (non ancora scritta sul database).";
            }
        }

        // Duplica la riga selezionata: apre la finestra di creazione già compilata con una copia
        // (nuovo id), così basta cambiare il campo che differisce e confermare con "Crea". La copia
        // resta solo in memoria finché non viene confermata con "Scrivi su DB...".
        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            var row = dgvUserCustomizations.CurrentRow?.DataBoundItem as GridRow;
            if (row == null)
            {
                MessageBox.Show(this, "Seleziona prima la riga da duplicare.",
                    "Nessuna riga selezionata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ReferenceDataCache.Products.Length == 0 || ReferenceDataCache.Clients.Length == 0)
            {
                MessageBox.Show(this, "Carica prima i dati dal database (Prodotti/Clienti non ancora disponibili).",
                    "Dati non disponibili", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new NewUserCustomizationForm(row.Source))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _currentData.Add(form.NewItem);
                BindGrid(_currentData);
                btnExportJson.Enabled = _currentData.Count > 0;
                lblStatus.Text = "Configurazione duplicata (non ancora scritta sul database).";
            }
        }

        private void btnBulkReplace_Click(object sender, EventArgs e)
        {
            if (_currentData.Count == 0)
            {
                MessageBox.Show(this, "Carica prima i dati dal database.",
                    "Dati non disponibili", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new BulkCodeReplaceForm(typeof(Cm50UserCustomization), _currentData, "cm50usercustomization"))
            {
                if (form.ShowDialog(this) != DialogResult.OK || form.ReplacedCount == 0)
                {
                    return;
                }

                BindGrid(_currentData);
                lblStatus.Text = $"Sostituiti {form.ReplacedCount} valori (non ancora scritti sul database). " +
                    "Usa \"Scrivi su DB...\" per rivedere e confermare.";
            }
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            if (_currentData.Count == 0)
            {
                return;
            }

            var menu = new ContextMenuStrip();
            menu.Items.Add("Riga selezionata...", null, (s, ev) => ExportSelectedRow());
            menu.Items.Add("Tabella intera...", null, (s, ev) => ExportRows(_currentData, "cm50usercustomization"));
            menu.Items.Add("Righe filtrate (visualizzate)...", null, (s, ev) =>
                ExportRows(_displayedRows.Select(r => r.Source).ToList(), "cm50usercustomization_filtrate"));

            menu.Show(btnExportJson, new Point(0, btnExportJson.Height));
        }

        private void ExportSelectedRow()
        {
            var row = dgvUserCustomizations.CurrentRow?.DataBoundItem as GridRow;
            if (row == null)
            {
                MessageBox.Show(this, "Seleziona prima una riga da esportare.",
                    "Nessuna riga selezionata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportRows(new List<Cm50UserCustomization> { row.Source }, "cm50usercustomization_riga");
        }

        private void ExportRows(List<Cm50UserCustomization> items, string fileNamePrefix)
        {
            if (items.Count == 0)
            {
                MessageBox.Show(this, "Non ci sono righe da esportare.",
                    "Nessuna riga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog
            {
                Filter = "File JSON (*.json)|*.json",
                FileName = $"{fileNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    JsonExportService.ExportItems(items, dialog.FileName);
                    lblStatus.Text = $"Esportati {items.Count} record in {dialog.FileName}";
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

                List<Cm50UserCustomization> imported;
                try
                {
                    imported = JsonImportService.ImportItems<Cm50UserCustomization>(dialog.FileName);
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

        // Sostituisce la riga esistente (stesso id) o la aggiunge come nuova; la scrittura effettiva
        // sul DB avviene solo più avanti, dalla finestra di revisione (btnReviewAndWrite_Click).
        private void MergeIntoCurrentData(List<Cm50UserCustomization> imported)
        {
            foreach (Cm50UserCustomization item in imported)
            {
                int index = item.IdUserCustomization > 0
                    ? _currentData.FindIndex(x => x.IdUserCustomization == item.IdUserCustomization)
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
            List<UserCustomizationDiff.PendingChange> pending = UserCustomizationDiff.ComputePendingChanges(_currentData, _baseline);

            if (pending.Count == 0)
            {
                MessageBox.Show(this, "Non ci sono modifiche da scrivere sul database.",
                    "Nessuna modifica", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool committed;
            using (var form = new ReviewChangesForm(pending, _databaseWriter))
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
            OpenDetail(dgvUserCustomizations.CurrentRow);
        }

        private void dgvUserCustomizations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            OpenDetail(dgvUserCustomizations.Rows[e.RowIndex]);
        }

        private void OpenDetail(DataGridViewRow gridRow)
        {
            var row = gridRow?.DataBoundItem as GridRow;
            if (row == null)
            {
                return;
            }

            using (var form = new UserCustomizationDetailForm(row.Source))
            {
                form.ShowDialog(this);
            }

            BindGrid(_currentData);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            BindGrid(_currentData);
        }

        private void BindGrid(IEnumerable<Cm50UserCustomization> items)
        {
            var rows = items.Select(i => new GridRow
            {
                Source = i,
                IdUserCustomization = i.IdUserCustomization,
                Prodotto = i.Product?.Code,
                Cliente = i.Client?.ClientName,
                DD = i.Dd?.Code,
                Asset = i.Asset?.Code,
                CDF = i.Cdf?.Code,
                UnitConfig = FormatUnitConfig(i.UnitConfiguration),
                ActiveBanks = i.ActiveBanks,
                BankConfig = i.BankConfig,
                RefBank0 = i.RefBanks?[0],
                RefBank1 = i.RefBanks?[1],
                RefBank2 = i.RefBanks?[2],
                RefBank3 = i.RefBanks?[3],
                Valuta = i.Currency,
                FitnessBank0 = i.FitnessBanks?[0],
                FitnessBank1 = i.FitnessBanks?[1],
                FitnessBank2 = i.FitnessBanks?[2],
                FitnessBank3 = i.FitnessBanks?[3],
                SetCassConfig = i.SetCassConfig,
                CountryCode = i.CountryCode,
                Lingua = i.DisplayLanguage,
                SystemRate = i.SystemRate,
                EcoTimeOut = i.EcoTimeOut
            })
            .OrderBy(r => r.Prodotto, StringComparer.OrdinalIgnoreCase)
            .ToList();

            _displayedRows = GridFilter.Apply(rows, txtFilter.Text);

            dgvUserCustomizations.DataSource = _displayedRows;
            dgvUserCustomizations.Columns["IdUserCustomization"].Visible = false;
            dgvUserCustomizations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private static string FormatUnitConfig(Cm50UnitConfiguration unitConfiguration)
        {
            if (unitConfiguration == null)
            {
                return null;
            }

            return string.Join("-",
                unitConfiguration.Cfg,
                unitConfiguration.OptCfg,
                unitConfiguration.OptOneCfg,
                unitConfiguration.OptTwoCfg,
                unitConfiguration.OptThreeCfg);
        }

        // Vista "piatta" ad uso esclusivo della griglia; l'export JSON usa invece il modello completo annidato.
        // Source è un campo (non una proprietà) apposta perché la DataGridView genera colonne solo dalle proprietà.
        private class GridRow
        {
            public Cm50UserCustomization Source;

            public int IdUserCustomization { get; set; }
            public string Prodotto { get; set; }
            public string Cliente { get; set; }
            public string DD { get; set; }
            public string Asset { get; set; }
            public string CDF { get; set; }
            public string UnitConfig { get; set; }
            public int ActiveBanks { get; set; }
            public string BankConfig { get; set; }
            public string RefBank0 { get; set; }
            public string RefBank1 { get; set; }
            public string RefBank2 { get; set; }
            public string RefBank3 { get; set; }
            public string Valuta { get; set; }
            public int? FitnessBank0 { get; set; }
            public int? FitnessBank1 { get; set; }
            public int? FitnessBank2 { get; set; }
            public int? FitnessBank3 { get; set; }
            public string SetCassConfig { get; set; }
            public string CountryCode { get; set; }
            public string Lingua { get; set; }
            public int SystemRate { get; set; }
            public int EcoTimeOut { get; set; }
        }
    }
}

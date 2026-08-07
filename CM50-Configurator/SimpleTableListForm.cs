using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CM50_Configurator.Data;
using CM50_Configurator.Services;

namespace CM50_Configurator
{
    // Form generica per le tabelle "semplici" senza chiavi esterne (ddmcm50, utmcm50, ltmcm50,
    // feedercm50): stessa UX di Form1/Cm50Form (Carica da DB, Nuova Riga, Dettaglio/Modifica,
    // Importa/Esporta JSON, Scrivi su DB con revisione colorata), ma senza bisogno di gestire
    // riferimenti. Essendo generica, la UI si costruisce in codice invece che in un Designer.cs
    // separato (il designer visuale di VS non supporta form generiche); la griglia principale
    // usa la generazione automatica delle colonne della DataGridView (i modelli sono "piatti").
    public partial class SimpleTableListForm<T> : Form where T : class, new()
    {
        private readonly string _entityName;
        private readonly string _idPropertyName;
        private readonly Func<List<T>> _load;
        private readonly Func<T, int> _getId;
        private readonly Func<T, T> _clone;
        private readonly SimpleTableConfig<T> _writerConfig;

        private List<T> _currentData = new List<T>();
        private List<T> _baseline = new List<T>();

        private DataGridView _grid;
        private Button _btnExportJson;
        private Label _lblStatus;

        public SimpleTableListForm(string entityName, string idPropertyName, Func<List<T>> load,
            Func<T, int> getId, Func<T, T> clone, SimpleTableConfig<T> writerConfig)
        {
            _entityName = entityName;
            _idPropertyName = idPropertyName;
            _load = load;
            _getId = getId;
            _clone = clone;
            _writerConfig = writerConfig;

            BuildUi();
        }

        private void BuildUi()
        {
            Text = $"CM50 Configurator - {_entityName}";
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 611);
            MinimumSize = new Size(700, 400);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersWidth = 25
            };
            _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) OpenDetail(_grid.Rows[e.RowIndex]); };

            var pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 90 };

            var btnLoad = new Button { Text = "Carica da DB", Location = new Point(12, 8), Size = new Size(120, 27) };
            btnLoad.Click += BtnLoadFromDb_Click;

            var btnAddNew = new Button { Text = "Nuova Riga...", Location = new Point(144, 8), Size = new Size(160, 27) };
            btnAddNew.Click += BtnAddNew_Click;

            var btnDetail = new Button { Text = "Dettaglio / Modifica...", Location = new Point(316, 8), Size = new Size(150, 27) };
            btnDetail.Click += (s, e) => OpenDetail(_grid.CurrentRow);

            var btnImportJson = new Button { Text = "Importa JSON...", Location = new Point(12, 41), Size = new Size(160, 27) };
            btnImportJson.Click += BtnImportJson_Click;

            var btnReviewAndWrite = new Button
            {
                Text = "Scrivi su DB...",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(184, 41),
                Size = new Size(160, 27)
            };
            btnReviewAndWrite.Click += BtnReviewAndWrite_Click;

            _btnExportJson = new Button { Text = "Esporta JSON...", Enabled = false, Location = new Point(356, 41), Size = new Size(120, 27) };
            _btnExportJson.Click += BtnExportJson_Click;

            _lblStatus = new Label { AutoSize = true, Location = new Point(600, 37) };

            pnlActions.Controls.Add(btnLoad);
            pnlActions.Controls.Add(btnAddNew);
            pnlActions.Controls.Add(btnDetail);
            pnlActions.Controls.Add(btnImportJson);
            pnlActions.Controls.Add(btnReviewAndWrite);
            pnlActions.Controls.Add(_btnExportJson);
            pnlActions.Controls.Add(_lblStatus);

            Controls.Add(_grid);
            Controls.Add(pnlActions);
        }

        private void BtnLoadFromDb_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _currentData = _load();
                _baseline = _currentData.Select(_clone).ToList();
                RefreshGrid();
                _btnExportJson.Enabled = _currentData.Count > 0;
                _lblStatus.Text = $"Caricati {_currentData.Count} record da {_entityName}.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Errore durante la lettura dal database:\n{ex.Message}",
                    "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus.Text = "Lettura dal database fallita.";
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            var newItem = new T();
            using (var form = new SimpleEntityDetailForm<T>($"Nuova riga {_entityName}", newItem, isNewMode: true))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _currentData.Add(newItem);
                RefreshGrid();
                _btnExportJson.Enabled = _currentData.Count > 0;
                _lblStatus.Text = $"Nuova riga {_entityName} aggiunta (non ancora scritta sul database).";
            }
        }

        private void BtnExportJson_Click(object sender, EventArgs e)
        {
            if (_currentData.Count == 0)
            {
                return;
            }

            using (var dialog = new SaveFileDialog
            {
                Filter = "File JSON (*.json)|*.json",
                FileName = $"{typeof(T).Name.ToLowerInvariant()}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    JsonExportService.ExportItems(_currentData, dialog.FileName);
                    _lblStatus.Text = $"Esportati {_currentData.Count} record in {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Errore durante l'esportazione del file JSON:\n{ex.Message}",
                        "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnImportJson_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Filter = "File JSON (*.json)|*.json" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                List<T> imported;
                try
                {
                    imported = JsonImportService.ImportItems<T>(dialog.FileName);
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
                RefreshGrid();
                _btnExportJson.Enabled = _currentData.Count > 0;
                _lblStatus.Text = $"Importati {imported.Count} record dal file (non ancora scritti sul database). " +
                    "Usa \"Scrivi su DB...\" per rivedere e confermare.";
            }
        }

        private void MergeIntoCurrentData(List<T> imported)
        {
            foreach (T item in imported)
            {
                int id = _getId(item);
                int index = id > 0 ? _currentData.FindIndex(x => _getId(x) == id) : -1;

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

        private void BtnReviewAndWrite_Click(object sender, EventArgs e)
        {
            List<SimpleEntityDiff.PendingChange<T>> pending = SimpleEntityDiff.ComputePendingChanges(_currentData, _baseline, _getId);

            if (pending.Count == 0)
            {
                MessageBox.Show(this, "Non ci sono modifiche da scrivere sul database.",
                    "Nessuna modifica", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool committed;
            using (var form = new SimpleTableReviewForm<T>(_entityName, pending, _writerConfig))
            {
                form.ShowDialog(this);
                committed = form.Committed;
            }

            if (committed)
            {
                BtnLoadFromDb_Click(sender, e);
            }
        }

        private void OpenDetail(DataGridViewRow gridRow)
        {
            T item = gridRow?.DataBoundItem as T;
            if (item == null)
            {
                return;
            }

            using (var form = new SimpleEntityDetailForm<T>($"Dettaglio {_entityName}", item, isNewMode: false))
            {
                form.ShowDialog(this);
            }

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            _grid.DataSource = null;
            _grid.DataSource = _currentData;

            if (_grid.Columns.Contains(_idPropertyName))
            {
                _grid.Columns[_idPropertyName].Visible = false;
            }

            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
    }
}

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CM50_Configurator.Data;
using CM50_Configurator.Services;

namespace CM50_Configurator
{
    // Come ReviewChangesForm/Cm50ReviewChangesForm, ma generica: costruisce le colonne della
    // griglia a runtime via reflection (una DataTable, invece di una classe GridRow per tabella),
    // dato che qui non c'è alcuna struttura annidata da conoscere in anticipo.
    public class SimpleTableReviewForm<T> : Form
    {
        private static readonly Color NewRowColor = Color.FromArgb(214, 255, 214);
        private static readonly Color ModifiedRowColor = Color.FromArgb(255, 249, 196);

        private readonly System.Collections.Generic.List<SimpleEntityDiff.PendingChange<T>> _pending;
        private readonly SimpleTableConfig<T> _config;
        private DataGridView _grid;

        public bool Committed { get; private set; }

        public SimpleTableReviewForm(string entityName, System.Collections.Generic.List<SimpleEntityDiff.PendingChange<T>> pending, SimpleTableConfig<T> config)
        {
            _pending = pending;
            _config = config;

            BuildUi(entityName);
            BindGrid();
        }

        private void BuildUi(string entityName)
        {
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 450);
            MinimumSize = new Size(600, 350);
            StartPosition = FormStartPosition.CenterParent;
            Text = $"Revisione modifiche {entityName} prima della scrittura sul DB";

            var lblLegend = new Label
            {
                Dock = DockStyle.Top,
                Padding = new Padding(12, 12, 12, 8),
                Height = 34,
                Text = "Verde = riga nuova · Giallo = riga modificata. Le righe invariate non sono mostrate."
            };

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersWidth = 25,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };

            var pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50 };

            var btnWrite = new Button
            {
                Text = "Scrivi su DB",
                Size = new Size(130, 27),
                Location = new Point(596, 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnWrite.Click += BtnWrite_Click;

            var btnCancel = new Button
            {
                Text = "Annulla",
                Size = new Size(100, 27),
                Location = new Point(736, 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCancel.Click += (s, e) => Close();

            AcceptButton = btnWrite;
            CancelButton = btnCancel;
            pnlButtons.Controls.Add(btnWrite);
            pnlButtons.Controls.Add(btnCancel);

            Controls.Add(_grid);
            Controls.Add(pnlButtons);
            Controls.Add(lblLegend);
        }

        private void BindGrid()
        {
            var table = new DataTable();
            table.Columns.Add("Stato");

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor property in properties)
            {
                table.Columns.Add(property.DisplayName, typeof(string));
            }

            table.Columns.Add("Campi Modificati");

            foreach (var pending in _pending)
            {
                DataRow row = table.NewRow();
                row["Stato"] = pending.StatusLabel;
                foreach (PropertyDescriptor property in properties)
                {
                    object value = property.GetValue(pending.Item);
                    row[property.DisplayName] = value?.ToString() ?? string.Empty;
                }
                row["Campi Modificati"] = pending.IsNew ? "(intera riga nuova)" : string.Join(", ", pending.ChangedFields);
                table.Rows.Add(row);
            }

            _grid.DataSource = table;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            for (int i = 0; i < _grid.Rows.Count; i++)
            {
                _grid.Rows[i].DefaultCellStyle.BackColor = _pending[i].IsNew ? NewRowColor : ModifiedRowColor;
            }
        }

        private void BtnWrite_Click(object sender, EventArgs e)
        {
            var items = new System.Collections.Generic.List<T>();
            foreach (var pending in _pending)
            {
                items.Add(pending.Item);
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                ImportResult result = new SimpleTableWriter().ImportAll(items, _config);
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
    }
}

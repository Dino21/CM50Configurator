using System;
using System.Drawing;
using System.Windows.Forms;
using CM50_Configurator.Services;
using CM50_Configurator.Support.UI;

namespace CM50_Configurator
{
    // Finestra generica per visualizzare/modificare (isNewMode=false, un solo bottone "Chiudi" +
    // "Esporta JSON") o creare da zero (isNewMode=true, "Crea"/"Annulla") una riga di una tabella
    // "semplice" senza riferimenti (ddmcm50, utmcm50, ltmcm50, feedercm50). Essendo generica, la UI
    // si costruisce in codice invece che in un Designer.cs separato (il designer visuale di VS non
    // supporta form generiche).
    public class SimpleEntityDetailForm<T> : Form where T : class
    {
        private readonly T _item;
        private readonly bool _isNewMode;
        private PropertyGrid _propertyGrid;

        public bool Confirmed { get; private set; }

        public SimpleEntityDetailForm(string title, T item, bool isNewMode)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _isNewMode = isNewMode;

            BuildUi();
            SesamiTheme.Apply(this);
            Text = title;
            _propertyGrid.SelectedObject = item;
        }

        private void BuildUi()
        {
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 561);
            MinimumSize = new Size(420, 400);
            StartPosition = FormStartPosition.CenterParent;

            _propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                HelpVisible = true,
                PropertySort = PropertySort.Categorized,
                ToolbarVisible = false
            };

            var pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50 };

            // pnlButtons va aggiunto al form (e quindi assumere la sua larghezza definitiva tramite
            // Dock.Bottom) PRIMA di aggiungergli i bottoni con Anchor Right: un controllo figlio
            // ancorato a destra "cattura" la distanza dal bordo destro del parent nel momento in cui
            // viene aggiunto ai suoi Controls, quindi se a quel momento pnlButtons non fosse ancora
            // agganciato al form avrebbe ancora la larghezza di default (non quella reale, 520px),
            // e il bottone finirebbe fuori dall'area visibile (riprodotto con "Chiudi" introvabile
            // nei dettagli di ddmcm50/utmcm50/ltmcm50/feedercm50).
            Controls.Add(_propertyGrid);
            Controls.Add(pnlButtons);

            if (_isNewMode)
            {
                var btnCreate = new Button
                {
                    Text = "Crea",
                    Size = new Size(100, 27),
                    Location = new Point(248, 11),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnCreate.Click += (s, e) => { Confirmed = true; DialogResult = DialogResult.OK; Close(); };

                var btnCancel = new Button
                {
                    Text = "Annulla",
                    Size = new Size(100, 27),
                    Location = new Point(356, 11),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

                AcceptButton = btnCreate;
                CancelButton = btnCancel;
                pnlButtons.Controls.Add(btnCreate);
                pnlButtons.Controls.Add(btnCancel);
            }
            else
            {
                var btnExport = new Button
                {
                    Text = "Esporta JSON...",
                    Size = new Size(140, 27),
                    Location = new Point(12, 11),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };
                btnExport.Click += BtnExport_Click;

                var btnClose = new Button
                {
                    Text = "Chiudi",
                    Size = new Size(100, 27),
                    Location = new Point(408, 11),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnClose.Click += (s, e) => Close();

                pnlButtons.Controls.Add(btnExport);
                pnlButtons.Controls.Add(btnClose);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
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
                    JsonExportService.ExportItem(_item, dialog.FileName);
                    MessageBox.Show(this, $"Configurazione esportata in:\n{dialog.FileName}",
                        "Esportazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Errore durante l'esportazione del file JSON:\n{ex.Message}",
                        "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

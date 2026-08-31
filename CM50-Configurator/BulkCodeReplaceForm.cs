using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CM50_Configurator.Support;
using CM50_Configurator.Support.UI;

namespace CM50_Configurator
{
    // Sostituisce in blocco tutte le occorrenze di un valore in un campo, su tutte le righe
    // attualmente caricate in memoria (es. sostituire il DD "FF20379" con un altro in tutte le
    // righe di cm50). Riusa lo stesso TypeConverter già usato dalla PropertyGrid per digitare un
    // codice: per i campi di riferimento (DD, Asset, CDF, Configurazione Unità, Cassette) il nuovo
    // valore viene risolto o creato esattamente come se fosse stato digitato a mano su una singola
    // riga (vedi ReferenceSync). I campi il cui TypeConverter non sa costruire un valore da una
    // stringa (Prodotto, Cliente, Reader, gli array Banconote) non compaiono nell'elenco: non hanno
    // un'identità testuale compatta, quindi non sono "sostituibili per codice".
    // Come import/nuova riga, la sostituzione resta solo in memoria: va confermata con
    // "Scrivi su DB..." per essere effettivamente salvata.
    public class BulkCodeReplaceForm : Form
    {
        private readonly List<object> _items;
        private readonly List<FieldOption> _fields;

        private ComboBox _cboField;
        private ComboBox _cboOldValue;
        private ComboBox _cboNewValue;
        private Label _lblMatchCount;

        public int ReplacedCount { get; private set; }

        public BulkCodeReplaceForm(Type entityType, IEnumerable<object> items, string entityDisplayName)
        {
            _items = items.ToList();
            _fields = GetReplaceableFields(entityType);

            BuildUi(entityDisplayName);
        }

        private static List<FieldOption> GetReplaceableFields(Type entityType)
        {
            return TypeDescriptor.GetProperties(entityType)
                .Cast<PropertyDescriptor>()
                .Where(p => !p.IsReadOnly && p.Converter.CanConvertFrom(typeof(string)) && p.Converter.CanConvertTo(typeof(string)))
                .OrderBy(GetOrder)
                .Select(p => new FieldOption(p))
                .ToList();
        }

        private static int GetOrder(PropertyDescriptor property)
        {
            var attribute = property.Attributes[typeof(PropertyOrderAttribute)] as PropertyOrderAttribute;
            return attribute?.Order ?? int.MaxValue;
        }

        private void BuildUi(string entityDisplayName)
        {
            Text = "Sostituisci Codice";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(430, 230);

            var lblIntro = new Label
            {
                Text = $"Sostituisce un valore in tutte le righe di \"{entityDisplayName}\" attualmente caricate.",
                Location = new Point(12, 12),
                Size = new Size(406, 32)
            };

            var lblField = new Label { Text = "Campo:", AutoSize = true, Location = new Point(12, 57) };
            _cboField = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(150, 54),
                Size = new Size(268, 21)
            };
            // Items popolati direttamente (non tramite DataSource): un ComboBox legato a DataSource
            // non completa il binding finché il controllo non ha un Handle (cioè non prima che il
            // form venga mostrato), quindi un SelectedIndex impostato qui nel costruttore troverebbe
            // Items ancora vuoto e lancerebbe ArgumentOutOfRangeException.
            _cboField.Items.AddRange(_fields.ToArray());
            _cboField.SelectedIndexChanged += (s, e) => OnFieldChanged();

            var lblOldValue = new Label { Text = "Valore attuale:", AutoSize = true, Location = new Point(12, 92) };
            _cboOldValue = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Location = new Point(150, 89), Size = new Size(268, 21) };
            _cboOldValue.TextChanged += (s, e) => UpdateMatchCount();

            var lblNewValue = new Label { Text = "Nuovo valore:", AutoSize = true, Location = new Point(12, 127) };
            _cboNewValue = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Location = new Point(150, 124), Size = new Size(268, 21) };

            _lblMatchCount = new Label
            {
                AutoSize = false,
                Location = new Point(12, 158),
                Size = new Size(406, 17),
                ForeColor = SesamiTheme.TextSecondary
            };

            var btnReplace = new Button
            {
                Text = "Sostituisci",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(228, 188),
                Size = new Size(100, 27)
            };
            btnReplace.Click += BtnReplace_Click;

            var btnCancel = new Button
            {
                Text = "Annulla",
                DialogResult = DialogResult.Cancel,
                Location = new Point(332, 188),
                Size = new Size(86, 27)
            };

            Controls.Add(lblIntro);
            Controls.Add(lblField);
            Controls.Add(_cboField);
            Controls.Add(lblOldValue);
            Controls.Add(_cboOldValue);
            Controls.Add(lblNewValue);
            Controls.Add(_cboNewValue);
            Controls.Add(_lblMatchCount);
            Controls.Add(btnReplace);
            Controls.Add(btnCancel);

            AcceptButton = btnReplace;
            CancelButton = btnCancel;

            SesamiTheme.Apply(this);

            if (_cboField.Items.Count > 0)
            {
                _cboField.SelectedIndex = 0;
            }
        }

        private FieldOption SelectedField => _cboField.SelectedItem as FieldOption;

        private void OnFieldChanged()
        {
            _cboOldValue.Items.Clear();
            _cboOldValue.Text = string.Empty;
            _cboNewValue.Items.Clear();
            _cboNewValue.Text = string.Empty;

            FieldOption field = SelectedField;
            if (field == null)
            {
                UpdateMatchCount();
                return;
            }

            TypeConverter converter = field.Property.Converter;

            string[] currentValues = _items
                .Select(item => ConvertToDisplayString(converter, field.Property.GetValue(item)))
                .Where(v => !string.IsNullOrEmpty(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            _cboOldValue.Items.AddRange(currentValues);

            if (converter.GetStandardValuesSupported())
            {
                string[] standardValues = converter.GetStandardValues()
                    .Cast<object>()
                    .Select(v => ConvertToDisplayString(converter, v))
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                _cboNewValue.Items.AddRange(standardValues);
            }

            UpdateMatchCount();
        }

        private static string ConvertToDisplayString(TypeConverter converter, object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            try
            {
                return converter.ConvertToString(value) ?? string.Empty;
            }
            catch (Exception)
            {
                return value.ToString();
            }
        }

        private List<object> GetMatchingItems()
        {
            FieldOption field = SelectedField;
            string oldValue = _cboOldValue.Text;

            if (field == null || string.IsNullOrEmpty(oldValue))
            {
                return new List<object>();
            }

            TypeConverter converter = field.Property.Converter;
            return _items
                .Where(item => string.Equals(ConvertToDisplayString(converter, field.Property.GetValue(item)), oldValue, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void UpdateMatchCount()
        {
            int count = GetMatchingItems().Count;
            _lblMatchCount.Text = count == 1
                ? "1 riga corrisponde al valore attuale."
                : $"{count} righe corrispondono al valore attuale.";
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            FieldOption field = SelectedField;
            if (field == null)
            {
                return;
            }

            string oldValue = _cboOldValue.Text?.Trim();
            string newValue = _cboNewValue.Text?.Trim();

            if (string.IsNullOrEmpty(oldValue))
            {
                MessageBox.Show(this, "Indica il valore attuale da sostituire.",
                    "Valore mancante", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(this, "Il nuovo valore è uguale a quello attuale.",
                    "Nessuna modifica", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<object> matches = GetMatchingItems();
            if (matches.Count == 0)
            {
                MessageBox.Show(this, $"Nessuna riga ha il valore '{oldValue}' nel campo \"{field.DisplayName}\".",
                    "Nessuna corrispondenza", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(this,
                $"Verranno aggiornate {matches.Count} righe: campo \"{field.DisplayName}\", '{oldValue}' → '{newValue}'.\n\n" +
                "La modifica resta in memoria: usa \"Scrivi su DB...\" per rivedere e confermare definitivamente. Continuare?",
                "Conferma sostituzione", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            object convertedNewValue;
            try
            {
                convertedNewValue = string.IsNullOrEmpty(newValue) ? null : field.Property.Converter.ConvertFromString(newValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Il nuovo valore '{newValue}' non è valido per il campo \"{field.DisplayName}\":\n{ex.Message}",
                    "Valore non valido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (object item in matches)
            {
                field.Property.SetValue(item, convertedNewValue);
            }

            ReplacedCount = matches.Count;
            DialogResult = DialogResult.OK;
            Close();
        }

        private class FieldOption
        {
            public PropertyDescriptor Property { get; }
            public string DisplayName { get; }

            public FieldOption(PropertyDescriptor property)
            {
                Property = property;
                DisplayName = property.DisplayName;
            }

            public override string ToString() => DisplayName;
        }
    }
}

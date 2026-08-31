using System;
using System.Windows.Forms;
using CM50_Configurator.Models;
using CM50_Configurator.Services;
using CM50_Configurator.Support;
using CM50_Configurator.Support.UI;

namespace CM50_Configurator
{
    public partial class UserCustomizationDetailForm : Form
    {
        private readonly Cm50UserCustomization _item;

        public UserCustomizationDetailForm(Cm50UserCustomization item)
        {
            InitializeComponent();
            SesamiTheme.Apply(this);

            _item = item ?? throw new ArgumentNullException(nameof(item));

            propertyGrid.SelectedObject = item;
            Text = $"Dettaglio personalizzazione #{item.IdUserCustomization} - {item.Product?.Code} / {item.Client?.ClientName}";
        }

        // Un codice digitato a mano (o un campo della Configurazione Unità modificato da espansa)
        // viene ricercato tra i dati di riferimento: se trovato i campi collegati vengono
        // riallineati, altrimenti svuotati per segnalare che si tratta di una voce nuova.
        private void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            string propertyName = e.ChangedItem.PropertyDescriptor?.Name;
            object container = e.ChangedItem.Parent?.Value;

            if (propertyName == nameof(Cm50DdCode.Code) && container is Cm50DdCode dd)
            {
                ReferenceSync.SyncDd(dd);
            }
            else if (propertyName == nameof(Cm50AssetCode.Code) && container is Cm50AssetCode asset)
            {
                ReferenceSync.SyncAsset(asset);
            }
            else if (propertyName == nameof(Cm50CdfCode.Code) && container is Cm50CdfCode cdf)
            {
                ReferenceSync.SyncCdf(cdf);
            }
            else if (container is Cm50UnitConfiguration unitConfiguration && IsUnitConfigurationIdentityField(propertyName))
            {
                ReferenceSync.SyncUnitConfiguration(unitConfiguration);
            }

            propertyGrid.Refresh();
        }

        private static bool IsUnitConfigurationIdentityField(string propertyName)
        {
            return propertyName == nameof(Cm50UnitConfiguration.Cfg)
                || propertyName == nameof(Cm50UnitConfiguration.OptCfg)
                || propertyName == nameof(Cm50UnitConfiguration.OptOneCfg)
                || propertyName == nameof(Cm50UnitConfiguration.OptTwoCfg)
                || propertyName == nameof(Cm50UnitConfiguration.OptThreeCfg);
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog
            {
                Filter = "File JSON (*.json)|*.json",
                FileName = $"cm50usercustomization_{_item.IdUserCustomization}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

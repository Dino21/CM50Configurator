using System;
using System.Windows.Forms;
using CM50_Configurator.Models;
using CM50_Configurator.Services;
using CM50_Configurator.Support;

namespace CM50_Configurator
{
    public partial class Cm50DetailForm : Form
    {
        private readonly Cm50 _item;

        public Cm50DetailForm(Cm50 item)
        {
            InitializeComponent();

            _item = item ?? throw new ArgumentNullException(nameof(item));

            propertyGrid.SelectedObject = item;
            Text = $"Dettaglio cm50 #{item.IdProduct} - {item.Product?.Code} / {item.Client?.ClientName}";
        }

        // Come in UserCustomizationDetailForm: un codice/combinazione digitato a mano viene
        // ricercato tra i dati di riferimento e i campi collegati riallineati o svuotati.
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
            else if (container is Cm50Cassette cassette && IsCassetteIdentityField(propertyName))
            {
                ReferenceSync.SyncCassette(cassette);
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

        private static bool IsCassetteIdentityField(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Cm50Cassette.CassA):
                case nameof(Cm50Cassette.CassB):
                case nameof(Cm50Cassette.CassC):
                case nameof(Cm50Cassette.CassD):
                case nameof(Cm50Cassette.CassE):
                case nameof(Cm50Cassette.CassF):
                case nameof(Cm50Cassette.CassG):
                case nameof(Cm50Cassette.CassH):
                case nameof(Cm50Cassette.CassI):
                case nameof(Cm50Cassette.CassJ):
                case nameof(Cm50Cassette.CassK):
                case nameof(Cm50Cassette.CassL):
                    return true;
                default:
                    return false;
            }
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog
            {
                Filter = "File JSON (*.json)|*.json",
                FileName = $"cm50_{_item.IdProduct}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
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

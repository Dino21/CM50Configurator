using System.Collections.Generic;
using System.Text;

namespace CM50_Configurator.Data
{
    public class ImportResult
    {
        public int Processed { get; set; }
        public int Inserted { get; set; }
        public int Updated { get; set; }

        public List<string> NewDdCodes { get; } = new List<string>();
        public List<string> NewAssetCodes { get; } = new List<string>();
        public List<string> NewCdfCodes { get; } = new List<string>();
        public List<string> NewUnitConfigurations { get; } = new List<string>();
        public List<string> NewCassetteConfigurations { get; } = new List<string>();
        public List<string> NewReaderConfigurations { get; } = new List<string>();

        public List<string> Errors { get; } = new List<string>();

        public string ToSummaryText()
        {
            var summary = new StringBuilder();
            summary.AppendLine($"Record elaborati: {Processed}");
            summary.AppendLine($"  - Aggiornati: {Updated}");
            summary.AppendLine($"  - Nuovi: {Inserted}");

            if (NewDdCodes.Count > 0)
            {
                summary.AppendLine($"Nuovi codici DD creati: {string.Join(", ", NewDdCodes)}");
            }
            if (NewAssetCodes.Count > 0)
            {
                summary.AppendLine($"Nuovi codici Asset creati: {string.Join(", ", NewAssetCodes)}");
            }
            if (NewCdfCodes.Count > 0)
            {
                summary.AppendLine($"Nuovi codici CDF creati: {string.Join(", ", NewCdfCodes)}");
            }
            if (NewUnitConfigurations.Count > 0)
            {
                summary.AppendLine($"Nuove Configurazioni Unità create: {string.Join(", ", NewUnitConfigurations)}");
            }
            if (NewCassetteConfigurations.Count > 0)
            {
                summary.AppendLine($"Nuove Configurazioni Cassette create: {string.Join(", ", NewCassetteConfigurations)}");
            }
            if (NewReaderConfigurations.Count > 0)
            {
                summary.AppendLine($"Nuove Configurazioni Reader create: {string.Join(", ", NewReaderConfigurations)}");
            }

            if (Errors.Count > 0)
            {
                summary.AppendLine();
                summary.AppendLine($"Errori ({Errors.Count}):");
                foreach (string error in Errors)
                {
                    summary.AppendLine($"  - {error}");
                }
            }

            return summary.ToString();
        }
    }
}

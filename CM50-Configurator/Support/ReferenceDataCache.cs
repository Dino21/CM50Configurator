using CM50_Configurator.Models;

namespace CM50_Configurator.Support
{
    // Elenchi completi delle tabelle di riferimento (DD, Asset, CDF, Configurazioni Unità) caricati
    // dal DB insieme ai dati principali: servono sia a proporre un elenco di selezione nella
    // PropertyGrid sia a riconoscere/riallineare un codice digitato a mano (vedi ReferenceSync).
    public static class ReferenceDataCache
    {
        public static Cm50DdCode[] DdCodes { get; set; } = new Cm50DdCode[0];
        public static Cm50AssetCode[] AssetCodes { get; set; } = new Cm50AssetCode[0];
        public static Cm50CdfCode[] CdfCodes { get; set; } = new Cm50CdfCode[0];
        public static Cm50UnitConfiguration[] UnitConfigurations { get; set; } = new Cm50UnitConfiguration[0];

        // Prodotti e Clienti esistenti, usati per la selezione obbligatoria in "Nuova Configurazione".
        public static Cm50ProdCode[] Products { get; set; } = new Cm50ProdCode[0];
        public static Client[] Clients { get; set; } = new Client[0];

        // Configurazioni Reader/Cassette esistenti (schema cfg), usate dal modulo cm50.
        public static Cm50Reader[] Readers { get; set; } = new Cm50Reader[0];
        public static Cm50Cassette[] Cassettes { get; set; } = new Cm50Cassette[0];
    }
}

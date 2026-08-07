using System;
using System.Linq;
using CM50_Configurator.Models;

namespace CM50_Configurator.Support
{
    // Quando un codice (o una configurazione unità) viene digitato a mano, va cercato tra i dati di
    // riferimento caricati dal DB: se esiste già, i campi collegati vengono riallineati al record
    // trovato; se non esiste, vengono svuotati per segnalare chiaramente che si tratta di una voce nuova.
    public static class ReferenceSync
    {
        public static void SyncDd(Cm50DdCode dd)
        {
            Cm50DdCode match = ReferenceDataCache.DdCodes.FirstOrDefault(x => Matches(x.Code, dd.Code));
            dd.InternalName = match?.InternalName;
            dd.TransitionDDCode = match?.TransitionDDCode;
            dd.TransitionFilename = match?.TransitionFilename;
            dd.ASISver = match?.ASISver;
        }

        public static void SyncAsset(Cm50AssetCode asset)
        {
            Cm50AssetCode match = ReferenceDataCache.AssetCodes.FirstOrDefault(x => Matches(x.Code, asset.Code));
            asset.InternalName = match?.InternalName;
            asset.ShortFilename = match?.ShortFilename;
            asset.TallFilename = match?.TallFilename;
            asset.Version = match?.Version;
            asset.Release = match?.Release;
            asset.Serial = match?.Serial;
        }

        public static void SyncCdf(Cm50CdfCode cdf)
        {
            Cm50CdfCode match = ReferenceDataCache.CdfCodes.FirstOrDefault(x => Matches(x.Code, cdf.Code));
            cdf.InternalName = match?.InternalName;
            cdf.CurrCodes = match?.CurrCodes;
        }

        public static void SyncUnitConfiguration(Cm50UnitConfiguration unitConfiguration)
        {
            Cm50UnitConfiguration match = ReferenceDataCache.UnitConfigurations.FirstOrDefault(x =>
                Matches(x.Cfg, unitConfiguration.Cfg) &&
                Matches(x.OptCfg, unitConfiguration.OptCfg) &&
                Matches(x.OptOneCfg, unitConfiguration.OptOneCfg) &&
                Matches(x.OptTwoCfg, unitConfiguration.OptTwoCfg) &&
                Matches(x.OptThreeCfg, unitConfiguration.OptThreeCfg));

            unitConfiguration.IdUnitConfiguration = match?.IdUnitConfiguration ?? 0;
            unitConfiguration.Description = match?.Description;
        }

        public static void SyncCassette(Cm50Cassette cassette)
        {
            Cm50Cassette match = ReferenceDataCache.Cassettes.FirstOrDefault(x =>
                Matches(x.CassA, cassette.CassA) && Matches(x.CassB, cassette.CassB) &&
                Matches(x.CassC, cassette.CassC) && Matches(x.CassD, cassette.CassD) &&
                Matches(x.CassE, cassette.CassE) && Matches(x.CassF, cassette.CassF) &&
                Matches(x.CassG, cassette.CassG) && Matches(x.CassH, cassette.CassH) &&
                Matches(x.CassI, cassette.CassI) && Matches(x.CassJ, cassette.CassJ) &&
                Matches(x.CassK, cassette.CassK) && Matches(x.CassL, cassette.CassL));

            cassette.IdCassetteCfg = match?.IdCassetteCfg ?? 0;
        }

        private static bool Matches(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}

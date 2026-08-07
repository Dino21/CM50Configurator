using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CM50_Configurator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CM50_Configurator.Services
{
    internal static class JsonExportService
    {
        // I modelli hanno un TypeConverter (ExpandableObjectConverter) per essere navigabili nella PropertyGrid.
        // Newtonsoft però lo interpreta come "convertibile a stringa" e serializza l'oggetto con ToString()
        // (es. "CM50_Configurator.Models.Client") invece dei suoi campi: qui forziamo il contratto a oggetto
        // per tutti i tipi del namespace Models, ignorando il TypeConverter.
        private class ModelObjectContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                if (objectType.Namespace == typeof(Cm50UserCustomization).Namespace)
                {
                    return CreateObjectContract(objectType);
                }

                return base.CreateContract(objectType);
            }
        }

        // Riutilizzate anche da JsonImportService per una lettura simmetrica alla scrittura.
        internal static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new ModelObjectContractResolver()
        };

        public static void ExportItems<T>(List<T> items, string filePath)
        {
            string json = JsonConvert.SerializeObject(items, Settings);
            File.WriteAllText(filePath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        public static void ExportItem<T>(T item, string filePath)
        {
            ExportItems(new List<T> { item }, filePath);
        }
    }
}

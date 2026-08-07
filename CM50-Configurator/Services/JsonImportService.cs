using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CM50_Configurator.Services
{
    internal static class JsonImportService
    {
        // Accetta sia un array (l'export "multiplo") sia un singolo oggetto (l'export di una sola
        // riga, o un file ricostruito a mano dall'ente esterno).
        public static List<T> ImportItems<T>(string filePath)
        {
            string json = File.ReadAllText(filePath, Encoding.UTF8);
            JToken token = JToken.Parse(json);
            JsonSerializer serializer = JsonSerializer.Create(JsonExportService.Settings);

            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>(serializer) ?? new List<T>();
            }

            T single = token.ToObject<T>(serializer);
            return single == null ? new List<T>() : new List<T> { single };
        }
    }
}

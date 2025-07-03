using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace SteamGamesApi
{
    public class AppState
    {
        public required UInt32 appid { get; set; }
        public required string name { get; set; }
        public required string installdir { get; set; }
        // ...
    };

    public class AppManifestACF
    {
        [JsonConstructor]
        public AppManifestACF() { }

        public required AppState AppState { get; set; }

        [DynamicDependency(nameof(AppManifestACF) + "()")]
        [DynamicDependency(nameof(SteamGamesApi.AppState) + "()", typeof(AppState))]
        public static AppManifestACF? FromPath(string path)
        {
            try
            {
                using StreamReader reader = File.OpenText(path);
                var deserializer = new JsonSerializer() { MissingMemberHandling = MissingMemberHandling.Ignore };
                return
                    new JObject([VdfConvert.Deserialize(reader).ToJson()])
                    .ToObject<AppManifestACF>(deserializer);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}

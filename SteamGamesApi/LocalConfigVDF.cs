using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace SteamGamesApi
{
    public class SoftwarePlayInfo
    {
        public int? LastPlayed { get; set; }
        public int? Playtime {  get; set; }
        public int? PlaytimeDisconnected { get; set; }
        // ...
    }

    public class SoftwareList
    {
        public required Dictionary<UInt32, SoftwarePlayInfo> apps {  get; set; }
    }

    public class UserLocalConfigStore
    {
        public required Dictionary<string, Dictionary<string, SoftwareList>> Software { get; set; }
        // ...
    };

    public class LocalConfigVDF
    {
        [JsonConstructor]
        public LocalConfigVDF() { }

        public required UserLocalConfigStore UserLocalConfigStore { get; set; }

        static readonly VdfSerializerSettings SerializerSettings = new () {
            MaximumTokenSize = 65536,
            UsesEscapeSequences = true 
        };

        [DynamicDependency(nameof(LocalConfigVDF) + "()")]
        [DynamicDependency(nameof(SteamGamesApi.UserLocalConfigStore) + "()", typeof(UserLocalConfigStore))]
        [DynamicDependency(nameof(SoftwareList) + "()", typeof(SoftwareList))]
        [DynamicDependency(nameof(SoftwarePlayInfo) + "()", typeof(SoftwarePlayInfo))]
        public static LocalConfigVDF? FromPath(string path)
        {
            try
            {
                using StreamReader reader = File.OpenText(path);
                var deserializer = new JsonSerializer() { MissingMemberHandling = MissingMemberHandling.Ignore };
                return
                    new JObject([VdfConvert.Deserialize(reader, SerializerSettings).ToJson()])
                    .ToObject<LocalConfigVDF>(deserializer);
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

using System.Globalization;
using ValveKeyValue;

namespace SteamGamesApi.Models
{
    public class SoftwarePlayInfo
    {
        [KVIgnore]
        public int? LastPlayed { get; set; }
        [KVProperty("LastPlayed")]
        internal string? _LastPlayed {
            get => LastPlayed.HasValue ? LastPlayed.Value.ToString(CultureInfo.InvariantCulture) : null;
            set => LastPlayed = string.IsNullOrEmpty(value) ? null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }
        //public int? Playtime { get; set; }
        //public int? PlaytimeDisconnected { get; set; }
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
        public required UserLocalConfigStore UserLocalConfigStore { get; set; }

        static readonly KVSerializerOptions SerializerOptions = new () {
            HasEscapeSequences = true 
        };

        public static LocalConfigVDF? FromPath(string path)
        {
            try
            {
                using var reader = File.OpenRead(path);
                var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
                var ulcs = kv.Deserialize<UserLocalConfigStore>(reader, SerializerOptions);
                return new() { UserLocalConfigStore = ulcs };
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

using ValveKeyValue;

namespace SteamGamesApi.Models
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
        public required AppState AppState { get; set; }

        public static AppManifestACF? FromPath(string path)
        {
            try
            {
                using var stream = File.OpenRead(path);
                var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
                var @as = kv.Deserialize<AppState>(stream);
                return new() { AppState = @as };
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

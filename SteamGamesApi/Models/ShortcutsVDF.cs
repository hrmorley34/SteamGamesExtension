using ValveKeyValue;

namespace SteamGamesApi.Models
{
    public class Shortcut
    {
        public required int appid { get; set; }
        public required string AppName { get; set; }
        public required string Exe { get; set; }
        public required string StartDir { get; set; }
        public required string icon { get; set; }
        public required string ShortcutPath { get; set; }
        public required string LaunchOptions { get; set; }
        public required bool IsHidden { get; set; }
        public required int LastPlayTime { get; set; }
        public required Dictionary<string, string> tags { get; set; }
        // ...
    }

    public class ShortcutsVDF
    {
        public required Dictionary<int, Shortcut> shortcuts { get; set; }

        public static ShortcutsVDF? FromPath(string path)
        {
            try
            {
                using var stream = File.OpenRead(path);
                var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary);
                var sc = kv.Deserialize<Dictionary<int, Shortcut>>(stream);
                return new() { shortcuts = sc };
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

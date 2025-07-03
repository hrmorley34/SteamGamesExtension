using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace SteamGamesApi
{
    public class LibraryFolder
    {
        public required string path { get; set; }
        public required string label { get; set; }
        // public UInt64 contentid;
        // public UInt64 totalsize;
        // public UInt64 update_clean_bytes_tally;
        // public UInt64 time_last_update_verified;
        public required Dictionary<UInt32, UInt64> apps { get; set; }

        public IEnumerable<string> GetManifestPaths()
        {
            if (!Path.Exists(path))
                yield break;
            //foreach (var id in apps.Keys)
            //{
            //    var manifestPath = Path.Combine(path, "steamapps", $"appmanifest_{id}.acf");
            //    if (!Path.Exists(manifestPath))
            //        continue;
            //    yield return manifestPath;
            //}
            var steamApps = Path.Combine(path, "steamapps");
            if (!Directory.Exists(steamApps))
                yield break;
            foreach (var manifestPath in Directory.EnumerateFiles(steamApps, "appmanifest_*.acf"))
                yield return manifestPath;
        }

        public IEnumerable<AppManifestACF> GetManifests()
        {
            foreach (var manifestPath in GetManifestPaths())
            {
                var manifest = AppManifestACF.FromPath(manifestPath);
                if (manifest is null)
                    continue;
                yield return manifest;
            }
        }
    };

    public class LibraryFoldersVDF
    {
        [JsonConstructor]
        public LibraryFoldersVDF() { }

        public required Dictionary<int, LibraryFolder> libraryfolders { get; set; }

        [DynamicDependency(nameof(LibraryFoldersVDF) + "()")]
        [DynamicDependency(nameof(LibraryFolder) + "()", typeof(LibraryFolder))]
        public static LibraryFoldersVDF? FromPath(string path)
        {
            try
            {
                using StreamReader reader = File.OpenText(path);
                var deserializer = new JsonSerializer() { MissingMemberHandling = MissingMemberHandling.Ignore };
                return
                    new JObject([VdfConvert.Deserialize(reader).ToJson()])
                    .ToObject<LibraryFoldersVDF>(deserializer);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public IEnumerable<AppManifestACF> GetManifests()
        {
            return libraryfolders.Values.SelectMany(f => f.GetManifests());
        }
    }
}

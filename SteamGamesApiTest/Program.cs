using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamGamesApi;
using SteamGamesApiTest;
using System.IO;
using Windows.System;

SteamInstance steamInstance = new();
var libraries = steamInstance.FindLibraries()!;

var libraryCount = libraries.libraryfolders.Values.Count(f => Path.Exists(f.path));
var manifests = libraries.GetManifests().ToList();

string libraryCountStr = libraryCount == 1 ? "1 library" : $"{libraryCount} libraries";
string manifestCountStr = manifests.Count == 1 ? "1 game" : $"{manifests.Count} games";
Console.WriteLine($"Steam Games: {manifestCountStr} found in {libraryCountStr}.");

var shortcuts = steamInstance.GetUserShortcuts()!;
foreach(var shortcut in shortcuts) { Console.WriteLine(shortcut); }

//var localConfigPath = Path.Combine(steamInstance.GetUserDir()!, "config", "localconfig.vdf");
//using StreamReader reader = File.OpenText(localConfigPath);
//var deserializer = new JsonSerializer() { MissingMemberHandling = MissingMemberHandling.Ignore };
//var json = new JObject([VdfConvert.Deserialize(reader, new VdfSerializerSettings() { MaximumTokenSize = 65536, UsesEscapeSequences=true}).ToJson()]);
//Console.WriteLine(json);
var localConfig = steamInstance.GetUserLocalConfig()!;
var localConfigApps = localConfig.UserLocalConfigStore.Software["Valve"]["Steam"].apps;
Console.WriteLine($"Games: {localConfigApps.Count}");
foreach ((var k, var v) in localConfigApps)
{
    Console.WriteLine($"{k}: {v.LastPlayed}");
}

var sp = new SteamGamesExtensionPage(steamInstance);
var items = sp.GetItems();
foreach (var item in items)
{
    Console.WriteLine($"{item.Name} ({item.AppId})");
    Console.WriteLine($"  {item.Uri}");
    Console.WriteLine($"  {item.LastPlayedTime}");
}

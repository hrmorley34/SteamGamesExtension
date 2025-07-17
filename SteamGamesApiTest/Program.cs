using SteamGamesApi;

SteamInstance steamInstance = new();
var libraries = steamInstance.FindLibraries()!;
Console.WriteLine(libraries);

var libraryCount = libraries.libraryfolders.Values.Count(f => Path.Exists(f.path));
var manifests = libraries.GetManifests().ToList();

string libraryCountStr = libraryCount == 1 ? "1 library" : $"{libraryCount} libraries";
string manifestCountStr = manifests.Count == 1 ? "1 game" : $"{manifests.Count} games";
Console.WriteLine($"Steam Games: {manifestCountStr} found in {libraryCountStr}.");

var shortcuts = steamInstance.GetUserShortcuts()!.shortcuts.Values;
foreach(var shortcut in shortcuts) { Console.WriteLine(shortcut); }

var localConfig = steamInstance.GetUserLocalConfig()!;
var localConfigApps = localConfig.UserLocalConfigStore.Software["Valve"]["Steam"].apps;
Console.WriteLine($"Games: {localConfigApps.Count}");
foreach ((var k, var v) in localConfigApps)
{
    Console.WriteLine($"{k}: {v.LastPlayed}");
}

//var sp = new SteamGamesExtensionPage(steamInstance);
//var items = sp.GetItems();
//foreach (var item in items)
//{
//    Console.WriteLine($"{item.Name} ({item.AppId})");
//    Console.WriteLine($"  {item.Uri}");
//    Console.WriteLine($"  {item.LastPlayedTime}");
//}

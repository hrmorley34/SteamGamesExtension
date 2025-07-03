using SteamGamesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SteamGamesApiTest;

internal sealed class SteamGamesExtensionPage
{
    public string Title { get; set; }

    SteamInstance SteamInstance { get; init; }

    public SteamGamesExtensionPage(SteamInstance steamInstance)
    {
        SteamInstance = steamInstance;
        Title = "Steam Games: ? games found in ? libraries";
    }

    (IEnumerable<SteamGameCommand>, string?) GetManifestCommands()
    {
        var libraries = SteamInstance.FindLibraries();
        if (libraries is null)
            return ([], null);
        var libraryCount = libraries.libraryfolders.Values.Count(f => Path.Exists(f.path));
        var manifests = libraries.GetManifests().ToList();

        var commands = manifests
            .Select(m => SteamGameCommand.FromManifest(m));

        var gameLocalData = SteamInstance.GetUserLocalConfig()
            ?.UserLocalConfigStore
            .Software
            .GetValueOrDefault("Valve")
            ?.GetValueOrDefault("Steam")
            ?.apps;
        if (gameLocalData is not null)
            commands = commands.Select(cmd =>
            {
                if (cmd.AppId is not null && gameLocalData.TryGetValue(cmd.AppId.Value, out var value))
                    cmd.LastPlayedTime ??= value.LastPlayed;
                return cmd;
            });

        var libraryCountStr = libraryCount == 1 ? "1 library" : $"{libraryCount} libraries";
        var manifestCountStr = manifests.Count == 1 ? "1 game" : $"{manifests.Count} games";
        var summary = $"{manifestCountStr} found in {libraryCountStr}";

        return (commands, summary);
    }

    (IEnumerable<SteamGameCommand>, string?) GetShortcutCommands()
    {
        var shortcuts = SteamInstance.GetUserShortcuts();
        if (shortcuts is null)
            return ([], null);
        var commands = shortcuts.Select(m => SteamGameCommand.FromVDFEntry(m));

        var shortcutsCountStr = shortcuts.Length == 1 ? "1 non-steam game" : $"{shortcuts.Length} non-steam games";
        var summary = $"{shortcutsCountStr} found";

        return (commands, summary);
    }

    class CommandSort : Comparer<SteamGameCommand>
    {
        static int CompareNulls<T>(T? x, T? y, Func<T, T, int>? @default = null)
            where T : class
        {
            if (x is null && y is null) return 0;
            else if (x is null) return -1;
            else if (y is null) return 1;
            else return @default is null ? 0 : @default(x, y);
        }

        static int CompareNulls<T>(T? x, T? y, Func<T, T, int>? @default = null)
            where T : struct
        {
            if (x is null && y is null) return 0;
            else if (x is null) return -1;
            else if (y is null) return 1;
            else return @default is null ? 0 : @default(x.Value, y.Value);
        }

        public override int Compare(SteamGameCommand? x, SteamGameCommand? y)
        {
            if (x is null || y is null) return CompareNulls(x, y);
            var comp = CompareNulls(x.LastPlayedTime, y.LastPlayedTime, (xp, yp) => xp.CompareTo(yp));
            if (comp == 0)
                comp = CompareNulls(x.Name, y.Name, (xp, yp) => String.Compare(xp, yp, StringComparison.OrdinalIgnoreCase));
            return comp;
        }
    }

    public SteamGameCommand[] GetItems()
    {
        var data = new List<(IEnumerable<SteamGameCommand>, string?)>() {
            GetManifestCommands(),
            GetShortcutCommands(),
        };

        List<SteamGameCommand> commands = [];
        List<string> summaries = [];
        foreach ((var cmds, var summary) in data)
        {
            commands.AddRange(cmds);
            if (summary is not null)
                summaries.Add(summary);
        }

        Title = summaries.Count <= 0 ? "No data found." : $"Steam games: " + String.Join("; ", summaries);
        commands.Sort(new CommandSort());
        return commands
            .ToArray();
    }
}

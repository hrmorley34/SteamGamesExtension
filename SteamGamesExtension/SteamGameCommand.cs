using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SteamGamesApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VDFParser.Models;

namespace SteamGamesExtension;

internal sealed partial class SteamGameCommand : InvokableCommand
{
    public string Uri { get; set; }
    public UInt32? AppId {  get; set; }
    public string? Subtitle { get; set; }
    public int? LastPlayedTime { get; set; }

    public SteamGameCommand(string name, IconInfo icon, string uri, UInt32? appid = null)
    {
        Name = name;
        Icon = icon;
        Uri = uri;
        AppId = appid;
    }

    public SteamGameCommand(string name, IconInfo icon, UInt32 id)
    {
        Name = name;
        Icon = icon;
        Uri = $"steam://rungameid/{id}";
        AppId = id;
    }

    public static SteamGameCommand FromManifest(AppManifestACF appManifest)
    {
        var appState = appManifest.AppState;
        return new SteamGameCommand(
            appState.name,
            new("\ue7fc"),  // Segoe Fluent 'Game'
            appState.appid
        )
        { Subtitle = $"ID: {appState.appid}; {appState.installdir}" };
    }

    public static SteamGameCommand FromVDFEntry(VDFEntry vdfEntry)
    {
        return new SteamGameCommand(
            vdfEntry.AppName,
            new("\ue7fc"),  // Segoe Fluent 'Game'
            unchecked((UInt32)vdfEntry.appid)
        )
        {
            Subtitle = $"Non-steam shortcut",
            LastPlayedTime = vdfEntry.LastPlayTime
        };
    }

    public override ICommandResult Invoke()
    {
        Process.Start(new ProcessStartInfo(Uri) { UseShellExecute = true });

        // Hides the Command Palette window, without changing the page that's open
        return CommandResult.Hide();
    }
}

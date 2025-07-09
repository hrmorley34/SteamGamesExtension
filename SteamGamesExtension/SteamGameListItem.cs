using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SteamGamesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDFParser.Models;

namespace SteamGamesExtension;

public sealed partial class SteamGameListItem : ListItem
{
    public UInt32 AppId { get; init; }
    public int? LastPlayedTime { get; set; }

    public SteamGameListItem(string title, IconInfo icon, UInt32 appId)
        : base()
    {
        Title = title;
        AppId = appId;
        Command = new OpenUrlCommand($"steam://rungameid/{AppId}")
        {
            Name = "Launch",
            Icon = icon,
            Result = CommandResult.Hide()
        };
    }

    public static SteamGameListItem FromManifest(AppManifestACF appManifest)
    {
        var appState = appManifest.AppState;
        return new SteamGameListItem(
            appState.name,
            new("\ue7fc"),  // Segoe Fluent 'Game'
            appState.appid
        )
        {
            Subtitle = $"ID: {appState.appid}; {appState.installdir}"
        };
    }

    public static SteamGameListItem FromVDFEntry(VDFEntry vdfEntry)
    {
        return new SteamGameListItem(
            vdfEntry.AppName,
            new("\ue7fc"),  // Segoe Fluent 'Game'
            unchecked((UInt32)vdfEntry.appid)
        )
        {
            Subtitle = $"Non-steam shortcut",
            LastPlayedTime = vdfEntry.LastPlayTime
        };
    }
}


using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SteamGamesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDFParser.Models;
using Windows.System;

namespace SteamGamesExtension;

public sealed partial class SteamGameListItem : ListItem
{
    public UInt32 AppId { get; init; }
    public int? LastPlayedTime { get; set; }

    public SteamGameListItem(string title, IconInfo icon, UInt32 appId, IContextItem[]? moreCommands = null)
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
        MoreCommands = [
            new CommandContextItem(
                new OpenUrlCommand($"steam://open/games/details/{AppId}")
                {
                    Name = "Show in Library",
                    Icon = new("\ue8f1"),  // Segoe Fluent 'Library'
                    Result = CommandResult.Hide()
                }
            ),
            new CommandContextItem(
                new CopyTextCommand($"{AppId}")
                {
                    Name = "Copy App ID",
                    Icon = new("\ue8c8"),  // Segoe Fluent 'Copy'
                    Result = CommandResult.Hide()
                }
            )
            {
                RequestedShortcut = KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.C)
            },
            ..moreCommands ?? []
        ];
    }

    public static SteamGameListItem FromManifest(AppManifestACF appManifest)
    {
        var appState = appManifest.AppState;
        return new SteamGameListItem(
            appState.name,
            new("\ue7fc"),  // Segoe Fluent 'Game'
            appState.appid,
            [
                new CommandContextItem(
                    new OpenUrlCommand($"steam://store/{appState.appid}")
                    {
                        Name = "Show in Store",
                        Icon = new("\ue7bf"),  // Segoe Fluent 'Shopping Cart'
                        Result = CommandResult.Hide()
                    }
                )
            ]
        )
        {
            Subtitle = $"ID: {appState.appid}; {appState.installdir}",
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


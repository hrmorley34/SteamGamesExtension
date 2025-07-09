using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SteamGamesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamGamesExtension;

public sealed partial class SteamGamesCommandItem : CommandItem
{
    public new SteamGamesPage Command
    {
        get => base.Command as SteamGamesPage ?? throw new InvalidCastException();
        set => base.Command = value;
    }

    public SteamGamesCommandItem(SteamInstance steamInstance)
        : base(new SteamGamesPage(steamInstance))
    {
        Title = "Steam Games";
        Subtitle = "Search for games installed through Steam";
        MoreCommands = [
            new CommandContextItem(
                new OpenUrlCommand("steam://open/games")
                {
                    Name = "Open Library",
                    Icon = new("\ue8f1"),  // Segoe Fluent 'Library'
                    Result = CommandResult.Hide()
                }
            ),
            new CommandContextItem(
                new OpenUrlCommand("steam://store")
                {
                    Name = "Open Store",
                    Icon = new("\ue7bf"),  // Segoe Fluent 'Shopping Cart'
                    Result = CommandResult.Hide()
                }
            ),
            new CommandContextItem(
                new OpenUrlCommand("steam://open/settings")
                {
                    Name = "Open Settings",
                    Icon = new("\ue713"),  // Segoe Fluent 'Settings'
                    Result = CommandResult.Hide()
                }
            ),
        ];
    }
}

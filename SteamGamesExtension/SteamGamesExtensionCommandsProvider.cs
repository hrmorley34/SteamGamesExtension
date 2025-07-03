using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SteamGamesApi;

namespace SteamGamesExtension;

public partial class SteamGamesExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public SteamGamesExtensionCommandsProvider()
    {
        var ep = new SteamGamesExtensionPage(new SteamInstance());
        DisplayName = ep.Name;
        Icon = ep.Icon;
        _commands = [
            new CommandItem(ep) { Subtitle = "Search for games installed through Steam" },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}

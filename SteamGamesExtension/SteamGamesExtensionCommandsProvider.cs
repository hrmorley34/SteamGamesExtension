using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SteamGamesApi;

namespace SteamGamesExtension;

public partial class SteamGamesExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public SteamGamesExtensionCommandsProvider()
    {
        DisplayName = "Steam Games Extension";
        var ci = new SteamGamesCommandItem(new SteamInstance());
        Icon = ci.Command.Icon;
        _commands = [ci];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}

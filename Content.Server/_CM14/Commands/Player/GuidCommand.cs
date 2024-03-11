using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._CM14.Commands.Player;

[AnyCommand]
public sealed class GuidCommand : IConsoleCommand
{
    public string Command => "guid";
    public string Description => "Returning guid of user.";
    public string Help => "Just run.";
    private static readonly string Gold = Color.Gold.ToHex();

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player == null)
        {
            return;
        }

        shell.WriteLine("Скопируй GUID ниже.");
        shell.WriteLine(shell.Player.UserId.ToString());
    }
}

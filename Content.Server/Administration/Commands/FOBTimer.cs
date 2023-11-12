using Content.Server.Xeno.Systems;
using Content.Shared.Administration;
using Content.Shared.Prayer;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Permissions)]
public sealed partial class FOBTimer : IConsoleCommand
{
    public const string CommandName = "fobtime";
    public string Command => CommandName;
    public string Description => "Work with fob time.";
    public string Help => $"fobtime <get/set> <time>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1)
        {
            shell.WriteError("Wrong number of arguments.");
            return;
        }

        var entityManager = IoCManager.Resolve<IEntityManager>();
        var sys = entityManager.System<XenoRuleSystem>();

        if (args[0] == "get")
        {
            shell.WriteLine($"Current: {sys.FobTime}, Max: {XenoRuleSystem.FOBTime}");
        }
        else if (args[0] == "set")
        {
            if (args.Length > 1)
            {
                if (float.TryParse(args[1], out var res))
                {
                    sys.FobTime = res;
                    sys.Fob = false;
                }
                else
                {
                    shell.WriteError($"wrong number");
                }
            }
        }
        else
        {
            shell.WriteError($"wrong argument: {args[0]}");
        }
    }
}

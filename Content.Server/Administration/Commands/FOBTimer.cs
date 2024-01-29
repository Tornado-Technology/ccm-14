using Content.Server._CM14.Barrier;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Round)]
public sealed partial class FOBTimer : IConsoleCommand
{
    public const string CommandName = "fobtime";
    public string Command => CommandName;
    public string Description => "Work with fob time.";
    public string Help => $"fobtime <get/set> <time> or fobtime off";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1)
        {
            shell.WriteError("Wrong number of arguments.");
            return;
        }

        var entityManager = IoCManager.Resolve<IEntityManager>();
        var sys = entityManager.System<CMBarrierSystem>();

        if (args[0] == "get")
        {
            shell.WriteLine($"Current: {sys.BarrierTimer}");
        }
        else if (args[0] == "set")
        {
            if (args.Length > 1)
            {
                if (float.TryParse(args[1], out var res))
                {
                    sys.BarrierTimer = res;
                    sys.BarrierCountdown = true;
                }
                else
                {
                    shell.WriteError($"wrong number");
                }
            }
        }
        else if (args[0] == "off")
        {
            sys.BarrierDisable();
        }
        else
        {
            shell.WriteError($"wrong argument: {args[0]}");
        }
    }
}

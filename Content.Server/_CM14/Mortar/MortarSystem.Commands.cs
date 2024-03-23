using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Map;

namespace Content.Server._CM14.Mortar;

public sealed partial class MortarSystem
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;
    [Dependency] private readonly IMapManager _map = default!;

    private void InitializeCommands()
    {
        _consoleHost.RegisterCommand("mortar_launch", Loc.GetString("mortar-command-mortar-launch-name"), "mortar_launch <uid> <x> <y> <map id>",
            TryLaunchCommand,
            GetTryLaunchCommandCompletion);
    }

    [AdminCommand(AdminFlags.Admin)]
    private void TryLaunchCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if (args.Length != 4)
        {
            shell.WriteError(Loc.GetString("shell-argument-count-must-be-between", ("lower", 4), ("upper", 4)));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var uidNet) || !TryGetEntity(uidNet, out var entityUid) || entityUid is not { } uid)
        {
            shell.WriteError(Loc.GetString("shell-could-not-find-entity", ("entity", args[0])));
            return;
        }

        if (!TryComp<MortarComponent>(uid, out var mortar))
        {
            shell.WriteError(Loc.GetString("shell-entity-with-uid-lacks-component", ("uid", uid), ("componentName", nameof(MortarComponent))));
            return;
        }

        if (!int.TryParse(args[1], out var x))
        {
            shell.WriteError(Loc.GetString("shell-invalid-int"));
            return;
        }

        if (!int.TryParse(args[2], out var y))
        {
            shell.WriteError(Loc.GetString("shell-invalid-int"));
            return;
        }

        if (!int.TryParse(args[3], out var parsedMapId))
        {
            shell.WriteError(Loc.GetString("shell-invalid-int"));
            return;
        }

        var mapId = new MapId(parsedMapId);
        if (!_map.MapExists(mapId))
        {
            shell.WriteError(Loc.GetString("shell-invalid-map-id "));
            return;
        }

        TryLaunch((uid, mortar), new MapCoordinates(x, y, mapId), shell.Player?.AttachedEntity);
    }

    private CompletionResult GetTryLaunchCommandCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(CompletionHelper.Components<MortarComponent>(args[0]), "<uid>"),
            4 => CompletionResult.FromOptions(CompletionHelper.MapIds()),
            _ => CompletionResult.Empty,
        };
    }
}

using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Database;
using Robust.Shared.Console;

namespace Content.Server._CM14.EmergencyRetreat;

public sealed partial class EmergencyRetreatSystem
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;

    private void InitializeCommands()
    {
        _consoleHost.RegisterCommand("emergency_retreat_run_ftl", Loc.GetString("emergency-retreat-command-run-ftl-name"), "emergency_retreat_run_ftl <uid> [forced = false]",
            RunFtlCommand,
            GetEmergencyRetreatCompletion);
    }

    [AdminCommand(AdminFlags.Admin)]
    private void RunFtlCommand(IConsoleShell shell, string _, string[] args)
    {
        if (args.Length is 0 or > 2)
        {
            shell.WriteError(Loc.GetString("shell-argument-count-must-be-between", ("lower", 1), ("upper", 2)));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var uidNet) || !TryGetEntity(uidNet, out var entityUid) || entityUid is not { } uid)
        {
            shell.WriteError(Loc.GetString("shell-could-not-find-entity", ("entity", args[0])));
            return;
        }

        var forced = false;
        if (args.Length == 2)
        {
            if (!bool.TryParse(args[1], out forced))
            {
                shell.WriteError(Loc.GetString("shell-invalid-bool"));
                return;
            }
        }

        if (!TryComp<EmergencyRetreatComponent>(uid, out var emergencyRetreat))
        {
            shell.WriteError(Loc.GetString("shell-entity-with-uid-lacks-component", ("uid", uid), ("componentName", nameof(EmergencyRetreatComponent))));
            return;
        }

        var user = shell.Player?.AttachedEntity;
        if (!Exists(user))
            return;

        if (TryRunFtl((uid, emergencyRetreat), forced))
        {
            _adminLogger.Add(LogType.Action, LogImpact.High, $"{ToPrettyString(user):player} started an emergency evacuation in command line");
            return;
        }

        _adminLogger.Add(LogType.Action, LogImpact.High, $"{ToPrettyString(user):player} tried to start an emergency evacuation in command line");
    }

    private CompletionResult GetEmergencyRetreatCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(CompletionHelper.Components<EmergencyRetreatComponent>(args[0]), "<uid>"),
            2 => CompletionResult.FromOptions(CompletionHelper.Booleans),
            _ => CompletionResult.Empty,
        };
    }
}

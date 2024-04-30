using Content.Shared._CM14.EmergencyRetreat;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Server.GameObjects;

namespace Content.Server._CM14.EmergencyRetreat;

public sealed partial class EmergencyRetreatSystem
{
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;

    private void InitializeConsole()
    {
        SubscribeLocalEvent<EmergencyRetreatConsoleComponent, EntParentChangedMessage>(OnEntParentChangedMessage);
        SubscribeLocalEvent<EmergencyRetreatConsoleComponent, EmergencyRetreatRunFtlMessage>(OnConsoleRunFtlMessage);

        _updatedState += OnUpdateState;
    }

    private void OnEntParentChangedMessage(Entity<EmergencyRetreatConsoleComponent> console, ref EntParentChangedMessage args)
    {
        var owner = Transform(console).ParentUid;
        UpdateState(console, owner);
    }

    private void OnConsoleRunFtlMessage(Entity<EmergencyRetreatConsoleComponent> console, ref EmergencyRetreatRunFtlMessage args)
    {
        var user = args.Actor;
        if (!Exists(user))
            return;

        var owner = Transform(console).ParentUid;
        if (!TryComp<EmergencyRetreatComponent>(owner, out var emergencyRetreat))
            return;

        if (TryRunFtl((owner, emergencyRetreat)))
        {
            _adminLogger.Add(LogType.Action, LogImpact.High, $"{ToPrettyString(user):player} started an emergency evacuation in {ToPrettyString(console)}");
            return;
        }

        _adminLogger.Add(LogType.Action, LogImpact.Medium, $"{ToPrettyString(user):player} tried to start an emergency evacuation in {ToPrettyString(console)}");
    }

    private void OnUpdateState(Entity<EmergencyRetreatComponent> emergencyRetreat)
    {
        // We update the UI of consoles when the state changes
        var query = EntityQueryEnumerator<EmergencyRetreatConsoleComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var console, out var transform))
        {
            // We update the consoles that are on our grid
            if (transform.ParentUid != emergencyRetreat.Owner)
                continue;

            UpdateState((uid, console), emergencyRetreat);
        }
    }

    private void UpdateState(Entity<EmergencyRetreatConsoleComponent> console, EntityUid owner)
    {
        var time = TimeSpan.Zero;
        var state = EmergencyRetreatState.None;

        if (TryComp<EmergencyRetreatComponent>(owner, out var emergencyRetreat))
        {
            // We need a timer, only for two states, for all other cases UI will figure out what to do himself
            time = emergencyRetreat.State switch
            {
                EmergencyRetreatState.FtlPreparation => emergencyRetreat.FtlPreparationTime,
                EmergencyRetreatState.IdlePreparation => emergencyRetreat.IdlePreparationTime,
                _ => TimeSpan.Zero,
            };

            state = emergencyRetreat.State;
        }

        _userInterface.SetUiState(owner, EmergencyRetreatUiKey.Key, new EmergencyRetreatBoundUserInterfaceState(state, time));
    }
}

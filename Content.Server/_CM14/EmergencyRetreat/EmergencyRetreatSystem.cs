using Content.Server.Chat.Systems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Shuttles.Systems;
using Content.Shared._CM14.EmergencyRetreat;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server._CM14.EmergencyRetreat;

public sealed partial class EmergencyRetreatSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly ShuttleSystem _shuttle = default!;

    [Dependency] private readonly ILogManager _log = default!;
    private ISawmill _sawmill = default!;

    private Action<Entity<EmergencyRetreatComponent>>? _updatedState;

    public override void Initialize()
    {
        base.Initialize();

        _sawmill = _log.GetSawmill(SawmillName);

        SubscribeLocalEvent<EmergencyRetreatComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<EmergencyRetreatComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<EmergencyRetreatComponent, FTLStartedEvent>(OnFTLStarted);
        SubscribeLocalEvent<EmergencyRetreatComponent, FTLCompletedEvent>(OnFTLCompleted);

        InitializeConsole();
        InitializeCommands();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<EmergencyRetreatComponent>();
        while (query.MoveNext(out var uid, out var emergencyRetreat))
        {
            var entity = (uid, emergencyRetreat);
            switch (emergencyRetreat.State)
            {
                case EmergencyRetreatState.Idle:
                    break;

                case EmergencyRetreatState.IdlePreparation:
                    // Reloading FTL so that fools don’t spam the chat
                    if (emergencyRetreat.IdlePreparationTime > _timing.CurTime)
                        break;

                    StartIdle(entity);
                    break;

                case EmergencyRetreatState.Ftl:
                    break;

                case EmergencyRetreatState.FtlStarted:
                    break;

                case EmergencyRetreatState.FtlPreparation:
                    // We are waiting for the start of FTL
                    if (emergencyRetreat.FtlPreparationTime > _timing.CurTime)
                        break;

                    StartFtl(entity);
                    break;

                // Of course I'm interested in how you do it, it will be funny
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void OnComponentStartup(Entity<EmergencyRetreatComponent> emergencyRetreat, ref ComponentStartup args)
    {
        _updatedState?.Invoke(emergencyRetreat);
    }

    private void OnComponentRemove(Entity<EmergencyRetreatComponent> emergencyRetreat, ref ComponentRemove args)
    {
        _updatedState?.Invoke(emergencyRetreat);
    }

    private void OnFTLStarted(Entity<EmergencyRetreatComponent> emergencyRetreat, ref FTLStartedEvent args)
    {
        SetState(emergencyRetreat, EmergencyRetreatState.Ftl);
    }

    private void OnFTLCompleted(Entity<EmergencyRetreatComponent> emergencyRetreat, ref FTLCompletedEvent args)
    {
        RunIdle(emergencyRetreat);

        if (args.MapUid != emergencyRetreat.Comp.TargetFtlMapUid)
            return;

        var ev = new EmergencyRetreatDoneEvent();
        RaiseLocalEvent(ev);
    }

    private void RunIdle(Entity<EmergencyRetreatComponent> emergencyRetreat, bool force = false)
    {
        if (force)
        {
            StartIdle(emergencyRetreat);
            return;
        }

        DispatchGlobalAnnouncement(emergencyRetreat,Loc.GetString("emergency-retreat-component-announcement-preparation-idle",
            ("time", Math.Round(emergencyRetreat.Comp.IdlePreparationDelay.TotalSeconds))));

        emergencyRetreat.Comp.IdlePreparationTime = _timing.CurTime + emergencyRetreat.Comp.IdlePreparationDelay;
        SetState(emergencyRetreat, EmergencyRetreatState.IdlePreparation);
    }

    private void StartIdle(Entity<EmergencyRetreatComponent> emergencyRetreat)
    {
        DispatchGlobalAnnouncement(emergencyRetreat, Loc.GetString("emergency-retreat-component-announcement-start-idle"));
        SetState(emergencyRetreat, EmergencyRetreatState.Idle);
    }

    private bool TryRunFtl(Entity<EmergencyRetreatComponent> emergencyRetreat, bool force = false)
    {
        if (force)
        {
            StartFtl(emergencyRetreat);
            return true;
        }

        if (emergencyRetreat.Comp.State != EmergencyRetreatState.Idle)
            return false;

        DispatchGlobalAnnouncement(emergencyRetreat,Loc.GetString("emergency-retreat-component-announcement-preparation-ftl",
            ("time", Math.Round(emergencyRetreat.Comp.FtlPreparationDelay.TotalSeconds))));

        // Let's start preparing to escape
        emergencyRetreat.Comp.FtlPreparationTime = _timing.CurTime + emergencyRetreat.Comp.FtlPreparationDelay;
        SetState(emergencyRetreat, EmergencyRetreatState.FtlPreparation);
        return true;
    }

    private void StartFtl(Entity<EmergencyRetreatComponent> emergencyRetreat)
    {
        DispatchGlobalAnnouncement(emergencyRetreat, Loc.GetString("emergency-retreat-component-announcement-start-ftl"));
        SetState(emergencyRetreat, EmergencyRetreatState.FtlStarted);

        // Forcibly turning the station into a shuttle, lol
        var shuttle = EnsureComp<ShuttleComponent>(emergencyRetreat);

        // We get a ready-made one, or create a map to move
        if (GetFtlTargetMap(emergencyRetreat) is not { } target)
        {
            _sawmill.Error("Unable to get destination, canceling FTL");
            FailFtl(emergencyRetreat);
            return;
        }

        _shuttle.FTLToDock(emergencyRetreat, shuttle, target, emergencyRetreat.Comp.FtlStartupTime, emergencyRetreat.Comp.FtlHyperspaceTime);
    }

    private void FailFtl(Entity<EmergencyRetreatComponent> emergencyRetreat)
    {
        DispatchGlobalAnnouncement(emergencyRetreat, Loc.GetString("emergency-retreat-component-announcement-error-no-target"));
        RunIdle(emergencyRetreat);
    }

    private EntityUid? GetFtlTargetMap(Entity<EmergencyRetreatComponent> emergencyRetreat)
    {
        // Can be used for gaming events
        if (emergencyRetreat.Comp.TargetFtlMapUid is { } targetMapUid && Exists(targetMapUid))
            return targetMapUid;

        // Creating a map, a common thing
        var mapId = _map.CreateMap();
        var mapUid = _map.GetMapEntityId(mapId);

        _map.AddUninitializedMap(mapId);

        if (!_mapLoader.TryLoad(mapId, emergencyRetreat.Comp.TargetFtlMap.CanonPath, out _))
        {
            _sawmill.Error("Error loading map, canceling FTL");
            return null;
        }

        // Save the created map so as not to shit on them
        emergencyRetreat.Comp.TargetFtlMapUid = mapUid;

        _map.DoMapInitialize(mapId);

        return mapUid;
    }

    private void DispatchGlobalAnnouncement(Entity<EmergencyRetreatComponent> emergencyRetreat, string message)
    {
        _chat.DispatchGlobalAnnouncement(message, Loc.GetString("emergency-retreat-component-announcement-sender"),
            announcementSound: emergencyRetreat.Comp.AnnouncementSound,
            colorOverride: emergencyRetreat.Comp.AnnouncementColor);
    }

    private void SetState(Entity<EmergencyRetreatComponent> emergencyRetreat, EmergencyRetreatState state)
    {
        emergencyRetreat.Comp.State = state;

        // Send an event to update all consoles
        _updatedState?.Invoke(emergencyRetreat);
    }
}

using Content.Shared.Actions;
using Content.Shared.Projectiles;
using Content.Shared.Xeno.Components;
using Content.Shared.Xeno.Events;

namespace Content.Shared.Xeno.Systems;

public sealed class XenoEvasionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoEvasionComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoEvasionComponent, XenoEvasionEvent>(OnEvasion);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<XenoEvasionComponent, XenoEvadesComponent>();
        while (query.MoveNext(out var uid, out var evasion, out var _))
        {
            evasion.DurationTime += frameTime;

            if (evasion.DurationTime < evasion.Duration)
                continue;

            evasion.DurationTime = 0;
            RemComp<XenoEvadesComponent>(uid);
        }
    }

    private void OnStartup(Entity<XenoEvasionComponent> ent, ref ComponentStartup args)
    {
        _actions.AddAction(ent, ent.Comp.Action);
    }

    private void OnEvasion(Entity<XenoEvasionComponent> ent, ref XenoEvasionEvent args)
    {
        if (args.Handled)
            return;

        if (HasComp<XenoEvadesComponent>(ent))
            return;

        args.Handled = true;
        AddComp<XenoEvadesComponent>(ent);
    }
}

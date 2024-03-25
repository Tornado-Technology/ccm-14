using Content.Shared.Actions;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using XenoEvadesComponent = Content.Shared._CM14.Xeno.Components.XenoEvadesComponent;
using XenoEvasionComponent = Content.Shared._CM14.Xeno.Components.XenoEvasionComponent;
using XenoEvasionEvent = Content.Shared._CM14.Xeno.Events.XenoEvasionEvent;

namespace Content.Shared._CM14.Xeno.Systems;

public sealed class XenoEvasionSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoEvasionComponent, XenoEvasionEvent>(OnEvasion);
        SubscribeLocalEvent<XenoEvadesComponent, ProjectileHitAttemptEvent>(OnHitAttempt);
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

    private void OnEvasion(Entity<XenoEvasionComponent> ent, ref XenoEvasionEvent args)
    {
        if (args.Handled)
            return;

        if (HasComp<XenoEvadesComponent>(ent))
            return;

        args.Handled = true;
        AddComp<XenoEvadesComponent>(ent);
    }

    private void OnHitAttempt(Entity<XenoEvadesComponent> ent, ref ProjectileHitAttemptEvent args)
    {
        _popup.PopupEntity("Уворот", ent);
        args.Cancel();
    }
}

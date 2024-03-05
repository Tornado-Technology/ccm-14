using Content.Server.Gravity;
using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;

namespace Content.Server._CM14.Xeno;

public sealed class GravityMovementSlowSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _speedModifierSystem = default!;
    [Dependency] private readonly GravitySystem _gravitySystem = default!;

    private const float TimeDelay = 1f;
    private float _currentTimeDelay = TimeDelay;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_currentTimeDelay > 0)
        {
            _currentTimeDelay -= frameTime;
            return;
        }
        _currentTimeDelay = TimeDelay;

        var query = EntityQueryEnumerator<GravityMovementSlowComponent, TransformComponent, MovementSpeedModifierComponent, MovementIgnoreGravityComponent>();
        while (query.MoveNext(out var uid, out var movementSlow, out var xform, out var speedModifier, out _))
        {
            var hasGravity = (TryComp<GravityComponent>(xform.GridUid, out var gravity) && gravity.Enabled ||
                             TryComp<GravityComponent>(xform.MapUid, out var mapGravity) && mapGravity.Enabled);

            if (xform.GridUid == null || !hasGravity)
            {
                if (!movementSlow.IsEnable)
                {
                    movementSlow.IsEnable = true;

                    _speedModifierSystem.ChangeBaseSpeed(uid, speedModifier.BaseWalkSpeed * movementSlow.SlowPercentage,
                        speedModifier.BaseSprintSpeed * movementSlow.SlowPercentage, speedModifier.Acceleration, speedModifier);
                }
            }
            else if (movementSlow.IsEnable)
            {
                movementSlow.IsEnable = false;

                _speedModifierSystem.ChangeBaseSpeed(uid, speedModifier.BaseWalkSpeed / movementSlow.SlowPercentage,
                    speedModifier.BaseSprintSpeed / movementSlow.SlowPercentage, speedModifier.Acceleration, speedModifier);
            }
        }
    }
}

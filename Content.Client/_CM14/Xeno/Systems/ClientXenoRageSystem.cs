using System.Numerics;
using Content.Shared._CM14.Xeno;
using Robust.Client.GameObjects;

namespace Content.Client._CM14.Xeno;

public sealed class ClientXenoRageSystem : EntitySystem
{
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<XenoRageComponent, SpriteComponent>();
        while (query.MoveNext(out _, out var comp, out var sprite))
        {
            sprite.Scale = comp.Enabled ? new Vector2(2, 2) : new Vector2(1, 1);
        }
    }
}

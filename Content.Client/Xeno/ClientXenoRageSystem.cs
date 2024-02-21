using Robust.Client.GameObjects;
using System.Numerics;
using Content.Shared._CM14.Xeno;

namespace Content.Client.Xeno;

public sealed class ClientXenoRageSystem : EntitySystem
{
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<XenoRageComponent, SpriteComponent>();
        while (query.MoveNext(out var uid, out var comp, out var sprite))
        {
            if (comp.Enabled)
                sprite.Scale = new Vector2(2, 2);
            else
                sprite.Scale = new Vector2(1, 1);
        }
    }
}

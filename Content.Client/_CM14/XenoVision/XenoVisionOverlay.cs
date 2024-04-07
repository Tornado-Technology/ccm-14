using Robust.Client.Graphics;

namespace Content.Client._CM14.XenoVision;

using Content.Shared.Eye;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

public sealed class XenoVisionOverlay : Overlay
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public override bool RequestScreenTexture => true;
    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    private ShaderInstance _shader;
    private XenoVisionComponent? _xenoVision;



    public XenoVisionOverlay()
    {
        IoCManager.InjectDependencies(this);
        _shader = _prototypeManager.Index<ShaderPrototype>("GreyscaleFullscreen").InstanceUnique();
    }


    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        var playerEntity = _playerManager.LocalSession?.AttachedEntity;

        if (playerEntity == null)
            return false;

        if (!_entityManager.TryGetComponent<XenoVisionComponent>(playerEntity.Value, out var vision))
            return _overlayMan.RemoveOverlay(this);
        _xenoVision = vision;
        return true;

    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null || _xenoVision == null)
            return;

        _shader.SetParameter("SCREEN_TEXTURE", ScreenTexture);

        var worldHandle = args.WorldHandle;

        worldHandle.UseShader(_shader);
        worldHandle.DrawRect(args.WorldBounds, _xenoVision.Color);
        worldHandle.UseShader(null);
    }
}

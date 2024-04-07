using Content.Shared._CM14.XenoVision;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client._CM14.XenoVision;

[RegisterComponent]
public sealed partial class XenoVisionComponent : SharedXenoVisionComponent
{
    public XenoVisionOverlay? VisionOverlay;

    [DataField]
    public Color Color = Color.FromHex("#00000000");
}

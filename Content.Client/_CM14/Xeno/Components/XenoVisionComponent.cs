using Content.Client._CM14.Xeno.Overlays;
using Content.Shared._CM14.Xeno.Components;

namespace Content.Client._CM14.Xeno.Components;

[RegisterComponent]
public sealed partial class XenoVisionComponent : SharedXenoVisionComponent
{
    public XenoVisionOverlay? VisionOverlay;

    [DataField]
    public Color Color = Color.FromHex("#00000000");

    [DataField]
    public bool Enabled = false;
}

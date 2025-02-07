﻿using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._CM14.Xenos.Acid;

[Serializable, NetSerializable]
public sealed partial class XenoCorrosiveAcidDoAfterEvent : DoAfterEvent
{
    [DataField]
    public EntProtoId AcidId = "XenoAcid";

    [DataField]
    public FixedPoint2 PlasmaCost = 75;

    [DataField]
    public TimeSpan Time = TimeSpan.FromSeconds(30);

    public XenoCorrosiveAcidDoAfterEvent(EntProtoId acidId, TimeSpan time)
    {
        AcidId = acidId;
        Time = time;
    }

    public override DoAfterEvent Clone()
    {
        return this;
    }
}

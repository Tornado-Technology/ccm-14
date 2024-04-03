using Robust.Shared.GameStates;
using Content.Shared.StatusIcon;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._CM14.FriendlyFire;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FriendlyFireComponent : Component
{
    /// <summary>
    /// Is friendly fire active or not
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Enabled=false;

    /// <summary>
    /// Icon of friendly fire enabled
    /// </summary>
    [DataField("enableStatusIcon", customTypeSerializer: typeof(PrototypeIdSerializer<StatusIconPrototype>))]
    public string EnableStatusIcon = "FriendlyFireEnabled";

    /// <summary>
    /// Icon of friendly fire disabled
    /// </summary>
    [DataField("disableStatusIcon", customTypeSerializer: typeof(PrototypeIdSerializer<StatusIconPrototype>))]
    public string DisableStatusIcon = "FriendlyFireDisabled";
}

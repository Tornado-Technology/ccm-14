using Content.Shared.Actions;
using Robust.Shared.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared.Xeno;

[RegisterComponent, NetworkedComponent]

public sealed partial class SharedXenoStunAbilitiesComponent : Component { }
public sealed partial class XenoStunEvent : EntityTargetActionEvent { }


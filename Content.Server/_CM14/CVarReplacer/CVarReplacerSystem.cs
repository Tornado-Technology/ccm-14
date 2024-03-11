using Robust.Shared.Configuration;

namespace Content.Server._CM14.CVarReplacer;

public sealed class CVarReplacerSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    private const string Url = "";
    private const string CvarName = "sponsor.api_url";

    public override void Initialize()
    {
        base.Initialize();
        if (_cfg.IsCVarRegistered(CvarName))
        {
            _cfg.SetCVar(CvarName, Url);
        }
    }
}

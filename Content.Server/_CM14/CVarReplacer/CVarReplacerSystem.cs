using Robust.Shared.Configuration;

namespace Content.Server._CM14.CVarReplacer;

public sealed class CVarReplacerSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    private const string? Url = null;
    private const string CvarName = "sponsor.api_url";

    public override void Initialize()
    {
        base.Initialize();
        if (_cfg.IsCVarRegistered(CvarName) && Url != null)
        {
            _cfg.SetCVar(CvarName, Url);
        }
    }
}

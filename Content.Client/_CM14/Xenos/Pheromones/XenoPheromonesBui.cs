﻿using Content.Client._CM14.Xenos.UI;
using Content.Shared._CM14.Xenos;
using Content.Shared._CM14.Xenos.Pheromones;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._CM14.Xenos.Pheromones;

[UsedImplicitly]
public sealed class XenoPheromonesBui : BoundUserInterface
{
    [Dependency] private readonly IEntityManager _entities = default!;

    private readonly SpriteSystem _sprite;

    [ViewVariables]
    private XenoPheromonesWindow? _window;

    public XenoPheromonesBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);

        _sprite = _entities.System<SpriteSystem>();
    }

    protected override void Open()
    {
        _window = new XenoPheromonesWindow();
        _window.OnClose += Close;

        if (EntMan.HasComponent<XenoComponent>(Owner))
        {
            foreach (var pheromones in Enum.GetValues<XenoPheromones>())
            {
                var name = pheromones.ToString().ToLowerInvariant();
                var control = new XenoChoiceControl();
                var controlSprite =  _sprite.Frame0(new SpriteSpecifier.Rsi(new ResPath("/Textures/_CM/Interface/xeno_pheromones.rsi"), name));
                control.Set(Loc.GetString($"cm-pheromones-{name}"), controlSprite);
                control.Button.OnPressed += _ => SendPredictedMessage(new XenoPheromonesChosenBuiMsg(pheromones));

                _window.Pheromones.AddChild(control);
            }
        }

        _window.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _window?.Dispose();
    }
}

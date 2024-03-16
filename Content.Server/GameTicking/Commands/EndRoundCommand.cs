using Content.Server._CM14.Rules.Xeno;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.GameTicking.Commands
{
    [AdminCommand(AdminFlags.Round)]
    sealed class EndRoundCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _entities = default!;
        public string Command => "endround";
        public string Description => "Ends the round and moves the server to PostRound.";
        public string Help => "Use command: endround {XenoMajor/XenoMinor/MarineMajor/MarineMinor/Neutral}";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var ticker = EntitySystem.Get<GameTicker>();

            if (ticker.RunLevel != GameRunLevel.InRound)
            {
                shell.WriteLine("This can only be executed while the game is in a round.");
                return;
            }

            if (args.Length < 1)
            {
                shell.WriteLine(Help);
                return;
            }

            if (!WinType.TryParse(typeof(WinType), args[0], true, out var wintype))
            {
                shell.WriteLine(Help);
                return;
            }

            if (wintype == null)
            {
                shell.WriteLine(Help);
                return;
            }

            var query = _entities.EntityQueryEnumerator<XenoRuleComponent>();
            while (query.MoveNext(out var uid, out var component))
            {
                component.WinType = (WinType) wintype;
            }

            ticker.EndRound();
        }
    }
}

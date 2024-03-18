using Content.Server._CM14.Requisitions;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._CM14.Commands.Admin;

[AdminCommand(AdminFlags.Admin)]
public sealed class AddBalanceCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entity = default!;

    public string Command => "add_balance";
    public string Description => "Changes the invoice for purchases and orders";
    public string Help => "add_balance <balance>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-argument-count-must-be-between", ("lower", 1), ("upper", 1)));
            return;
        }

        if (!int.TryParse(args[0], out var balance))
        {
            shell.WriteError(Loc.GetString("shell-invalid-int"));
            return;
        }

        var requisitions = _entity.System<RequisitionsSystem>();
        var account = requisitions.GetAccount();

        account.Comp.Balance += balance;
        shell.WriteLine($"Added {balance} to requisition account.");
    }
}

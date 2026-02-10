using System;

namespace Varta.Store.Client.Services;

public interface IBalanceService
{
    decimal CurrentBalance { get; }
    event Action OnChange;
    void SetBalance(decimal balance);
}

public class BalanceService : IBalanceService
{
    public decimal CurrentBalance { get; private set; }
    public event Action? OnChange;

    public void SetBalance(decimal balance)
    {
        CurrentBalance = balance;
        OnChange?.Invoke();
    }
}

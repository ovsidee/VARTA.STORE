using Blazored.LocalStorage;
using Varta.Store.Shared.DTO;

namespace Varta.Store.Client.Services;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    private const string CartKey = "cart";

    public event Action OnChange;

    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task AddToCart(ProductDto product, int quantity = 1)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItemDto>>(CartKey) ?? new List<CartItemDto>();

        var existingItem = cart.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = quantity
            });
        }

        await _localStorage.SetItemAsync(CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task<List<CartItemDto>> GetCartItems()
    {
        var cart = await _localStorage.GetItemAsync<List<CartItemDto>>(CartKey);
        return cart ?? new List<CartItemDto>();
    }

    public async Task RemoveFromCart(int productId)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItemDto>>(CartKey);
        if (cart == null) return;

        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
            await _localStorage.SetItemAsync(CartKey, cart);
            OnChange?.Invoke();
        }
    }

    public async Task ClearCart()
    {
        await _localStorage.RemoveItemAsync(CartKey);
        OnChange?.Invoke();
    }

    public async Task<int> GetCartCount()
    {
        var cart = await _localStorage.GetItemAsync<List<CartItemDto>>(CartKey);
        if (cart == null) return 0;
        return cart.Sum(x => x.Quantity);
    }
}

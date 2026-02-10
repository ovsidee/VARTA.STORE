using Varta.Store.Shared.DTO;

namespace Varta.Store.Client.Services;

public interface ICartService
{
    event Action OnChange;
    Task AddToCart(ProductDto product, int quantity = 1);
    Task<List<CartItemDto>> GetCartItems();
    Task RemoveFromCart(int productId);
    Task ClearCart();
    Task<int> GetCartCount();
}

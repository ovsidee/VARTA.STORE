using Varta.Store.Shared;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Mappers;

public static class ProductMapper
{
    public static Product FromDto(ProductUpdateDto productDto)
    {
        var newProduct = new Product();
        
        newProduct.Name = productDto.Name;
        newProduct.Description = productDto.Description;
        newProduct.Price = productDto.Price;
        newProduct.ImageUrl = productDto.ImageUrl;
        newProduct.CategoryId = productDto.CategoryId;
        newProduct.ServerTagId = productDto.ServerTagId;

        return newProduct;
    }
    
    public static ProductUpdateDto ToDto(Product product)
    {
        var newDto = new ProductUpdateDto();

        newDto.Id = product.Id;
        newDto.Name = product.Name;
        newDto.Description = product.Description;
        newDto.Price = product.Price;
        newDto.ImageUrl = product.ImageUrl;
        newDto.CategoryId = product.CategoryId;
        newDto.ServerTagId = product.ServerTagId;

        return newDto;
    }
}
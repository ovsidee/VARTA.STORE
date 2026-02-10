namespace Varta.Store.Shared.DTO;

public class ProductUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price  { get; set; }
    public string ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int ServerTagId { get; set; }
}
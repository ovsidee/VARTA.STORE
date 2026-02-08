using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varta.Store.Shared;

[Table("Product")]
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } 
    
    [Required]
    [MaxLength(250)]
    public string Description { get; set; } 
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    [Required]
    [MaxLength(220)]
    public string ImageUrl { get; set; } 
    
    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }
    
    [ForeignKey(nameof(ServerTag))]
    public int ServerTagId { get; set; }
    
    public Category Category { get; set; }
    public ServerTag ServerTag { get; set; }
}
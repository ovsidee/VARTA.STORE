using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varta.Store.Shared;

[Table("Category")]
public class Category 
{ 
    [Key] 
    public int Id { get; set; } 
    
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } 
}
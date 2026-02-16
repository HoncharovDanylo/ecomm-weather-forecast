using System.ComponentModel.DataAnnotations;

namespace DomainEntities.Entities;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
}
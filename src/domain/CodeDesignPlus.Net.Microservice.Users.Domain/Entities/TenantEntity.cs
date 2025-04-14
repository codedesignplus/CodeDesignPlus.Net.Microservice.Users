namespace CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

public class TenantEntity : IEntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

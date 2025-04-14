using System;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.DataTransferObjects;

public class TenantDto: IDtoBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

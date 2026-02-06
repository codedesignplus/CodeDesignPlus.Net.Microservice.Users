using System;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Dtos;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

using System;
using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

public abstract class UserBaseDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string FirstName { get;  set; } = null!;
    public string LastName { get;  set; } = null!;
    public string Email { get;  set; } = null!;
    public string Phone { get;  set; } = null!;
    public string? DisplayName { get;  set; } = null!;
    public bool IsActive { get;  set; }
}

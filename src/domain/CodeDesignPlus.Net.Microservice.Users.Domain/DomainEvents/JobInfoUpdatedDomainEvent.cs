using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "JobInfoUpdatedDomainEvent", autoCreate: false)]
public class JobInfoUpdatedDomainEvent(
    Guid aggregateId,
    JobInfo job,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public JobInfo Job { get; } = job;

    public static JobInfoUpdatedDomainEvent Create(Guid aggregateId, JobInfo job)
    {
        return new JobInfoUpdatedDomainEvent(aggregateId, job);
    }
}

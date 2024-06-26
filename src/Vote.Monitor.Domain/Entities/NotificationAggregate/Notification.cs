﻿using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
namespace Vote.Monitor.Domain.Entities.NotificationAggregate;

public class Notification : BaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid SenderId { get; private set; }
    public NgoAdmin Sender { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public IReadOnlyList<MonitoringObserver> TargetedObservers { get; private set; }

    private Notification(ElectionRound electionRound,
        NgoAdmin sender,
        IEnumerable<MonitoringObserver> observers,
        string title,
        string body,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRoundId = electionRound.Id;
        ElectionRound = electionRound;

        Sender = sender;
        SenderId = sender.Id;

        TargetedObservers = observers.ToList().AsReadOnly();
        Timestamp = timeProvider.UtcNow;
        Title = title;
        Body = body;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal Notification()
    {
    }
#pragma warning restore CS8618
}

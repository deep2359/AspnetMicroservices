using System;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvent
    {
        public readonly Guid Id;
        public readonly DateTimeOffset CreationDate;

        public IntegrationBaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTimeOffset.Now;
        }

        public IntegrationBaseEvent(Guid id, DateTimeOffset creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
}

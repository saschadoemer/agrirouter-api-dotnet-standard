﻿using com.dke.data.agrirouter.api.service.parameters;

namespace com.dke.data.agrirouter.api.service.messaging
{
    public interface ISubscriptionService : IMessagingService<SubscriptionParameters>,
        IEncodeMessageService<SubscriptionParameters>
    {
    }
}
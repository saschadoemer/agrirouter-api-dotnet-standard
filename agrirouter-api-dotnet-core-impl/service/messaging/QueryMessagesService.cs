using com.dke.data.agrirouter.api.definitions;
using com.dke.data.agrirouter.impl.service.common;
using com.dke.data.agrirouter.impl.service.messaging.abstraction;

namespace com.dke.data.agrirouter.impl.service.messaging
{
    public class QueryMessagesService : QueryMessageBaseService
    {
        public QueryMessagesService(MessagingService messagingService) : base(messagingService)
        {
        }

        protected override string TechnicalMessageType => TechnicalMessageTypes.DkeFeedMessageQuery;
    }
}
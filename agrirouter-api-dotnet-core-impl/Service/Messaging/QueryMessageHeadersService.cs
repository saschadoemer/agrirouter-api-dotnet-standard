using System;
using Agrirouter.Api.Definitions;
using Agrirouter.Api.Exception;
using Agrirouter.Api.Service.Messaging;
using Agrirouter.Feed.Response;
using Agrirouter.Impl.Service.Common;
using Agrirouter.Impl.Service.messaging.abstraction;
using Google.Protobuf.WellKnownTypes;

namespace Agrirouter.Impl.Service.messaging
{
    /// <summary>
    /// Service to query message headers.
    /// </summary>
    public class QueryMessageHeadersService : QueryMessageBaseService,
        IDecodeMessageResponseService<HeaderQueryResponse>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpMessagingService">-</param>
        /// <param name="encodeMessageService">-</param>
        public QueryMessageHeadersService(HttpMessagingService httpMessagingService, EncodeMessageService encodeMessageService)
            : base(httpMessagingService, encodeMessageService)
        {
        }

        protected override string TechnicalMessageType => TechnicalMessageTypes.DkeFeedHeaderQuery;

        public HeaderQueryResponse Decode(Any messageResponse)
        {
            try
            {
                return HeaderQueryResponse.Parser.ParseFrom(messageResponse.Value);
            }
            catch (Exception e)
            {
                throw new CouldNotDecodeMessageException("Could not decode query message header response.", e);
            }
        }
    }
}
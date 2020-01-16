using System;
using System.Collections.Generic;
using Agrirouter.Request;
using Agrirouter.Request.Payload.Endpoint;
using Agrirouter.Api.Definitions;
using Agrirouter.Api.Dto.Messaging;
using Agrirouter.Api.Service.Messaging;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Impl.Service.Common;
using Google.Protobuf;

namespace Agrirouter.Impl.Service.messaging
{
    /**
     * Service to send capabilites messages.
     */
    public class CapabilitiesService : ICapabilitiesServices
    {
        private readonly MessagingService _messagingService;
        private readonly EncodeMessageService _encodeMessageService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messagingService">-</param>
        /// <param name="encodeMessageService">-</param>
        public CapabilitiesService(MessagingService messagingService, EncodeMessageService encodeMessageService)
        {
            _messagingService = messagingService;
            _encodeMessageService = encodeMessageService;
        }

        /// <summary>
        /// Please see <seealso cref="IMessagingService{T}.Send"/> for documentation.
        /// </summary>
        /// <param name="capabilitiesParameters">-</param>
        /// <returns>-</returns>
        public MessagingResult Send(CapabilitiesParameters capabilitiesParameters)
        {
            var encodedMessages = new List<string> {Encode(capabilitiesParameters).Content};
            var messagingParameters = capabilitiesParameters.BuildMessagingParameter(encodedMessages);
            return _messagingService.Send(messagingParameters);
        }

        /// <summary>
        /// Please see <seealso cref="IEncodeMessageService{T}.Encode"/> for documentation.
        /// </summary>
        /// <param name="capabilitiesParameters"></param>
        /// <returns>-</returns>
        public EncodedMessage Encode(CapabilitiesParameters capabilitiesParameters)
        {
            var messageHeaderParameters = new MessageHeaderParameters
            {
                ApplicationMessageId = capabilitiesParameters.ApplicationMessageId,
                TeamSetContextId = capabilitiesParameters.TeamsetContextId ?? "",
                TechnicalMessageType = TechnicalMessageTypes.DkeCapabilities,
                Mode = RequestEnvelope.Types.Mode.Direct
            };

            var messagePayloadParameters = new MessagePayloadParameters
            {
                TypeUrl = CapabilitySpecification.Descriptor.FullName
            };

            var capabilitySpecification = new CapabilitySpecification
            {
                AppCertificationId = capabilitiesParameters.ApplicationId,
                AppCertificationVersionId = capabilitiesParameters.CertificationVersionId
            };
            capabilitiesParameters.CapabilityParameters.ForEach(capabilityParameter =>
            {
                var capability = new CapabilitySpecification.Types.Capability
                {
                    TechnicalMessageType = capabilityParameter.TechnicalMessageType,
                    Direction = capabilityParameter.Direction
                };
                capabilitySpecification.Capabilities.Add(capability);
            });
            messagePayloadParameters.Value = capabilitySpecification.ToByteString();

            var encodedMessage = new EncodedMessage
            {
                Id = Guid.NewGuid().ToString(),
                Content = _encodeMessageService.Encode(messageHeaderParameters, messagePayloadParameters)
            };

            return encodedMessage;
        }
    }
}
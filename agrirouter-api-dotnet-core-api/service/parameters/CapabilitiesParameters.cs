using System.Collections.Generic;
using Agrirouter.Request.Payload.Endpoint;
using com.dke.data.agrirouter.api.service.parameters.inner;

namespace com.dke.data.agrirouter.api.service.parameters
{
    /// <summary>
    /// Parameter container definition.
    /// </summary>
    public class CapabilitiesParameters : MessageParameters
    {
        public string ApplicationId { get; set; }

        public string CertificationVersionId { get; set; }

        public CapabilitySpecification.Types.PushNotification EnablePushNotifications { get; set; }

        public List<CapabilityParameter> CapabilityParameters { get; set; }
    }
}
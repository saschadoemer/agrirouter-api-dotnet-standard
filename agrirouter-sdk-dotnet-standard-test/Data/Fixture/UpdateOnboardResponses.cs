﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Agrirouter.Api.Definitions;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Api.Env;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Api.Service.Parameters.Inner;
using Agrirouter.Impl.Service.Common;
using Agrirouter.Impl.Service.Messaging;
using Agrirouter.Impl.Service.Onboard;
using Agrirouter.Request.Payload.Endpoint;
using Agrirouter.Test.Helper;
using Xunit;
using Environment = Agrirouter.Api.Env.Environment;

namespace Agrirouter.Test.Data.Fixture
{
    /// <summary>
    /// Fixture creator to update the onboard responses.
    /// </summary>
    [Collection("fixture")]
    public class UpdateOnboardResponses
    {
        private static readonly UtcDataService UtcDataService = new();
        private static readonly HttpClient HttpClient = HttpClientFactory.HttpClient();
        private static readonly Environment Environment = new QualityAssuranceEnvironment();


        [Fact]
        public void Recipient()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee961", "4bb8753248");
            OnboardResponseIntegrationService.Save(Identifier.Http.CommunicationUnit.Recipient, onboardResponse);
        }

        [Fact]
        public void RecipientWithEnabledPushMessages()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee962", "5c51344686");
            OnboardResponseIntegrationService.Save(Identifier.Http.CommunicationUnit.RecipientWithEnabledPushMessages,
                onboardResponse);
        }

        [Fact]
        public void Sender()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee963", "647b976569");
            OnboardResponseIntegrationService.Save(Identifier.Http.CommunicationUnit.Sender, onboardResponse);
        }

        [Fact]
        public void SenderWithMultipleRecipients()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee964", "998e66acb9");
            OnboardResponseIntegrationService.Save(Identifier.Http.CommunicationUnit.SenderWithMultipleRecipients,
                onboardResponse);
        }

        [Fact]
        public void SingleEndpointWithoutRoute()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee965", "1525e3fbe1");
            OnboardResponseIntegrationService.Save(Identifier.Http.CommunicationUnit.SingleEndpointWithoutRoute,
                onboardResponse);
        }

        [Fact]
        public void SingleEndpointWithP12Certificate()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee966", "b0dbe9bd37");
            OnboardResponseIntegrationService.Save(Identifier.Http.CommunicationUnit.SingleEndpointWithP12Certificate,
                onboardResponse);
        }

        [Fact]
        public void SingleEndpointWithPemCertificate()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee967", "b221c182af",
                CertificationTypeDefinition.Pem);
            OnboardResponseIntegrationService.Save(Identifier.Http.CommunicationUnit.SingleEndpointWithPemCertificate,
                onboardResponse);
        }

        [Fact]
        public void SingleMqttEndpointWithoutRoute()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee968", "f6803cbad2",
                gatewayId: GatewayTypeDefinition.Mqtt);
            OnboardResponseIntegrationService.Save(Identifier.Mqtt.CommunicationUnit.SingleEndpointWithoutRoute,
                onboardResponse);
        }

        [Fact]
        public void SingleMqttEndpointWithP12Certificate()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee969", "d90998c89e",
                gatewayId: GatewayTypeDefinition.Mqtt);
            OnboardResponseIntegrationService.Save(Identifier.Mqtt.CommunicationUnit.SingleEndpointWithP12Certificate,
                onboardResponse);
        }

        [Fact]
        public void SingleMqttEndpointWithPemCertificate()
        {
            var onboardResponse = Onboard("97cce58c-7eb0-4284-993b-f0069fdee970", "b4f100f53d",
                gatewayId: GatewayTypeDefinition.Mqtt);
            OnboardResponseIntegrationService.Save(Identifier.Mqtt.CommunicationUnit.SingleEndpointWithPemCertificate,
                onboardResponse);
        }

        private OnboardResponse Onboard(string uuid, string registrationCode,
            string certificationTypeDefinition = "P12", string gatewayId = "3")
        {
            var onboardService = new OnboardService(Environment, UtcDataService, HttpClient);

            var parameters = new OnboardParameters
            {
                Uuid = uuid,
                ApplicationId = Applications.CommunicationUnit.ApplicationId,
                ApplicationType = ApplicationTypeDefinitions.Application,
                CertificationType = certificationTypeDefinition,
                GatewayId = gatewayId,
                RegistrationCode = registrationCode,
                CertificationVersionId = Applications.CommunicationUnit.CertificationVersionId
            };

            var onboardResponse = onboardService.Onboard(parameters);

            Assert.NotEmpty(onboardResponse.DeviceAlternateId);
            Assert.NotEmpty(onboardResponse.SensorAlternateId);
            Assert.NotEmpty(onboardResponse.CapabilityAlternateId);

            Assert.NotEmpty(onboardResponse.Authentication.Certificate);
            Assert.NotEmpty(onboardResponse.Authentication.Secret);
            Assert.NotEmpty(onboardResponse.Authentication.Type);

            Assert.NotEmpty(onboardResponse.ConnectionCriteria.Commands);
            Assert.NotEmpty(onboardResponse.ConnectionCriteria.Measures);
            return onboardResponse;
        }

        private void ValidateConnection(OnboardResponse onboardResponse)
        {
            Thread.Sleep(5000);
            var fetchMessageService = new FetchMessageService(HttpClient);
            var fetch = fetchMessageService.Fetch(onboardResponse);
            Assert.Empty(fetch);
        }

        private void EnableAllCapabilitiesViaHttp(OnboardResponse onboardResponse)
        {
            var capabilitiesServices =
                new CapabilitiesService(new HttpMessagingService(HttpClient));
            var capabilitiesParameters = new CapabilitiesParameters
            {
                OnboardResponse = onboardResponse,
                ApplicationId = Applications.CommunicationUnit.ApplicationId,
                CertificationVersionId = Applications.CommunicationUnit.CertificationVersionId,
                EnablePushNotifications = CapabilitySpecification.Types.PushNotification.Disabled,
                CapabilityParameters = new List<CapabilityParameter>()
            };

            capabilitiesParameters.CapabilityParameters.AddRange(CapabilitiesHelper.AllCapabilities);
            capabilitiesServices.Send(capabilitiesParameters);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            var fetchMessageService = new FetchMessageService(HttpClient);
            var fetch = fetchMessageService.Fetch(onboardResponse);
            Assert.Single(fetch);

            var decodedMessage = DecodeMessageService.Decode(fetch[0].Command.Message);
            Assert.Equal(201, decodedMessage.ResponseEnvelope.ResponseCode);
        }
    }
}
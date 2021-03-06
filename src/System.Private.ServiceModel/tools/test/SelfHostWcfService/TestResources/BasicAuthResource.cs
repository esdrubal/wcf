﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class BasicAuthResource : EndpointResource<WcfUserNameService, IWcfCustomUserNameService>
    {
        protected override string Address { get { return "https-basic"; } }

        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpsPort;
        }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            return binding;
        }

        private ServiceCredentials GetServiceCredentials()
        {
            var serviceCredentials = new ServiceCredentials();
            serviceCredentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            serviceCredentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNameValidator();
            return serviceCredentials;
        }

        protected override void ModifyBehaviors(ServiceDescription desc)
        {
            base.ModifyBehaviors(desc);

            desc.Behaviors.Remove<ServiceCredentials>();
            desc.Behaviors.Add(GetServiceCredentials());
        }
    }
}

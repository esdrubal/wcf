﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class DuplexWebSocketResource : EndpointResource<WcfWebSocketService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "http-defaultduplexwebsockets"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpBinding();
        }
    }

    internal class WebSocketTransportResource : EndpointResource<WcfWebSocketTransportUsageAlwaysService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "http-requestreplywebsockets-transportusagealways"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }
    }

    internal class WebSocketHttpDuplexBinaryStreamedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpDuplexBinaryStreamedResource"; } }

        protected override Binding GetBinding()
        {
            int DefaultMaxReceivedMessageSize = 64 * 1024 * 1024;

            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MaxReceivedMessageSize = DefaultMaxReceivedMessageSize;
            binding.MaxBufferSize = DefaultMaxReceivedMessageSize;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            return binding;
        }
    }

    internal class WebSocketHttpRequestReplyBinaryStreamedResource : EndpointResource<WSRequestReplyService, IWSRequestReplyService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpRequestReplyBinaryStreamedResource"; } }

        protected override Binding GetBinding()
        {
            int DefaultMaxReceivedMessageSize = 64 * 1024 * 1024;

            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MaxReceivedMessageSize = DefaultMaxReceivedMessageSize;
            binding.MaxBufferSize = DefaultMaxReceivedMessageSize;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            return binding;
        }
    }

    internal class WebSocketHttpsDuplexBinaryStreamedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort + 1;
        }

        protected override string Address { get { return "WebSocketHttpsDuplexBinaryStreamedResource"; } }

        protected override Binding GetBinding()
        {
            int DefaultMaxReceivedMessageSize = 64 * 1024 * 1024;

            BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement();
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            httpsTransportBindingElement.MaxReceivedMessageSize = DefaultMaxReceivedMessageSize;
            httpsTransportBindingElement.MaxBufferSize = DefaultMaxReceivedMessageSize;
            httpsTransportBindingElement.TransferMode = TransferMode.Streamed;
            CustomBinding binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);
            return binding;
        }
    }
}

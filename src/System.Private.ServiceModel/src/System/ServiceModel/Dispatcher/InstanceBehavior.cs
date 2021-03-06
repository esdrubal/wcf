﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Diagnostics.Application;

namespace System.ServiceModel.Dispatcher
{
    public class InstanceBehavior
    {
        private IInstanceContextProvider _instanceContextProvider;
        private IInstanceProvider _provider;

        internal InstanceBehavior(DispatchRuntime dispatch, ImmutableDispatchRuntime immutableRuntime)
        {
            _provider = dispatch.InstanceProvider;
            _instanceContextProvider = dispatch.InstanceContextProvider;
        }

        internal IInstanceContextProvider InstanceContextProvider
        {
            get
            {
                return _instanceContextProvider;
            }
        }

        internal void AfterReply(ref MessageRpc rpc, ErrorBehavior error)
        {
            InstanceContext context = rpc.InstanceContext;

            if (context != null)
            {
                try
                {
                    context.UnbindRpc(ref rpc);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    error.HandleError(e);
                }
            }
        }

        internal void EnsureInstanceContext(ref MessageRpc rpc)
        {
            if (rpc.InstanceContext == null)
            {
                throw new ArgumentNullException("rpc.InstanceContext");
            }

            rpc.OperationContext.SetInstanceContext(rpc.InstanceContext);
            rpc.InstanceContext.Behavior = this;

            if (rpc.InstanceContext.State == CommunicationState.Created)
            {
                lock (rpc.InstanceContext.ThisLock)
                {
                    if (rpc.InstanceContext.State == CommunicationState.Created)
                    {
                        rpc.InstanceContext.Open(rpc.Channel.CloseTimeout);
                    }
                }
            }
            rpc.InstanceContext.BindRpc(ref rpc);
        }

        internal object GetInstance(InstanceContext instanceContext)
        {
            if (_provider == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxNoDefaultConstructor));
            }

            return _provider.GetInstance(instanceContext);
        }

        internal object GetInstance(InstanceContext instanceContext, Message request)
        {
            if (_provider == null)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SR.SFxNoDefaultConstructor), request);
            }

            return _provider.GetInstance(instanceContext, request);
        }

        internal void EnsureServiceInstance(ref MessageRpc rpc)
        {
            if (TD.GetServiceInstanceStartIsEnabled())
            {
                TD.GetServiceInstanceStart(rpc.EventTraceActivity);
            }

            rpc.Instance = rpc.InstanceContext.GetServiceInstance(rpc.Request);

            if (TD.GetServiceInstanceStopIsEnabled())
            {
                TD.GetServiceInstanceStop(rpc.EventTraceActivity);
            }
        }
    }
}
﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WcfTestBridgeCommon;

namespace WcfService.CertificateResources
{
    // Resource for generating machine certificates
    // X509 extensions are added for Subject Alt Name (alternative DNS Name) support
    // PUT with a comma-separated list of subject names to create a new certificate
    internal class MachineCertificateResource : EndCertificateResource
    {
        public MachineCertificateResource() : base() { }

        // Requests a certificate to be generated by the Bridge
        // If the certificate requested is for the local machine, for example if 
        // server hostname is: foo.bar.com
        // local address is considered to be: 127.0.0.1, localhost, foo, foo.bar.com
        // Then we also install the certificate to the local machine, because it means we are about to run an HTTPS/SSL test against 
        // this machine. 
        // Otherwise, don't bother installing as the cert is for a remote machine. 
        public override ResourceResponse Put(ResourceRequestContext context)
        {
            X509Certificate2 certificate;

            string subject; 
            if (!context.Properties.TryGetValue(subjectKeyName, out subject) || string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("When PUTting to this resource, specify an non-empty 'subject'", "context.Properties");
            }

            // There can be multiple subjects, separated by ,
            string[] subjects = subject.Split(',');

            bool isLocal = IsLocalMachineResource(subjects[0]);

            lock (s_certificateResourceLock)
            {
                if (!s_createdCertsBySubject.TryGetValue(subjects[0], out certificate))
                {
                    CertificateGenerator generator = CertificateResourceHelpers.GetCertificateGeneratorInstance(context.BridgeConfiguration);

                    if (isLocal)
                    {
                        // If we're PUTting a cert that refers to a hostname local to the bridge, 
                        // return the Local Machine cert that CertificateManager caches and add it to the collection
                        //
                        // If we are receiving a PUT to the same endpoint address as the bridge server, it means that 
                        // a test is going to be run on this box
                        //
                        // In keeping with the semantic of these classes, we must PUT before we can GET a cert
                        certificate = CertificateManager.CreateAndInstallLocalMachineCertificates(generator);
                    }
                    else
                    {
                        certificate = generator.CreateMachineCertificate(subjects).Certificate;
                    }
                    // Cache the certificates
                    s_createdCertsBySubject.Add(subjects[0], certificate);
                    s_createdCertsByThumbprint.Add(certificate.Thumbprint, certificate);
                }
            }

            ResourceResponse response = new ResourceResponse();
            response.Properties.Add(thumbprintKeyName, certificate.Thumbprint);
            response.Properties.Add(isLocalKeyName, isLocal.ToString());

            return response;
        }
    }
}
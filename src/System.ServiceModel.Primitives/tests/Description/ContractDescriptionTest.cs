// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using TestTypes;
using Xunit;

public static class ContractDescriptionTest
{
    [Fact]
    public static void Manually_Generated_Service_Type()
    {
        // -----------------------------------------------------------------------------------------------
        // IDescriptionTestsService:
        //    Contains 2 operations, synchronous versions only.
        // -----------------------------------------------------------------------------------------------
        string results = ContractDescriptionTestHelper.ValidateContractDescription<IDescriptionTestsService>(new ContractDescriptionData
        {
            Operations = new OperationDescriptionData[]
            {
                new OperationDescriptionData
                {
                    Name = "MessageRequestReply",
                    IsOneWay = false,
                    HasTask = false,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                 },
                new OperationDescriptionData
                {
                    Name = "Echo",
                    IsOneWay = false,
                    HasTask = false,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/Echo",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/EchoResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                }
            }
        });

        // Assert.True because results contains informative error failure
        Assert.True(results == null, results);
    }


    [Fact]
    public static void SvcUtil_Generated_Service_Type()
    {
        // -----------------------------------------------------------------------------------------------
        // IDescriptionTestsServiceGenerated:
        //    Generated via SvcUtil and contains sync and Task versions of same 2 operations as above.
        // -----------------------------------------------------------------------------------------------
        string results = ContractDescriptionTestHelper.ValidateContractDescription<IDescriptionTestsServiceGenerated>(new ContractDescriptionData
        {
            Operations = new OperationDescriptionData[]
            {
                new OperationDescriptionData
                {
                    Name = "MessageRequestReply",
                    IsOneWay = false,
                    HasTask = true,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                },
                new OperationDescriptionData
                {
                    Name = "Echo",
                    IsOneWay = false,
                    HasTask = true,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/Echo",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/EchoResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                }
            }
        });

        // Assert.True because results contains informative error failure
        Assert.True(results == null, results);
    }

    [Fact]
    public static void MessageContract_Service_Type()
    {
        // -----------------------------------------------------------------------------------------------
        // IFeedbackService:
        //    Service exposes a single async operation that uses MessageContract.
        //    This variant tests the a MessageContract can build "typed messages" for the ContractDescription.
        // -----------------------------------------------------------------------------------------------
        string results = ContractDescriptionTestHelper.ValidateContractDescription<IFeedbackService>(new ContractDescriptionData
        {
            Operations = new OperationDescriptionData[]
            {
                new OperationDescriptionData
                {
                    Name = "Feedback",
                    IsOneWay = false,
                    HasTask = true,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://app.my.com/MyFeedback/Feedback",
                            Direction = MessageDirection.Input,
                            MessageType = typeof(FeedbackRequest)
                        },
                        new MessageDescriptionData
                        {
                            Action = "*",
                            Direction = MessageDirection.Output,
                            MessageType = typeof(FeedbackResponse)
                        }
                    }
               }
            }
        });

        // Assert.True because results contains informative error failure
        Assert.True(results == null, results);
    }

    [Fact]
    public static void Duplex_ContractDescription_Builds_From_ServiceContract()
    {
        // Arrange
        CustomBinding binding = new CustomBinding();
        binding.Elements.Add(new TextMessageEncodingBindingElement());
        binding.Elements.Add(new HttpTransportBindingElement());
        EndpointAddress address = new EndpointAddress(FakeAddress.HttpAddress);

        //Act
        ChannelFactory<IDuplexHello> factory = new ChannelFactory<IDuplexHello>(binding, address);
        ContractDescription contract = factory.Endpoint.Contract;

        // Assert
        Assert.NotNull(contract);
        Assert.Equal<Type>(typeof(IHelloCallbackContract), contract.CallbackContractType);

        // Duplex contracts capture operations from both the service and the callback type
        Assert.Equal(2, contract.Operations.Count);
        OperationDescription operation = contract.Operations.Find("Hello");
        Assert.True(operation != null, "Failed to find Hello operation in contract.");
        Assert.True(operation.IsOneWay, "Expected Hello operation to be IsOneWay.");

        operation = contract.Operations.Find("Reply");
        Assert.True(operation != null, "Failed to find Reply operation in contract.");
        Assert.True(operation.IsOneWay, "Expected Reply operation to be IsOneWay.");
    }
}

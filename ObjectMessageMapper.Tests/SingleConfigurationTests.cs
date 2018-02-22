//    MIT License
//
//    Copyright(c) 2016
//    Scott Nelson
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Data;
using ObjectMessageMapper.Tests.TestObject;
using ObjectMessageMapper.Tests.TestObject.Exceptions;
using ObjectMessageMapper.Tests.Helpers;

namespace ObjectMessageMapper.Tests
{
    [TestClass]
    public class SingleConfigurationTests
    {
        private enum MyTestEnum
        {
            Type1,
            Type2,
            Type3
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            ObjectMessageMap.Clear();
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ObjectMessageMap.Clear();

            try
            {
                ObjectMessageMap.SetResourceManager(TestResources.ResourceManager);

                // Configurations for all Exceptions
                ObjectMessageMap.Configure<Exception>(cfg =>
                {
                    // Basic simple configuration
                    cfg.For<EndpointNotFoundException>().UseMessage("SERVER_NOT_FOUND");

                    // Nested Configurations with no sub property accessor
                    cfg.For<BaseException>().Configure(cfg2 =>
                    {
                        cfg2.For<PermissionException>().UseMessage("PERMISSION_DENIED");
                        cfg2.For<NotFoundException>().UseMessage("EXCEPTION_NOT_FOUND").WithValue(ex => ObjectMessageMap.GetMessage(ex.EntityType));

                        // Nested Configuration with sub property accessor.
                        cfg2.For<MyCommunicationException>().Configure<Exception>(ex => ex.InnerException, cfg3 =>
                        {
                            // Default Configuration with sub property accessor and type change
                            cfg3.Default().Configure<int>(ex => ex.Data.Count, cfg4 =>
                            {
                                cfg4.For(0).UseMessage("COMMUNICATION_ERROR_0").WithValue(value => string.Format("Count: {0:d}", value));
                                cfg4.For(1).UseMessage("COMMUNICATION_ERROR_1").WithValue(value => string.Format("Count: {0:d}", value));
                                cfg4.For(2).UseMessage("COMMUNICATION_ERROR_2").WithValue(value => string.Format("Count: {0:d}", value));
                            });

                            cfg3.For<ChannelTerminatedException>().UseMessage("SERVER_ERROR");
                            cfg3.For<EndpointNotFoundException>().UseMessage("SERVER_NOT_FOUND2").WithValue(ex => ex.HelpLink);
                            cfg3.For<ServerTooBusyException>().UseMessage(ex => TestResources.SERVER_TO_BUSY);
                        });

                    });

                    // Sub Configuration with Conditionals and a Default
                    cfg.For<AppReturnedErrorException>()
                        .When(ex => ex.ErrorCode == ErrorCodes.Success).UseMessage("RESULT_SUCCESS").WithValue(ex => ex.AppDisplayName)
                        .When(ex => ex.ErrorCode == ErrorCodes.ApplicationSpecificError)
                            .UseMessage(ex => ex.Message)
                                .WithValue(ex => ex.ErrorLogEntryId)
                                .WithValue(ex => ex.AppDisplayName)
                                .WithValue(ex => ex.InstanceDisplayName)
                        .Default().Configure<ErrorCodes>(ex => ex.ErrorCode, cfg2 =>
                        {
                            cfg2.For(ErrorCodes.NoLicenses).UseMessage("NO_LICENSES");
                            cfg2.For(ErrorCodes.AccessDenied).UseMessage("ACCESS_DENIED");
                        });

                    // Sub Configuration with no Default
                    cfg.For<DataException>().Configure(cfg2 =>
                    {
                        cfg2.For<ReadOnlyException>().UseMessage("READ_ONLY");
                        cfg2.For<InvalidConstraintException>().UseMessage("INVALID_CONSTRAINT");

                    });

                    // Default for main Exception configuration
                    cfg.Default().UseMessage("UNKNOWN_SERVER_ERROR").WithValue(ex => ex.Message).WithValue(ex => ex.Source);
                });

                // Another call to ObjectMessageMap to configure entity object.
                // TODO[SN] - When Entity object have a base class, then this should be that class and not object.
                ObjectMessageMap.Configure<object>(cfg =>
                {
                    cfg.For<TestUser>().UseMessage("EntityUser");
                    cfg.For<ApplicationInstance>().UseMessage("ENTITY_APPLICATION_INSTANCE");

                    cfg.For<BaseObject>().Configure(cfg2 =>
                    {
                        cfg2.For<Object1>()
                            .When(obj => obj.Value > 100).UseMessage("OVER_LIMIT")
                            .When(obj => obj.Value < 1).UseMessage("UNDER_LIMIT")
                            .Default().UseMessage("JUST_RIGHT");
                        cfg2.For<Object2>()
                            .When(obj => obj.Value > 1000).UseMessage("OVER_LIMIT")
                            .When(obj => obj.Value < 100).UseMessage("UNDER_LIMIT")
                            .Default().UseMessage(obj => TestResources.JUST_RIGHT);
                        cfg2.For<Object3>()
                            .When(obj => obj.Value > 5000).UseMessage("OVER_LIMIT")
                            .When(obj => obj.Value < 1000).UseMessage("UNDER_LIMIT")
                            .Default().Configure(cfg3 =>
                            {
                                cfg.For<Object3>().UseMessage("JUST_RIGHT");
                            });
                        cfg2.For<Object4>()
                            .When(obj => obj.Value > 1000).Configure(cfg3 =>
                            {
                                cfg3.For<Object4>().Configure<int>(obj => obj.Value, cfg4 =>
                                {
                                    cfg4.For(5000).UseMessage(obj => "Bullseye!");
                                    cfg4.Default().UseMessage(obj => "Missed");
                                });
                            });
                        cfg2.For<Object5>()
                            .When(obj => obj.Value > 1000).UseMessage("OVER_LIMIT")
                            .When(obj => obj.Value < 100).Configure<int>(obj => obj.Value, cfg3 =>
                            {
                                cfg3.For(50).UseMessage(value => "Bullseye!");
                            });

                        cfg2.For<Object6>()
                            .When(obj => obj.Value > 1000).UseMessage("OVER_LIMIT")
                            .Default().UseMessage("JUST_RIGHT");
                    });

                    cfg.For<Customer>().Default().UseMessage("ENTITY_CUSTOMER");

                    cfg.Default().UseMessage(obj => obj.ToString());
                });

                // Another call to ObjectMessageMap to configure enumerated types.
                ObjectMessageMap.Configure<Enum>(cfg =>
                {
                    cfg.For<UserType>("ENUM_USER_TYPE").Configure(cfg2 =>
                    {
                        cfg2.For(UserType.Unknown).UseMessage("ENUM_USER_TYPE_UNKNOWN");
                        cfg2.For(UserType.CustomerAdministrator).UseMessage(userType => "Customer Administrator");
                        cfg2.For(UserType.User).UseMessage(userType => ObjectMessageMap.GetMessage(typeof(TestUser)));
                    });

                    cfg.For<OwnerType>().UseMessage("ENUM_OWNER_TYPE");
                    cfg.For<MyTestEnum>().UseMessage(obj => "My Test Type");

                    cfg.For<UserConfigureStatus>(status => "Configuration Status").Configure(cfg2 =>
                    {
                        cfg2.For(UserConfigureStatus.Configured).UseMessage(status => "Configured");
                        cfg2.For(UserConfigureStatus.NotConfigured).UseMessage(status => "Not Configured");
                    });

                    cfg.Default().Configure(cfg2 =>
                    {
                        cfg2.For<ActorRole>().UseMessage("ENUM_ACTOR");
                    });

                });
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                throw;
            }
        }


        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_Conditional_SubConfigure_NoNewType_ConditionNotFound()
        {
            Object4 obj = new Object4()
            {
                Value = 999
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(typeof(Object4));

            Assert.AreEqual("", value1);
            Assert.AreEqual("", value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_Conditional_Default_UseMessage_ValueNotFound()
        {
            Object5 obj = new Object5()
            {
                Value = 51
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(typeof(Object5));

            Assert.AreEqual("", value1);
            Assert.AreEqual("", value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_Conditional_Default_UseMessage()
        {
            Object6 obj = new Object6()
            {
                Value = 50
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(typeof(Object6));

            Assert.AreEqual(TestResources.JUST_RIGHT, value1);
            Assert.AreEqual("", value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_Conditional_SubConfigure_NewType()
        {
            Object5 obj = new Object5()
            {
                Value = 50
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(typeof(Object5));

            Assert.AreEqual("Bullseye!", value1);
            Assert.AreEqual("", value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_Conditional_SubConfigure_NoNewType()
        {
            Object4 obj = new Object4()
            {
                Value = 5000
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(typeof(Object4));

            Assert.AreEqual("Bullseye!", value1);
            Assert.AreEqual("", value2);
        }

        /// <summary>
        /// Tests that the correct string is returned when the enum configuration
        /// uses a typename accessor lambda in the For methods and also has sub
        /// value configurations and the status object as well as the enum.value
        /// is passed to GetMessage
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_Default_SubConfiguration_Actor_TypeValue()
        {
            string typeName = ObjectMessageMap.GetMessage(typeof(ActorRole));

            Assert.AreEqual(TestResources.ENUM_ACTOR, typeName);
        }

        /// <summary>
        /// Tests when an object uses the default and the default config
        /// is using a lambda to get the message.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_EntityType_SimpleMapping_Default_MessageLambda()
        {
            UnMappedObject unMappedObj = new UnMappedObject();

            string value1 = ObjectMessageMap.GetMessage(unMappedObj);
            string value2 = ObjectMessageMap.GetMessage(unMappedObj.GetType());

            Assert.AreEqual(unMappedObj.ToString(), value1);
            Assert.AreEqual(unMappedObj.ToString(), value2);
        }

        /// <summary>
        /// Tests when an enum type if configured to get the name
        /// from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_EntityType_SimpleMapping_Default_Resources()
        {
            Customer customer = new Customer();

            string value1 = ObjectMessageMap.GetMessage(customer);
            string value2 = ObjectMessageMap.GetMessage(customer.GetType());

            Assert.AreEqual(TestResources.ENTITY_CUSTOMER, value1);
            Assert.AreEqual(TestResources.ENTITY_CUSTOMER, value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_ConditionalDefault_SubConfigure_NoNewType()
        {
            Object3 obj = new Object3()
            {
                Value = 3254
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(typeof(Object3));

            Assert.AreEqual(TestResources.JUST_RIGHT, value1);
            Assert.AreEqual(TestResources.JUST_RIGHT, value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_ConditionalDefault_UseMessage_Accessor()
        {
            Object2 obj = new Object2()
            {
                Value = 99
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(obj.GetType());

            Assert.AreEqual(TestResources.UNDER_LIMIT, value1);
            Assert.AreEqual("", value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with with a
        /// default configuration that has a UseMessage from the resource
        /// table.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_Object_ConditionalDefault_UseMessage()
        {
            Object1 obj = new Object1()
            {
                Value = 101
            };

            string value1 = ObjectMessageMap.GetMessage(obj);
            string value2 = ObjectMessageMap.GetMessage(typeof(Object1));

            Assert.AreEqual(TestResources.OVER_LIMIT, value1);
            Assert.AreEqual("", value2);
        }

        /// <summary>
        /// Tests when an object type has a sub configuration with no default
        /// configuration. The concreate object derives from this type but
        /// its not configured. Therefore it is not found and since there is
        /// no default, an empty message should be returned.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_SubConfig_NotFound_NoDefault()
        {
            DuplicateNameException user = new DuplicateNameException();

            string value1 = ObjectMessageMap.GetMessage(user);
            string value2 = ObjectMessageMap.GetMessage(typeof(DuplicateNameException));

            Assert.AreEqual("", value1);
            Assert.AreEqual(value1, value2);
        }

        /// <summary>
        /// Tests when an enum type if configured to get the name
        /// from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_EntityType_SimpleMapping_Resources_SecondEntry()
        {
            ApplicationInstance user = new ApplicationInstance();

            string value1 = ObjectMessageMap.GetMessage(user);
            string value2 = ObjectMessageMap.GetMessage(typeof(ApplicationInstance));

            Assert.AreEqual(TestResources.ENTITY_APPLICATION_INSTANCE, value1);
            Assert.AreEqual(value1, value2);
        }

        /// <summary>
        /// Tests when an enum type if configured to get the name
        /// from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_EntityType_SimpleMapping_Resources()
        {
            TestUser user = new TestUser();
            
            string value1 = ObjectMessageMap.GetMessage(user);
            string value2 = ObjectMessageMap.GetMessage(typeof(TestUser));

            Assert.AreEqual(TestResources.EntityUser, value1);
            Assert.AreEqual(value1, value2);
        }

        /// <summary>
        /// Tests when an exception is in a sub configured but is not matched.
        /// It then uses the default configuration that uses two data values.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_Default_WithData()
        {
            string errorMessage = DataGenerator.RandomString(50);
            string errorSource = DataGenerator.RandomString(25);
            ServerTooBusyException ex = new ServerTooBusyException(errorMessage)
            {
                Source = errorSource
            };
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(string.Format(TestResources.UNKNOWN_SERVER_ERROR, errorMessage, errorSource), value);
        }

        /// <summary>
        /// Tests when an exception is in a sub configured that is configured
        /// with some conditionals. The expeption does not match the Whens and
        /// uses the default. It then matches to a Sub Value (ErrorCode).
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithConditional_Default_SubValueConfiguration_AccessDenied()
        {
            AppReturnedErrorException ex = new AppReturnedErrorException("")
            {
                ErrorCode = ErrorCodes.AccessDenied
            };
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(TestResources.ACCESS_DENIED, value);
        }

        /// <summary>
        /// Tests when an exception is in a sub configured that is configured
        /// with some conditionals. The expeption does not match the Whens and
        /// uses the default. It then matches to a Sub Value (ErrorCode).
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithConditional_Default_SubValueConfiguration_NoLicenses()
        {
            AppReturnedErrorException ex = new AppReturnedErrorException("")
            {
                ErrorCode = ErrorCodes.NoLicenses
            };
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(TestResources.NO_LICENSES, value);
        }

        /// <summary>
        /// Tests when an exception is in a sub configured that is configured
        /// with some conditionals. The expeption then matches to one of the When
        /// conditions that in turn returns a resource string with a single format
        /// data.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithConditional_WhenMatched_LambdaMessage_WithManyDatas()
        {
            string appName = DataGenerator.RandomString(8);
            string instanceName = DataGenerator.RandomString(19);
            string logEntryId = DataGenerator.RandomString(25);
            string errorMessage = "You had an error. LogEntryID: {0}, Applicaiton: {1}, Instance: {2}";
            AppReturnedErrorException ex = new AppReturnedErrorException(errorMessage)
            {
                ErrorCode = ErrorCodes.ApplicationSpecificError,
                AppDisplayName = appName,
                InstanceDisplayName = instanceName,
                ErrorLogEntryId = logEntryId
            };
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(string.Format(errorMessage, logEntryId, appName, instanceName), value);
        }

        /// <summary>
        /// Tests when an exception is in a sub configured that thin is configured
        /// with some conditionals. The expeption then matches to one of the When
        /// conditions that in turn returns a resource string with a single format
        /// data.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithConditional_WhenMatched_ResourceMessage_WithData()
        {
            string appName = DataGenerator.RandomString(25);
            AppReturnedErrorException ex = new AppReturnedErrorException("")
            {
                ErrorCode = ErrorCodes.Success,
                AppDisplayName = appName
            };
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(string.Format(TestResources.RESULT_SUCCESS, appName), value);
        }

        /// <summary>
        /// Tests when an exception is configured in a nested configuration of a
        /// sub configuration that does not find a match and has to use it's
        /// default Item. The default item is then configured with a sub value
        /// accessor and is mapped to a value type.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithSubAccessor_Default_WithValueConfig()
        {
            PermissionException inner = new PermissionException();
            inner.Data.Add("key1", "value1");
            inner.Data.Add("key2", "value2");
            MyCommunicationException ex = new MyCommunicationException("ServerTooBusy", inner);
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(string.Format(TestResources.COMMUNICATION_ERROR_2, "Count: 2"), value);
        }

        /// <summary>
        /// Tests when an exception is configured in a nested configuration of a
        /// sub configuration that uses an accessor get the string.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithSubAccessor_WithMessageLambda()
        {
            MyCommunicationException ex = new MyCommunicationException("ServerTooBusy", new ServerTooBusyException());
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(TestResources.SERVER_TO_BUSY, value);
        }

        
        /// <summary>
        /// Tests when an exception is configured in a nested configuration of a
        /// sub configuration that uses an accessor to get a sub value and then
        /// gets the string from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithSubAccessor_SimpleMapping_WithFormatData()
        {
            string myLink = DataGenerator.RandomString(15);
            MyCommunicationException ex = new MyCommunicationException("EndpointNotFound", new EndpointNotFoundException() { HelpLink = myLink });
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(string.Format(TestResources.SERVER_NOT_FOUND2, myLink), value);
        }

        /// <summary>
        /// Tests when an exception is configured in a nested configuration of a
        /// sub configuration that uses an accessor get at a sub value and then
        /// gets the string from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_WithSubAccessor_SimpleMapping()
        {
            MyCommunicationException ex = new MyCommunicationException("ChannelTerminated", new ChannelTerminatedException());
            string value = ObjectMessageMap.GetMessage(ex);

            Assert.AreEqual(TestResources.SERVER_ERROR, value);
        }

        /// <summary>
        /// Tests when an exception is configured in a sub configuration and
        /// that it gets the name from the resource file and has a format data
        /// that recusively calls back into the ObjectMessageMap.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_MappedToResourcese_WithData_Recursive()
        {
            NotFoundException ex1 = new NotFoundException(typeof(TestUser));
            NotFoundException ex2 = new NotFoundException(typeof(ApplicationInstance));

            string message1 = ObjectMessageMap.GetMessage(ex1);
            string message2 = ObjectMessageMap.GetMessage(ex2);

            Assert.AreEqual(string.Format(TestResources.EXCEPTION_NOT_FOUND, TestResources.EntityUser), message1);
            Assert.AreEqual(string.Format(TestResources.EXCEPTION_NOT_FOUND, TestResources.ENTITY_APPLICATION_INSTANCE), message2);
        }

        /// <summary>
        /// Tests when an exception is configured in a sub configuration and
        /// that it gets the name from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_SubConfiguration_MappedToResourcese()
        {
            PermissionException ex = new PermissionException();
            string value1 = ObjectMessageMap.GetMessage(ex);
            string value2 = ObjectMessageMap.GetMessage(typeof(PermissionException));

            Assert.AreEqual(value1, value2);

            Assert.AreEqual(TestResources.PERMISSION_DENIED, value1);

        }

        /// <summary>
        /// Tests when an exception is configured with not sub configuration and
        /// that it gets the name from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Exception_Simple_MappedToResourcese_NoSubConfiguration()
        {
            EndpointNotFoundException ex = new EndpointNotFoundException();
            string value1 = ObjectMessageMap.GetMessage(ex);
            string value2 = ObjectMessageMap.GetMessage(typeof(EndpointNotFoundException));

            Assert.AreEqual(value1, value2);

            Assert.AreEqual(TestResources.SERVER_NOT_FOUND, value1);

        }

        /// <summary>
        /// Tests when an enum type if configured to get the name
        /// from the resource file and there is no sub value configuraiton.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_TypeName_MappedToResources_Via_UseMessage_NoTypeValueConfiguration()
        {
            string value = ObjectMessageMap.GetMessage(typeof(MyTestEnum));

            Assert.AreEqual("My Test Type", value);

        }

        /// <summary>
        /// Tests when an enum type if configured to get the name
        /// from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_TypeName_LambdaCallback_Via_UseMessage_NoTypeValueConfiguration()
        {
            string value = ObjectMessageMap.GetMessage(typeof(OwnerType));

            Assert.AreEqual(TestResources.ENUM_OWNER_TYPE, value);

        }

        /// <summary>
        /// Tests that when an enum type if configured to get the type
        /// name from the resources and the enum value is also configure
        /// to get the string from a callback lambda and it recursively
        /// calls back into the ObjectMessageMap for the string of
        /// another type/value.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_UserType_MappedToResources_EnumValue_Lambda_RecursiveCallback()
        {
            UserType userType = UserType.User;

            string value1 = ObjectMessageMap.GetMessage(userType);
            string value2 = ObjectMessageMap.GetMessage(UserType.User);

            Assert.AreEqual(value1, value2);
            Assert.AreEqual(TestResources.EntityUser, value1);
        }


        /// <summary>
        /// Tests that when an enum type if configured to get the type
        /// name from the resources and the enum value is also configure
        /// to get the string from a callback lambda.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_UserType_MappedToResources_EnumValue_WithLambda()
        {
            UserType userType = UserType.CustomerAdministrator;

            string value1 = ObjectMessageMap.GetMessage(userType);
            string value2 = ObjectMessageMap.GetMessage(UserType.CustomerAdministrator);

            Assert.AreEqual(value1, value2);
            Assert.AreEqual("Customer Administrator", value1);
        }

        /// <summary>
        /// Tests when an enum type if configured to get the name
        /// from the resource file.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_UserType_MappedToResources_WithMappedEnumTypes()
        {
            string value = ObjectMessageMap.GetMessage(typeof(UserType));

            Assert.AreEqual(TestResources.ENUM_USER_TYPE, value);

        }

        /// <summary>
        /// Tests that when an enum type if configured to get the type
        /// name from the resources and the enum value is also configure
        /// to get the string from the resources.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_UserType_MappedToResources_EnumType_MappedToResources()
        {
            UserType userType = UserType.Unknown;

            string value1 = ObjectMessageMap.GetMessage(userType);
            string value2 = ObjectMessageMap.GetMessage(UserType.Unknown);

            Assert.AreEqual(value1, value2);
            Assert.AreEqual(TestResources.ENUM_USER_TYPE_UNKNOWN, value1);
        }


        /// <summary>
        /// Tests that the correct string is returned when the enum configuration
        /// uses a typename accessor lambda in the For methods and also has sub
        /// value configurations.
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_HasTypeNameAccessor_WithSubValueConfigurations()
        {
            string typeName = ObjectMessageMap.GetMessage(typeof(UserConfigureStatus));

            Assert.AreEqual("Configuration Status", typeName);
        }

        /// <summary>
        /// Tests that the correct string is returned when the enum configuration
        /// uses a typename accessor lambda in the For methods and also has sub
        /// value configurations and the status object as well as the enum.value
        /// is passed to GetMessage
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_HasTypeNameAccessor_TypeValue_HasAccessor_Configured()
        {
            UserConfigureStatus status = UserConfigureStatus.Configured;
            string typeName1 = ObjectMessageMap.GetMessage(status);
            string typeName2 = ObjectMessageMap.GetMessage(UserConfigureStatus.Configured);

            Assert.AreEqual(typeName1, typeName2);
            Assert.AreEqual("Configured", typeName1);
        }

        /// <summary>
        /// Tests that the correct string is returned when the enum configuration
        /// uses a typename accessor lambda in the For methods and also has sub
        /// value configurations and the status object as well as the enum.value
        /// is passed to GetMessage
        /// </summary>
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_Enum_HasTypeNameAccessor_SubValueConfigurations_HasAccessor_NotConfigured()
        {
            UserConfigureStatus status = UserConfigureStatus.NotConfigured;
            string typeName1 = ObjectMessageMap.GetMessage(status);
            string typeName2 = ObjectMessageMap.GetMessage(UserConfigureStatus.NotConfigured);

            Assert.AreEqual(typeName1, typeName2);
            Assert.AreEqual("Not Configured", typeName1);
        }

    }
}

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KeycloakClient.Test
{
    public class CheckPathConventions
    {
        public CheckPathConventions()
        {
            client = new KeycloakClient(new KeycloakAdminClientOptions
            {
                ClientId = "admin-cli",
                Username = "Admin",
                Password = "Admin",
                ClientScope = "testClientScope",
                Realm = "testRealm",
                Url = new Uri("http://localhost:8080/auth/")
            });
        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsPath()
        {
            var path = client.Realms();
            Assert.Equal("admin/realms", GetFinalPath(path));

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNamePath()
        {
            var path = client.Realms().Name("testName");
            Assert.Equal("admin/realms/testName", GetFinalPath(path));

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsPath()
        {
            var path = client.Realms().Name("testName").Clients();
            Assert.Equal("admin/realms/testName/clients", GetFinalPath(path));

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsIdPath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId");
            Assert.Equal("admin/realms/testName/clients/testClientId", GetFinalPath(path));

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsIdRolesPath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles();
            Assert.Equal("admin/realms/testName/clients/testClientId/roles", GetFinalPath(path));

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsIdRolesNamePath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles().Name("testRoleName");
            Assert.Equal("admin/realms/testName/clients/testClientId/roles/testRoleName", GetFinalPath(path));

        }

        [Trait("Category", "UnitTest")]
        [Fact(Skip = "not ready yet")]
        public void CheckRealmsNameClientsIdRolesNameRoleCompositePath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles().Name("testRoleName").GetRoleCompositesAsync();
            Assert.Equal("admin/realms/testName/clients/testClientId/roles/testRoleName", GetFinalPath(path));

        }

        private IKeycloakClient client;

        private string GetFinalPath(object path)
        {
            try
            {
                const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                var proxyType = path.GetType();
                var instanceField = proxyType.GetField("__interceptors", flags);
                var fieldValue = instanceField.GetValue(path) as object[];

                proxyType = fieldValue[0].GetType();
                instanceField = proxyType.GetField("<Path>k__BackingField", flags);
                return instanceField.GetValue(fieldValue[0]).ToString();
            }
            catch (RuntimeBinderException)
            {
                return "";
            }
        }
    }
}

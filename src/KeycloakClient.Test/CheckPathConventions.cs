using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using System;
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
            GetFinalPath(path).Should().BeEquivalentTo("admin/realms");

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNamePath()
        {
            var path = client.Realms().Name("testName");
            GetFinalPath(path).Should().BeEquivalentTo("admin/realms/testName");

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsPath()
        {
            var path = client.Realms().Name("testName").Clients();
            GetFinalPath(path).Should().BeEquivalentTo("admin/realms/testName/clients");

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsIdPath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId");
            GetFinalPath(path).Should().BeEquivalentTo("admin/realms/testName/clients/testClientId");

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsIdRolesPath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles();
            GetFinalPath(path).Should().BeEquivalentTo("admin/realms/testName/clients/testClientId/roles");

        }

        [Trait("Category", "UnitTest")]
        [Fact]
        public void CheckRealmsNameClientsIdRolesNamePath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles().Name("testRoleName");
            GetFinalPath(path).Should().BeEquivalentTo("admin/realms/testName/clients/testClientId/roles/testRoleName");

        }

        [Trait("Category", "UnitTest")]
        [Fact(Skip = "not ready yet")]
        public void CheckRealmsNameClientsIdRolesNameRoleCompositePath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles().Name("testRoleName").GetRoleCompositesAsync();
            GetFinalPath(path).Should().BeEquivalentTo("admin/realms/testName/clients/testClientId/roles/testRoleName");

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

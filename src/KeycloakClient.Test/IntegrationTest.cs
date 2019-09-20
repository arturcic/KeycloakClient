using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace KeycloakClient.Test
{
    public class IntegrationTests
    {
        public IntegrationTests()
        {
            client = new KeycloakClient(new KeycloakAdminClientOptions
            {
                ClientId = "admin-cli",
                Username = "Admin",
                Password = "Admin",
                Realm = "master",
                Url = new Uri("http://localhost:8080/auth/"),
                
            });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact]
        public async Task GetRealmList()
        {
            var res = await client.Realms().GetAllAsync();


            res.Should().HaveCount(1, "The default lenth of realms should be equal to 1");
            res.First().Enabled.Should().BeTrue("The default realm should be enabled");
            res.First().Id.Should().BeEquivalentTo("master", "The default realm id should be equal to 'master'");
            res.First().Name.Should().BeEquivalentTo("master", "The default realm name should be equal to 'master'");
            res.First().DisplayName.Should().BeEquivalentTo("Keycloak", "The default realm DisplayName should be equal to 'Keycloak'");


        }

        private KeycloakClient client;
       
    }
}

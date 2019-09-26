using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Net.Http;
using KeycloakClient.User.Models;

namespace KeycloakClient.Test
{
    [TestCaseOrderer("KeycloakClient.Test.PriorityOrderer", "KeycloakClient.Test")]
    public class IntegrationTestsClient : IntegrationTestBase
    {
        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetClientList()
        {
            var res = await client.Realms().Name("master").Clients().GetAllAsync();

            res.Should().HaveCount(5, "The count of realm clients should be equal to 5");
            res.First().Enabled.Should().BeTrue("The default realm client should be enabled");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task CreateNewClient()
        {
            var newClient = new Client.Models.Client
            {
                ClientId = "testClientId",
            };
            var res = await client.Realms().Name("master").Clients().CreateAsync(newClient);

            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().HaveLength(36, "The returned Id should have a length of 36");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task GetNewClient()
        {
            var newClient = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter
            {
                ClientId = "testClientId",
            });

            newClient.Should().HaveCount(1, "We should be able to find just created client");
            newClient.First().Enabled.Should().BeFalse("The new client by default is disabled");
            newClient.First().ClientId.Should().BeEquivalentTo("testClientId");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task UpdateNewClient()
        {
            var newClient = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter
            {
                ClientId = "testClientId",
            });

            newClient.Should().HaveCount(1, "The user should be easily found");
            newClient.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var clientID = newClient.First().Id;

            var updatedClient = new Client.Models.Client
            {
                ClientId = newClient.First().ClientId,
                Name = "testClientNameUpdate",
                Description = "testDescriptionUpdate",
                Enabled = true
            };

            var res = await client.Realms().Name("master").Clients().Id(clientID).UpdateAsync(updatedClient);

            res.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task GetUpdatedClient()
        {
            var newClient = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter
            {
                ClientId = "testClientId",
            });

            newClient.Should().HaveCount(1, "The user should be easily found");
            newClient.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var clientID = newClient.First().Id;

            var res2 = await client.Realms().Name("master").Clients().Id(clientID).GetAsync();

            res2.Enabled.Should().BeTrue("The client should be enabled");
            res2.Name.Should().BeEquivalentTo("testClientNameUpdate");
            res2.Description.Should().BeEquivalentTo("testDescriptionUpdate");
        }


        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task DeleteTheUpdatedClient()
        {
            var newClient = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter
            {
                ClientId = "testClientId",
            });

            newClient.Should().HaveCount(1, "The user should be easily found");
            newClient.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var clientID = newClient.First().Id;

            var res = await client.Realms().Name("master").Clients().Id(clientID).DeleteAsync();
            res.Should().BeTrue("The delete client result should be true");


            Exception ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.Realms().Name("testRealm").Clients().Id(clientID).GetAsync());

            ex.Message.Should().Contain("404 (Not Found)");
        }
    }
}

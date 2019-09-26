using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Net.Http;

namespace KeycloakClient.Test
{
    [TestCaseOrderer("KeycloakClient.Test.PriorityOrderer", "KeycloakClient.Test")]
    public class IntegrationTestsRealmAdmin : IntegrationTestBase
    {
        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetRealmList()
        {
            var res = await client.Realms().GetAllAsync();

            res.Should().HaveCount(1, "The default lenth of realms should be equal to 1");
            res.First().Enabled.Should().BeTrue("The default realm should be enabled");
            res.First().Id.Should().BeEquivalentTo("master", "The default realm id should be equal to 'master'");
            res.First().Name.Should().BeEquivalentTo("master", "The default realm name should be equal to 'master'");
            res.First().DisplayName.Should().BeEquivalentTo("Keycloak", "The default realm DisplayName should be equal to 'Keycloak'");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task CreateNewRealm()
        {
            var newRealm = new Realm.Models.Realm
            {
                DisplayName = "test Display name",
                Name = "testRealm",
                Enabled = true
            };
            var res = await client.Realms().CreateAsync(newRealm);

            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().BeEquivalentTo("testRealm".ToString(), "The returned Id shoulb be the same as the sent one");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task UpdateNewRealm()
        {
            var newRealm = new Realm.Models.Realm
            {
                DisplayName = "test Display name update",
                Enabled = false
            };
            var res = await client.Realms().Name("testRealm").UpdateAsync(newRealm);

            res.Should().BeTrue("The update result should be true");

            var res2 = await client.Realms().Name("testRealm").GetAsync();

            res2.Enabled.Should().BeFalse("The realm should be disabled");
            res2.Name.Should().BeEquivalentTo("testRealm", "The default realm name should be equal to 'testRealm'");
            res2.DisplayName.Should().BeEquivalentTo("test Display name update", "The default realm DisplayName should be equal to 'test Display name update'");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task GetUpdatedRealm()
        {
            var res = await client.Realms().Name("testRealm").GetAsync();

            res.Enabled.Should().BeFalse("The realm should be disabled");
            res.Name.Should().BeEquivalentTo("testRealm", "The default realm name should be equal to 'testRealm'");
            res.DisplayName.Should().BeEquivalentTo("test Display name update", "The default realm DisplayName should be equal to 'test Display name update'");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task DeleteTheUpdatedRealm()
        {
            var res = await client.Realms().Name("testRealm").DeleteAsync();
            res.Should().BeTrue("The delete realm result should be true");

            Exception ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.Realms().Name("testRealm").GetAsync());

            ex.Message.Should().Contain("404 (Not Found)");
        }
    }
}

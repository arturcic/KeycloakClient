using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Net.Http;
using KeycloakClient.User.Models;
using KeycloakClient.Group.Models;

namespace KeycloakClient.Test
{
    [TestCaseOrderer("KeycloakClient.Test.PriorityOrderer", "KeycloakClient.Test")]
    public class IntegrationTestsGroup : IntegrationTestBase
    {
        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task CreateNewUser()
        {
            var newGroup = new Group.Models.Group
            {
                Name = "testGroup"
                
            };
            var res = await client.Realms().Name("master").Groups().CreateAsync(newGroup);

            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().HaveLength(36, "The returned Id should have a length of 36");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task GetGroupsList()
        {
            var res = await client.Realms().Name("master").Groups().GetAllAsync();

            res.Should().HaveCount(1, "The default lenth of realm users should be equal to 1");
            res.First().Name.Should().BeEquivalentTo("testGroup", "The group name should be equal to 'testGroup'");
            res.First().Path.Should().BeEquivalentTo("/testGroup", "The group name should be equal to '/testGroup'");
            
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task UpdateGroup()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new GroupFilter()
            {
                Search = "testGroup"
            });

            group.Should().HaveCount(1, "The group should be easily found");
            group.First().Id.Should().NotBeNull("Did the new group creation test run successfully???");

            var groupID = group.First().Id;

            var updatedGroup = new Group.Models.Group
            {
                Name = "updatedGroup"
            };

            var res = await client.Realms().Name("master").Groups().Id(groupID).UpdateAsync(updatedGroup);

            res.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task GetUpdatedGroup()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new GroupFilter()
            {
                Search = "updatedGroup"
            });

            group.Should().HaveCount(1, "The group should be easily found");
            group.First().Id.Should().NotBeNull("Did the new group creation test run successfully???");

            var groupID = group.First().Id;

            var res2 = await client.Realms().Name("master").Groups().Id(groupID).GetAsync();

            res2.Name.Should().BeEquivalentTo("updatedGroup", "The group name should be equal to 'updatedGroup'");
            res2.Path.Should().BeEquivalentTo("/updatedGroup", "The group name should be equal to '/updatedGroup'");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task AddAnUserToGroup()
        {

            var group = await client.Realms().Name("master").Groups().FindAsync(new GroupFilter()
            {
                Search = "updatedGroup"
            });

            group.Should().HaveCount(1, "The group should be easily found");
            group.First().Id.Should().NotBeNull("Did the new group creation test run successfully???");

            var groupID = group.First().Id;

            var user = await client.Realms().Name("master").Users().FindAsync(new UserFilter()
            {
                Username = "admin"
            });

            user.Should().HaveCount(1, "The user should be easily found");
            user.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var userID = user.First().Id;

            var res2 = await client.Realms().Name("master").Users().Id(userID).JoinGroupAsync(groupID);

            res2.Should().BeTrue("The user should be added to test group");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetGroupMembers()
        {

            var group = await client.Realms().Name("master").Groups().FindAsync(new GroupFilter()
            {
                Search = "updatedGroup"
            });

            group.Should().HaveCount(1, "The group should be easily found");
            group.First().Id.Should().NotBeNull("Did the new group creation test run successfully???");

            var groupID = group.First().Id;

            var users = await client.Realms().Name("master").Groups().Id(groupID).GetUsersAsync();

            users.Should().HaveCount(1, "The user should be easily found");
            users.First().Username.Should().BeEquivalentTo("admin", "The user Admin should be added to test group");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task DeleteTheGroup()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new GroupFilter()
            {
                Search = "updatedGroup"
            });

            group.Should().HaveCount(1, "The group should be easily found");
            group.First().Id.Should().NotBeNull("Did the new group creation test run successfully???");

            var groupID = group.First().Id;

            var res = await client.Realms().Name("master").Groups().Id(groupID).DeleteAsync();
            res.Should().BeTrue("The delete group result should be true");


            Exception ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.Realms().Name("testRealm").Groups().Id(groupID).GetAsync());

            ex.Message.Should().Contain("404 (Not Found)");
        }
    }
}

using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using KeycloakClient.User.Models;

namespace KeycloakClient.Test
{
    [TestCaseOrderer("KeycloakClient.Test.PriorityOrderer", "KeycloakClient.Test")]
    public class IntegrationTestsUser : IDisposable
    {
        public IntegrationTestsUser()
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
        [Fact, TestPriority(1)]
        public async Task GetUsersList()
        {
            var res = await client.Realms().Name("master").Users().GetAllAsync();

            res.Should().HaveCount(1, "The default lenth of realm users should be equal to 1");
            res.First().Enabled.Should().BeTrue("The default realm user should be enabled");
            res.First().EmailVerified.Should().BeFalse("The default realm user's email should not be verified");
            res.First().Username.Should().BeEquivalentTo("admin", "The default realm user name should be equal to 'admin'");
            res.First().Email.Should().BeNull("The default realm user email should be 'EMPTY string'");
            res.First().FirstName.Should().BeNull("The default realm user first name should be 'EMPTY string'");
            res.First().LastName.Should().BeNull("The default realm user last name should be 'EMPTY string'");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task CreateNewUser()
        {
            var newUser = new User.Models.User
            {
                Username = "testUserName",
                Email = "testUserMail",
                FirstName = "testUserFirstName",
                LastName = "testUserLastName",
                Enabled = false,
                Id = "testUserId"
            };
            var res = await client.Realms().Name("master").Users().CreateAsync(newUser);

            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().HaveLength(36, "The returned Id should have a length of 36");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task UpdateNewUser()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new UserFilter()
            {
                FirstName = "testUserFirstName",
                LastName = "testUserLastName"
            });

            user.Should().HaveCount(1, "The user should be easily found");
            user.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var userID = user.First().Id;

            var updatedUser = new User.Models.User
            {
                Email = "testUserMailUpdate",
                FirstName = "testUserFirstNameUpdate",
                LastName = "testUserLastNameUpdate",
                Enabled = true
            };

            var res = await client.Realms().Name("master").Users().Id(userID).UpdateAsync(updatedUser);

            res.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task GetUpdatedUser()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new UserFilter()
            {
                FirstName = "testUserFirstNameUpdate",
                LastName = "testUserLastNameUpdate"
            });

            user.Should().HaveCount(1, "The user should be easily found");
            user.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var userID = user.First().Id;

            var res2 = await client.Realms().Name("master").Users().Id(userID).GetAsync();

            res2.Enabled.Should().BeTrue("The user should be enabled");
            res2.FirstName.Should().BeEquivalentTo("testUserFirstNameUpdate", "The new user first name should be equal to 'testUserFirstNameUpdate'");
            res2.LastName.Should().BeEquivalentTo("testUserLastNameUpdate", "The new user last name should be equal to 'testUserLastNameUpdate'");
            res2.Email.Should().BeEquivalentTo("testUserMailUpdate", "The new user email should be equal to 'testUserMailUpdate'");
            res2.Enabled.Should().BeTrue("The user should be enabled");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task AddUpdatedUserToAGroup()
        {

            var groupRes = await client.Realms().Name("master").Groups().CreateAsync(new Group.Models.Group() { Name = "TestUserTestGroup" });

            groupRes.Success.Should().BeTrue("The group should be created");
            groupRes.Id.Should().NotBeNull("The group id should be valid");

            var groupID = groupRes.Id;

            var user = await client.Realms().Name("master").Users().FindAsync(new UserFilter()
            {
                FirstName = "testUserFirstNameUpdate",
                LastName = "testUserLastNameUpdate"
            });

            user.Should().HaveCount(1, "The user should be easily found");
            user.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var userID = user.First().Id;

            var res2 = await client.Realms().Name("master").Users().Id(userID).JoinGroupAsync(groupID);

            res2.Should().BeTrue("The user should be added to test group");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetUpdatedUserGroups()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new UserFilter()
            {
                FirstName = "testUserFirstNameUpdate",
                LastName = "testUserLastNameUpdate"
            });

            user.Should().HaveCount(1, "The user should be easily found");
            user.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var userID = user.First().Id;

            var res2 = await client.Realms().Name("master").Users().Id(userID).GetGroupsAsync();

            res2.Should().HaveCount(1, "The user should be added to test group");
            res2.First().Name.Should().BeEquivalentTo("TestUserTestGroup", "The user should be added to the right group 'TestUserTestGroup'");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task RemoveUpdatedUserFromAGroup()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new UserFilter()
            {
                FirstName = "testUserFirstNameUpdate",
                LastName = "testUserLastNameUpdate"
            });

            user.Should().HaveCount(1, "The user should be easily found");
            user.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var userID = user.First().Id;

            var res2 = await client.Realms().Name("master").Users().Id(userID).GetGroupsAsync();

            res2.Should().HaveCount(1, "The user should be added to test group");
            res2.First().Name.Should().BeEquivalentTo("TestUserTestGroup", "The user should be added to the right group 'TestUserTestGroup'");

            var groupID = res2.First().Id;

            var res = await client.Realms().Name("master").Users().Id(userID).LeaveGroupAsync(groupID);

            res.Should().BeTrue("The user should be removed from the test group");

            res2 = await client.Realms().Name("master").Users().Id(userID).GetGroupsAsync();

            res2.Should().HaveCount(0, "The user should have no groups assigned to...");


            res = await client.Realms().Name("master").Groups().Id(groupID).DeleteAsync();
            res.Should().BeTrue("The delete group result should be true");

        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task DeleteTheUpdatedUser()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new UserFilter()
            {
                FirstName = "testUserFirstNameUpdate",
                LastName = "testUserLastNameUpdate"
            });

            user.Should().HaveCount(1, "The user should be easily found");
            user.First().Id.Should().NotBeNull("Did the new user creation test run successfully???");

            var userID = user.First().Id;

            var res = await client.Realms().Name("master").Users().Id(userID).DeleteAsync();
            res.Should().BeTrue("The delete user result should be true");


            Exception ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.Realms().Name("testRealm").Users().Id(userID).GetAsync());

            ex.Message.Should().Contain("404 (Not Found)");
        }

        public void Dispose()
        {
            client = null;
        }

        private KeycloakClient client;
       
    }
}

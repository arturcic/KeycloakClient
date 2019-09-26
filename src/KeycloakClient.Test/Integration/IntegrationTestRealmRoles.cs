using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Net.Http;
using KeycloakClient.User.Models;
using System.Collections.Generic;

namespace KeycloakClient.Test
{
    [TestCaseOrderer("KeycloakClient.Test.PriorityOrderer", "KeycloakClient.Test")]
    public class IntegrationTestsRealmRoles : IDisposable
    {
        public IntegrationTestsRealmRoles()
        {
            client = new KeycloakClient(new KeycloakAdminClientOptions
            {
                ClientId = "admin-cli",
                Username = "Admin",
                Password = "Admin",
                Realm = "master",
                Url = new Uri("http://localhost:8080/auth/")                
            });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetRealmRolesList()
        {
            var res = await client.Realms().Name("master").Roles().GetAllAsync();

            res.Should().HaveCount(4, "The count of realm rols should be equal to 4");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task CreateNewRole()
        {
            var newRole = new Role.Models.Role
            {
                Name = "newRole"
            };
            var res = await client.Realms().Name("master").Roles().CreateAsync(newRole);

            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().BeEquivalentTo("newRole", "The current implementation should return the role name for new role creation");

        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task GetNewRoleByName()
        {
            var newRole = await client.Realms().Name("master").Roles().Name("newRole").GetAsync();

            newRole.Id.Should().HaveLength(36, "The returned Id should have a length of 36");
            newRole.Name.Should().BeEquivalentTo("newRole");
            newRole.Composite.Should().BeFalse("By default all new roles are not composite");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task UpdateNewRole()
        {
            var updatedRole = new Role.Models.Role
            {
                Description = "testDescriptionUpdate",
                Composite = true, // this is ignored
                Name = "newRole",
                ClientRole = true // this is ignored                
            };

            var res = await client.Realms().Name("master").Roles().Name(updatedRole.Name).UpdateAsync(updatedRole);

            res.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task UpdateNewRoleAddComposite()
        {
            var admin = await client.Realms().Name("master").Roles().Name("admin").GetAsync();
            var roles = new List< Role.Models.Role> { admin };

            var res = await client.Realms().Name("master").Roles().Name("newRole").AddAsync(roles);

            res.Success.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetUpdatedRoleComposite()
        {
            var composites = await client.Realms().Name("master").Roles().Name("newRole").GetRealmRoleCompositesAsync();

            composites.Should().HaveCount(1, "The just added composite role");
            composites.First().Name.Should().BeEquivalentTo("admin");
            composites.First().ClientRole.Should().BeFalse();
        }


        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task GetRealmRolesRealms()
        {
            var realms = await client.Realms().Name("master").Roles().Name("newRole").GetRealmRoleCompositesAsync();

            realms.Should().HaveCount(1, "By default only admin is added as main realm to role");
            realms.First().Name.Should().BeEquivalentTo("admin");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task DeleteTheUpdatedRoleComposite()
        {
            var composites = await client.Realms().Name("master").Roles().Name("newRole").GetRealmRoleCompositesAsync();

            var res = await client.Realms().Name("master").Roles().Name("newRole").RemoveAsync(composites);
            res.Should().BeTrue("The delete client result should be true");

            composites = await client.Realms().Name("master").Roles().Name("newRole").GetRealmRoleCompositesAsync();
            composites.Should().HaveCount(0, "All composites should be deleted");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(9)]
        public async Task GetRealmRolesUsers()
        {
            var users = await client.Realms().Name("master").Roles().Name("admin").GetUsersAsync();

            users.Should().HaveCount(1, "By default only admin is added to admin role");
            users.First().Username.Should().BeEquivalentTo("admin");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(10)]
        public async Task GetRealmRolesClientRoles()
        {
            var clientRole = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter(){ ClientId = "account" });

            var roles = await client.Realms().Name("master").Roles().Name("admin").GetClientRoleCompositesAsync(clientRole.First().Id);

            roles.Should().HaveCount(0, "By default should be empty");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(11)]
        public async Task DeleteTheUpdatedRole()
        {

            var res = await client.Realms().Name("master").Roles().Name("newRole").DeleteAsync();
            res.Should().BeTrue("The delete client result should be true");


            Exception ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.Realms().Name("testRealm").Roles().Name("newRole").GetAsync());

            ex.Message.Should().Contain("404 (Not Found)");
        }

        public void Dispose()
        {
            client = null;
        }

        private KeycloakClient client;
       
    }
}

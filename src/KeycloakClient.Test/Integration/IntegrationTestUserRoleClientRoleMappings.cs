using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace KeycloakClient.Test
{

    [TestCaseOrderer("KeycloakClient.Test.PriorityOrderer", "KeycloakClient.Test")]
    public class IntegrationTestsUserClientRolesMapping : IntegrationTestBase
    {

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetEffectiveClientLevelRoleMappingsForUser()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).GetEffectiveAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "manage-account", "manage-account-links", "view-profile" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task GetActiveClientLevelRoleMappingsForUser()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).GetAllAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "manage-account", "view-profile" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task GetAvailableClientLevelRoleMappingsForUser()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).GetAvailableAsync();

            mappings.Select(el => el.Name).Should().NotContain(new[] { "manage-account", "view-profile", "manage-account-links" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task AddClienLevelRolesToUserRoleMappings()
        {

            var newRole = new Role.Models.Role
            {
                Name = "newClientRoleMapping"
            };
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            
            var res = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().CreateAsync(newRole);
            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().BeEquivalentTo("newClientRoleMapping", "The current implementation should return the role name for new role creation");

            var role = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name(newRole.Name).GetAsync();
            var roles = new List<Role.Models.Role> { role };
            res = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).AddRolesAsync(roles);
            res.Success.Should().BeTrue("The creation result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task GetActiveClientLevelNewRoleMappingsForUser()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).GetAllAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "newClientRoleMapping" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task DeleteRealmRoleMappingFromUserWithID()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });

            var role = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newClientRoleMapping").GetAsync();
            var roles = new List<Role.Models.Role> { role };
            var res = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).RemoveRolesAsync(roles);

            res.Should().BeTrue("The deletion result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task GetActiveClientLevelRemovedRoleMappingsForUser()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).GetAllAsync();

            mappings.Select(el => el.Name).Should().NotContain(new[] { "newClientRoleMapping" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task GetAvailableClientLevelNewRoleMappingsForUser()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Client(clients.First().Id).GetAvailableAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "newClientRoleMapping" });
        }
                
        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(9)]
        public async Task DeleteMappingRole()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });

            var res = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newClientRoleMapping").DeleteAsync();
            res.Should().BeTrue("The deletion result should be true");
        }
    }
}

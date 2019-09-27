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
    public class IntegrationTestsGroupClientRolesMapping : IntegrationTestBase
    {

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetRealmRoleMappingToGroupWithID()
        {
            var newGroup = new Group.Models.Group()
            {
                Name = "groupClientRolsTest"
            };

            var res = await client.Realms().Name("master").Groups().CreateAsync(newGroup);
            res.Success.Should().BeTrue();

        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task GetEffectiveClientLevelRoleMappingsForGroup()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
                        
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).GetEffectiveAsync();

            mappings.Select(el => el.Name).Should().BeEmpty("By default new group has no effective roles attached");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task GetActiveClientLevelRoleMappingsForGroup()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });

            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).GetAllAsync();

            mappings.Select(el => el.Name).Should().BeEmpty("By default new group has no active roles attached");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task GetAvailableClientLevelRoleMappingsForGroup()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).GetAvailableAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "manage-account", "view-profile", "manage-account-links" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task AddClienLevelRolesToGroupMappings()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var role = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("view-profile").GetAsync();
            var roles = new List<Role.Models.Role> { role };
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
            var res = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).AddRolesAsync(roles);
            res.Success.Should().BeTrue("The creation result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetActiveClientLevelNewRoleMappingsForGroup()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).GetAllAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "view-profile" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task DeleteRealmRoleMappingFromGroupWithID()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            
            var role = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("view-profile").GetAsync();
            var roles = new List<Role.Models.Role> { role };
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
            var res = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).RemoveRolesAsync(roles);

            res.Should().BeTrue("The deletion result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task GetActiveClientLevelRemovedRoleMappingsForUser()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).GetAllAsync();

            mappings.Select(el => el.Name).Should().NotContain(new[] { "view-profile" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(9)]
        public async Task GetAvailableClientLevelNewRoleMappingsForGroups()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Client(clients.First().Id).GetAvailableAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "view-profile" });
        }
                
        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(10)]
        public async Task DeleteTheGroup()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupClientRolsTest" });
            var res = await client.Realms().Name("master").Groups().Id(group.First().Id).DeleteAsync();
            res.Should().BeTrue("The creation result should be true");
        }
    }
}

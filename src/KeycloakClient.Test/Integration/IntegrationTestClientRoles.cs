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
    public class IntegrationTestsClientRoles : IntegrationTestBase
    {

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetRolesForClientWithID()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });

            clients.Should().HaveCount(1);

            var roles = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().GetAllAsync();

            roles.Should().HaveCount(3);
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task CreateNewRoleForClientWithID()
        {
            var newRole = new Role.Models.Role
            {
                Name = "newRoleForClientWithID"
            };
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var res = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().CreateAsync(newRole);

            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().BeEquivalentTo("newRoleForClientWithID");

        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task GetNewRoleByNameForClientWithID()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var role = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").GetAsync();

            role.Name.Should().BeEquivalentTo("newRoleForClientWithID");
            role.Description.Should().BeEquivalentTo("testDescriptionUpdate");
            role.Id.Should().BeEquivalentTo(role.Id);
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task UpdateNewRoleByNameForClientWithID()
        {
            var updatedRole = new Role.Models.Role
            {
                Description = "testDescriptionUpdate",
                Name = "newRoleForClientWithID",
                Composite = true, // this is ignored
                ClientRole = true // this is ignored                
            };
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            
            var res = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name(updatedRole.Name).UpdateAsync(updatedRole);

            res.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task UpdateNewRoleByNameForClientWithIDAddComposite()
        {
            var admin = await client.Realms().Name("master").Roles().Name("admin").GetAsync();
            var roles = new List< Role.Models.Role> { admin };

            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var res = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").AddAsync(roles);

            res.Success.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetUpdatedRoleByNameForClientWithIDComposite()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var role = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").GetAsync();
            var composites = await client.Realms().Name("master").RolesById(role.Id).GetRealmRoleCompositesAsync();

            composites.Should().HaveCount(1, "The just added composite role");
            composites.First().Name.Should().BeEquivalentTo("admin");
            composites.First().ClientRole.Should().BeFalse();
        }


        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task GetRealmRolesByNameForClientWithIDRealms()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var realms = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").GetRealmRoleCompositesAsync();

            realms.Should().HaveCount(1, "By default only admin is added as main realm to role");
            realms.First().Name.Should().BeEquivalentTo("admin");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task DeleteTheUpdatedRoleByNameForClientWithIDComposite()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var composites = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").GetRealmRoleCompositesAsync();

            var res = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").RemoveAsync(composites);
            res.Should().BeTrue("The delete client result should be true");

            composites = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").GetRealmRoleCompositesAsync();
            composites.Should().HaveCount(0, "All composites should be deleted");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(9)]
        public async Task GetRealmRolesClientRolesByNameForClientWith()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var clientRole = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter(){ ClientId = "account" });

            var roles = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").GetClientRoleCompositesAsync(clientRole.First().Id);

            roles.Should().HaveCount(0, "By default should be empty");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(10)]
        public async Task DeleteTheUpdatedRoleByNameForClientWith()
        {
            var clients = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter() { ClientId = "account" });
            var res = await client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").DeleteAsync();
            res.Should().BeTrue("The delete client result should be true");


            Exception ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.Realms().Name("master").Clients().Id(clients.First().Id).Roles().Name("newRoleForClientWithID").GetAsync());

            ex.Message.Should().Contain("404 (Not Found)");
        }
    }
}

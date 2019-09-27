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
    public class IntegrationTestsRealmRolesByID : IntegrationTestBase
    {

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetRoleByID()
        {
            var role = await client.Realms().Name("master").Roles().Name("admin").GetAsync();

            role.Name.Should().BeEquivalentTo("admin");

            var roleByID = await client.Realms().Name("master").RolesById(role.Id).GetAsync();

            roleByID.Name.Should().BeEquivalentTo("admin");
            roleByID.Id.Should().BeEquivalentTo(role.Id);
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task CreateNewRole()
        {
            var newRole = new Role.Models.Role
            {
                Name = "newRoleByID"
            };
            var res = await client.Realms().Name("master").Roles().CreateAsync(newRole);

            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().BeEquivalentTo("newRoleByID");

        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task GetNewRoleByID()
        {
            var role = await client.Realms().Name("master").Roles().Name("newRoleByID").GetAsync();

            role.Name.Should().BeEquivalentTo("newRoleByID");

            var roleByID = await client.Realms().Name("master").RolesById(role.Id).GetAsync();

            roleByID.Name.Should().BeEquivalentTo("newRoleByID");
            roleByID.Description.Should().BeEquivalentTo("testDescriptionUpdate");
            roleByID.Id.Should().BeEquivalentTo(role.Id);
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task UpdateNewRoleByID()
        {
            var updatedRole = new Role.Models.Role
            {
                Description = "testDescriptionUpdate",
                Name = "newRoleByID",
                Composite = true, // this is ignored
                ClientRole = true // this is ignored                
            };
            var role = await client.Realms().Name("master").Roles().Name("newRoleByID").GetAsync();

            var res = await client.Realms().Name("master").RolesById(role.Id).UpdateAsync(updatedRole);

            res.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task UpdateNewRoleByIDAddComposite()
        {
            var role = await client.Realms().Name("master").Roles().Name("newRoleByID").GetAsync();

            var admin = await client.Realms().Name("master").Roles().Name("admin").GetAsync();
            var roles = new List< Role.Models.Role> { admin };

            var res = await client.Realms().Name("master").RolesById(role.Id).AddAsync(roles);

            res.Success.Should().BeTrue("The update result should be true");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetUpdatedRoleByIDComposite()
        {
            var role = await client.Realms().Name("master").Roles().Name("newRoleByID").GetAsync();
            var composites = await client.Realms().Name("master").RolesById(role.Id).GetRealmRoleCompositesAsync();

            composites.Should().HaveCount(1, "The just added composite role");
            composites.First().Name.Should().BeEquivalentTo("admin");
            composites.First().ClientRole.Should().BeFalse();
        }


        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task GetRealmRolesByIDRealms()
        {
            var role = await client.Realms().Name("master").Roles().Name("newRoleByID").GetAsync();
            var realms = await client.Realms().Name("master").RolesById(role.Id).GetRealmRoleCompositesAsync();

            realms.Should().HaveCount(1, "By default only admin is added as main realm to role");
            realms.First().Name.Should().BeEquivalentTo("admin");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task DeleteTheUpdatedRoleByIDComposite()
        {
            var role = await client.Realms().Name("master").Roles().Name("newRoleByID").GetAsync();
            var composites = await client.Realms().Name("master").RolesById(role.Id).GetRealmRoleCompositesAsync();

            var res = await client.Realms().Name("master").RolesById(role.Id).RemoveAsync(composites);
            res.Should().BeTrue("The delete client result should be true");

            composites = await client.Realms().Name("master").RolesById(role.Id).GetRealmRoleCompositesAsync();
            composites.Should().HaveCount(0, "All composites should be deleted");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(9)]
        public async Task GetRealmRolesClientRolesByID()
        {
            var role = await client.Realms().Name("master").Roles().Name("admin").GetAsync();
            var clientRole = await client.Realms().Name("master").Clients().FindAsync(new Client.Models.ClientFilter(){ ClientId = "account" });

            var roles = await client.Realms().Name("master").RolesById(role.Id).GetClientRoleCompositesAsync(clientRole.First().Id);

            roles.Should().HaveCount(0, "By default should be empty");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(10)]
        public async Task DeleteTheUpdatedRoleByID()
        {
            var role = await client.Realms().Name("master").Roles().Name("newRoleByID").GetAsync();
            var res = await client.Realms().Name("master").RolesById(role.Id).DeleteAsync();
            res.Should().BeTrue("The delete client result should be true");


            Exception ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.Realms().Name("master").RolesById(role.Id).GetAsync());

            ex.Message.Should().Contain("404 (Not Found)");
        }
    }
}

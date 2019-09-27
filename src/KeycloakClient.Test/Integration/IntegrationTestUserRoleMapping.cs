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
    public class IntegrationTestsUserRolesMapping : IntegrationTestBase
    {

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task GetUserRoleMappingsForClientWithID()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().GetAsync();
            
            mappings.RealmMappings.Should().HaveCount(4);
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task GetRolesForClientWithID()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var roles = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Realm().GetAllAsync();
            
            roles.Should().HaveCount(4);
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task AddRealmRoleMappingToUserWithID()
        {

            var newRole = new Role.Models.Role
            {
                Name = "newRoleMapping"
            };
            var res = await client.Realms().Name("master").Roles().CreateAsync(newRole);
            res.Success.Should().BeTrue("The creation result should be true");
            res.Id.Should().BeEquivalentTo("newRoleMapping", "The current implementation should return the role name for new role creation");

            var admin = await client.Realms().Name("master").Roles().Name("newRoleMapping").GetAsync();
            var roles = new List<Role.Models.Role> { admin };

            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            res = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Realm().AddRolesAsync(roles);

            res.Success.Should().BeTrue();
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task DeleteRealmRoleMappingFromUserWithID()
        {
            var newRoleMapping = await client.Realms().Name("master").Roles().Name("newRoleMapping").GetAsync();
            var roles = new List<Role.Models.Role> { newRoleMapping };

            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var res = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Realm().RemoveRolesAsync(roles);

            res.Should().BeTrue();
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task GetUpdatedUserRoleMappingsForClientWithID()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var mappings = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().GetAsync();

            mappings.RealmMappings.Should().HaveCount(3);
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetRealmLevelRolsForUserWithId()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var roles = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Realm().GetAvailableAsync();

            roles.Should().HaveCount(1);
            roles.First().Name.Should().BeEquivalentTo("newRoleMapping");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task GetRealmEfectiveRolsForUserWithID()
        {
            var user = await client.Realms().Name("master").Users().FindAsync(new User.Models.UserFilter() { Username = "admin" });
            var roles = await client.Realms().Name("master").Users().Id(user.First().Id).Roles().Realm().GetEffectiveAsync();

            roles.Should().HaveCount(4);
            roles.Select(el => el.Name).Should().Contain(new[] { "uma_authorization", "admin", "create-realm", "offline_access"});
        }
        
        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task DeleteMappingRole()
        {
            var res = await client.Realms().Name("master").Roles().Name("newRoleMapping").DeleteAsync();
            res.Should().BeTrue("The creation result should be true");
        }
    }
}

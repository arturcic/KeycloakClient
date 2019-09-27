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
    public class IntegrationTestsGroupRolesMapping : IntegrationTestBase
    {

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(1)]
        public async Task GetRealmRoleMappingToGroupWithID()
        {
            var newGroup = new Group.Models.Group()
            {
                Name = "groupRealmRolsTest"
            };

            var res = await client.Realms().Name("master").Groups().CreateAsync(newGroup);
            res.Success.Should().BeTrue();

            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });

            var mapping = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().GetAsync();

            mapping.RealmMappings.Should().BeNull("By default there are no mapings");
            mapping.ClientMappings.Should().BeNull("By default there are no mapings");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(2)]
        public async Task GetAvailableRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().GetAvailableAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "admin", "create-realm", "offline_access", "uma_authorization" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(3)]
        public async Task GetEffectiveRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().GetEffectiveAsync();

            mappings.Select(el => el.Name).Should().BeEmpty("No applied roles by default");
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(4)]
        public async Task AddRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var admin = await client.Realms().Name("master").Roles().Name("admin").GetAsync();
            var roles = new List<Role.Models.Role> { admin };
            var res = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().AddRolesAsync(roles);

            res.Success.Should().BeTrue();
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(5)]
        public async Task GetAdminRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().GetAllAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "admin" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(6)]
        public async Task GetAdminEffectiveRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().GetEffectiveAsync();

            mappings.Select(el => el.Name).Should().Contain(new[] { "admin", "create-realm" });
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(7)]
        public async Task DeleteRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var admin = await client.Realms().Name("master").Roles().Name("admin").GetAsync();
            var roles = new List<Role.Models.Role> { admin };
            var res = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().RemoveRolesAsync(roles);

            res.Should().BeTrue();
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(8)]
        public async Task GetDeletedAdminRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().GetAllAsync();

            mappings.Select(el => el.Name).Should().BeEmpty();
        }

        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(9)]
        public async Task GetDeletedAdminEffectiveRealmRoleMappingToGroupWithID()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var mappings = await client.Realms().Name("master").Groups().Id(group.First().Id).Roles().Realm().GetEffectiveAsync();

            mappings.Select(el => el.Name).Should().BeEmpty();
        }
        
        [Trait("Category", "IntegrationTest")]
        [Fact, TestPriority(10)]
        public async Task DeleteMappingRole()
        {
            var group = await client.Realms().Name("master").Groups().FindAsync(new Group.Models.GroupFilter() { Search = "groupRealmRolsTest" });
            var res = await client.Realms().Name("master").Groups().Id(group.First().Id).DeleteAsync();
            res.Should().BeTrue("The creation result should be true");
        }
    }
}

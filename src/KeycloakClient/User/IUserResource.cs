using System.Collections.Generic;
using System.Threading.Tasks;
using KeycloakClient.Internals.Resources;
using KeycloakClient.RoleMapping;
using KeycloakClient.User.Models;

namespace KeycloakClient.User
{
    public interface IUserResource : IResource<Models.User>
    {
        [Get("groups")]
        Task<List<Group.Models.Group>> GetGroupsAsync();
        
        [Put("groups/{groupId}")]
        Task<bool> JoinGroupAsync([PathParam("groupId")]string groupId);
        
        [Delete("groups/{groupId}")]
        Task<bool> LeaveGroupAsync([PathParam("groupId")]string groupId);
        
        [Put("reset-password")]
        Task<bool> ResetPasswordAsync([Body]Credential credential);
        
        [Put("send-verify-email")]
        Task<bool> SendVerifyEmailAsync([Query]SendVerifyEmail verifyEmail);
        
        [Put("execute-actions-email")]
        Task<bool> ExecuteActionsEmailAsync([Query]ExecuteActionsEmail executeActionsEmail);

        [Path("role-mappings")]
        IRolesMappingResource Roles();
    }
}

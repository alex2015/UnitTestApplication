using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace ConsoleApplication.Privileges
{
    public static class ProcessExtensions
    {
        [MethodImpl(MethodImplOptions.Synchronized),
        PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static AdjustPrivilegeResult DisablePrivilege(this AccessTokenHandle accessTokenHandle, Privilege privilege)
        {
            return Privileges.DisablePrivilege(accessTokenHandle, privilege);
        }

        [MethodImpl(MethodImplOptions.Synchronized),
        PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static AdjustPrivilegeResult EnablePrivilege(this AccessTokenHandle accessTokenHandle, Privilege privilege)
        {
            return Privileges.EnablePrivilege(accessTokenHandle, privilege);
        }

        [MethodImpl(MethodImplOptions.Synchronized),
        PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static AccessTokenHandle GetAccessTokenHandle(this Process process, TokenAccessRights tokenAccessRights)
        {
            return new AccessTokenHandle(new ProcessHandle(process.Handle, false), tokenAccessRights);
        }

        [MethodImpl(MethodImplOptions.Synchronized),
        PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static PrivilegeAttributes GetPrivilegeAttributes(this AccessTokenHandle accessTokenHandle, Privilege privilege)
        {
            return Privileges.GetPrivilegeAttributes(privilege, GetPrivileges(accessTokenHandle));
        }

        [MethodImpl(MethodImplOptions.Synchronized),
        PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static PrivilegeAndAttributesCollection GetPrivileges(this AccessTokenHandle accessTokenHandle)
        {
            return Privileges.GetPrivileges(accessTokenHandle);
        }

        [MethodImpl(MethodImplOptions.Synchronized),
        PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static PrivilegeState GetPrivilegeState(this AccessTokenHandle accessTokenHandle, Privilege privilege)
        {
            return GetPrivilegeState(GetPrivilegeAttributes(accessTokenHandle, privilege));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static PrivilegeState GetPrivilegeState(PrivilegeAttributes privilegeAttributes)
        {
            if ((privilegeAttributes & PrivilegeAttributes.Enabled) == PrivilegeAttributes.Enabled)
            {
                return PrivilegeState.Enabled;
            }

            if ((privilegeAttributes & PrivilegeAttributes.Removed) == PrivilegeAttributes.Removed)
            {
                return PrivilegeState.Removed;
            }

            return PrivilegeState.Disabled;
        }
    }
}

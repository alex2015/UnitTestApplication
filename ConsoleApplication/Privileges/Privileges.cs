using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApplication.Privileges
{
    internal static class Privileges
    {
        private const int PrivilegesCount = 1;

        private const string SeTakeOwnershipPrivilege = "SeTakeOwnershipPrivilege";

        private static readonly Dictionary<Privilege, Luid> LuidDictionary = new Dictionary<Privilege, Luid>(PrivilegesCount);

        private static readonly Dictionary<Privilege, string> PrivilegeConstantsDictionary = new Dictionary<Privilege, string>(PrivilegesCount)
        {
           { Privilege.TakeOwnership, SeTakeOwnershipPrivilege },
        };

        private static readonly Dictionary<string, Privilege> PrivilegesDictionary = new Dictionary<string, Privilege>(PrivilegesCount)
        {
           { SeTakeOwnershipPrivilege, Privilege.TakeOwnership },
        };

        internal static AdjustPrivilegeResult DisablePrivilege(AccessTokenHandle accessTokenHandle, Privilege privilege)
        {
            return AdjustPrivilege(accessTokenHandle, privilege, PrivilegeAttributes.Disabled);
        }

        internal static AdjustPrivilegeResult EnablePrivilege(AccessTokenHandle accessTokenHandle, Privilege privilege)
        {
            return AdjustPrivilege(accessTokenHandle, privilege, PrivilegeAttributes.Enabled);
        }

        internal static PrivilegeAttributes GetPrivilegeAttributes(Privilege privilege, PrivilegeAndAttributesCollection privileges)
        {
            foreach (PrivilegeAndAttributes privilegeAndAttributes in privileges)
            {
                if (privilegeAndAttributes.Privilege == privilege)
                {
                    return privilegeAndAttributes.PrivilegeAttributes;
                }
            }

            GetLuid(privilege);

            return PrivilegeAttributes.Removed;
        }

        internal static PrivilegeAndAttributesCollection GetPrivileges(AccessTokenHandle accessTokenHandle)
        {
            LuidAndAttributes[] luidAndAttributesArray = GetTokenPrivileges(accessTokenHandle);
            int length = luidAndAttributesArray.Length;
            List<PrivilegeAndAttributes> privilegeAndAttributes = new List<PrivilegeAndAttributes>(length);
            for (int i = 0; i < length; i++)
            {
                LuidAndAttributes luidAndAttributes = luidAndAttributesArray[i];
                string name = GetPrivilegeName(luidAndAttributes.Luid);
                if (PrivilegesDictionary.ContainsKey(name))
                {
                    privilegeAndAttributes.Add(new PrivilegeAndAttributes(
                        PrivilegesDictionary[name],
                        luidAndAttributes.Attributes));
                }
            }

            return new PrivilegeAndAttributesCollection(privilegeAndAttributes);
        }

        private static AdjustPrivilegeResult AdjustPrivilege(
            AccessTokenHandle accessTokenHandle,
            Luid luid,
            PrivilegeAttributes privilegeAttributes)
        {
            TokenPrivilege newState = new TokenPrivilege
            {
                PrivilegeCount = 1,
                Privilege = new LuidAndAttributes
                {
                    Attributes = privilegeAttributes,
                    Luid = luid
                }
            };
            TokenPrivilege previousState = new TokenPrivilege();
            int returnLength = 0;

            if (!NativeMethods.AdjustTokenPrivileges(
                accessTokenHandle,
                false,
                ref newState,
                Marshal.SizeOf(previousState),
                ref previousState,
                ref returnLength))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return (AdjustPrivilegeResult)previousState.PrivilegeCount;
        }

        private static AdjustPrivilegeResult AdjustPrivilege(
            AccessTokenHandle accessTokenHandle,
            Privilege privilege,
            PrivilegeAttributes privilegeAttributes)
        {
            return AdjustPrivilege(accessTokenHandle, GetLuid(privilege), privilegeAttributes);
        }

        private static Luid GetLuid(Privilege privilege)
        {
            if (LuidDictionary.ContainsKey(privilege))
            {
                return LuidDictionary[privilege];
            }

            Luid luid = new Luid();
            if (!NativeMethods.LookupPrivilegeValue(String.Empty, PrivilegeConstantsDictionary[privilege], ref luid))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            LuidDictionary.Add(privilege, luid);
            return luid;
        }

        private static string GetPrivilegeName(Luid luid)
        {
            StringBuilder nameBuilder = new StringBuilder();
            int nameLength = 0;
            if (NativeMethods.LookupPrivilegeName(String.Empty, ref luid, nameBuilder, ref nameLength))
            {
                return String.Empty;
            }

            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != NativeMethods.ErrorInsufficientBuffer)
            {
                throw new Win32Exception(lastWin32Error);
            }

            nameBuilder.EnsureCapacity(nameLength);
            if (!NativeMethods.LookupPrivilegeName(String.Empty, ref luid, nameBuilder, ref nameLength))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return nameBuilder.ToString();
        }

        private static LuidAndAttributes[] GetTokenPrivileges(AccessTokenHandle accessTokenHandle)
        {
            int tokenInformationLength = 0;
            int returnLength = 0;
            if (NativeMethods.GetTokenInformation(
                accessTokenHandle,
                TokenInformationClass.TokenPrivileges,
                IntPtr.Zero,
                tokenInformationLength,
                ref returnLength))
            {
                return new LuidAndAttributes[0];
            }

            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != NativeMethods.ErrorInsufficientBuffer)
            {
                throw new Win32Exception(lastWin32Error);
            }

            tokenInformationLength = returnLength;
            returnLength = 0;

            using (AllocatedMemory allocatedMemory = new AllocatedMemory(tokenInformationLength))
            {
                if (!NativeMethods.GetTokenInformation(
                    accessTokenHandle,
                    TokenInformationClass.TokenPrivileges,
                    allocatedMemory.Pointer,
                    tokenInformationLength,
                    ref returnLength))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                int privilegeCount = Marshal.ReadInt32(allocatedMemory.Pointer);
                LuidAndAttributes[] luidAndAttributes = new LuidAndAttributes[privilegeCount];
                long pointer = allocatedMemory.Pointer.ToInt64() + Marshal.SizeOf(privilegeCount);
                Type type = typeof(LuidAndAttributes);
                long size = Marshal.SizeOf(type);
                for (int i = 0; i < privilegeCount; i++)
                {
                    luidAndAttributes[i] = (LuidAndAttributes)Marshal.PtrToStructure(new IntPtr(pointer), type);
                    pointer += size;
                }

                return luidAndAttributes;
            }
        }
    }
}

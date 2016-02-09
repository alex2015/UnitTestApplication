using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;

namespace ConsoleApplication.Privileges
{
    public sealed class PrivilegeEnabler : IDisposable
    {
        private static readonly Dictionary<Privilege, PrivilegeEnabler> SharedPrivileges = new Dictionary<Privilege, PrivilegeEnabler>();
        private static readonly Dictionary<Process, AccessTokenHandle> AccessTokenHandles = new Dictionary<Process, AccessTokenHandle>();

        private AccessTokenHandle _accessTokenHandle;

        private bool _disposed;

        private bool _ownsHandle;

        private Process _process;

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public PrivilegeEnabler(Process process)
        {
            lock (AccessTokenHandles)
            {
                if (AccessTokenHandles.ContainsKey(process))
                {
                    _accessTokenHandle = AccessTokenHandles[process];
                }
                else
                {
                    _accessTokenHandle =
                        process.GetAccessTokenHandle(TokenAccessRights.AdjustPrivileges | TokenAccessRights.Query);
                    AccessTokenHandles.Add(process, _accessTokenHandle);
                    _ownsHandle = true;
                }
            }

            _process = process;
        }


        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public PrivilegeEnabler(Process process, params Privilege[] privileges)
            : this(process)
        {
            foreach (var privilege in privileges)
            {
                EnablePrivilege(privilege);
            }
        }

        ~PrivilegeEnabler()
        {
            InternalDispose();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Dispose()
        {
            InternalDispose();
            GC.SuppressFinalize(this);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public AdjustPrivilegeResult EnablePrivilege(Privilege privilege)
        {
            lock (SharedPrivileges)
            {
                if (!SharedPrivileges.ContainsKey(privilege) &&
                    _accessTokenHandle.GetPrivilegeState(privilege) == PrivilegeState.Disabled &&
                    _accessTokenHandle.EnablePrivilege(privilege) == AdjustPrivilegeResult.PrivilegeModified)
                {
                    SharedPrivileges.Add(privilege, this);
                    return AdjustPrivilegeResult.PrivilegeModified;
                }

                return AdjustPrivilegeResult.None;
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private void InternalDispose()
        {
            if (!_disposed)
            {
                lock (SharedPrivileges)
                {
                    var privileges = SharedPrivileges
                        .Where(keyValuePair => keyValuePair.Value == this)
                        .Select(keyValuePair => keyValuePair.Key)
                        .ToArray();

                    foreach (var privilege in privileges)
                    {
                        _accessTokenHandle.DisablePrivilege(privilege);
                        SharedPrivileges.Remove(privilege);
                    }

                    if (_ownsHandle)
                    {
                        _accessTokenHandle.Dispose();
                        lock (_accessTokenHandle)
                        {
                            AccessTokenHandles.Remove(_process);
                        }
                    }

                    _accessTokenHandle = null;
                    _ownsHandle = false;
                    _process = null;

                    _disposed = true;
                }
            }
        }
    }
}

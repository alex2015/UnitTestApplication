using System;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApplication.Privileges
{
    [Flags,SuppressMessage("Microsoft.Design","CA1008:EnumsShouldHaveZeroValue",Justification = "Native enum."),
    SuppressMessage("Microsoft.Usage","CA2217:DoNotMarkEnumsWithFlags",Justification = "Native enum.")]
    public enum PrivilegeAttributes
    {
        Disabled = 0,
        EnabledByDefault = 1,
        Enabled = 2,
        Removed = 4,
        UsedForAccess = -2147483648
    }

    [Flags, SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Native enum."),
    SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "Native enum.")]
    public enum TokenAccessRights
    {
        AssignPrimary = 0,
        Duplicate = 1,
        Impersonate = 4,
        Query = 8,
        QuerySource = 16,
        AdjustPrivileges = 32,
        AdjustGroups = 64,
        AdjustDefault = 128,
        AdjustSessionId = 256,
        AllAccess = AccessTypeMasks.StandardRightsRequired |
            AssignPrimary |
            Duplicate |
            Impersonate |
            Query |
            QuerySource |
            AdjustPrivileges |
            AdjustGroups |
            AdjustDefault |
            AdjustSessionId,

        Read = AccessTypeMasks.StandardRightsRead | Query,
        Write = AccessTypeMasks.StandardRightsWrite | AdjustPrivileges | AdjustGroups | AdjustDefault,
        Execute = AccessTypeMasks.StandardRightsExecute | Impersonate
    }

    [Flags]
    internal enum AccessTypeMasks
    {
        Delete = 65536,
        ReadControl = 131072,
        WriteDAC = 262144,
        WriteOwner = 524288,
        Synchronize = 1048576,
        StandardRightsRequired = 983040,
        StandardRightsRead = ReadControl,
        StandardRightsWrite = ReadControl,
        StandardRightsExecute = ReadControl,
        StandardRightsAll = 2031616,
        SpecificRightsAll = 65535
    }

    internal enum TokenInformationClass
    {
        None,
        TokenUser,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation,
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel,
        TokenUIAccess,
        TokenMandatoryPolicy,
        TokenLogonSid,
        MaxTokenInfoClass
    }
}

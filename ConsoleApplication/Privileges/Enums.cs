namespace ConsoleApplication.Privileges
{
    public enum AdjustPrivilegeResult
    {
        None,
        PrivilegeModified
    }

    public enum Privilege
    {
        TakeOwnership,
    }

    public enum PrivilegeState
    {
        Disabled,
        Enabled,
        Removed
    }
}

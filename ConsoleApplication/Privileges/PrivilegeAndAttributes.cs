using System;

namespace ConsoleApplication.Privileges
{
    public struct PrivilegeAndAttributes : IEquatable<PrivilegeAndAttributes>
    {
        private readonly Privilege _privilege;

        private readonly PrivilegeAttributes _privilegeAttributes;

        internal PrivilegeAndAttributes(Privilege privilege, PrivilegeAttributes privilegeAttributes)
        {
            _privilege = privilege;
            _privilegeAttributes = privilegeAttributes;
        }

        public Privilege Privilege
        {
            get
            {
                return _privilege;
            }
        }

        public PrivilegeAttributes PrivilegeAttributes
        {
            get
            {
                return _privilegeAttributes;
            }
        }

        public static bool operator ==(PrivilegeAndAttributes first, PrivilegeAndAttributes second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(PrivilegeAndAttributes first, PrivilegeAndAttributes second)
        {
            return !first.Equals(second);
        }

        public override int GetHashCode()
        {
            return _privilege.GetHashCode() ^ _privilegeAttributes.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is PrivilegeAttributes && Equals((PrivilegeAttributes)obj);
        }

        public bool Equals(PrivilegeAndAttributes other)
        {
            return _privilege == other.Privilege && _privilegeAttributes == other.PrivilegeAttributes;
        }
    }
}

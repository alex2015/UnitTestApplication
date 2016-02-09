using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ConsoleApplication.Privileges
{
    [Serializable]
    public sealed class PrivilegeAndAttributesCollection : ReadOnlyCollection<PrivilegeAndAttributes>
    {
        internal PrivilegeAndAttributesCollection(IList<PrivilegeAndAttributes> list)
            : base(list)
        {
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int maxPrivilegeLength = this.Max(privilegeAndAttributes => privilegeAndAttributes.Privilege.ToString().Length);
            foreach (PrivilegeAndAttributes privilegeAndAttributes in this)
            {
                stringBuilder.Append(privilegeAndAttributes.Privilege);
                int paddingLength = maxPrivilegeLength - privilegeAndAttributes.Privilege.ToString().Length;
                char[] padding = new char[paddingLength];
                for (int i = 0; i < paddingLength; i++)
                {
                    padding[i] = ' ';
                }

                stringBuilder.Append(padding);
                stringBuilder.Append(" => ");
                stringBuilder.AppendLine(privilegeAndAttributes.PrivilegeAttributes.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}

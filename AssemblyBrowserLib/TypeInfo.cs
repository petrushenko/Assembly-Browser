using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLib
{
    public class TypeInfo
    {
        public TypeInfo()
        {
            Members = new List<MemberInfo>();
        }
        public string Name { get; set; }        
        public List<MemberInfo> Members { get; set; }
        internal void AddMember(MemberInfo member)
        {
            Members.Add(member);
        }
    }
}

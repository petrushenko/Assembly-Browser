using System;
using System.Collections.Generic;

namespace AssemblyBrowserLib
{
    public class NamespaceInfo
    {
        public NamespaceInfo()
        {
            Members = new List<TypeInfo>();
        }
        public string Name { get; set; }
        public List<TypeInfo> Members { get; set; }
        internal void AddMember(TypeInfo member)
        {
            Members.Add(member);
        }
    }
}

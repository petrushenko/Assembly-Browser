using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLib
{
    public abstract class Member
    {
        public abstract MemberType GetContainerType { get; }

        public string Name { get; set; }
    }
}

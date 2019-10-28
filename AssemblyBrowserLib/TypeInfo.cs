using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLib
{
    public class TypeInfo : ContainerInfo
    {
        public TypeInfo() : base()
        {
        }

        public override MemberType GetContainerType => MemberType.Type;
    }
}

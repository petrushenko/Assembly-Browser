using System;
using System.Collections.Generic;

namespace AssemblyBrowserLib
{
    public class NamespaceInfo : ContainerInfo
    {
        public NamespaceInfo() : base()
        {
        }

        public override MemberType GetContainerType => MemberType.Namespace;
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLib
{
    public class MemberInfo : Member
    {
        public override MemberType GetContainerType => MemberType.Member;
    }
}

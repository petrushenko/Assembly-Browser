namespace AssemblyBrowserLib
{
    public abstract class Member
    {
        public abstract MemberType GetContainerType { get; }

        public string Name { get; set; }
    }
}

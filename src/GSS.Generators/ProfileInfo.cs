namespace GSS.Generators
{
	public sealed partial class ApiBindingGenerator
	{
		private class ProfileInfo
        {
            public string ProfileFullName { get; }
            public string TargetTypeFullName { get; }

            public ProfileInfo(string profileFullName, string targetTypeFullName)
            {
                ProfileFullName = profileFullName;
                TargetTypeFullName = targetTypeFullName;
            }

            public override bool Equals(object obj) => obj is ProfileInfo other && ProfileFullName == other.ProfileFullName;
            public override int GetHashCode() => ProfileFullName.GetHashCode();
        }
    }
}
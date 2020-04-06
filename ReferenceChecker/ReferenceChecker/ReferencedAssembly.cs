using System;
using System.Reflection;
namespace ReferenceChecker
{
	public class ReferencedAssembly
	{
		public Version VersionReferenced { get; private set; }
		public Assembly ReferencedBy { get; private set; }
        public string CodeBase { get; private set; }
		public ReferencedAssembly(Version versionReferenced, string codeBase, Assembly referencedBy)
		{
			VersionReferenced = versionReferenced;
			ReferencedBy = referencedBy;
            CodeBase = codeBase;
		}
	}

    public class Model
    {
        //public int SNo { get; set; }
        public string ReferenceAssemblyName { get; set; }
        public string ReferenceAssemblyVersion { get; set; }
        public string ReferenceAssemblyPath { get; set;}
        public string ReferencedByAssemblyName { get; set; }
    }
}

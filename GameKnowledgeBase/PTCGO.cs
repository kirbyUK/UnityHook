using System;
using System.Collections.Generic;
using System.Text;

namespace GameKnowledgeBase
{
	class PTCGO : IKnowledge
	{
		public const string WIN_REL_LIBRARY_PATH = "Pokemon Trading Card Game Online_Data\\Managed";

		// File names of all assemblies, with dll extension.
		private static string[] _assemblyFileNames = new string[]
		{
			"", // Empty/Default entry.
			"core.dll",
			"pie-core.dll",
			"pie-src.dll",
			"ProtobufMessages.dll",
			"sausage-core.dll",
			"System.dll",
			"UnityEngine.dll",
		};

		public string[] LibraryFileNames
		{
			get
			{
				return _assemblyFileNames;
			}
		}

		public string LibraryRelativePath
		{
			get
			{
				return WIN_REL_LIBRARY_PATH;
			}
		}
	}
}

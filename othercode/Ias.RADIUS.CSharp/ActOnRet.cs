using System;
using System.Runtime.InteropServices;

namespace Ias.RADIUS.CSharp
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct ActOnRet
	{
		public int status;

		public string state;

		public string message;
		
		public string password;
		
		public string groups;

        public int panic;
	}
}



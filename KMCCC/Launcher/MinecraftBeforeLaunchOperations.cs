using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMCCC.Launcher.Operations
{
	public class MinecraftBeforeLaunchOperations
	{

	}

	public interface IOperation
	{
		bool Operate();
	}

	public class CopyOperations
	{
		public String Source { get; set; }

		public String Target { get; set; }
	}
}

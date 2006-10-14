using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
	public interface IClientProvider
	{
		NSvn.Core.Client Client { get; }
	}
}

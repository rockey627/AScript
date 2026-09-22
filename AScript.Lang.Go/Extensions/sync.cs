using System;
using System.Threading;

namespace AScript.Lang.Go.Extensions
{
	public class sync
	{
		public class Mutex
		{
			public void Lock()
			{
				Monitor.Enter(this);
			}

			public bool TryLock()
			{
				return Monitor.TryEnter(this);
			}

			public void Unlock()
			{
				Monitor.Exit(this);
			}
		}
	}
}

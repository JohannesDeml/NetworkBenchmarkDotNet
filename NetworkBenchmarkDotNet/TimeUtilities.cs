// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeUtilities.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Threading;

namespace NetworkBenchmark
{
	public static class TimeUtilities
	{
		#if WINDOWS
		/// Get higher precision for Thread.Sleep on Windows
		/// See https://web.archive.org/web/20051125042113/http://www.dotnet247.com/247reference/msgs/57/289291.aspx
		/// See https://docs.microsoft.com/en-us/windows/win32/api/timeapi/nf-timeapi-timebeginperiod
		[DllImport("winmm.dll")]
		private static extern uint timeBeginPeriod(uint period);

		[DllImport("winmm.dll")]
		private static extern uint timeEndPeriod(uint period);
		#endif

		public static void HighPrecisionThreadSleep(int milliseconds)
		{
			#if WINDOWS
			timeBeginPeriod(1);
			#endif

			Thread.Sleep(milliseconds);

			#if WINDOWS
			timeEndPeriod(1);
			#endif
		}
	}
}

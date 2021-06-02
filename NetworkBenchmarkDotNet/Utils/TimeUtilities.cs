// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeUtilities.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NetworkBenchmark
{
	public static class TimeUtilities
	{
		#if WINDOWS
		// See https://github.com/Leandros/WindowsHModular/blob/7f5df60fe3711b9a878cd3cba755f9f71b5d01ca/include/win32/windows.h#L2655
		private const uint TIMERR_NOERROR = 0u;

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
			var resultCode = timeBeginPeriod(1u);
			Debug.Assert(resultCode == TIMERR_NOERROR);
			#endif

			Thread.Sleep(milliseconds);

			#if WINDOWS
			resultCode = timeEndPeriod(1u);
			Debug.Assert(resultCode == TIMERR_NOERROR);
			#endif
		}
	}
}

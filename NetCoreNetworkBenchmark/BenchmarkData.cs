using System;

namespace NetCoreNetworkBenchmark
{
	public class BenchmarkData
	{
		public DateTime StartTime { get; private set; }
		public DateTime StopTime { get; private set; }
		public TimeSpan Duration { get; private set; }

		public bool Running { get; private set; }

		public long MessagesClientSent;
		public long MessagesClientReceived;
		public long MessagesServerSent;
		public long MessagesServerReceived;
		public long Errors;

		public void Reset()
		{
			MessagesClientSent = 0L;
			MessagesClientReceived = 0L;
			MessagesServerSent = 0L;
			MessagesServerReceived = 0L;
			Errors = 0;
		}

		public void PrepareBenchmark()
		{
			Running = true;
		}

		public void StartBenchmark()
		{
			StartTime = DateTime.Now;
			Running = true;
		}

		public void StopBenchmark()
		{
			Running = false;
			StopTime = DateTime.Now;
			Duration = StopTime.Subtract(StartTime);
		}
	}
}

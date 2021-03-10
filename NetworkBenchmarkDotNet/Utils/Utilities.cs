// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkBenchmark
{
	internal static class Utilities
	{
		public static string EnumToString(Type enumType)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('[');
			foreach (var value in Enum.GetValues(enumType))
			{
				sb.Append(value.ToString());
				sb.Append(", ");
			}

			// Remove last ", "
			sb.Remove(sb.Length - 2, 2);
			sb.Append(']');

			return sb.ToString();
		}

		public static string EnumToString<T>() where T : Enum
		{
			return EnumToString(typeof(T));
		}

		public static void WriteVerbose(string text)
		{
			if (!BenchmarkCoordinator.Config.Verbose)
			{
				return;
			}

			Console.Write(text);
		}

		public static void WriteVerboseLine(string text)
		{
			if (!BenchmarkCoordinator.Config.Verbose)
			{
				return;
			}

			Console.WriteLine(text);
		}

		/// <summary>
		/// Calculate the timeout in milliseconds fore a given tickrate
		/// </summary>
		/// <param name="tickRate">Given tickrate</param>
		/// <returns>0 if tickrate is <= 0, otherwise tick duration in ms</returns>
		public static int CalculateTimeout(int tickRate)
		{
			if (tickRate <= 0)
			{
				return 0;
			}

			return Math.Max(1000 / tickRate, 1);
		}

		public static Task WaitForClientGroupsToConnect<T>(List<T> clientGroups) where T : ClientGroup
		{
			return WaitForClientGroups(clientGroups, (T clientGroup) => { return clientGroup.IsConnected; });
		}

		public static Task WaitForClientGroupsToDisconnect<T>(List<T> clientGroups) where T : ClientGroup
		{
			return WaitForClientGroups(clientGroups, (T clientGroup) => { return !clientGroup.IsConnected; });
		}

		public static Task WaitForClientGroupsToStop<T>(List<T> clientGroups) where T : ClientGroup
		{
			return WaitForClientGroups(clientGroups, (T clientGroup) => { return clientGroup.IsStopped; });
		}

		public static Task WaitForClientGroupsToDispose<T>(List<T> clientGroups) where T : ClientGroup
		{
			return WaitForClientGroups(clientGroups, (T clientGroup) => { return clientGroup.IsDisposed; });
		}

		public static Task WaitForClientGroups<T>(List<T> clientGroups, Func<T, bool> condition) where T : ClientGroup
		{
			var waitForAllClientGroups = Task.Run(() =>
			{
				for (int i = 0; i < clientGroups.Count; i++)
				{
					while (!condition(clientGroups[i]))
					{
						Thread.Sleep(10);
					}
				}
			});
			return waitForAllClientGroups;
		}

		public static Task WaitForServerToStart<T>(T server) where T : IServer
		{
			return WaitForServer(server, (T s) => { return s.IsStarted; });
		}

		public static Task WaitForServerToStop<T>(T server) where T : IServer
		{
			return WaitForServer(server, (T s) => { return !s.IsStarted; });
		}

		private static Task WaitForServer<T>(T server, Func<T, bool> condition) where T : IServer
		{
			var waitForServer = Task.Run(() =>
			{
				while (!condition(server))
				{
					Thread.Sleep(10);
				}
			});
			return waitForServer;
		}
	}
}

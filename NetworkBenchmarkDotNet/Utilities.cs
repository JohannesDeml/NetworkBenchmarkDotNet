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
			sb.Append("[");
			foreach (var value in Enum.GetValues(enumType))
			{
				sb.Append(value.ToString());
				sb.Append(", ");
			}

			// Remove last ", "
			sb.Remove(sb.Length - 2, 2);
			sb.Append("]");

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

		public static Task WaitForClientsToConnect<T>(List<T> clients) where T : IClient
		{
			return WaitForClients(clients, (T client) => { return client.IsConnected; });
		}

		public static Task WaitForClientsToDisconnect<T>(List<T> clients) where T : IClient
		{
			return WaitForClients(clients, (T client) => { return !client.IsConnected; });
		}

		public static Task WaitForClientsToStop<T>(List<T> clients) where T : IClient
		{
			return WaitForClients(clients, (T client) => { return client.IsStopped; });
		}

		public static Task WaitForClientsToDispose<T>(List<T> clients) where T : IClient
		{
			return WaitForClients(clients, (T client) => { return client.IsDisposed; });
		}

		public static Task WaitForClients<T>(List<T> clients, Func<T, bool> condition) where T : IClient
		{
			var waitForAllClients = Task.Run(() =>
			{
				for (int i = 0; i < clients.Count; i++)
				{
					while (!condition(clients[i]))
					{
						Thread.Sleep(10);
					}
				}
			});
			return waitForAllClients;
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

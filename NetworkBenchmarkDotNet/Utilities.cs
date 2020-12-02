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
using System.Text;

namespace NetCoreNetworkBenchmark
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

		public static string EnumToString<T>() where T: Enum
		{
			return EnumToString(typeof(T));
		}

		public static void WriteVerbose(string text)
		{
			if (!Benchmark.Config.Verbose)
			{
				return;
			}

			Console.Write(text);
		}

		public static void WriteVerboseLine(string text)
		{
			if (!Benchmark.Config.Verbose)
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
	}
}

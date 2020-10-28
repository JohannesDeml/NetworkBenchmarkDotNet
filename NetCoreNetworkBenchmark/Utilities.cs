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
using NDesk.Options;

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

		public static bool ParseOption<TEnum>(string value, out TEnum target) where TEnum: Enum
		{
			if (Enum.TryParse(typeof(TEnum), value, true, out var parsedValue))
			{
				target = (TEnum) parsedValue;
				if (target != null && (Enum.IsDefined(typeof(TEnum), target) || target.ToString().Contains(",")))
				{
					return true;
				}
			}

			throw new OptionException(
				$"Option \"{value}\" not valid for type {typeof(TEnum).Name}\n" +
				$"Valid Options: {EnumToString<TEnum>()}", nameof(value));
		}

		public static bool ParseOption(string value, out int target, int minValue = 0, int maxValue = Int32.MaxValue)
		{
			if (int.TryParse(value, out var parsedValue))
			{
				target = parsedValue;
				if (target >= minValue && target <= maxValue)
				{
					return true;
				}
			}

			throw new OptionException($"Option {value} not valid\n" +
			                          $"Allowed Range: [{minValue} , {maxValue}]", nameof(value));
		}

		public static void WriteVerbose(string text)
		{
			if (!Program.Config.Verbose)
			{
				return;
			}

			Console.Write(text);
		}

		public static void WriteVerboseLine(string text)
		{
			if (!Program.Config.Verbose)
			{
				return;
			}

			Console.WriteLine(text);
		}
	}
}

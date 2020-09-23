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

		public static TEnum ParseEnum<TEnum>(string value) where TEnum : Enum
		{
			return (TEnum) Enum.Parse(typeof(TEnum), value, true);
		}
	}
}

using System;

namespace NFive.Debug.Client.Extensions
{
	public static class StringExtensions
	{
		public static bool IsTruthy(this string str)
		{
			if (str.Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase)) return true;
			if (str.Trim().Equals("1", StringComparison.InvariantCultureIgnoreCase)) return true;
			if (str.Trim().Equals("y", StringComparison.InvariantCultureIgnoreCase)) return true;
			if (str.Trim().Equals("yes", StringComparison.InvariantCultureIgnoreCase)) return true;

			return false;
		}
		public static bool IsFalsy(this string str)
		{
			return !str.IsTruthy();
		}
	}
}

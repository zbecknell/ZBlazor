using System.Collections.Generic;
using ZBlazor.QuickAutocomplete;

namespace ZBlazor
{
	internal static class SimpleMatcher
	{
		public static MatchData MatchStartsWith(string pattern, string word)
		{
			var result = new MatchData();

			if (string.IsNullOrEmpty(pattern))
			{
				return result;
			}

			if (word.StartsWith(pattern, System.StringComparison.OrdinalIgnoreCase))
			{
				result.Matches = CreateMatches(pattern, word);

				result.Score = 1;
			}

			return result;
		}

		public static MatchData MatchContains(string pattern, string word)
		{
			var result = new MatchData();

			if (word.Contains(pattern, System.StringComparison.OrdinalIgnoreCase))
			{
				result.Matches = CreateMatches(pattern, word);

				result.Score = 1;
			}

			return result;
		}

		private static List<FilterMatch> CreateMatches(string pattern, string word)
		{
			var start = word.IndexOf(pattern, System.StringComparison.OrdinalIgnoreCase);

			return new List<FilterMatch>()
			{
				new FilterMatch
				{
					Start = start,
					End = start + pattern.Length
				}
			};
		}
	}
}

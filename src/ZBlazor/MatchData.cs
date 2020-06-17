using System.Collections.Generic;

namespace ZBlazor.QuickAutocomplete
{
	/// <summary>
	/// A container for matching data from a filter operation.
	/// </summary>
	public class MatchData
	{
		/// <summary>
		/// A list of matches, if relevant.
		/// </summary>
		public List<FilterMatch>? Matches { get; set; }

		/// <summary>
		/// The score of the matches, a higher score indicating a more relevant match.
		/// </summary>
		public int Score { get; set; } = -100;
	}
}

using System.Collections.Generic;

namespace ZBlazor.QuickInput
{
	public class MatchData
	{
		public List<FilterMatch>? Matches { get; set; }
		public int Score { get; set; } = -100;
	}
}

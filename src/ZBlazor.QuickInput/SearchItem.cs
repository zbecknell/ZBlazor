using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace ZBlazor.QuickInput
{
	public class SearchItem<TDataObject>
	{
		public MarkupString GetDisplayText(bool highlightMatches = true)
		{
			if (!highlightMatches)
			{
				return (MarkupString)Text;
			}

			if (Matches == null || string.IsNullOrWhiteSpace(Text))
			{
				return (MarkupString)Text;
			}

			var result = new System.Text.StringBuilder();

			var position = 0;

			foreach (var match in Matches)
			{
				if (match.End == match.Start)
				{
					continue;
				}

				if (position < match.Start)
				{
					result.Append("<span>");

					var outerSubstring = Text[position..match.Start];
					result.Append(outerSubstring);
					result.Append("</span>");
					position = match.End;
				}

				result.Append("<strong>");
				var innerSubstring = Text[match.Start..match.End];
				result.Append(innerSubstring);
				result.Append("</strong>");
				position = match.End;
			}

			if (position < Text.Length)
			{
				result.Append("<span>");
				var endSubstring = Text.Substring(position);
				result.Append(endSubstring);
				result.Append("</span>");
			}

			return (MarkupString)result.ToString();
		}

		public string Text { get; set; } = "";
		public int Score { get; set; }
		public bool IsMatch => Matches != null;
		public List<FilterMatch>? Matches { get; set; }
		public int? ShowingIndex { get; set; }
		public bool IsSelected = false;
		public bool ShouldItemShow = false;

		public TDataObject DataObject { get; set; } = default!;
	}
}

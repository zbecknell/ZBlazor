using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace ZBlazor.QuickInput
{
	public class SearchItem<TDataObject>
	{
		public MarkupString GetDisplayText(bool highlightMatches = true, bool showOtherMatches = true)
		{
			if (!highlightMatches)
			{
				return (MarkupString)Text;
			}

			if (Matches == null || string.IsNullOrWhiteSpace(Text))
			{
				return (MarkupString)Text;
			}

			if (showOtherMatches && OtherMatchFieldName != null && OtherMatchFieldValue != null)
			{
				var result = new System.Text.StringBuilder();
				result.Append(Text);
				result.Append($"<br/><small>Matches {OtherMatchFieldName}: {GetMatchesMarkup(Matches, OtherMatchFieldValue)}</small>");
				return (MarkupString)result.ToString();
			}
			else
			{
				return (MarkupString)GetMatchesMarkup(Matches, Text);
			}
		}

		private string GetMatchesMarkup(List<FilterMatch> matches, string value)
		{
			var result = new System.Text.StringBuilder();

			var position = 0;

			result.Append("<span class=\"zb-quick-input-filter-match\">");

			foreach (var match in matches)
			{
				if (match.End == match.Start)
				{
					continue;
				}

				if (position < match.Start)
				{
					result.Append("<span>");

					var outerSubstring = value[position..match.Start];
					result.Append(outerSubstring);
					result.Append("</span>");
					position = match.End;
				}

				result.Append("<strong>");
				var innerSubstring = value[match.Start..match.End];
				result.Append(innerSubstring);
				result.Append("</strong>");
				position = match.End;
			}

			if (position < value.Length)
			{
				result.Append("<span>");
				var endSubstring = value.Substring(position);
				result.Append(endSubstring);
				result.Append("</span>");
			}

			result.Append("</span>");

			return result.ToString();
		}

		public string Text { get; set; } = "";
		public int Score { get; set; }
		public bool IsMatch => Matches != null;
		public List<FilterMatch>? Matches { get; set; }
		public int? ShowingIndex { get; set; }
		public bool IsSelected = false;
		public bool ShouldItemShow = false;

		/// <summary>
		/// Populated when the match is against another field other than the primary.
		/// </summary>
		public string? OtherMatchFieldName { get; set; }

		/// <summary>
		/// Populated when the match is against another field other than the primary.
		/// </summary>
		public string? OtherMatchFieldValue { get; set; }

		public TDataObject DataObject { get; set; } = default!;
	}
}

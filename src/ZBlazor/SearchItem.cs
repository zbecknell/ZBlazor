using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace ZBlazor.QuickAutocomplete
{
	/// <summary>
	/// The container for an individual item in a QuickAutocomplete.
	/// </summary>
	public class SearchItem<TDataObject>
	{
		#region FIELDS

		/// <summary>
		/// Applied to the actual item element within the QuickAutocomplete items list.
		/// </summary>
		public readonly string Id = Guid.NewGuid().ToString();

		/// <summary>
		/// The last time this item was selected.
		/// </summary>
		public DateTime? LastHit;

		/// <summary>
		/// A unique key to use for this item.
		/// </summary>
		public string Key = null!;

		#endregion FIELDS

		#region PROPERTIES

		/// <summary>
		/// The text that is the primary means of searching for this item.
		/// </summary>
		public string Text { get; set; } = "";

		/// <summary>
		/// The score of the matches. This determines how relevant the match is when using fuzzy matching.
		/// </summary>
		public int Score { get; set; }

		/// <summary>
		/// True when any <see cref="Matches"/> exist.
		/// </summary>
		public bool IsMatch => Matches != null;

		/// <summary>
		/// A list of <see cref="FilterMatch"/> matches for this item.
		/// </summary>
		public List<FilterMatch>? Matches { get; set; }

		/// <summary>
		/// True when this item is matching the primary field (not a secondary field noted in OtherMatchFieldName).
		/// </summary>
		public bool IsPrimaryMatch => IsMatch && OtherMatchFieldName == null;

		/// <summary>
		/// The zero-based position of this item in the list of showing items.
		/// </summary>
		public int? ShowingIndex { get; set; }

		/// <summary>
		/// When true, this is the item currently highlighted in the items list.
		/// </summary>
		public bool IsSelected = false;

		/// <summary>
		/// When true, this item should show up in the items list when it is open.
		/// </summary>
		public bool ShouldItemShow = false;

		/// <summary>
		/// Populated when the match is against another field other than the primary.
		/// </summary>
		public string? OtherMatchFieldName { get; set; }

		/// <summary>
		/// Populated when the match is against another field other than the primary.
		/// </summary>
		public string? OtherMatchFieldValue { get; set; }

		/// <summary>
		/// The actual object encapsulated in the SearchItem.
		/// </summary>
		public TDataObject DataObject { get; set; } = default!;

		#endregion PROPERTIES

		#region METHODS

		/// <summary>
		/// Gets markup for a SearchItem for display in the items list. Use this in a template to highlight the matching characters of an item.
		/// </summary>
		public MarkupString GetDisplayText(bool highlightMatches = true, bool showOtherMatches = true)
		{
			if (!highlightMatches || Matches == null || string.IsNullOrWhiteSpace(Text))
			{
				return (MarkupString)$"<span class\"zb-quick-autocomplete-filter-match\">{Text}</span>";
			}

			if (highlightMatches && Matches != null && OtherMatchFieldValue == null && OtherMatchFieldValue == null)
			{
				return (MarkupString)$"<span class\"zb-quick-autocomplete-filter-match\">{GetMatchesMarkup(Matches, Text)}</span>";
			}

			if (highlightMatches && showOtherMatches && Matches != null && OtherMatchFieldName != null && OtherMatchFieldValue != null)
			{
				var result = new System.Text.StringBuilder();
				result.Append(Text);
				result.Append($"<span class=\"zb-quick-autocomplete-filter-match\"><br/><small>Matches {OtherMatchFieldName}: {GetMatchesMarkup(Matches, OtherMatchFieldValue)}</small></span>");
				return (MarkupString)result.ToString();
			}

			return (MarkupString)$"<span class\"zb-quick-autocomplete-filter-match\">{Text}</span>";
		}

		private string GetMatchesMarkup(List<FilterMatch> matches, string value)
		{
			var result = new System.Text.StringBuilder();

			var position = 0;

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

			return result.ToString();
		}

		#endregion METHODS
	}
}

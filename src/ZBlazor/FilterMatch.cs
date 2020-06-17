namespace ZBlazor.QuickAutocomplete
{
	/// <summary>
	/// Represents the range of matching characters in an item.
	/// </summary>
	public class FilterMatch
	{
		/// <summary>
		/// The start of the <see cref="FilterMatch"/> range.
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// The end of the <see cref="FilterMatch"/> range.
		/// </summary>
		public int End { get; set; }
	}
}

namespace ZBlazor
{
	/// <summary>
	/// Filter options.
	/// </summary>
	public enum FilterType : byte
	{
		/// <summary>
		/// Match values which match the search term in a fuzzy manner.
		/// </summary>
		Fuzzy = 0,

		/// <summary>
		/// Match values which contain the search term.
		/// </summary>
		Contains = 1,

		/// <summary>
		/// Match values which start with the search term.
		/// </summary>
		StartsWith = 2
	}
}

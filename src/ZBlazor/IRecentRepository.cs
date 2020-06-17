using System;
using System.Threading.Tasks;

namespace ZBlazor
{
	/// <summary>
	/// An interface used for storing recent hits.
	/// </summary>
	public interface IRecentRepository
	{
		/// <summary>
		/// Indicates this key was recently selected.
		/// </summary>
		public ValueTask AddHit(string key);

		/// <summary>
		/// Gets the last time this key was hit, if at all.
		/// </summary>
		public ValueTask<DateTime?> GetHitForKey(string key);
	}
}

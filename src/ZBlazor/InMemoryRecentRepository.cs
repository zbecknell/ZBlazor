using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ZBlazor
{
	/// <summary>
	/// An ephemeral recent repository that adds entries until the program is restarted.
	/// </summary>
	public class InMemoryRecentRepository : IRecentRepository
	{
		private readonly ConcurrentDictionary<string, DateTime> _recents = new ConcurrentDictionary<string, DateTime>();
		private readonly string _repositoryName;

		/// <summary>
		/// Instantiates a new <see cref="InMemoryRecentRepository"/> partitioned by the given name.
		/// </summary>
		public InMemoryRecentRepository(string name)
		{
			_repositoryName = name ?? throw new ArgumentNullException(nameof(name));
		}

		/// <inheritdoc/>
		public ValueTask AddHit(string key)
		{
			key = GetFullKey(key);

			if (_recents.TryGetValue(key, out DateTime value))
			{
				_recents.TryUpdate(key, DateTime.Now, value);
			}
			else
			{
				_recents.TryAdd(key, DateTime.Now);
			}

			return new ValueTask();
		}

		/// <inheritdoc/>
		public ValueTask<DateTime?> GetHitForKey(string key)
		{
			if (_recents.TryGetValue(GetFullKey(key), out DateTime value))
			{
				return new ValueTask<DateTime?>(value);
			}

			return new ValueTask<DateTime?>(result: null);
		}

		private string GetFullKey(string key)
			=> $"{_repositoryName}_{key}";
	}
}

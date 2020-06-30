using System;
using System.Collections.Generic;
using System.Linq;

namespace ZBlazor.QuickAutocomplete
{
	/// <summary>
	/// A port of VS Code's matchFuzzy2 filter.
	/// </summary>
	public class FuzzyMatcher
	{
		private const int _maxLength = 53;

		private readonly List<List<int>> _table = InitializeTable();
		private readonly List<List<int>> _scores = InitializeTable();
		private readonly List<List<Arrow>> _arrows = InitializeArrowTable();

		private int _matchesCount;
		private int _topScore = -100;
		private int _patternStartPosition;
		private bool _firstMatchCanBeWeak;
		private long _topMatch2 = 0;

		public MatchData Match(string pattern, string word)
		{
			var score = GetFuzzyScore(pattern, pattern.ToLower(), 0, word, word.ToLower(), 0, true);

			var result = new MatchData
			{
				Matches = score != null ? CreateMatches(score) : null,
				Score = score?.Score ?? -100
			};

			return result;
		}

		private List<FilterMatch> CreateMatches(FuzzyScore score)
		{
			if (score == null)
			{
				return new List<FilterMatch>();
			}

			var matches = Convert.ToString(score.Matches, 2);
			var wordStart = score.Offset;
			var result = new List<FilterMatch>();

			for (var position = wordStart; position < _maxLength; position++)
			{
				if (matches.ElementAtOrDefault(matches.Length - (position + 1)) == '1')
				{
					var last = result.ElementAtOrDefault(result.Count - 1);
					if (last != null && last.End == position)
					{
						last.End = position + 1;
					}
					else
					{
						result.Add(new FilterMatch { Start = position, End = position + 1 });
					}
				}
			}
			return result;
		}

		private FuzzyScore? GetFuzzyScore(string pattern, string patternLow, int patternPosition, string word, string wordLow, int wordPosition, bool firstMatchCanBeWeak)
		{
			var patternLength = pattern.Length > _maxLength ? _maxLength : pattern.Length;
			var wordLength = word.Length > _maxLength ? _maxLength : word.Length;


			if (patternPosition >= patternLength || wordPosition >= wordLength || patternLength > wordLength)
			{
				return null;
			}

			// Run a simple check if the characters of pattern occur
			// (in order) at all in word. If that isn't the case we
			// stop because no match will be possible
			if (!IsPatternInWord())
			{
				return null;
			}

			int patternStartPosition = patternPosition;
			int wordStartPosition = wordPosition;

			// There will be a match, fill in tables
			for (patternPosition = patternStartPosition + 1; patternPosition <= patternLength; patternPosition++)
			{

				for (wordPosition = 1; wordPosition <= wordLength; wordPosition++)
				{

					var score = -1;

					if (patternLow[patternPosition - 1] == wordLow[wordPosition - 1])
					{

						if (wordPosition == (patternPosition - patternStartPosition))
						{
							// common prefix: `foobar <-> foobaz`
							//                            ^^^^^
							if (pattern[patternPosition - 1] == word[wordPosition - 1])
							{
								score = 7;
							}
							else
							{
								score = 5;
							}
						}
						else if (IsUpperCaseAtPosition(wordPosition - 1) && (wordPosition == 1 || !IsUpperCaseAtPosition(wordPosition - 2)))
						{
							// hitting upper-case: `foo <-> forOthers`
							//                              ^^ ^
							if (pattern[patternPosition - 1] == word[wordPosition - 1])
							{
								score = 7;
							}
							else
							{
								score = 5;
							}
						}
						else if (IsSeparatorAtPosition(wordLow, wordPosition - 2) || IsWhitespaceAtPosition(wordLow, wordPosition - 2))
						{
							// post separator: `foo <-> bar_foo`
							//                              ^^^
							score = 5;

						}
						else
						{
							score = 1;
						}
					}

					_scores[patternPosition][wordPosition] = score;

					int diag = _table[patternPosition - 1][wordPosition - 1] + (score > 1 ? 1 : score);
					int top = _table[patternPosition - 1][wordPosition] + -1;
					int left = _table[patternPosition][wordPosition - 1] + -1;

					if (left >= top)
					{
						// left or diag
						if (left > diag)
						{
							_table[patternPosition][wordPosition] = left;
							_arrows[patternPosition][wordPosition] = Arrow.Left;
						}
						else if (left == diag)
						{
							_table[patternPosition][wordPosition] = left;
							_arrows[patternPosition][wordPosition] = Arrow.Left | Arrow.Diag;
						}
						else
						{
							_table[patternPosition][wordPosition] = diag;
							_arrows[patternPosition][wordPosition] = Arrow.Diag;
						}
					}
					else
					{
						// top or diag
						if (top > diag)
						{
							_table[patternPosition][wordPosition] = top;
							_arrows[patternPosition][wordPosition] = Arrow.Top;
						}
						else if (top == diag)
						{
							_table[patternPosition][wordPosition] = top;
							_arrows[patternPosition][wordPosition] = Arrow.Top | Arrow.Diag;
						}
						else
						{
							_table[patternPosition][wordPosition] = diag;
							_arrows[patternPosition][wordPosition] = Arrow.Diag;
						}
					}
				}
			}

			_matchesCount = 0;
			_topScore = -100;
			_patternStartPosition = patternStartPosition;
			_firstMatchCanBeWeak = firstMatchCanBeWeak;

			FindAllMatches(patternLength, wordLength, patternLength == wordLength ? 1 : 0, 0, false);

			if (_matchesCount == 0)
			{
				return null;
			}

			return new FuzzyScore
			{
				Score = _topScore,
				Matches = _topMatch2,
				Offset = wordStartPosition
			};

			bool IsUpperCaseAtPosition(int position)
			{
				return word[position] != wordLow[position];
			}

			bool IsPatternInWord()
			{
				int workingPatternPosition = patternPosition;
				int workingWordPosition = wordPosition;
				while (workingPatternPosition < patternLength && workingWordPosition < wordLength)
				{
					if (patternLow[workingPatternPosition] == wordLow[workingWordPosition])
					{
						workingPatternPosition += 1;
					}
					workingWordPosition += 1;
				}
				// Pattern must be exhausted
				return workingPatternPosition == patternLength;
			}
		}

		private void FindAllMatches(int patternPosition, int wordPosition, int total, long matches, bool lastMatched)
		{
			if (_matchesCount >= 10 || total < -25)
			{
				// stop when having already 10 results, or
				// when a potential alignment as already 5 gaps
				return;
			}

			var simpleMatchCount = 0;


			while (patternPosition > _patternStartPosition && wordPosition > 0)
			{

				var score = _scores[patternPosition][wordPosition];
				var arrow = _arrows[patternPosition][wordPosition];

				if (arrow == Arrow.Left)
				{
					// left -> no match, skip a word character
					wordPosition -= 1;
					if (lastMatched)
					{
						total -= 5; // new gap penalty
					}
					else if (matches != 0)
					{
						total -= 1; // gap penalty after first match
					}
					lastMatched = false;
					simpleMatchCount = 0;

				}
				// TODO: this may not work?
				else if ((arrow & Arrow.Diag) > 0)
				{
					// TODO: this may not work?
					if ((arrow & Arrow.Left) > 0)
					{
						// left
						FindAllMatches(
							patternPosition,
							wordPosition - 1,
							matches != 0 ? total - 1 : total, // gap penalty after first match
							matches,
							lastMatched
						);
					}

					// diag
					total += score;
					patternPosition -= 1;
					wordPosition -= 1;
					lastMatched = true;

					// match -> set a 1 at the word pos
					matches += (long)Math.Pow(2, wordPosition);

					// count simple matches and boost a row of
					// simple matches when they yield in a
					// strong match.
					if (score == 1)
					{
						simpleMatchCount += 1;

						if (patternPosition == _patternStartPosition && !_firstMatchCanBeWeak)
						{
							// when the first match is a weak
							// match we discard it
							return;
						}

					}
					else
					{
						// boost
						total += 1 + (simpleMatchCount * (score - 1));
						simpleMatchCount = 0;
					}

				}
				else
				{
					return;
				}
			}

			total -= wordPosition >= 3 ? 9 : wordPosition * 3; // late start penalty

			// dynamically keep track of the current top score
			// and insert the current best score at head, the rest at tail
			_matchesCount += 1;
			if (total > _topScore)
			{
				_topScore = total;
				_topMatch2 = matches;
			}
		}

		private bool IsSeparatorAtPosition(string value, int index)
		{
			if (index < 0 || index >= value.Length)
			{
				return false;
			}
			var code = value.ElementAt(index);

			switch (code)
			{
				case '_':
				case '-':
				case '.':
				case ' ':
				case '/':
				case '\\':
				case '\'':
				case '"':
				case ':':
				case '$':
					return true;
				default:
					return false;
			}
		}

		private bool IsWhitespaceAtPosition(string value, int index)
		{
			if (index < 0 || index >= value.Length)
			{
				return false;
			}
			var code = value.ElementAt(index);

			switch (code)
			{
				case ' ':
				case '\t':
					return true;
				default:
					return false;
			}
		}

		private static List<List<int>> InitializeTable()
		{
			List<List<int>> table = new List<List<int>>();
			List<int> row = new List<int>();
			for (var i = 1; i <= _maxLength; i++)
			{
				row.Add(-i);
			}
			for (var i = 0; i <= _maxLength; i++)
			{
				int[] thisRow = new int[_maxLength];

				Array.Copy(row.ToArray(), thisRow, row.Count);

				thisRow[0] = -i;

				table.Add(thisRow.ToList());
			}
			return table;
		}

		private static List<List<Arrow>> InitializeArrowTable()
		{
			var table = new List<List<Arrow>>();
			var row = new List<Arrow>();
			for (var i = 1; i <= _maxLength; i++)
			{
				row.Add((Arrow)(-i));
			}
			for (var i = 0; i <= _maxLength; i++)
			{
				Arrow[] thisRow = new Arrow[_maxLength];

				Array.Copy(row.ToArray(), thisRow, row.Count);

				thisRow[0] = (Arrow)(-i);

				table.Add(thisRow.ToList());
			}
			return table;
		}

		private enum Arrow { Top = 0b1, Diag = 0b10, Left = 0b100 }

		private class FuzzyScore
		{
			public int Score { get; set; }

			/// <summary>
			/// Bitmask of the matches.
			/// </summary>
			public long Matches { get; set; }

			/// <summary>
			/// The offset at which matching started.
			/// </summary>
			public int Offset { get; set; }
		}
	}
}

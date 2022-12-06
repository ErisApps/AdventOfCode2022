using System.Buffers;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day06 : HappyPuzzleBase
{
	public override object SolvePart1() => Check(4);

	public override object SolvePart2() => Check(14);

	private int Check(int markerSize)
	{
		var dataStreamSpan = File.ReadAllText(AssetPath()).AsSpan();

		var i = 0;
		var buffer = new char[markerSize];
		do
		{
			dataStreamSpan.Slice(i, markerSize).CopyTo(buffer);
			if (HasUniqueCharacters(buffer))
			{
				break;
			}

			i++;
		} while (i < dataStreamSpan.Length - markerSize);

		return i + markerSize;
	}

	private static bool HasUniqueCharacters(char[] input) => new HashSet<char>(input).Count == input.Length;
}
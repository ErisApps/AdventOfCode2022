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
		do
		{
			var bufferAsSpan = dataStreamSpan.Slice(i, markerSize);
			if (HasUniqueCharacters(ref bufferAsSpan))
			{
				break;
			}

			i++;
		} while (i < dataStreamSpan.Length - markerSize);

		return i + markerSize;
	}

	private static bool HasUniqueCharacters(ref ReadOnlySpan<char> input)
	{
		for (var i = 0; i < input.Length; i++)
		for (var j = i + 1; j < input.Length; j++)
		{
			if (input[i] == input[j])
			{
				return false;
			}
		}

		return true;
	}
}
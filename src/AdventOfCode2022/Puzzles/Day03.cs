using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day03 : HappyPuzzleBase
{
	public override object SolvePart1() =>
		File.ReadLines(AssetPath())
			.Aggregate<string, uint>(0, (acc, ruckSack) =>
			{
				var ruckSackCompartimentLength = ruckSack.Length / 2;
				var ruckSackCompartimentA = ruckSack[..ruckSackCompartimentLength];
				var ruckSackCompartimentB = ruckSack[ruckSackCompartimentLength..];
				return acc + FindDuplicatesAndSumReduce(ruckSackCompartimentA, ruckSackCompartimentB);
			});

	public override object SolvePart2() =>
		File.ReadAllLines(AssetPath())
			.Chunk(3)
			.Aggregate<string[], uint>(0, (acc, bags) => acc + FindDuplicatesAndSumReduce(bags));

	private static uint FindDuplicatesAndSumReduce(params string[] dataSets)
	{
		var duplicates = dataSets[0].Intersect(dataSets[1]);
		for (var i = 2; i < dataSets.Length; i++)
		{
			duplicates = duplicates.Intersect(dataSets[i]);
		}

		return duplicates.Aggregate<char, uint>(0, (acc, c) => acc + ConvertToPriority(c));
	}

	private static uint ConvertToPriority(char c)
	{
		const int asciiUpperCaseOffset = 64;
		const int asciiLowerCaseOffset = 96;
		const int additionalAsciiOffset = 26;

		if (char.IsAsciiLetterUpper(c))
		{
			return (uint) (c - asciiUpperCaseOffset + additionalAsciiOffset);
		}

		if (char.IsAsciiLetterLower(c))
		{
			return (uint) (c - asciiLowerCaseOffset);
		}

		throw new ArgumentException("Invalid character");
	}
}
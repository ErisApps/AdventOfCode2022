using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day03 : HappyPuzzleBase
{
	public override void SolvePart1()
	{
		uint prioritySum = 0;
		var ruckSacksData = File.ReadLines(AssetPath());
		foreach (var ruckSack in ruckSacksData)
		{
			var ruckSackCompartimentLength = ruckSack.Length / 2;
			var ruckSackCompartimentA = ruckSack[..ruckSackCompartimentLength];
			var ruckSackCompartimentB = ruckSack[ruckSackCompartimentLength..];
			prioritySum += FindDuplicatesAndSumReduce(ruckSackCompartimentA, ruckSackCompartimentB);
		}

		Console.WriteLine(prioritySum);
	}

	public override void SolvePart2()
	{
		uint prioritySum = 0;
		var ruckSacksData = File.ReadAllLines(AssetPath());
		var bagIndex = 0;
		do
		{
			var localIndex = bagIndex * 3;
			var ruckSackA = ruckSacksData.ElementAt(localIndex);
			var ruckSackB = ruckSacksData.ElementAt(localIndex + 1);
			var ruckSackC = ruckSacksData.ElementAt(localIndex + 2);
			prioritySum += FindDuplicatesAndSumReduce(ruckSackA, ruckSackB, ruckSackC);
		} while (++bagIndex < (ruckSacksData.Length / 3));

		Console.WriteLine(prioritySum);
	}

	private static uint FindDuplicatesAndSumReduce(params string[] dataSets)
	{
		var duplicates = dataSets[0].Intersect(dataSets[1]);
		for (var i = 2; i < dataSets.Length; i++)
		{
			duplicates = duplicates.Intersect(dataSets[i]);
		}

		;

		return duplicates.Aggregate((uint) 0, (acc, c) => acc + ConvertToPriority(c));
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
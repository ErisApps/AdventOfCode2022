using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day01 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		var caloriesList = ParseCaloriesData();
		return caloriesList.Max();
	}

	public override object SolvePart2()
	{
		var caloriesList = ParseCaloriesData();
		return caloriesList.OrderDescending().Take(3).Sum(x => x);
	}

	private IEnumerable<uint> ParseCaloriesData()
	{
		var caloriesEnumerable = File.ReadLines(AssetPath());
		var caloriesList = new List<uint> { 0 };
		foreach (var calorieRaw in caloriesEnumerable)
		{
			if (string.IsNullOrWhiteSpace(calorieRaw))
			{
				caloriesList.Add(0);
			}
			else
			{
				caloriesList[^1] += uint.Parse(calorieRaw);
			}
		}

		return caloriesList;
	}
}
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

	var currentCalories = 0u;
	foreach (var calorieRaw in caloriesEnumerable)
	{
		if (string.IsNullOrWhiteSpace(calorieRaw))
		{
			yield return currentCalories;
			currentCalories = 0;
		}
		else
		{
			currentCalories += uint.Parse(calorieRaw);
		}
	}
}
}
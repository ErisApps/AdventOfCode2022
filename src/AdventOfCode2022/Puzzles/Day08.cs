using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day08 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		var treeHeightMap = ParseTreeHeightMap();

		var forestHeight = treeHeightMap.Length;
		var forestWidth = treeHeightMap[0].Length;
		var visibleTreeCount = 2 * forestHeight + 2 * forestWidth - 4;
		// -4 so we don't count the corners twice

		for (var y = 1; y < forestHeight - 1; y++)
		for (var x = 1; x < forestWidth - 1; x++)
		{
			if (CheckTreeVisible(treeHeightMap, x, y))
			{
				visibleTreeCount++;
			}
		}

		return visibleTreeCount;
	}

	public override object SolvePart2()
	{
		var treeHeightMap = ParseTreeHeightMap();

		IEnumerable<int> ScenicScoreEnumerator()
		{
			var forestHeight = treeHeightMap.Length;
			var forestWidth = treeHeightMap[0].Length;

			for (var x = 1; x < forestWidth - 1; x++)
			for (var y = 1; y < forestHeight - 1; y++)
			{
				yield return CalculateScenicScore(treeHeightMap, x, y);
			}
		}

		return ScenicScoreEnumerator().Max();
	}

	private int[][] ParseTreeHeightMap()
	{
		return File.ReadLines(AssetPath())
			.Select(treeLine => treeLine.Select(treeHeight => (int) char.GetNumericValue(treeHeight)).ToArray())
			.ToArray();
	}

	// Helpers part 1
	private static bool CheckTreeVisible(int[][] treeHeightMap, int x, int y)
	{
		var visible = false;

		var treeRow = treeHeightMap[y];
		// Check if numbers to the left are lower than the number from current position
		visible |= CheckTreeVisibilityInDirection(treeRow, x, -1);

		// Check if numbers to the right are lower than the number from current position
		visible |= CheckTreeVisibilityInDirection(treeRow, x, 1);

		var treeColumn = treeHeightMap.Select(line => line[x]).ToArray();
		// Check if numbers to the top are lower than the number from current position
		visible |= CheckTreeVisibilityInDirection(treeColumn, y, -1);

		// Check if numbers to the bottom are lower than the number from current position
		visible |= CheckTreeVisibilityInDirection(treeColumn, y, 1);

		return visible;
	}

	private static bool CheckTreeVisibilityInDirection(int[] treeHeightsForOrientation, int index, int direction)
	{
		var heightThreshold = treeHeightsForOrientation[index];
		index += direction;

		bool CanContinue() => direction switch
		{
			-1 => index >= 0,
			1 => index < treeHeightsForOrientation.Length,
			_ => throw new NotSupportedException()
		};

		while (CanContinue())
		{
			var localThreeHeight = treeHeightsForOrientation[index];
			if (localThreeHeight >= heightThreshold)
			{
				return false;
			}

			index += direction;
		}

		return true;
	}

	// Helpers part 2
	private static int CalculateScenicScore(int[][] treeHeightMap, int x, int y)
	{
		var scenicScore = 1;

		var treeRow = treeHeightMap[y];
		// Calculate scenic score for trees to the left of the current position
		scenicScore *= CalculateScenicScoreInDirection(treeRow, x, -1);

		// Calculate scenic score for trees to the right of the current position
		scenicScore *= CalculateScenicScoreInDirection(treeRow, x, 1);

		var treeColumn = treeHeightMap.Select(line => line[x]).ToArray();
		// Calculate scenic score for trees above the current position
		scenicScore *= CalculateScenicScoreInDirection(treeColumn, y, -1);

		// Calculate scenic score for trees below the current position
		scenicScore *= CalculateScenicScoreInDirection(treeColumn, y, 1);

		return scenicScore;
	}

	private static int CalculateScenicScoreInDirection(int[] treeHeightsForOrientation, int index, int direction)
	{
		var heightThreshold = treeHeightsForOrientation[index];
		index += direction;

		bool CanContinue() => direction switch
		{
			-1 => index >= 0,
			1 => index < treeHeightsForOrientation.Length,
			_ => throw new NotSupportedException()
		};

		var scenicScore = 0;
		while (CanContinue())
		{
			scenicScore ++;
			var localThreeHeight = treeHeightsForOrientation[index];
			if (localThreeHeight >= heightThreshold)
			{
				return scenicScore;
			}

			index += direction;
		}

		return scenicScore;
	}
}
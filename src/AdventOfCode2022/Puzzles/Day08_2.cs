using System.Runtime.CompilerServices;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day08_2 : HappyPuzzleBase
{
	protected override string AssetName => "day08.txt";

	public override object SolvePart1()
	{
		var (treeHeightMap, height, width) = ParseTreeHeightMap();
		var visibleTreeCount = 2 * height + 2 * width - 4;
		// -4 so we don't count the corners twice

		ReadOnlySpan<int> treeHeightSpan = treeHeightMap.AsSpan();

		for (var y = 1; y < height - 1; y++)
		for (var x = 1; x < width - 1; x++)
		{
			if (CheckTreeVisible(ref treeHeightSpan, width, x, y))
			{
				visibleTreeCount++;
			}
		}

		return visibleTreeCount;
	}

	public override object SolvePart2()
	{
		var (treeHeightMap, height, width) = ParseTreeHeightMap();
		ReadOnlySpan<int> treeHeightSpan = treeHeightMap.AsSpan();

		var max = 0;
		for (var x = 1; x < width - 1; x++)
		for (var y = 1; y < height - 1; y++)
		{
			var score = CalculateScenicScore(ref treeHeightSpan, width, x, y);
			if (score > max)
			{
				max = score;
			}
		}

		return max;
	}

	private (int[] treeHeightMap, int height, int width) ParseTreeHeightMap()
	{
		var treeHeightMapData = File.ReadAllLines(AssetPath());
		return (
			treeHeightMap: File.ReadAllLines(AssetPath())
				.SelectMany(treeLine => treeLine.Select(treeHeight => (int) treeHeight))
				.ToArray(),
			height: treeHeightMapData.Length,
			width: treeHeightMapData[0].Length);
	}

	// Helpers part 1
	private static bool CheckTreeVisible(ref ReadOnlySpan<int> treeHeightList, int width, int x, int y)
	{
		var treeRow = treeHeightList.Slice(y * width, width);
		// Check if numbers to the left are lower than the number from current position
		if (CheckTreeVisibilityInDirection(ref treeRow, x, -1) ||
		    // Check if numbers to the right are lower than the number from current position
		    CheckTreeVisibilityInDirection(ref treeRow, x, 1))
		{
			return true;
		}

		var treeColumn = treeHeightList[x..];
		// Check if numbers to the top are lower than the number from current position
		return CheckTreeVisibilityInDirection(ref treeColumn, y * width, -width) ||
		       // Check if numbers to the bottom are lower than the number from current position
		       CheckTreeVisibilityInDirection(ref treeColumn, y * width, width);
	}

	private static bool CheckTreeVisibilityInDirection(ref ReadOnlySpan<int> treeHeightsForOrientation, int index, int offset)
	{
		var heightThreshold = treeHeightsForOrientation[index];
		index += offset;

		while (offset switch
		       {
			       < 0 => index >= 0,
			       > 0 => index < treeHeightsForOrientation.Length,
			       _ => throw new NotSupportedException()
		       })
		{
			var localThreeHeight = treeHeightsForOrientation[index];
			if (localThreeHeight >= heightThreshold)
			{
				return false;
			}

			index += offset;
		}

		return true;
	}

	// Helpers part 2
	private static int CalculateScenicScore(ref ReadOnlySpan<int> treeHeightList, int width, int x, int y)
	{
		var treeRow = treeHeightList.Slice(y * width, width);
		var treeColumn = treeHeightList[x..];

		return CalculateScenicScoreInBiDirectional(ref treeRow, x, 1) * // Check if numbers to the left are lower than the number from current position
		       CalculateScenicScoreInBiDirectional(ref treeColumn, y * width, width); // Check if numbers to the bottom are lower than the number from current position
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int CalculateScenicScoreInBiDirectional(ref ReadOnlySpan<int> treeHeightsForOrientation, int index, int offset)
	{
		return CalculateScenicScoreInDirection(ref treeHeightsForOrientation, index, -offset) *
		       CalculateScenicScoreInDirection(ref treeHeightsForOrientation, index, offset);
	}

	private static int CalculateScenicScoreInDirection(ref ReadOnlySpan<int> treeHeightsForOrientation, int index, int offset)
	{
		var heightThreshold = treeHeightsForOrientation[index];
		index += offset;

		var scenicScore = 0;

		while (offset switch
		       {
			       < 0 => index >= 0,
			       > 0 => index < treeHeightsForOrientation.Length,
			       _ => throw new NotSupportedException()
		       })
		{
			scenicScore ++;
			var localThreeHeight = treeHeightsForOrientation[index];
			if (localThreeHeight >= heightThreshold)
			{
				return scenicScore;
			}

			index += offset;
		}

		return scenicScore;
	}
}
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day12 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		ReadOnlySpan<string> terrainRowDataSpans = File.ReadAllLines(AssetPath());
		var terrainHeight = terrainRowDataSpans.Length;
		var terrainWidth = terrainRowDataSpans[0].Length;
		var terrainSize = terrainHeight * terrainWidth;

		// var terrainData = GC.AllocateUninitializedArray<int>(terrainSize).AsSpan();
		Span<int> terrainData = stackalloc int[terrainSize];

		var startingIndex = PrepareDataAndFindStartingIndex(ref terrainRowDataSpans, terrainHeight, terrainWidth, terrainData, 'E');

		return FindShortestPathDescending(terrainData, terrainHeight, terrainWidth, terrainSize, startingIndex, 'S');
	}

	public override object SolvePart2()
	{
		ReadOnlySpan<string> terrainRowDataSpans = File.ReadAllLines(AssetPath());
		var terrainHeight = terrainRowDataSpans.Length;
		var terrainWidth = terrainRowDataSpans[0].Length;
		var terrainSize = terrainHeight * terrainWidth;

		// var terrainData = GC.AllocateUninitializedArray<int>(terrainSize).AsSpan();
		Span<int> terrainData = stackalloc int[terrainSize];

		var startingIndex = PrepareDataAndFindStartingIndex(ref terrainRowDataSpans, terrainHeight, terrainWidth, terrainData, 'E');

		return FindShortestPathDescending(terrainData, terrainHeight, terrainWidth, terrainSize, startingIndex, 'a');
	}

	private static int PrepareDataAndFindStartingIndex(ref ReadOnlySpan<string> terrainDataRaw, int terrainHeight, int terrainWidth, scoped Span<int> terrainData, char startingChar)
	{
		var startingIndex = -1;
		var terrainIndex = 0;
		for (var rowIndex = 0; rowIndex < terrainHeight; rowIndex++)
		{
			var rawRowData = terrainDataRaw[rowIndex];
			for (var columnIndex = 0; columnIndex < terrainWidth; columnIndex++)
			{
				var terrainValue = rawRowData[columnIndex];
				if (terrainValue == startingChar)
				{
					startingIndex = terrainIndex;
				}

				// Rewrite the start and end points data so it's just 1 lower/higher compared to the second lowest/highest point in the map
				terrainData[terrainIndex++] = RemapTerrainValue(terrainValue);
			}
		}

		return startingIndex;
	}

	private static int FindShortestPathDescending(ReadOnlySpan<int> terrainData, int terrainHeight, int terrainWidth, int terrainSize, int startingIndex, char endingChar)
	{
		var endingValue = RemapTerrainValue(endingChar);

		// According to actual testing against the puzzle input, 19 should be enough to reach the end
		// But given that the input is not guaranteed to be solvable within that buffer size, we'll use a much higher value
		var waveSearchBufferMaxSize = terrainHeight * 2 + terrainWidth * 2;

		// Queue-like stoof for BFS
		// Contains the indexes of map tiles that need to have their neighbours searched for the current waveStep
		var waveSearchBufferSize = 0;
		Span<int> waveSearchBuffer = stackalloc int[waveSearchBufferMaxSize];
		// Placeholder buffer for the next waveStep round checks
		var nextWaveSearchBufferSize = 0;
		Span<int> nextWaveSearchBuffer = stackalloc int[waveSearchBufferMaxSize];

		// Indicates whether a given index has already been visited
		Span<bool> terrainVisitedData = stackalloc bool[terrainSize];

		// Initialize the first waveSearchBuffer with the starting index
		waveSearchBuffer[waveSearchBufferSize++] = startingIndex;
		terrainVisitedData[startingIndex] = true;

		for (var currentWaveStep = 1;; currentWaveStep++)
		{
			for (var waveSearchBufferIndex = 0; waveSearchBufferIndex < waveSearchBufferSize; waveSearchBufferIndex++)
			{
				ref var terrainIndex = ref waveSearchBuffer[waveSearchBufferIndex];

				// Get the x-y coordinates for the current index
				var terrainRow = terrainIndex / terrainWidth;
				var terrainColumn = terrainIndex % terrainWidth;

				var terrainValue = terrainData[terrainIndex];
				var declineThreshold = terrainValue - 1;

				// Check top
				if (terrainRow > 0)
				{
					var topIndex = terrainIndex - terrainWidth;
					var topValue = terrainData[topIndex];

					if (!terrainVisitedData[topIndex] && topValue >= declineThreshold)
					{
						if (topValue == endingValue)
						{
							return currentWaveStep;
						}

						terrainVisitedData[topIndex] = true;
						nextWaveSearchBuffer[nextWaveSearchBufferSize++] = topIndex;
					}
				}

				// Check bottom
				if (terrainRow < terrainHeight - 1)
				{
					var bottomIndex = terrainIndex + terrainWidth;
					var bottomValue = terrainData[bottomIndex];

					if (!terrainVisitedData[bottomIndex] && bottomValue >= declineThreshold)
					{
						if (bottomValue == endingValue)
						{
							return currentWaveStep;
						}

						terrainVisitedData[bottomIndex] = true;
						nextWaveSearchBuffer[nextWaveSearchBufferSize++] = bottomIndex;
					}
				}

				// Check left
				if (terrainColumn > 0)
				{
					var leftIndex = terrainIndex - 1;
					var leftValue = terrainData[leftIndex];

					if (!terrainVisitedData[leftIndex] && leftValue >= declineThreshold)
					{
						if (leftValue == endingValue)
						{
							return currentWaveStep;
						}

						terrainVisitedData[leftIndex] = true;
						nextWaveSearchBuffer[nextWaveSearchBufferSize++] = leftIndex;
					}
				}

				// Check right
				if (terrainColumn < terrainWidth - 1)
				{
					var rightIndex = terrainIndex + 1;
					var rightValue = terrainData[rightIndex];

					if (!terrainVisitedData[rightIndex] && rightValue >= declineThreshold)
					{
						if (rightValue == endingValue)
						{
							return currentWaveStep;
						}

						terrainVisitedData[rightIndex] = true;
						nextWaveSearchBuffer[nextWaveSearchBufferSize++] = rightIndex;
					}
				}
			}

			// Copy the nextWaveSearchBuffer to the waveSearchBuffer so we can reuse it
			for (var i = 0; i < nextWaveSearchBufferSize; i++)
			{
				waveSearchBuffer[i] = nextWaveSearchBuffer[i];
			}

			waveSearchBufferSize = nextWaveSearchBufferSize;
			nextWaveSearchBufferSize = 0;
		}
	}

	private static int RemapTerrainValue(char terrainValue) => terrainValue switch
	{
		'S' => 'a' - 1, // Start point is supposedly equal to a, but bc the algo searches from highest to lowest, we need to make it lower than the lowest point
		'E' => 'z', // End point height is equal to z
		_ => terrainValue
	};
}
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

		var terrainData = GC.AllocateUninitializedArray<int>(terrainSize).AsSpan();

		var startingIndex = PrepareDataAndFindStartingIndex(ref terrainRowDataSpans, terrainHeight, terrainWidth, ref terrainData, 'E');

		return FindShortestPathDescending(ref terrainData, terrainHeight, terrainWidth, terrainSize, startingIndex, 'S');
	}

	public override object SolvePart2()
	{
		throw new NotImplementedException();
	}

	private static int PrepareDataAndFindStartingIndex(ref ReadOnlySpan<string> terrainDataRaw, int terrainHeight, int terrainWidth, ref Span<int> terrainData, char startingChar)
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

	private static int FindShortestPathDescending(ref Span<int> terrainData, int terrainHeight, int terrainWidth, int terrainSize, int startingIndex, char endingChar)
	{
		var endingValue = RemapTerrainValue(endingChar);

		// Queue-like stoof for BFS
		// Contains the indexes of map tiles that need to have their neighbours searched for the current waveStep
		var waveSearchBufferSize = 0;
		Span<int> waveSearchBuffer = stackalloc int[terrainSize];
		// Placeholder buffer for the next waveStep round checks
		var nextWaveSearchBufferSize = 0;
		Span<int> nextWaveSearchBuffer = stackalloc int[terrainSize];

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

				ref var terrainValue = ref terrainData[terrainIndex];
				var declineThreshold = terrainValue - 1;

				// Check top
				if (terrainRow > 0)
				{
					var topIndex = terrainIndex - terrainWidth;
					ref var topValue = ref terrainData[topIndex];

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
					ref var bottomValue = ref terrainData[bottomIndex];

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
					ref var leftValue = ref terrainData[leftIndex];

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
					ref var rightValue = ref terrainData[rightIndex];

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
			'S' => ('a' - 1),
			'E' => ('z' + 1),
			_ => terrainValue
		};
}
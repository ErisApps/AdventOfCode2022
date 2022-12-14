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

		var terrainData = GC.AllocateUninitializedArray<char>(terrainSize).AsSpan();

		var startingIndex = PrepareDataAndFindStartingIndex(ref terrainRowDataSpans, terrainHeight, terrainWidth, ref terrainData);

		return FindShortestPathDescending(ref terrainData, terrainHeight, terrainWidth, terrainSize, startingIndex, 'S');
	}

	public override object SolvePart2()
	{
		throw new NotImplementedException();
	}

	private static int PrepareDataAndFindStartingIndex(ref ReadOnlySpan<string> terrainDataRaw, int terrainHeight, int terrainWidth, ref Span<char> terrainData)
	{
		var startingIndex = -1;
		var terrainIndex = 0;
		for (var rowIndex = 0; rowIndex < terrainHeight; rowIndex++)
		{
			var rawRowData = terrainDataRaw[rowIndex];
			for (var columnIndex = 0; columnIndex < terrainWidth; columnIndex++)
			{
				var terrainValue = rawRowData[columnIndex];
				if (terrainValue == 'E')
				{
					startingIndex = terrainIndex;
				}

				terrainData[terrainIndex++] = terrainValue;
			}
		}

		// Rewrite the starting point data so it's just 1 higher than the highest point in the terrain
		terrainData[startingIndex] = (char) (1 + 'z');

		return startingIndex;
	}

	private static int FindShortestPathDescending(ref Span<char> terrainData, int terrainHeight, int terrainWidth, int terrainSize, int startingIndex, char endingChar)
	{
		// Queue-like stoof for BFS
		// Contains the indexes of map tiles that need to have their neighbours searched for the current waveStep
		var waveSearchBufferSize = 0;
		Span<int> waveSearchBuffer = stackalloc int[terrainSize];
		// Placeholder buffer for the next waveStep round checks
		var nextWaveSearchBufferSize = 0;
		Span<int> nextWaveSearchBuffer = stackalloc int[terrainSize];

		// Indicates whether a given index has already been visited
		Span<bool> terrainVisitedData = stackalloc bool[terrainSize];

		// Purely for debugging
		Span<int> terrainStepData = stackalloc int[terrainSize];

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

					if (topValue == endingChar)
					{
						WriteDebugData(ref terrainStepData, terrainWidth, terrainHeight);
						return currentWaveStep;
					}

					if (topValue >= declineThreshold && !terrainVisitedData[topIndex])
					{
						terrainStepData[topIndex] = currentWaveStep;

						terrainVisitedData[topIndex] = true;
						nextWaveSearchBuffer[nextWaveSearchBufferSize++] = topIndex;
					}
				}

				// Check bottom
				if (terrainRow < terrainHeight - 1)
				{
					var bottomIndex = terrainIndex + terrainWidth;
					ref var bottomValue = ref terrainData[bottomIndex];

					if (bottomValue == endingChar)
					{
						WriteDebugData(ref terrainStepData, terrainWidth, terrainHeight);
						return currentWaveStep;
					}

					if (bottomValue >= declineThreshold && !terrainVisitedData[bottomIndex])
					{
						terrainStepData[bottomIndex] = currentWaveStep;

						terrainVisitedData[bottomIndex] = true;
						nextWaveSearchBuffer[nextWaveSearchBufferSize++] = bottomIndex;
					}
				}

				// Check left
				if (terrainColumn > 0)
				{
					var leftIndex = terrainIndex - 1;
					ref var leftValue = ref terrainData[leftIndex];

					if (leftValue == endingChar)
					{
						WriteDebugData(ref terrainStepData, terrainWidth, terrainHeight);
						return currentWaveStep;
					}

					if (leftValue >= declineThreshold && !terrainVisitedData[leftIndex])
					{
						terrainStepData[leftIndex] = currentWaveStep;

						terrainVisitedData[leftIndex] = true;
						nextWaveSearchBuffer[nextWaveSearchBufferSize++] = leftIndex;
					}
				}

				// Check right
				if (terrainColumn < terrainWidth - 1)
				{
					var rightIndex = terrainIndex + 1;
					ref var rightValue = ref terrainData[rightIndex];

					if (rightValue == endingChar)
					{
						WriteDebugData(ref terrainStepData, terrainWidth, terrainHeight);
						return currentWaveStep;
					}

					if (rightValue >= declineThreshold && !terrainVisitedData[rightIndex])
					{
						terrainStepData[rightIndex] = currentWaveStep;

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

	private static void WriteDebugData(ref Span<int> heightMapData, int terrainWidth, int terrainHeight)
	{
		var debugOutputFolderPath = Path.Combine(Environment.CurrentDirectory, "Debug");
		if (Directory.Exists(debugOutputFolderPath))
		{
			Directory.Delete(debugOutputFolderPath, true);
		}

		Directory.CreateDirectory(debugOutputFolderPath);

		using var fileStream = File.Create(Path.Combine(debugOutputFolderPath, "day12.txt"));
		using var streamWriter = new StreamWriter(fileStream);

		for (var i = 0; i < terrainHeight; i++)
		{
			streamWriter.WriteLine(heightMapData.Slice(i * terrainHeight, terrainWidth).ToArray().ToString());
		}
	}
}
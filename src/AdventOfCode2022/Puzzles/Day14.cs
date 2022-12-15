using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day14 : HappyPuzzleBase
{
	private const int SAND_PARTICLE_START_X = 500;
	private const int SAND_PARTICLE_START_Y = 0;

	// ReSharper disable once CognitiveComplexity
	public override object SolvePart1()
	{
		ReadOnlySpan<string> rockPlacementDescriptors = File.ReadAllLines(AssetPath());

		// Will be used to preparse the rock placement descriptors into a more efficient format
		// this way we're also able to determine the bounds of the cave more efficiently (read: not having to estimate)
		// Data format is as follows:
		// - 2 numbers indicating the x and y values of a coordinate, this is repeated at least once to indicate the end of the segment.
		// - The second coordinate will be reused as the start for the next segment, unless the x value of the new coordinate pair is 0.
		// - In that case, the next coordinate pair (starting at index + 1) will be the start of a new segment.
		Span<int> rockPlacementCoordinatesBuffer = stackalloc int[4000];

		// Preparse the rock placement descriptors into a more efficient format
		var rockPlacementCoordinateBufferLength = PrepareRockPlacementCoordinateBuffer(ref rockPlacementDescriptors, rockPlacementCoordinatesBuffer, out var minX, out var maxX, out var height);
		var caveHeight = height + 1;
		var caveWidth = maxX - minX + 1;
		Span<bool> caveData = stackalloc bool[caveHeight * caveWidth];

		var xOffset = minX;
		for (var i = 0; i < rockPlacementCoordinateBufferLength; i += 2)
		{
			// Grab slice indicating the start and end coordinates of a rocket placement line
			var slice = rockPlacementCoordinatesBuffer.Slice(i, 4);

			if (slice[2] == 0)
			{
				i++;
				continue;
			}

			var xStart = slice[0] - xOffset;
			var yStart = slice[1];
			var xEnd = slice[2] - xOffset;
			var yEnd = slice[3];

			if (xStart > xEnd)
			{
				(xStart, xEnd) = (xEnd, xStart);
			}

			if (yStart > yEnd)
			{
				(yStart, yEnd) = (yEnd, yStart);
			}

			for (var y = yStart; y <= yEnd; y++)
			for (var x = xStart; x <= xEnd; x++)
			{
				caveData[y * caveWidth + x] = true;
			}
		}

		return SimulateSandParticles(ref caveData, (SAND_PARTICLE_START_X - xOffset, SAND_PARTICLE_START_Y), caveHeight, caveWidth);
	}

	// ReSharper disable once CognitiveComplexity
	public override object SolvePart2()
	{
		ReadOnlySpan<string> rockPlacementDescriptors = File.ReadAllLines(AssetPath());

		// Will be used to preparse the rock placement descriptors into a more efficient format
		// this way we're also able to determine the bounds of the cave more efficiently (read: not having to estimate)
		// Data format is as follows:
		// - 2 numbers indicating the x and y values of a coordinate, this is repeated at least once to indicate the end of the segment.
		// - The second coordinate will be reused as the start for the next segment, unless the x value of the new coordinate pair is 0.
		// - In that case, the next coordinate pair (starting at index + 1) will be the start of a new segment.
		Span<int> rockPlacementCoordinatesBuffer = stackalloc int[4000];

		// Preparse the rock placement descriptors into a more efficient format
		var rockPlacementCoordinateBufferLength = PrepareRockPlacementCoordinateBuffer(ref rockPlacementDescriptors, rockPlacementCoordinatesBuffer, out _, out _, out var heightIndex);

		// Do some more magick regarding cave bounds calculation
		var caveHeight = heightIndex + 3;

		var minX = SAND_PARTICLE_START_X - caveHeight + 1;
		var maxX = SAND_PARTICLE_START_X + caveHeight + 1;
		var caveWidth = maxX - minX + 1;

		Span<bool> caveData = stackalloc bool[caveHeight * caveWidth];

		var xOffset = minX;
		for (var i = 0; i < rockPlacementCoordinateBufferLength; i += 2)
		{
			// Grab slice indicating the start and end coordinates of a rocket placement line
			var slice = rockPlacementCoordinatesBuffer.Slice(i, 4);

			if (slice[2] == 0)
			{
				i++;
				continue;
			}

			var xStart = slice[0] - xOffset;
			var yStart = slice[1];
			var xEnd = slice[2] - xOffset;
			var yEnd = slice[3];

			if (xStart > xEnd)
			{
				(xStart, xEnd) = (xEnd, xStart);
			}

			if (yStart > yEnd)
			{
				(yStart, yEnd) = (yEnd, yStart);
			}

			for (var y = yStart; y <= yEnd; y++)
			for (var x = xStart; x <= xEnd; x++)
			{
				caveData[y * caveWidth + x] = true;
			}
		}

		caveData[((caveHeight -1) * caveWidth)..].Fill(true);


		return SimulateSandParticles(ref caveData, (SAND_PARTICLE_START_X - xOffset, SAND_PARTICLE_START_Y), caveHeight, caveWidth);
	}

	// ReSharper disable once CognitiveComplexity
	private static int PrepareRockPlacementCoordinateBuffer(ref ReadOnlySpan<string> rockPlacementDescriptors, scoped Span<int> rockPlacementCoordinatesBuffer,
		out int minX, out int maxX,
		out int height)
	{
		minX = SAND_PARTICLE_START_X;
		maxX = SAND_PARTICLE_START_X;
		height = SAND_PARTICLE_START_Y;

		var rockPlacementCoordinatesBufferIndex = -1;
		foreach (ReadOnlySpan<char> rockPlacementDescriptor in rockPlacementDescriptors)
		{
			var startIndex = 0;
			var currentIndex = 0;

			rockPlacementCoordinatesBufferIndex++;

			do
			{
				ref var x = ref rockPlacementCoordinatesBuffer[rockPlacementCoordinatesBufferIndex++];
				ref var y = ref rockPlacementCoordinatesBuffer[rockPlacementCoordinatesBufferIndex++];

				// Find and parse X
				do
				{
					currentIndex++;
				} while (char.IsDigit(rockPlacementDescriptor[currentIndex]));

				x = SpecializedCaedenIntParser(rockPlacementDescriptor.Slice(startIndex, currentIndex - startIndex));

				if (x < minX)
				{
					minX = x;
				}
				else if (x > maxX)
				{
					maxX = x;
				}

				// Advance past the comma and reconfigure start index for y
				currentIndex++;
				startIndex = currentIndex;

				// Find and parse Y
				do
				{
					currentIndex++;
				} while (currentIndex < rockPlacementDescriptor.Length && char.IsDigit(rockPlacementDescriptor[currentIndex]));

				y = SpecializedCaedenIntParser(rockPlacementDescriptor.Slice(startIndex, currentIndex - startIndex));
				if (height < y)
				{
					height = y;
				}

				// Skip the arrow pointer
				currentIndex += 4;
				startIndex = currentIndex;
			} while (currentIndex < rockPlacementDescriptor.Length);
		}

		return rockPlacementCoordinatesBufferIndex;
	}

	// ReSharper disable once CognitiveComplexity
	private static int SimulateSandParticles(ref Span<bool> caveData, (int x, int y) sandParticle, int caveHeight, int caveWidth)
	{
		var simulatedSandParticleCount = 0;

		int sandParticleY;

		do
		{
			var sandParticleX = sandParticle.x;
			sandParticleY = sandParticle.y;

			while (true)
			{
				var referenceTargetIndex = (sandParticleY + 1) * caveWidth + sandParticleX;

				// Try moving down
				if (!caveData[referenceTargetIndex])
				{
					sandParticleY++;
					continue;
				}

				// Try moving left-down
				if (sandParticleX > 0)
				{
					if (!caveData[referenceTargetIndex - 1])
					{
						sandParticleX--;
						sandParticleY++;
						continue;
					}
				}
				else
				{
					return simulatedSandParticleCount;
				}

				// Try moving right-down
				if (sandParticleX < caveWidth)
				{
					if (!caveData[referenceTargetIndex + 1])
					{
						sandParticleX++;
						sandParticleY++;
						continue;
					}
				}
				else
				{
					return simulatedSandParticleCount;
				}

				caveData[referenceTargetIndex - caveWidth] = true;
				break;
			}

			simulatedSandParticleCount++;
		} while (sandParticleY <= caveHeight && sandParticleY > sandParticle.y);

		return simulatedSandParticleCount;
	}

	/*private static void PrintCaveData(ref Span<int> caveData, int height, int width)
	{
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				Console.Write(caveData[y * width + x] switch
				{
					0 => '.',
					1 => 'o',
					2 => '#',
					_ => throw new UnreachableException()
				});
			}

			Console.WriteLine();
		}
	}*/

	private static int SpecializedCaedenIntParser(ReadOnlySpan<char> span)
	{
		return span.Length switch
		{
			3 => (span[0] - '0') * 100 + (span[1] - '0') * 10 + (span[2] - '0'),
			2 => (span[0] - '0') * 10 + (span[1] - '0'),
			_ => span[0] - '0'
		};
	}
}
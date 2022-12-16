using System.Diagnostics;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day15 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		const int rowToCheckAgainst = 2000000;

		ReadOnlySpan<string> sensorReports = File.ReadAllLines(AssetPath());

		var sensorReportsCoordinateBufferSize = sensorReports.Length * 2;
		Span<Coordinate> sensorReportsCoordinateBuffer = stackalloc Coordinate[sensorReportsCoordinateBufferSize];
		Span<int> sensorReportsCoordinateDistanceBuffer = stackalloc int[sensorReports.Length];

		PrepareSensorReportsCoordinateBuffer(ref sensorReports, sensorReportsCoordinateBuffer, sensorReportsCoordinateDistanceBuffer);

		Span<CoordinateRange> sensorRowData = stackalloc CoordinateRange[sensorReports.Length];
		HashSet<Coordinate> uniqueBeaconCoordinatesForRow = new();

		var sensorRowDataIndex = 0;
		for (var i = 0; i < sensorReports.Length; i++)
		{
			var coordinatesSlice = sensorReportsCoordinateBuffer.Slice(i * 2, 2);
			var sensorCoordinate = coordinatesSlice[0];

			var beaconCoordinate = coordinatesSlice[1];
			if (beaconCoordinate.Y == rowToCheckAgainst)
			{
				uniqueBeaconCoordinatesForRow.Add(beaconCoordinate);
			}

			var distanceBetweenSensorAndBeacon = sensorReportsCoordinateDistanceBuffer[i];

			var distanceToTargetRow = Math.Abs(sensorCoordinate.Y - rowToCheckAgainst);
			var remainingDistance = distanceBetweenSensorAndBeacon - distanceToTargetRow;
			if (remainingDistance >= 0)
			{
				var startingPosition = sensorCoordinate.X - remainingDistance;
				var endingPosition = sensorCoordinate.X + remainingDistance;

				sensorRowData[sensorRowDataIndex++] = new CoordinateRange(startingPosition, endingPosition);
			}
		}

		var usedSensorRowData = sensorRowData[..sensorRowDataIndex];
		usedSensorRowData.Sort(CoordinateRangeAscendingXComparison);

		var minX = usedSensorRowData[0].StartIndex;

		var maxX = int.MinValue;
		for (var i = 0; i < usedSensorRowData.Length; i++)
		{
			if (usedSensorRowData[i].EndIndex > maxX)
			{
				maxX = usedSensorRowData[i].EndIndex;
			}
		}

		return maxX - minX - uniqueBeaconCoordinatesForRow.Count + 1;
	}

	public override object SolvePart2()
	{
		const int minBound = 0;
		const int maxBound = 4000000;
		const long tuningFrequency = 4000000;

		ReadOnlySpan<string> sensorReports = File.ReadAllLines(AssetPath());

		var sensorReportsCoordinateBufferSize = sensorReports.Length * 2;
		Span<Coordinate> sensorReportsCoordinateBuffer = stackalloc Coordinate[sensorReportsCoordinateBufferSize];
		Span<int> sensorReportsCoordinateDistanceBuffer = stackalloc int[sensorReports.Length];

		PrepareSensorReportsCoordinateBuffer(ref sensorReports, sensorReportsCoordinateBuffer, sensorReportsCoordinateDistanceBuffer);

		Span<CoordinateRange> sensorRowData = stackalloc CoordinateRange[sensorReports.Length];

		// No larger than 4000000
		for (var row = minBound; row <= maxBound; row++)
		{
			var sensorRowDataIndex = 0;
			for (var i = 0; i < sensorReports.Length; i++)
			{
				var coordinatesSlice = sensorReportsCoordinateBuffer.Slice(i * 2, 2);
				var sensorCoordinate = coordinatesSlice[0];

				var distanceBetweenSensorAndBeacon = sensorReportsCoordinateDistanceBuffer[i];

				var distanceToTargetRow = Math.Abs(sensorCoordinate.Y - row);
				var remainingDistance = distanceBetweenSensorAndBeacon - distanceToTargetRow;
				if (remainingDistance >= 0)
				{
					var startingPosition = sensorCoordinate.X - remainingDistance;
					var endingPosition = sensorCoordinate.X + remainingDistance;

					sensorRowData[sensorRowDataIndex++] = new CoordinateRange(startingPosition, endingPosition);
				}
			}

			var usedSensorRowData = sensorRowData[..sensorRowDataIndex];
			usedSensorRowData.Sort(CoordinateRangeAscendingXComparison);

			var lastX = minBound - 1;

			for (var i = 0; i < usedSensorRowData.Length; i++)
			{
				var currentRange = usedSensorRowData[i];
				if (currentRange.EndIndex < minBound)
				{
					continue;
				}

				if (currentRange.StartIndex > maxBound)
				{
					break;
				}

				if (currentRange.StartIndex - lastX > 1)
				{
					return (lastX + 1) * tuningFrequency + row;
				}

				if (lastX < currentRange.EndIndex)
				{
					lastX = currentRange.EndIndex;
				}
			}
		}

		throw new UnreachableException("Bonk!");
	}

	private static void PrepareSensorReportsCoordinateBuffer(ref ReadOnlySpan<string> sensorReportsRaw,
		scoped Span<Coordinate> sensorReportsCoordinateBuffer,
		scoped Span<int> sensorReportsCoordinateDistanceBuffer)
	{
		const int sensorAtOffset = 12;
		const int closestBeaconOffset = 25;

		var sensorReportsCoordinateBufferIndex = 0;
		for (var i = 0; i < sensorReportsRaw.Length; i++)
		{
			var sensorReportRaw = sensorReportsRaw[i].AsSpan()[sensorAtOffset..];

			var sensorReportRawTraversalIndex = 0;

			ref var sensorCoordinate = ref sensorReportsCoordinateBuffer[sensorReportsCoordinateBufferIndex++];
			ExtractAndParseCoordinate(sensorReportRaw, ref sensorReportRawTraversalIndex, ref sensorCoordinate);

			sensorReportRawTraversalIndex += closestBeaconOffset;

			ref var beaconCoordinate = ref sensorReportsCoordinateBuffer[sensorReportsCoordinateBufferIndex++];
			ExtractAndParseCoordinate(sensorReportRaw, ref sensorReportRawTraversalIndex, ref beaconCoordinate);

			sensorReportsCoordinateDistanceBuffer[i] = sensorCoordinate - beaconCoordinate;
		}
	}

	private static void ExtractAndParseCoordinate(
		ReadOnlySpan<char> sensorReportRaw, ref int sensorReportRawTraversalIndex, ref Coordinate coordinate)
	{
		var startIndex = sensorReportRawTraversalIndex;

		do
		{
			sensorReportRawTraversalIndex++;
		} while (sensorReportRaw[sensorReportRawTraversalIndex] != ',');

		var x = SpecializedCaedenIntParser(sensorReportRaw.Slice(startIndex, sensorReportRawTraversalIndex - startIndex));

		sensorReportRawTraversalIndex += 4;
		startIndex = sensorReportRawTraversalIndex;

		do
		{
			sensorReportRawTraversalIndex++;
		} while (sensorReportRawTraversalIndex < sensorReportRaw.Length && sensorReportRaw[sensorReportRawTraversalIndex] != ':');

		var y = SpecializedCaedenIntParser(sensorReportRaw.Slice(startIndex, sensorReportRawTraversalIndex - startIndex));

		coordinate = new Coordinate(x, y);
	}

	private static int SpecializedCaedenIntParser(ReadOnlySpan<char> span)
	{
		if (span[0] == '-')
		{
			return -SpecializedCaedenIntParser(span[1..]);
		}

		return span.Length switch
		{
			7 => (span[0] - '0') * 1000000 + (span[1] - '0') * 100000 + (span[2] - '0') * 10000 + (span[3] - '0') * 1000 + (span[4] - '0') * 100 + (span[5] - '0') * 10 + (span[6] - '0'),
			6 => (span[0] - '0') * 100000 + (span[1] - '0') * 10000 + (span[2] - '0') * 1000 + (span[3] - '0') * 100 + (span[4] - '0') * 10 + (span[5] - '0'),
			5 => (span[0] - '0') * 10000 + (span[1] - '0') * 1000 + (span[2] - '0') * 100 + (span[3] - '0') * 10 + (span[4] - '0'),
			_ => throw new UnreachableException("Bonk!")
		};
	}

	private readonly struct Coordinate : IEquatable<Coordinate>
	{
		public readonly int X;
		public readonly int Y;

		public Coordinate(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static int operator -(Coordinate left, Coordinate right)
		{
			return Math.Abs(left.X - right.X) + Math.Abs(left.Y - right.Y);
		}

		public bool Equals(Coordinate other)
		{
			return X == other.X && Y == other.Y;
		}
	}

	private readonly struct CoordinateRange
	{
		public readonly int StartIndex;
		public readonly int EndIndex;

		public CoordinateRange(int startIndex, int endIndex)
		{
			StartIndex = startIndex;
			EndIndex = endIndex;
		}
	}

	// Changing this reduces allocations with more than 99%, ~214MBs for part 2 are reduced to 384B
	private static readonly Comparison<CoordinateRange>  CoordinateRangeAscendingXComparison = (r1, r2) => r1.StartIndex - r2.StartIndex;
	// private static readonly Comparer<CoordinateRange> CoordinateRangeAscendingXComparer = Comparer<CoordinateRange>.Create((range1, range2) => range1.StartIndex - range2.StartIndex);
}
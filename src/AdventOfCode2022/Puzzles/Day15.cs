using System.Diagnostics;
using System.Numerics;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day15 : HappyPuzzleBase
{
	private const int SENSOR_AT_OFFSET = 12;
	private const int CLOSEST_BEACON_OFFSET = 25;

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

		var ascendingXComparer = Comparer<CoordinateRange>.Create((range1, range2) => range1.StartIndex - range2.StartIndex);
		var usedSensorRowData = sensorRowData[..sensorRowDataIndex];
		usedSensorRowData.Sort(ascendingXComparer);

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
		throw new NotImplementedException();
	}

	private static void PrepareSensorReportsCoordinateBuffer(ref ReadOnlySpan<string> sensorReportsRaw,
		scoped Span<Coordinate> sensorReportsCoordinateBuffer,
		scoped Span<int> sensorReportsCoordinateDistanceBuffer)
	{
		var sensorReportsCoordinateBufferIndex = 0;
		for (var i = 0; i < sensorReportsRaw.Length; i++)
		{
			var sensorReportRaw = sensorReportsRaw[i].AsSpan()[SENSOR_AT_OFFSET..];

			var sensorReportRawTraversalIndex = 0;

			ref var sensorCoordinate = ref sensorReportsCoordinateBuffer[sensorReportsCoordinateBufferIndex++];
			ExtractAndParseCoordinate(sensorReportRaw, ref sensorReportRawTraversalIndex, ref sensorCoordinate);

			sensorReportRawTraversalIndex += CLOSEST_BEACON_OFFSET;

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
			8 => (span[0] - '0') * 10000000 + (span[1] - '0') * 1000000 + (span[2] - '0') * 100000 + (span[3] - '0') * 10000 + (span[4] - '0') * 1000 + (span[5] - '0') * 100 + (span[6] - '0') * 10 +
			     (span[7] - '0'),
			7 => (span[0] - '0') * 1000000 + (span[1] - '0') * 100000 + (span[2] - '0') * 10000 + (span[3] - '0') * 1000 + (span[4] - '0') * 100 + (span[5] - '0') * 10 + (span[6] - '0'),
			6 => (span[0] - '0') * 100000 + (span[1] - '0') * 10000 + (span[2] - '0') * 1000 + (span[3] - '0') * 100 + (span[4] - '0') * 10 + (span[5] - '0'),
			5 => (span[0] - '0') * 10000 + (span[1] - '0') * 1000 + (span[2] - '0') * 100 + (span[3] - '0') * 10 + (span[4] - '0'),
#if DEBUG
			4 => (span[0] - '0') * 1000 + (span[1] - '0') * 100 + (span[2] - '0') * 10 + (span[3] - '0'),
			3 => (span[0] - '0') * 100 + (span[1] - '0') * 10 + (span[2] - '0'),
			2 => (span[0] - '0') * 10 + (span[1] - '0'),
			1 => span[0] - '0',
#endif
			_ => throw new UnreachableException("Bonk!")
		};
	}

	private static void PrintMapData(ref Span<int> mapData, int height, int width)
	{
		var outPutFolderPath = Path.Combine(Environment.CurrentDirectory, "Output");
		if (Directory.Exists(outPutFolderPath))
		{
			Directory.Delete(outPutFolderPath, true);
		}

		Directory.CreateDirectory(outPutFolderPath);

		using var fileStream = File.Create(Path.Combine(outPutFolderPath, "map.txt"));
		using var streamWriter = new StreamWriter(fileStream);

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				streamWriter.Write(mapData[y * width + x] switch
				{
					0 => '.',
					1 => 'S',
					2 => 'B',
					_ => throw new UnreachableException()
				});
			}

			streamWriter.WriteLine();
		}
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

		public override bool Equals(object? obj)
		{
			return obj is Coordinate other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
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
}
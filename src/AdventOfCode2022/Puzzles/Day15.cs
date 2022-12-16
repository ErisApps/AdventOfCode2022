using System.Diagnostics;
using System.Numerics;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day15 : HappyPuzzleBase
{
	private const int SENSOR_AT_OFFSET = 12;
	private const int CLOSEST_BEACON_OFFSET = 25;

	private const int ROW_TO_CHECK_AGAINST = 2000000;

	public override object SolvePart1()
	{
		ReadOnlySpan<string> sensorReports = File.ReadAllLines(AssetPath());

		var sensorReportsCoordinateBufferSize = sensorReports.Length * 4;
		Span<int> sensorReportsCoordinateBuffer = stackalloc int[sensorReportsCoordinateBufferSize];

		PrepareSensorReportsCoordinateBuffer(ref sensorReports, sensorReportsCoordinateBuffer, out var minX, out var maxX, out _, out _);

		var xOffset = -4 * minX;
		var mapWidth = 4 * maxX + xOffset + 1;
		var sensorRowData = new bool[mapWidth];

		for (var i = 0; i < sensorReportsCoordinateBuffer.Length; i += 4)
		{
			var slice = sensorReportsCoordinateBuffer.Slice(i, 4);

			var xSensor = slice[0];
			var ySensor = slice[1];

			var xBeacon = slice[2];
			var yBeacon = slice[3];

			var distanceBetweenSensorAndBeacon = CalculateManhattenDistance(xSensor, ySensor, xBeacon, yBeacon);

			var distanceToTargetRow = Math.Abs(ySensor - ROW_TO_CHECK_AGAINST);
			var remainingDistance = distanceBetweenSensorAndBeacon - distanceToTargetRow;
			if (remainingDistance >= 0)
			{
				var startingPosition = Math.Max(0, xSensor - remainingDistance + xOffset);
				var endingPosition = Math.Min(mapWidth - 1, xSensor + remainingDistance + xOffset);
				var length = endingPosition - startingPosition + 1;
				sensorRowData.AsSpan().Slice(startingPosition, length).Fill(true);
			}
		}

		for (var i = 2; i < sensorReportsCoordinateBuffer.Length; i += 4)
		{
			if (sensorReportsCoordinateBuffer[i + 1] == ROW_TO_CHECK_AGAINST)
			{
				sensorRowData[sensorReportsCoordinateBuffer[i] + xOffset] = false;
			}
		}

		return sensorRowData.Count(x => x);
	}

	public override object SolvePart2()
	{
		throw new NotImplementedException();
	}

	private static void PrepareSensorReportsCoordinateBuffer(ref ReadOnlySpan<string> sensorReportsRaw, scoped Span<int> sensorReportsCoordinateBuffer,
		out int minX, out int maxX, out int minY, out int maxY)
	{
		minX = int.MaxValue;
		maxX = int.MinValue;
		minY = int.MaxValue;
		maxY = int.MinValue;

		var sensorReportsCoordinateBufferIndex = 0;
		for (var i = 0; i < sensorReportsRaw.Length; i++)
		{
			var sensorReportRaw = sensorReportsRaw[i].AsSpan()[SENSOR_AT_OFFSET..];

			var sensorReportRawTraversalIndex = 0;
			ExtractAndParseCoordinate(sensorReportRaw, ref sensorReportRawTraversalIndex,
				sensorReportsCoordinateBuffer, ref sensorReportsCoordinateBufferIndex,
				ref minX, ref maxX, ref minY, ref maxY);

			sensorReportRawTraversalIndex += CLOSEST_BEACON_OFFSET;

			ExtractAndParseCoordinate(sensorReportRaw, ref sensorReportRawTraversalIndex,
				sensorReportsCoordinateBuffer, ref sensorReportsCoordinateBufferIndex,
				ref minX, ref maxX, ref minY, ref maxY);
		}
	}

	private static void ExtractAndParseCoordinate(
		ReadOnlySpan<char> sensorReportRaw, ref int sensorReportRawTraversalIndex,
		scoped Span<int> sensorReportsCoordinateBuffer, ref int sensorReportsCoordinateBufferIndex,
		ref int minX, ref int maxX, ref int minY, ref int maxY)
	{
		ref var x = ref sensorReportsCoordinateBuffer[sensorReportsCoordinateBufferIndex++];
		ref var y = ref sensorReportsCoordinateBuffer[sensorReportsCoordinateBufferIndex++];

		var startIndex = sensorReportRawTraversalIndex;

		do
		{
			sensorReportRawTraversalIndex++;
		} while (sensorReportRaw[sensorReportRawTraversalIndex] != ',');

		x = SpecializedCaedenIntParser(sensorReportRaw.Slice(startIndex, sensorReportRawTraversalIndex - startIndex));

		if (x < minX)
		{
			minX = x;
		}

		if (x > maxX)
		{
			maxX = x;
		}

		sensorReportRawTraversalIndex += 4;
		startIndex = sensorReportRawTraversalIndex;

		do
		{
			sensorReportRawTraversalIndex++;
		} while (sensorReportRawTraversalIndex < sensorReportRaw.Length && sensorReportRaw[sensorReportRawTraversalIndex] != ':');

		y = SpecializedCaedenIntParser(sensorReportRaw.Slice(startIndex, sensorReportRawTraversalIndex - startIndex));

		if (y < minY)
		{
			minY = y;
		}

		if (y > maxY)
		{
			maxY = y;
		}
	}

	private static int CalculateManhattenDistance(int x1, int y1, int x2, int y2)
	{
		return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
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
}
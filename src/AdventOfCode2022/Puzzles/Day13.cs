using System.Text.Json;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day13 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		ReadOnlySpan<string> packetDataRaw = File.ReadAllLines(AssetPath());

		var packetsInOrderIndicesSum = 0;
		for (var i = 0; i < packetDataRaw.Length; i += 3)
		{
			var leftPacket = packetDataRaw[i];
			var rightPacket = packetDataRaw[i + 1];

			var leftPacketParsed = JsonDocument.Parse(leftPacket).RootElement;
			var rightPacketParsed = JsonDocument.Parse(rightPacket).RootElement;

			if (ComparePackets(leftPacketParsed, rightPacketParsed) < 0)
			{
				packetsInOrderIndicesSum += i / 3 + 1;
			}
		}

		return packetsInOrderIndicesSum;
	}

	public override object SolvePart2()
	{
		var dividerPacket1 = JsonDocument.Parse("[[2]]").RootElement;
		var dividerPacket2 = JsonDocument.Parse("[[6]]").RootElement;

		ReadOnlySpan<string> packetDataRaw = File.ReadAllLines(AssetPath());

		var totalPacketCount = ((packetDataRaw.Length / 3) + 1) * 2 + 2;
		var packetData = new JsonElement[totalPacketCount];

		var packetDataIndex = 0;
		for (var i = 0; i < packetDataRaw.Length; i += 3)
		{
			packetData[packetDataIndex++] = JsonDocument.Parse(packetDataRaw[i]).RootElement;
			packetData[packetDataIndex++] = JsonDocument.Parse(packetDataRaw[i + 1]).RootElement;
		}

		packetData[packetDataIndex++] = dividerPacket1;
		packetData[packetDataIndex] = dividerPacket2;

		var packetComparer = Comparer<JsonElement>.Create(ComparePackets);
		Array.Sort(packetData, packetComparer);

		var dividerPacket1Index = Array.IndexOf(packetData, dividerPacket1) + 1;
		var dividerPacket2Index = Array.IndexOf(packetData, dividerPacket2) + 1;

		return dividerPacket1Index * dividerPacket2Index;
	}

	// ReSharper disable once CognitiveComplexity
	private static int ComparePackets(JsonElement leftPacket, JsonElement rightPacket)
	{
		while (true)
		{
			if (leftPacket.ValueKind == JsonValueKind.Number && rightPacket.ValueKind == JsonValueKind.Number)
			{
				return leftPacket.GetInt32().CompareTo(rightPacket.GetInt32());
			}

			if (leftPacket.ValueKind == JsonValueKind.Number)
			{
				leftPacket = ConvertToArrayPacket(leftPacket);
			}
			else if (rightPacket.ValueKind == JsonValueKind.Number)
			{
				rightPacket = ConvertToArrayPacket(rightPacket);
			}
			else
			{
				foreach (var (nextLeft, nextRight) in leftPacket.EnumerateArray().Zip(rightPacket.EnumerateArray()))
				{
					var comparisonResult = ComparePackets(nextLeft, nextRight);
					if (comparisonResult != 0)
					{
						return comparisonResult;
					}
				}

				return leftPacket.GetArrayLength() - rightPacket.GetArrayLength();
			}
		}
	}

	private static JsonElement ConvertToArrayPacket(JsonElement packet)
	{
		return JsonDocument.Parse($"[{packet.GetInt32()}]").RootElement;
	}
}
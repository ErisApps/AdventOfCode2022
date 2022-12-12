using System.Diagnostics;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day11 : HappyPuzzleBase
{
	private const int MONKEY_DESCRIPTORS_LINE_COUNT = 7;

	public override object SolvePart1()
	{
		ReadOnlySpan<string> monkeyDescriptorsRaw = File.ReadAllLines(AssetPath());

		var monkeyCount = (monkeyDescriptorsRaw.Length + 1) / MONKEY_DESCRIPTORS_LINE_COUNT;
		Span<MonkeyDescriptor> monkeyDescriptors = stackalloc MonkeyDescriptor[monkeyCount];
		Span<MonkeyRealTimeInformation> monkeyRealTimeInfo = stackalloc MonkeyRealTimeInformation[monkeyCount];

		var itemCount = CalculateItemStoofs(monkeyDescriptorsRaw);
		Span<int> itemWorryLevels = stackalloc int[itemCount];
		Span<int> monkeyItemHolder = stackalloc int[itemCount];

		ParseInput(monkeyDescriptorsRaw, ref monkeyDescriptors, ref monkeyRealTimeInfo, ref itemWorryLevels, ref monkeyItemHolder);

		DoMonkeyStoofs(ref monkeyDescriptors, ref monkeyRealTimeInfo, ref itemWorryLevels, ref monkeyItemHolder);

		return Multiply2LargestNumbers(ref monkeyRealTimeInfo);
	}

	private static int CalculateItemStoofs(ReadOnlySpan<string> monkeyDescriptorsRaw)
	{
		var totalItemCount = 0;
		for (var itemsIndex = 1; itemsIndex < monkeyDescriptorsRaw.Length; itemsIndex += MONKEY_DESCRIPTORS_LINE_COUNT)
		{
			totalItemCount += (monkeyDescriptorsRaw[itemsIndex].Length - 18 + 2) / 4;
		}

		return totalItemCount;
	}

	private static void ParseInput(ReadOnlySpan<string> monkeyDescriptorsRaw,
		ref Span<MonkeyDescriptor> monkeyDescriptors,
		ref Span<MonkeyRealTimeInformation> monkeyRealTimeInfoDescriptors,
		ref Span<int> itemWorryLevels,
		ref Span<int> monkeyItemHolder)
	{
		var rawLineIndex = 0;
		var itemFillIndex = 0;
		do
		{
			var monkeyId = SingleCharIntParser(monkeyDescriptorsRaw[rawLineIndex++][7]);

			var startingItemsDescriptorSpan = monkeyDescriptorsRaw[rawLineIndex++].AsSpan()[18..];
			ref var monkeyRealTimeInfo = ref monkeyRealTimeInfoDescriptors[monkeyId];
			for (var i = 0; i < startingItemsDescriptorSpan.Length; i += 4)
			{
				itemWorryLevels[itemFillIndex] = DualCharIntParser(startingItemsDescriptorSpan[i], startingItemsDescriptorSpan[i + 1]);
				monkeyItemHolder[itemFillIndex] = monkeyId;

				monkeyRealTimeInfo.HoldCount++;
				itemFillIndex++;
			}

			var operationDescriptorSpan = monkeyDescriptorsRaw[rawLineIndex++].AsSpan()[23..];
			var monkeyOperation = operationDescriptorSpan[0] switch
			{
				'+' => Operation.Add,
				'*' => Operation.Multiply,
				_ => throw new UnreachableException("Bonk!")
			};

			var monkeyOperand = 0;
			if (operationDescriptorSpan[2] == 'o')
			{
				monkeyOperation = Operation.Squared;
			}
			else
			{
				var monkeyOperandDescriptorSpan = operationDescriptorSpan[2..];
				monkeyOperand = SpecializedCaedenIntParser(ref monkeyOperandDescriptorSpan);
			}

			var testOperandSpan = monkeyDescriptorsRaw[rawLineIndex++].AsSpan()[21..];
			var monkeyTestOperand = SpecializedCaedenIntParser(ref testOperandSpan);

			var trueMonkeyTarget = SingleCharIntParser(monkeyDescriptorsRaw[rawLineIndex++][29]);
			var falseMonkeyTarget = SingleCharIntParser(monkeyDescriptorsRaw[rawLineIndex++][30]);

			monkeyDescriptors[monkeyId] = new MonkeyDescriptor(monkeyOperation, monkeyOperand, monkeyTestOperand, trueMonkeyTarget, falseMonkeyTarget);

			rawLineIndex++;
		} while (rawLineIndex < monkeyDescriptorsRaw.Length);
	}

	private static void DoMonkeyStoofs(ref Span<MonkeyDescriptor> monkeyDescriptors,
		ref Span<MonkeyRealTimeInformation> monkeyRealTimeInfoDescriptors,
		ref Span<int> itemWorryLevels,
		ref Span<int> monkeyItemHolderInfos)
	{
		for (var round = 0; round < 20; round++)
		{
			for (var monkeyIndex = 0; monkeyIndex < monkeyDescriptors.Length; monkeyIndex++)
			{
				ref var monkeyRealTimeInfo = ref monkeyRealTimeInfoDescriptors[monkeyIndex];
				if (monkeyRealTimeInfo.HoldCount == 0)
				{
					continue;
				}

				ref var monkeyDescriptor = ref monkeyDescriptors[monkeyIndex];

				for (var monkeyItemHolderIndex = 0; monkeyItemHolderIndex < monkeyItemHolderInfos.Length; monkeyItemHolderIndex++)
				{
					ref var monkeyItemHolder = ref monkeyItemHolderInfos[monkeyItemHolderIndex];
					if (monkeyItemHolder != monkeyIndex)
					{
						continue;
					}

					ref var itemWorryLevel = ref itemWorryLevels[monkeyItemHolderIndex];
					itemWorryLevel = monkeyDescriptor.Operation switch
					{
						Operation.Add => itemWorryLevel + monkeyDescriptor.Operand,
						Operation.Multiply => itemWorryLevel * monkeyDescriptor.Operand,
						Operation.Squared => itemWorryLevel * itemWorryLevel,
						_ => throw new UnreachableException("Bonk!")
					};

					itemWorryLevel /= 3;

					monkeyItemHolder = itemWorryLevel % monkeyDescriptor.TestOperand == 0
						? monkeyDescriptor.TrueTargetMonkey
						: monkeyDescriptor.FalseTargetMonkey;

					monkeyRealTimeInfoDescriptors[monkeyItemHolder].HoldCount++;
					monkeyRealTimeInfo.HoldCount--;

					monkeyRealTimeInfo.InspectedCount++;
				}
			}
		}
	}

	private static int Multiply2LargestNumbers(ref Span<MonkeyRealTimeInformation> monkeyRealTimeInfos)
	{
		var largest = 0;
		var secondLargest = 0;

		foreach (var monkeyRealTimeInfo in monkeyRealTimeInfos)
		{
			var inspectionCount = monkeyRealTimeInfo.InspectedCount;
			if (inspectionCount > largest)
			{
				secondLargest = largest;
				largest = inspectionCount;
			}
			else if (inspectionCount > secondLargest)
			{
				secondLargest = inspectionCount;
			}
		}

		return largest * secondLargest;
	}

	public override object SolvePart2()
	{
		throw new NotImplementedException();
	}

	private static int SpecializedCaedenIntParser(ref ReadOnlySpan<char> span)
	{
		return span.Length == 2
			? DualCharIntParser(span[0], span[1])
			: SingleCharIntParser(span[0]);
	}

	private static int DualCharIntParser(char charDigit1, char charDigit2) => SingleCharIntParser(charDigit1) * 10 + SingleCharIntParser(charDigit2);
	private static int SingleCharIntParser(char charDigit) => charDigit - '0';

	// Split Monkey information into 2 structs to prevent excessive defensive copying of the struct when passing it around
	private readonly struct MonkeyDescriptor
	{
		public readonly Operation Operation;
		public readonly int Operand;

		public readonly int TestOperand;

		public readonly int TrueTargetMonkey;
		public readonly int FalseTargetMonkey;

		public MonkeyDescriptor(Operation operation, int operand, int testOperand, int trueTargetMonkey, int falseTargetMonkey)
		{
			Operation = operation;
			Operand = operand;
			TestOperand = testOperand;
			TrueTargetMonkey = trueTargetMonkey;
			FalseTargetMonkey = falseTargetMonkey;
		}
	}

	private struct MonkeyRealTimeInformation
	{
		public int InspectedCount { get; set; }
		public int HoldCount { get; set; }
	}

	private enum Operation
	{
		Add,
		Multiply,
		Squared
	}
}
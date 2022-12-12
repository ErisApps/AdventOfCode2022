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

		var itemCount = CalculateItemStoofs(monkeyDescriptorsRaw, monkeyCount);
		Span<int> itemWorryLevels = stackalloc int[itemCount];
		Span<int> monkeyItemHolder = stackalloc int[itemCount];

		ParseInput(monkeyDescriptorsRaw, ref monkeyDescriptors, ref monkeyRealTimeInfo, ref itemWorryLevels, ref monkeyItemHolder);

		throw new NotImplementedException();
	}

	private static int CalculateItemStoofs(ReadOnlySpan<string> monkeyDescriptorsRaw, int monkeyCount)
	{
		var totalItemCount = 0;
		for (var itemsIndex = 0; itemsIndex < monkeyDescriptorsRaw.Length; itemsIndex += monkeyCount)
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
		var lineIndex = 0;
		do
		{
			var monkeyId = SingleCharIntParser(monkeyDescriptorsRaw[lineIndex++][7]);

			var startingItemsDescriptorSpan = monkeyDescriptorsRaw[lineIndex++].AsSpan()[18..];
			// TODO: Parse starting items

			var operationDescriptorSpan = monkeyDescriptorsRaw[lineIndex++].AsSpan()[23..];
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

			var testOperandSpan = monkeyDescriptorsRaw[lineIndex++].AsSpan()[21..];
			var monkeyTestOperand = SpecializedCaedenIntParser(ref testOperandSpan);

			var trueMonkeyTarget = SingleCharIntParser(monkeyDescriptorsRaw[lineIndex++][29]);
			var falseMonkeyTarget = SingleCharIntParser(monkeyDescriptorsRaw[lineIndex++][30]);

			monkeyDescriptors[monkeyId] = new MonkeyDescriptor(monkeyOperation, monkeyOperand, monkeyTestOperand, trueMonkeyTarget, falseMonkeyTarget);

			lineIndex++;
		} while (lineIndex < monkeyDescriptorsRaw.Length);
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
	}

	private enum Operation
	{
		Add,
		Multiply,
		Squared
	}
}
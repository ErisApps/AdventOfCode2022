using System.Text;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day05 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		var (stacks, instructionSet) = ParseAssetData();
		foreach (var instruction in instructionSet)
		{
			for (var i = 0; i < instruction.amountToMove; i++)
			{
				var crate = stacks[instruction.fromStack].Pop();
				stacks[instruction.toStack].Push(crate);
			}
		}

		return CombineTopStacks(stacks);
	}

	public override object SolvePart2()
	{
		var (stacks, instructionSet) = ParseAssetData();
		foreach (var instruction in instructionSet)
		{
			var moveBuffer = new Stack<char>(instruction.amountToMove);
			for (var i = 0; i < instruction.amountToMove; i++)
			{
				var crate = stacks[instruction.fromStack].Pop();
				moveBuffer.Push(crate);
			}

			for (var i = 0; i < instruction.amountToMove; i++)
			{
				var crate = moveBuffer.Pop();
				stacks[instruction.toStack].Push(crate);
			}
		}

		return CombineTopStacks(stacks);
	}

	private (List<Stack<char>> stacks, IEnumerable<(int amountToMove, int fromStack, int toStack)> instructionSet) ParseAssetData()
	{
		var data = File.ReadAllLines(AssetPath()).ToList();
		var dataSeparatorIndex = data.FindIndex(string.IsNullOrWhiteSpace);

		var stacks = new List<Stack<char>>();
		var stackLayerData = data.GetRange(0, dataSeparatorIndex - 1);
		stackLayerData.Reverse();
		foreach (var stackLayer in stackLayerData)
		{
			stackLayer
				.ToCharArray()
				.Chunk(4)
				.Select((crateInfo, index) => (crate: crateInfo[1], Index: index))
				.ToList()
				.ForEach((crateInfo) =>
				{
					var (crate, index) = crateInfo;

					var crateStack = stacks.ElementAtOrDefault(index);
					if (crateStack == null)
					{
						crateStack = new Stack<char>();
						stacks.Add(crateStack);
					}

					if (!char.IsWhiteSpace(crate))
					{
						crateStack.Push(crate);
					}
				});
		}

		var instructionSet = data.GetRange(dataSeparatorIndex + 1, data.Count - dataSeparatorIndex - 1)
			.Select(instruction => instruction.Split(' '))
			.Select(instructionParts => (
				amountToMove: int.Parse(instructionParts[1]),
				fromStack: int.Parse(instructionParts[3]) - 1,
				toStack: int.Parse(instructionParts[5]) - 1));

		return (stacks, instructionSet);
	}

	private static string CombineTopStacks(List<Stack<char>> stacks)
	{
		var stringBuilder = new StringBuilder();
		foreach (var stack in stacks)
		{
			stringBuilder.Append(stack.Peek());
		}

		return stringBuilder.ToString();
	}
}
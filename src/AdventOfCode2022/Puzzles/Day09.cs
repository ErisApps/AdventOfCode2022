using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day09 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		var instructions = GetInstructions();

		var uniquePositions = new HashSet<Vector2>();
		Vector2 headPosition = new(0, 0);
		Vector2 tailPosition = new(0, 0);
		foreach (var (moveDirection, stepCount) in instructions)
		{
			var moveVector = ConvertDirectionToMoveVector(moveDirection);
			for (var i = 0; i < stepCount; i++)
			{
				headPosition += moveVector;

				var diff = CalculatePositionDifference(headPosition, tailPosition);
				MoveTailIfNeeded(ref tailPosition, diff);

				uniquePositions.Add(tailPosition);
			}
		}

		return uniquePositions.Count;
	}

	public override object SolvePart2()
	{
		var instructions = GetInstructions();

		var knots = new Vector2[10];
		for (var i = 0; i < 10; i++)
		{
			knots[i] = new Vector2(0, 0);
		}

		var uniquePositions = new HashSet<Vector2>();
		foreach (var (moveDirection, stepCount) in instructions)
		{
			var moveVector = ConvertDirectionToMoveVector(moveDirection);
			for (var i = 0; i < stepCount; i++)
			{
				knots[0] += moveVector;

				for (var j = 1; j < knots.Length; j++)
				{
					var diff = CalculatePositionDifference(knots[j - 1], knots[j]);
					if (!MoveTailIfNeeded(ref knots[j], diff))
					{
						break;
					}
				}

				uniquePositions.Add(knots.Last());
			}
		}

		return uniquePositions.Count;
	}

	public IEnumerable<(char moveDirection, int stepCount)> GetInstructions() => File.ReadLines(AssetPath()).Select(instruction => (instruction[0], int.Parse(instruction[2..])));

	private static bool MoveTailIfNeeded(ref Vector2 tailPosition, Vector2 diff)
	{
		bool MoveInternal(float diffLeadingPart, ref float positionLeadingPart, float diffLesserPart, ref float positionLesserPart)
		{
			if (diffLeadingPart is > 1 or < -1)
			{
				positionLeadingPart += diffLeadingPart - Math.Sign(diffLeadingPart);

				if (diffLesserPart != 0)
				{
					positionLesserPart += Math.Sign(diffLesserPart);
				}

				return true;
			}

			return false;
		}

		return Math.Abs(diff.X) > Math.Abs(diff.Y)
			? MoveInternal(diff.X, ref tailPosition.X, diff.Y, ref tailPosition.Y)
			: MoveInternal(diff.Y, ref tailPosition.Y, diff.X, ref tailPosition.X);
	}

	private static Vector2 CalculatePositionDifference(Vector2 headPosition, Vector2 tailPosition) => headPosition - tailPosition;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector2 ConvertDirectionToMoveVector(char moveDirection)
	{
		return moveDirection switch
		{
			'U' => new Vector2(0, 1),
			'D' => new Vector2(0, -1),
			'R' => new Vector2(1, 0),
			'L' => new Vector2(-1, 0),
			_ => throw new InvalidOperationException("Bonk")
		};
	}
}
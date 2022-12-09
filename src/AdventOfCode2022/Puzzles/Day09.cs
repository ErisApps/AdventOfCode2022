using System.Diagnostics;
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
		PrepareForVisualizePositionsFrames();

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
				if (frameCount >= 30)
				{
					Debugger.Break();
				}

				knots[0] += moveVector;

				for (var j = 1; j < knots.Length; j++)
				{
					var diff = CalculatePositionDifference(knots[j - 1], knots[j]);
					if (!MoveTailIfNeeded(ref knots[j], diff))
					{
						break;
					}
				}

				VisualizePositionsFrame(knots);

				uniquePositions.Add(knots.Last());
			}
		}

		VisualizeUniquePositions(uniquePositions);

		return uniquePositions.Count;
	}

	public IEnumerable<(char moveDirection, int stepCount)> GetInstructions()
	{
		return File.ReadLines(AssetPath()).Select(instruction => (instruction[0], int.Parse(instruction[2..])));
	}

	private static bool MoveTailIfNeeded(ref Vector2 tailPosition, Vector2 diff)
	{
		bool MoveInternal(float diffLeadingPart, ref float positionLeadingPart, float diffLesserPart, ref float positionLesserPart)
		{
			if (diffLeadingPart > 2)
			{
				Console.WriteLine($"diffLeadingPart: {diffLeadingPart} - diffLesserPart: {diffLesserPart}");
			}

			var moved = false;
			if (diffLeadingPart is >1 or <-1)
			{
				positionLeadingPart += diffLeadingPart - Math.Sign(diffLeadingPart);
				moved = true;
			}

			if (moved && diffLesserPart != 0)
			{
				positionLesserPart += diffLesserPart;
			}

			return moved;
		}

		var absDiffX = Math.Abs(diff.X);
		var absDiffY = Math.Abs(diff.Y);

		return absDiffX > absDiffY
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

	private static void VisualizeUniquePositions(HashSet<Vector2> uniquePositions)
	{
		var minX = -100;
		var maxX = 200;
		var minY = -100;
		var maxY = 5;

		var assetPath = Path.Combine(Environment.CurrentDirectory, "Assets", "Day09_output.txt");
		using var fileStream = File.Create(assetPath);
		using var textWriter = new StreamWriter(fileStream);

		for (var y = maxY - 1; y >= minY; y--)
		{
			for (var x = minX; x <= maxX; x++)
			{
				if ((x, y) == (0, 0))
				{
					textWriter.Write("S");
					continue;
				}

				var position = new Vector2(x, y);
				textWriter.Write(uniquePositions.Contains(position)
					? "#"
					: ".");
			}

			textWriter.WriteLine();
		}

		textWriter.WriteLine();
	}

	private int frameCount = 0;

	private static string GetFrameAssetPath() => Path.Combine(Environment.CurrentDirectory, "Assets", "Frames");
	private static void PrepareForVisualizePositionsFrames()
	{
		var path = GetFrameAssetPath();
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}

		Directory.CreateDirectory(path);
	}

	private void VisualizePositionsFrame(Vector2[] positions)
	{
		const int minX = -100;
		const int maxX = 200;
		const int minY = -100;
		const int maxY = 5;

		var assetPath = Path.Combine(GetFrameAssetPath(), $"Day09_output_{++frameCount:D4}.txt");
		using var fileStream = File.Create(assetPath);
		using var textWriter = new StreamWriter(fileStream);

		for (var y = maxY - 1; y >= minY; y--)
		{
			for (var x = minX; x <= maxX; x++)
			{
				if ((x, y) == (0, 0))
				{
					textWriter.Write("S");
					continue;
				}

				var position = new Vector2(x, y);
				var indexOf = Array.IndexOf(positions, position);

				textWriter.Write(indexOf != -1
					? indexOf
					: ".");
			}

			textWriter.WriteLine();
		}

		textWriter.WriteLine();
		textWriter.Flush();
	}
}
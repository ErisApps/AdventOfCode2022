using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day02 : HappyPuzzleBase
{
	private const int ASCII_OFFSET = 65;
	private const int ADDITIONAL_ASCII_OFFSET = 23;

	public override void SolvePart1()
	{
		var totalScore = 0;

		var rawPlays = File.ReadLines(AssetPath());
		foreach (var rawPlay in rawPlays)
		{
			var player1MoveRaw = rawPlay[0] - ASCII_OFFSET;
			var player2MoveRaw = rawPlay[^1] - ASCII_OFFSET - ADDITIONAL_ASCII_OFFSET;

			var outcome = CalculateOutcome((Move) player1MoveRaw, (Move) player2MoveRaw);

			totalScore += player2MoveRaw + 1 + MapOutcome(outcome);
		}

		Console.WriteLine(totalScore);
	}

	public override void SolvePart2()
	{
		var totalScore = 0;

		var rawPlays = File.ReadLines(AssetPath());
		foreach (var rawPlay in rawPlays)
		{
			var player1MoveRaw = rawPlay[0] - ASCII_OFFSET;
			var strategyRaw = rawPlay[^1] - ASCII_OFFSET - ADDITIONAL_ASCII_OFFSET;

			// The strategy offset is calculated by subtracting one and inverting the result
			// -1 means a winning move
			// 0 means a draw
			// 1 means a losing move
			var strategyOffset = -strategyRaw - 1;

			// Calculate our move by offsetting the raw opponent move by 3 (the amount of possible moves)
			// as well as adding the strategy offset, resulting in our own move
			var player2MoveRaw = ((player1MoveRaw + 3 - strategyOffset) % 3);

			var outcome = CalculateOutcome((Move) player1MoveRaw, (Move) player2MoveRaw);

			totalScore += player2MoveRaw + 1 + MapOutcome(outcome);
		}

		Console.WriteLine(totalScore);
	}

	private static Outcome CalculateOutcome(Move player1Move, Move player2Move)
	{
		if (player1Move == player2Move)
		{
			return Outcome.Draw;
		}

		return (player1Move + 3 - player2Move) % 3 == 1 ? Outcome.Player1Won : Outcome.Player2Won;
	}

	private static int MapOutcome(Outcome outcome) => outcome switch
	{
		Outcome.Player1Won => 0,
		Outcome.Draw => 3,
		Outcome.Player2Won => 6,
		_ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null)
	};

	// ReSharper disable thrice UnusedMember.Local
	private enum Move
	{
		Rock,
		Paper,
		Scissors
	}

	private enum Outcome
	{
		Player1Won,
		Player2Won,
		Draw
	}
}
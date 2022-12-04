using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day04 : HappyPuzzleBase
{
	public override object SolvePart1() =>
		File.ReadLines(AssetPath())
			.Select(x => x
				.Split(',')
				.Select(y => y
					.Split('-')
					.Select(int.Parse)
					.ToArray())
				.Select(y => (start: y.First(), end: y.Last()))
				.ToArray())
			.Count(x => (x[0].start >= x[1].start && x[0].end <= x[1].end) ||
			            (x[1].start >= x[0].start && x[1].end <= x[0].end));

	public override object SolvePart2() =>
		File.ReadLines(AssetPath())
			.Select(x => x
				.Split(',')
				.Select(y => y
					.Split('-')
					.Select(int.Parse)
					.ToArray())
				.Select(y => (start: y.First(), end: y.Last()))
				.ToArray())
			.Count(x => (x[0].start <= x[1].end && x[1].start <= x[0].end));
}
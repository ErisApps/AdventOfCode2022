using System.Collections;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day04 : HappyPuzzleBase
{
	public override object SolvePart1() =>
		ParseAssetData()
			.Count(tuples =>
			{
				var (first, second) = tuples;
				return IsFullyContainedIn(first, second);
			});

	public override object SolvePart2() =>
		ParseAssetData()
			.Count(sections =>
			{
				var (first, second) = sections;
				return IsOverlapping(first, second);
			});

	private IEnumerable<((int start, int end) first, (int start, int end) second)> ParseAssetData() =>
		File.ReadLines(AssetPath())
			.SelectMany(line => line
				.Split(',')
				.Select(y => y
					.Split('-')
					.Select(int.Parse)
					.ToArray())
				.Select(sections => (start: sections.First(), end: sections.Last()))
				.Chunk(2))
			.Select(sections => (first: sections.First(), second: sections.Last()));

	private static bool IsContainedIn((int start, int end) a, (int start, int end) b) =>
		a.start >= b.start && a.end <= b.end;

	private static bool IsFullyContainedIn((int start, int end) a, (int start, int end) b) =>
		IsContainedIn(a, b) || IsContainedIn(b, a);

	private static bool IsOverlapping((int start, int end) a, (int start, int end) b) =>
		(a.start <= b.end && b.start <= a.end);
}
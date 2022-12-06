namespace AdventOfCode2022.Shared;

public static class HappyPuzzleHelpers
{
	public static IEnumerable<Type> DiscoverPuzzles(bool onlyLast = false) =>
		typeof(HappyPuzzleBase)
			.Assembly
			.GetTypes()
			.Where(x => x.IsAssignableTo(typeof(HappyPuzzleBase)) && x.IsClass && !x.IsAbstract)
			.OrderBy(x => x.Name)
			.TakeLast(onlyLast ? 1 : int.MaxValue);
}
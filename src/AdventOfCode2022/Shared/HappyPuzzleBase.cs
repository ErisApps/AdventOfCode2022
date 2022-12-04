namespace AdventOfCode2022.Shared;

public abstract class HappyPuzzleBase
{
	protected virtual string AssetName => GetType().Name.ToLowerInvariant() + ".txt";
	protected string AssetPath() => Path.Combine(Environment.CurrentDirectory, "Assets", AssetName);

	public abstract object SolvePart1();

	public abstract object SolvePart2();
}
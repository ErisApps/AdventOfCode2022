// See https://aka.ms/new-console-template for more information

using AdventOfCode2022.Shared;

Console.WriteLine("Hello, World!");

const bool executeAll = true;
var puzzleInstances = typeof(Program)
	.Assembly
	.GetTypes()
	.Where(x => x.IsAssignableTo(typeof(HappyPuzzleBase)) && x.IsClass && !x.IsAbstract)
	.OrderBy(x => x.Name)
	// ReSharper disable once HeuristicUnreachableCode
	.TakeLast(executeAll ? int.MaxValue : 1)
	.Select(x => (HappyPuzzleBase) Activator.CreateInstance(x)!)
	.ToList();

foreach (var puzzleInstance in puzzleInstances)
{
	Console.WriteLine("Executing puzzle instance {0}", puzzleInstance.GetType().Name);

	Console.WriteLine("Solving part 1...");
	SolveAndPrintOutputFor(() => puzzleInstance.SolvePart1());

	Console.WriteLine("\nSolving part 2...");
	SolveAndPrintOutputFor(() => puzzleInstance.SolvePart2());

	Console.WriteLine();
}

static void SolveAndPrintOutputFor(Func<object> func)
{
	try
	{
		var output = func();
		Console.WriteLine(output);
	}
	catch (Exception e)
	{
		Console.Error.WriteLine(e);
	}
}
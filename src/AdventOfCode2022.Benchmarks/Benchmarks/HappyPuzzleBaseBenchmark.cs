using AdventOfCode2022.Shared;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2022.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
public class HappyPuzzleBaseBenchmark<TPuzzle> where TPuzzle : HappyPuzzleBase, new()
{
	private TPuzzle _sut;

	[GlobalSetup]
	public void Setup()
	{
		_sut = new TPuzzle();
	}

	[Benchmark]
	public void SolvePart1() => _sut.SolvePart1();

	[Benchmark]
	public void SolvePart2() => _sut.SolvePart2();
}
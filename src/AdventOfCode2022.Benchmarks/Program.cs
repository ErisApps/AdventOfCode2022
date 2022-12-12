// See https://aka.ms/new-console-template for more information

using AdventOfCode2022.Benchmarks.Benchmarks;
using AdventOfCode2022.Shared;
using BenchmarkDotNet.Running;

var benchmarkCases = HappyPuzzleHelpers
	.DiscoverPuzzles()
	.Select(x => typeof(HappyPuzzleBaseBenchmark<>).MakeGenericType(x))
	.ToArray();

BenchmarkRunner.Run(benchmarkCases);

// BenchmarkRunner.Run(new[] { typeof(SingleCharIntParserBenchmark)/*, typeof(DualCharIntParserBenchmark), typeof(VariableCharIntParserBenchmark)*/ });
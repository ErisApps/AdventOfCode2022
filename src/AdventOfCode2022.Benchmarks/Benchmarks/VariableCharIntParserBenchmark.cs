using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2022.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class VariableCharIntParserBenchmark
{
	[Params("2", "9", "12", "32", "87")]
	public string StringNumber;

	[Benchmark(Baseline = true)]
	public int DotNetIntParse()
	{
		return int.Parse(StringNumber);
	}

	[Benchmark]
	public int GeneralizedCaedenIntParse()
	{
		var readOnlySpan = StringNumber.AsSpan();
		return CaedenIntParser(ref readOnlySpan);
	}

	[Benchmark]
	public int SpecializedCaedenIntParse()
	{
		var readOnlySpan = StringNumber.AsSpan();
		return SpecializedCaedenIntParser(ref readOnlySpan);
	}

	private static int CaedenIntParser(ref ReadOnlySpan<char> span)
	{
		var result = 0;
		var length = span.Length;
		for (var i = 0; i < length; i++)
		{
			var digit = span[i] - '0';
			result = result * 10 + digit;
		}

		return result;
	}

	private static int SpecializedCaedenIntParser(ref ReadOnlySpan<char> span)
	{
		if (span.Length == 2)
		{
			return SingleCharIntParser(span[0]) * 10 + SingleCharIntParser(span[1]);
		}

		return SingleCharIntParser(span[0]);
	}

	private static int SingleCharIntParser(char charDigit)
	{
		return charDigit - '0';
	}
}
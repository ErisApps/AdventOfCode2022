using BenchmarkDotNet.Attributes;

namespace AdventOfCode2022.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class DualCharIntParserBenchmark
{
	[Params("12", "32", "87")]
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
	public int ExplicitDualCharIntParse()
	{
		var readOnlySpan = StringNumber.AsSpan();
		return DualCharIntParser(readOnlySpan[0], readOnlySpan[1]);
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

	private static int DualCharIntParser(char charDigit1, char charDigit2) => SingleCharIntParser(charDigit1) * 10 + SingleCharIntParser(charDigit2);

	private static int SingleCharIntParser(char charDigit) => charDigit - '0';
}
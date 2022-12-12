using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2022.Benchmarks.Benchmarks;

public class SingleCharIntParserBenchmark
{
	[Params("2", "9")]
	public string StringNumber;

	[Benchmark(Baseline = true)]
	public int DotNetIntParse()
	{
		var readOnlySpan = StringNumber.AsSpan();
		return int.Parse(readOnlySpan, NumberStyles.None, NumberFormatInfo.InvariantInfo);
	}

	[Benchmark]
	public int GeneralizedCaedenIntParse()
	{
		var readOnlySpan = StringNumber.AsSpan();
		return CaedenIntParser(ref readOnlySpan);
	}

	[Benchmark]
	public int ExplicitSingleCharIntParse()
	{
		var readOnlySpan = StringNumber.AsSpan();
		return SingleCharIntParser(readOnlySpan[0]);
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

	private static int SingleCharIntParser(char charDigit)
	{
		return charDigit - '0';
	}

	private static int DualCharIntParser(char charDigit1, char charDigit2) => (charDigit1 - '0') * 10 + (charDigit2 - '0');
}
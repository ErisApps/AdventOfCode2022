using System.Buffers;
using System.Text;
using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day10 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		var clockCycle = 1;
		var signalStrengthSum = 0;

		var registerX = 1;

		void IncrementClockCycleAndCheck()
		{
			clockCycle++;

			if ((clockCycle + 20) % 40 == 0)
			{
				var currentSignalStrength = registerX * clockCycle;
				signalStrengthSum += currentSignalStrength;
			}
		}

		foreach (var (command, value) in ParseInput())
		{
			switch (command)
			{
				case Command.NoOp:
					break;
				case Command.AddX:
					IncrementClockCycleAndCheck();
					registerX += value;

					break;
				default:
					throw new NotSupportedException("Bonk!");
			}

			IncrementClockCycleAndCheck();
		}

		return signalStrengthSum;
	}

	public override object SolvePart2()
	{
		const int crtWidth = 40;
		const int crtHeight = 6;

		const int crtSize = crtWidth * crtHeight;

		const int crtWidthWithDelimiters = crtWidth + 1;

		const int crtSizeWithDelimiters = crtWidthWithDelimiters * crtHeight;

		Span<char> crtDrawBuffer = stackalloc char[crtSizeWithDelimiters];

		var clockCycle = 0;
		var registerX = 1;

		void IncrementClockCycleAndAdvanceCrtBuffer(ref Span<char> drawBuffer)
		{
			clockCycle++;

			var spriteDrawIndex = (clockCycle) % crtWidth;
			var shouldLightUpPixel = spriteDrawIndex >= registerX && spriteDrawIndex <= registerX + 2;

			var delimiterOffset = (clockCycle - 1) / crtWidth;
			var drawBufferIndex = clockCycle - 1 + delimiterOffset;
			drawBuffer[drawBufferIndex] = shouldLightUpPixel ? '#' : '.';

			if (clockCycle % crtWidth == 0)
			{
				drawBuffer[drawBufferIndex + 1] = '\n';
			}
		}

		using var inputEnumerable = ParseInput().GetEnumerator();
		while (clockCycle < crtSize)
		{
			var (command, value) = inputEnumerable.Current;

			switch (command)
			{
				case Command.NoOp:
					break;
				case Command.AddX:
					IncrementClockCycleAndAdvanceCrtBuffer(ref crtDrawBuffer);
					registerX += value;

					break;
				default:
					throw new NotSupportedException("Bonk!");
			}

			IncrementClockCycleAndAdvanceCrtBuffer(ref crtDrawBuffer);

			inputEnumerable.MoveNext();
		}

		return crtDrawBuffer.ToString();
	}

	private IEnumerable<(Command command, int value)> ParseInput()
	{
		using var fileStream = File.OpenRead(AssetPath());
		var binaryReader = new BinaryReader(fileStream, Encoding.UTF8, true);

		var byteArrayPool = ArrayPool<byte>.Shared;
		var commandRawBuffer = byteArrayPool.Rent(4);

		var charArrayPool = ArrayPool<char>.Shared;
		var valueRawBuffer = charArrayPool.Rent(3);

		while (fileStream.Position != fileStream.Length)
		{
			// Not really a best practice, but it works for this puzzle
			_ = binaryReader.Read(commandRawBuffer, 0, 4);
			var commandRawBufferSpan = commandRawBuffer.AsSpan(0, 4);
			if (commandRawBufferSpan.SequenceEqual("noop"u8))
			{
				yield return (Command.NoOp, 0);

				fileStream.Position++;
			}
			else if (commandRawBufferSpan.SequenceEqual("addx"u8))
			{
				fileStream.Position++;
				var valueRawBufferIndex = 0;
				int nextChar;
				while ((nextChar = binaryReader.ReadChar()) != '\n')
				{
					valueRawBuffer[valueRawBufferIndex++] = (char) nextChar;
				}

				// Could probably be replaced by the supposedly faster int parser that Caeden used in day 09
				var value = int.Parse(valueRawBuffer.AsSpan()[..valueRawBufferIndex]);
				yield return (Command.AddX, value);
			}
			else
			{
				throw new InvalidOperationException("Bonk!");
			}
		}

		byteArrayPool.Return(commandRawBuffer);
		charArrayPool.Return(valueRawBuffer);
	}

	private enum Command
	{
		NoOp,
		AddX
	}
}
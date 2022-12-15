using AdventOfCode2022.Shared;

namespace AdventOfCode2022.Puzzles;

public class Day07 : HappyPuzzleBase
{
	public override object SolvePart1()
	{
		var root = BuildFileTree();

		long sum = 0;

		void RecursiveFilteredSum(FolderObject folder, long folderSizeThreshold)
		{
			if (folder.Size < folderSizeThreshold)
			{
				sum += folder.Size;
			}

			foreach (var folderInternal in folder.ChildObjects.OfType<FolderObject>())
			{
				RecursiveFilteredSum(folderInternal, folderSizeThreshold);
			}
		}

		RecursiveFilteredSum(root, 100000);

		return sum;
	}

	public override object SolvePart2()
	{
		var root = BuildFileTree();
		const long diskSize = 70000000;
		var freeDiskSpace = diskSize - root.Size;

		const long requiredFreeSpace = 30000000;
		var amountToFreeUp = requiredFreeSpace - freeDiskSpace;

		IEnumerable<FolderObject> RecursiveIterator(FolderObject folder)
		{
			foreach (var folderInternal in folder.ChildObjects.OfType<FolderObject>())
			{
				yield return folderInternal;
				foreach (var folderInternalInternal in RecursiveIterator(folderInternal))
				{
					yield return folderInternalInternal;
				}
			}
		}

		return RecursiveIterator(root).Where(x => x.Size > amountToFreeUp).MinBy(x => x.Size)!.Size;
	}

	private FolderObject BuildFileTree()
	{
		var commandLineOutput = File.ReadAllLines(AssetPath());
		var index = 0;
		FolderObject? root = null;
		FolderObject? current = null;
		do
		{
			var line = commandLineOutput[index];

			if (IsCommand(ref line))
			{
				var commandInput = line[2..];
				var commandParts = commandInput.Split(' ');
				switch (commandParts[0])
				{
					case "cd":
						root = HandleCdCommand(commandParts, ref current, ref root, ref index);
						break;
					case "ls":
						index = HandleLsCommand(commandLineOutput, current, ref index);

						break;
				}
			}
		} while (index < commandLineOutput.Length);

		return root!;
	}

	private static FolderObject? HandleCdCommand(string[] commandParts, ref FolderObject? current, ref FolderObject? root, ref int index)
	{
		switch (commandParts[1])
		{
			case "/":
				current = root ??= new FolderObject { Name = commandParts[1] };
				break;
			case "..":
				current = current!.Parent;
				break;
			default:
				var localCurrent = current!.ChildObjects.OfType<FolderObject>().FirstOrDefault(x => x.Name == commandParts[1]);
				if (localCurrent == null)
				{
					localCurrent = new FolderObject { Name = commandParts[1], Parent = current };
					current.ChildObjects.Add(localCurrent);
				}

				current = localCurrent;
				break;
		}

		index++;
		return root;
	}

	private static int HandleLsCommand(string[] commandLineOutput, FolderObject? current, ref int index)
	{
		current!.ChildObjects.Clear();
		index++;

		do
		{
			var line = commandLineOutput[index];
			if (IsCommand(ref line))
			{
				break;
			}

			var fileSystemObjectParts = line.Split(' ');
			current.ChildObjects.Add(fileSystemObjectParts[0] == "dir"
				? new FolderObject { Name = fileSystemObjectParts[1], Parent = current }
				: new FileObject
				{
					Size = int.Parse(fileSystemObjectParts[0])
				});
			index++;
		} while (index < commandLineOutput.Length);

		return index;
	}

	private static bool IsCommand(ref string line) => line[0] == '$';

	private class FileObject : IFileSystemObject
	{
		public long Size { get; init; }
	}

	private class FolderObject : IFileSystemObject
	{
		public required string Name { get; init; }
		public long Size => ChildObjects.Sum(x => x.Size);
		public FolderObject? Parent { get; init; }
		public List<IFileSystemObject> ChildObjects { get; } = new();
	}

	private interface IFileSystemObject
	{
		long Size { get; }
	}
}
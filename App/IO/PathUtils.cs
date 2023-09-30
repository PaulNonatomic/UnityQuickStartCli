namespace UnityQuickStart.App.IO;

public class PathUtils
{
	public static string CommaSeperatedDirectoryList(string path)
	{
		try
		{
			var directories = Directory.GetDirectories(path);
			return string.Join(", ", directories.Select(Path.GetFileName));
		}
		catch (Exception e)
		{
			return string.Empty;
		}
	}

	public static bool PathContainsDirectory(string path, string directoryName)
	{
		var fullPath = Path.Combine(path, directoryName);
		return Directory.Exists(fullPath);
	}
}
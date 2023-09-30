namespace UnityQuickStart.App.IO;

public class Spinner
{
	public static void Spin(CancellationToken token, string message = "Processing")
	{
		var spinner = new char[] { '|', '/', '-', '\\' };
		var i = 0;
		
		while (!token.IsCancellationRequested)
		{
			Console.Write($"\r{message} {spinner[i++ % 4]}");
			Thread.Sleep(200);
		}
		
		Console.Write("\r                  \r");
	}
}
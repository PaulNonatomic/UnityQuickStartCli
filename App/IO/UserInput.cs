namespace UnityQuickStart.App.IO
{
	/// <summary>
	/// Provides methods for obtaining user input from the console.
	/// </summary>
	public static class UserInput
	{
		/// <summary>
		/// Gets a string input from the user.
		/// </summary>
		/// <param name="prompt">The prompt to display to the user.</param>
		/// <param name="required">Indicates whether the input is required.</param>
		/// <param name="additionalInfo">Provides accompanying information for the prompt</param>
		/// <returns>The user's input as a string.</returns>
		public static string GetString(string prompt, bool required = true, string additionalInfo = null, ConsoleColor infoColor = ConsoleColor.Cyan)
		{
			string input;
			do
			{
				DisplayPrompt(prompt, additionalInfo, infoColor);
				
				input = Console.ReadLine()?.Trim();

				if (!required && string.IsNullOrEmpty(input))
				{
					return null;
				}

				if (required && string.IsNullOrEmpty(input))
				{
					Output.WriteLine("Input is required. Please try again.");
				}
			} while (string.IsNullOrEmpty(input));

			return input;
		}

		/// <summary>
		/// Gets an integer input from the user.
		/// </summary>
		/// <param name="prompt">The prompt to display to the user.</param>
		/// <param name="min">The minimum acceptable value.</param>
		/// <param name="max">The maximum acceptable value.</param>
		/// <returns>The user's input as an integer.</returns>
		public static int GetInt(string prompt, int? min = null, int? max = null, string additionalInfo = null, ConsoleColor infoColor = ConsoleColor.Cyan)
		{
			int value;
			do
			{
				DisplayPrompt(prompt, additionalInfo, infoColor);
				
				var input = GetString(prompt, required: true);

				if (int.TryParse(input, out value))
				{
					if ((min == null || value >= min) && (max == null || value <= max))
					{
						return value;
					}
					else
					{
						Output.WriteLine($"Please enter a number between {min} and {max}.");
					}
				}
				else
				{
					Output.WriteLine("Invalid input. Please enter a valid integer.");
				}
			} while (true);
		}

		/// <summary>
		/// Gets a yes/no confirmation from the user.
		/// </summary>
		/// <param name="prompt">The prompt to display to the user.</param>
		/// <returns>True if the user confirms, otherwise false.</returns>
		public static bool GetYesNo(string prompt, string additionalInfo = null, ConsoleColor infoColor = ConsoleColor.Cyan)
		{
			string input;
			do
			{
				DisplayPrompt($"{prompt} (y/n): ", additionalInfo, infoColor);
				
				input = Console.ReadLine()?.Trim().ToLower();

				switch (input)
				{
					case "y":
					case "yes":
						return true;
					case "n":
					case "no":
						return false;
					default:
						Output.WriteLine("Invalid input. Please enter 'y' for yes or 'n' for no.");
						break;
				}
				
			} while (true);
		}
		
		/// <summary>
		/// Displays additional information to accompany the prompt.
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="additionalInfo"></param>
		private static void DisplayPrompt(string prompt, string additionalInfo, ConsoleColor infoColor)
		{
			if (!string.IsNullOrEmpty(additionalInfo))
			{
				Output.WriteLine($"{additionalInfo}", infoColor);
			}
			
			Output.Write($">> {prompt}");
		}
	}
}

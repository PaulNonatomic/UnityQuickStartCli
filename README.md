# UnityQuickStartCli

## Description

UnityQuickStartCli is a command-line interface (CLI) tool designed to automate the setup and creation of Unity projects, as well as local and remote Git repositories. It streamlines the process of initializing a new Unity project, setting up Git repositories, and configuring project settings.

## Features

- **Unity Project Creation**: Automatically sets up a new Unity project.
- **Git Integration**: Initializes local and remote Git repositories.
- **Customizable Settings**: Allows you to set Unity installation paths and versions.
- **User-Friendly Interface**: Provides a clean and straightforward CLI.

## Dependencies

To run this project, you'll need the following software installed:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [GitHub CLI](https://cli.github.com/)
- [Unity](https://unity.com/)

### Installation Steps

1. **.NET SDK**: Download and install from [here](https://dotnet.microsoft.com/download).
2. **GitHub CLI**: Follow the installation guide [here](https://cli.github.com/).
3. **Unity**: Download and install Unity Hub from [here](https://unity.com/download).

## Installation

### Global Installation

1. Download the latest NuGet package from the [releases page](https://github.com/octo-org/octo-repo/releases/latest).
2. To install the tool globally replacing <path> with the path to the directory containing the NuGet package.

<b>Note do not set the path directly to the NuGet package!</b>

```bash
dotnet tool install --global --add-source <path> UnityQuickStart
```

### Uninstallation

To uninstall the tool:

```bash
dotnet tool uninstall --global UnityQuickStart
```

### Build from Rider

1. Open the project in Rider.
2. Run `dotnet pack` to package the application.
3. Follow the global installation steps above to install the tool.

## Usage

Run the CLI tool with the following command:

```bash
unityquick [options]
```

### Options

- `-h, --help`: Show the help page.
- `-c, --clear`: Resets the settings to default.
- `-p, --path`: Set the Unity installation path.

### Examples

```bash
unityquick -p "C:\Program Files\Unity\Hub\Editor"
```

## Code Structure

- [`Git.cs`](https://github.com/PaulNonatomic/UnityQuickStartCli/blob/develop/App/Git/Git.cs): Handles Git-related functionalities.
- [`UnityCli.cs`](https://github.com/PaulNonatomic/UnityQuickStartCli/blob/develop/App/Unity/UnityCli.cs): Manages Unity project creation and settings.
- [`QuickStartProject.cs`](https://github.com/PaulNonatomic/UnityQuickStartCli/blob/develop/App/Project/QuickStartProject.cs): Represents the Unity project.
- [`UserSettings.cs`](https://github.com/PaulNonatomic/UnityQuickStartCli/blob/develop/App/Settings/UserSettings.cs): Manages user settings like Unity paths and versions.

## Contributing

// Contribution guidelines to come

## License

This project is licensed under the MIT License. 

Copyright (c) 2023 Noantomic Ltd

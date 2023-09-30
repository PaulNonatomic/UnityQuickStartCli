# UnityQuickStartCli

## Description

UnityQuickStartCli is a command-line interface (CLI) tool designed to automate the setup and creation of Unity projects, as well as local and remote Git repositories. It streamlines the process of initializing a new Unity project, setting up Git repositories, and configuring project settings.

## Features

- **Unity Project Creation**: Automatically sets up a new Unity project.
- **Git Integration**: Initializes local and remote Git repositories.
- **Customizable Settings**: Allows you to set Unity installation paths and versions.
- **User-Friendly Interface**: Provides a clean and straightforward CLI.

## Installation

### Global Installation

To install the tool globally, navigate to the project directory and run:

```bash
dotnet tool install UnityQuickStart --global --add-source ./dist
```

### Uninstallation

To uninstall the tool:

```bash
dotnet tool uninstall UnityQuickStart --global
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
- `-sup, --set-unity-path`: Set the Unity installation path.
- `-cs, --clear-settings`: Resets the settings to default.

### Examples

```bash
unityquick --set-unity-path "C:\\Program Files\\Unity\\Hub\\Editor"
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

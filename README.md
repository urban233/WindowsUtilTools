# WindowsUtilTools

WindowsUtilTools is a collection of powerful and convenient utilities designed to simplify and enhance various Microsoft Windows operations. This toolkit provides command-line tools and utilities for managing Windows Subsystem for Linux (WSL), PowerShell, and other Windows features with ease and elevated privileges.

### Supported Platforms

![Windows](https://img.shields.io/badge/Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)

## Features

- **WSL2 Management:** Check if WSL2 is installed and install it programmatically.
- **PowerShell Utilities:** Run PowerShell commands with elevated privileges.
- **Command Line Parsing:** Robust command-line argument parsing with support for short and long options.
- **Administrator Privilege Handling:** Utilities to restart processes with elevated (administrator) rights.

## C# Projects

- **WindowsCli:** A command-line interface application that exposes Windows utilities such as WSL2 installation and status checking.
- **Utils:** A utility library containing core functionalities like command execution, privilege elevation, and command-line parsing.

## Getting Started

### Prerequisites

- Windows 10 or later
- .NET 9.0 SDK

### Building the Solution

```powershell
cd WindowsUtilTools
dotnet build
```

### Usage

Run the CLI tool with the following options:

```powershell
WindowsCli.exe --install-wsl2       # Installs WSL2
WindowsCli.exe --check-wsl2-install # Checks if WSL2 is installed
```

## Example

```powershell
# Check if WSL2 is installed
WindowsCli.exe --check-wsl2-install

# Install WSL2
WindowsCli.exe --install-wsl2
```

## License

This project is licensed under the BSD 3-Clause License - see the [LICENSE](LICENSE) file for details.

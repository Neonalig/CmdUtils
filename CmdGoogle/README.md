# CmdGoogle
Allows users to query the google search engine in their default browser from the command prompt.

## Usage
`> google rick astley`
The above example will open a google search with the specified `rick astley` query into your default system browser.

## Installation
Get the [latest release](../../../releases/tag/CmdGoogle/latest) from the [Releases](../../../releases) tab, and extract the `Google.exe` executable to a known location.
This location must then be [added to the PATH environment variable](adding-the-program-to-path).

## Manual Installation
1. Clone this repository using your preferred method (`git clone`, [GitHub Desktop](https://desktop.github.com/), [GitKraken](https://gitkraken.com/), and so on...)
2. Open the `CmdGoogle.sln` file with Visual Studio 2022 (other versions should work, but the project was made with this version.), ensuring the '.NET desktop development' module is already installed.
3. Build the project using the `Build/Publish Selection` dialog, using either the `FolderProfile` or `FolderProfile1` preset.
4. Rename the resultant `CmdGoogle.exe` executable to `Google.exe`
5. Copy the executable to a known location. This location must then be [added to the PATH environment variable](adding-the-program-to-path).

## Adding the program to PATH
This step only applies for Windows systems.
There are two main methods to add a program to your PATH. Method #1 involves copying the executable to the System32 directory, or some other directory already assigned to the PATH variable. Method #2 involves choosing your own location and adding it to the PATH variable.
### Method #1
- Copy the `Google.exe` executable to `%WINDIR%\system32\Google.exe` (becomes C:\Windows\system32\Google.exe on a default installation of Windows.)
### Method #2
1. Copy the `Google.exe` executable to a location of your choice (for this example, we will use `E:\Programs\_Terminal\Google.exe`)
2. Open the 'Edit Environment Variables' dialog.
	a. Method #1: Search for 'Edit environment variables for your account'.
	b. Method #2: Open 'System Properties' (either via search or `sysdm.cpl` in the `[⊞ Win]`+`[R]` run dialog), click on the 'Advanced' tab, and click on the 'Environment Variables' button.
	c. Method #3: Execute `rundll32.exe sysdm.cpl,EditEnvironmentVariables` in the `[⊞ Win]`+`[R]` run dialog.
3. In the `User variables for (username)` section, select the `PATH` variable and click the `Edit...` button.
4. Click the `New` button and paste the folder containing the executable (i.e. `E:\Programs\_Terminal\Google.exe`)
5. Press the carriage return (`[Enter]`) key to finalise the new variable, then press `OK` and `OK` again.
6. Try and run a test command from the command prompt. If the executable is not found, you may have to restart the command prompt first, and if that doesn't work, try either signing out and in (restarting `explorer.exe`), or restarting the computer then try the command again.

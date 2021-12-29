using System.Diagnostics;
using System.Reflection;

using Newtonsoft.Json;

#region Functions

//Attempts to convert the path into a valid DirectoryInfo instance, returning true if successful. Will not check if the path actually exists.
bool GetDirectory( string Path, out DirectoryInfo Dir ) {
    try {
        if ( !string.IsNullOrEmpty(Path) ) {
            Dir = new DirectoryInfo(Path);
            return true;
        }
    } catch { }
    Dir = null!;
    return false;
}

//Attempts to retrieve the first item in the collection, returning true if successful.
//bool GetFirst<T>( IList<T> Ls, out T First ) {
//    if (Ls.Count > 0 ) {
//        First = Ls[0];
//        return true;
//    }
//    First = default!;
//    return false;
//}

//Generates a random number in the given range, excluding a specific value.
int GetRandomNotIncluding( Random Rnd, int Min, int Max, int Exc ) => Rnd.Next(0, 2) == 0 ? Rnd.Next(Min, Exc) : Rnd.Next(Exc + 1, Max);

//Gets a random directory form the collection, allowing a 'Last' value to be passed to ensure the user is never offered the same directory twice in a row.
DirectoryInfo GetRandom( DirectoryInfo[] Possible, int Cnt, int? Last, Random Rnd ) {
    int ChosenInd = Last.HasValue ? GetRandomNotIncluding(Rnd, 0, Cnt, Last.Value) : Rnd.Next(0, Cnt);
    DirectoryInfo Chosen = Possible[Rnd.Next(0, Cnt)];
    while ( true ) {
        Console.Write($"Play from '{Chosen.Name}'? [Y]es/[N]o: ");
        ConsoleKey Input = Console.ReadKey().Key;
        Console.Write('\n');
        switch ( Input ) {
            case ConsoleKey.Y:
                return Chosen;
            case ConsoleKey.N:
                return GetRandom(Possible, Cnt, ChosenInd, Rnd);
        }
    }
}

//Deserialises the json data from the given file.
T? Read<T>( FileInfo File, JsonSerializer Serialiser ) {
    using ( FileStream FS = File.OpenRead() ) {
        using ( StreamReader SR = new StreamReader(FS) ) {
            using ( JsonTextReader JTR = new JsonTextReader(SR) ) {
                return Serialiser.Deserialize<T>(JTR);
            }
        }
    }
}

//Serialises the given data object into json data, written into the destination file (which will be cleared first if it exists, or created if it does not).
void Write<T>( FileInfo Dest, T Data, JsonSerializer Serialiser ) {
    using ( FileStream FS = Dest.Exists ? Dest.Open(FileMode.Truncate, FileAccess.Write) : Dest.Create() ) {
        using ( StreamWriter SW = new StreamWriter(FS) ) {
            Serialiser.Serialize(SW, Data);
        }
    }
}

#endregion

//Get the directory from either the user's supplied arguments, or if that is empty or invalid, the current working directory instead.
DirectoryInfo Base = /*GetFirst(args, out string Arg)
                     && GetDirectory(Arg, out DirectoryInfo D)*/
                    GetDirectory(string.Join(' ', args), out DirectoryInfo D)
                        ? D
                        : new DirectoryInfo(Environment.CurrentDirectory);

//Find all album directories and enumerate
DirectoryInfo[] Options = Base.GetDirectories();
int Count = Options.Length;

if (Count == 0 ) {
    Console.WriteLine("No child directories could be found. Ensure that a directory is given as an argument, or that the application is ran from a main folder containing multiple album directories.");
    Environment.Exit(0);
    return;
}

//Get the user to choose an album directory at random. The method ensures the user is never supplied the same directory twice. In the case there is only one directory, we will always use that regardless.
DirectoryInfo UserChosen = Count == 0 ? Options[0] : GetRandom(Options, Count, null, new Random());
Console.WriteLine($"Will play from '{UserChosen.Name}'.");

//Locate the 'settings.json' file and prepare the serialiser.
JsonSerializer Ser = new JsonSerializer { Formatting = Formatting.Indented };
FileInfo LocalFile = new FileInfo(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName!, "settings.json"));

//In the case the 'settings.json' file does not exist, is malformed json data, or does not supply an executable path, we create a default example file and open it in the user's default json text editor.
if ( !LocalFile.Exists || Read<KnownData>(LocalFile, Ser) is not var (Executable, Args) || string.IsNullOrEmpty(Executable) ) {
    Write(LocalFile, new KnownData(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Windows Media Player\\wmplayer.exe").Replace('\\', '/'), "\"$(folder)\""), Ser);
    //_ = Process.Start("notepad.exe", $"\"{LocalFile.FullName}\"");
    _ = Process.Start($"\"{LocalFile.FullName}\"");
    Environment.Exit(0);
    return;
}

//If the 'settings.json' file is valid, we start the supplied executable and arguments, replacing $(folder) with the chosen folder name.
_ = Process.Start(Executable.Replace('/', '\\').Trim(' '), Args.Replace("$(folder)", UserChosen.FullName));
Environment.Exit(0);
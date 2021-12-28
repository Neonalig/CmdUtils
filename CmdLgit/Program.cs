using System.Diagnostics;

/// <summary/>
internal class Program {

    /// <summary/>
    internal static readonly List<string> ArgsToPass = new List<string>();

    /// <summary/>
    internal static void Main( string[] Args ) {
        bool HasLogCommand = false;
        foreach ( string Arg in Args ) {
            if ( Arg.Equals("log", StringComparison.OrdinalIgnoreCase) ) { HasLogCommand = true; }
            ArgsToPass.Add(Arg);
        }

        if ( HasLogCommand ) {
            RunDecoratedLog();
            Quit();
            return;
        }

        if ( ArgsToPass.Count > 0 ) { //Run command verbose, verbatim
            RunVerboseCommand();
            Quit();
            return;
        }

        //No params were supplied
        NoParamsSpecified();
        Quit();
    }

    /// <summary/>
    internal static void Quit() {
#if DEBUG
        WriteLine("Code execution has halted. The application may now be closed.");
        _ = Console.ReadKey();
#endif
        Environment.Exit(0);
    }

    /// <summary/>
    internal static bool TryRemove( ref string A, string AttemptedRemove ) {
        if ( A.Contains(AttemptedRemove) ) {
            A = A.Replace(AttemptedRemove, null);
            return true;
        }
        return false;
    }

    /// <summary/>
    internal static string RemoveArg( string Args, string ToRemove ) {
        string Return = Args;
        return TryRemove(ref Return, $" {ToRemove} ")
               || TryRemove(ref Return, $" {ToRemove}")
               || TryRemove(ref Return, $"{ToRemove} ")
            ? Return
            : Args;
    }

    //Using explicit methods instead of params to avoid object array allocation
    /// <summary/>
    internal static string RemoveArgsTwo( string Args, string A, string B ) => RemoveArg(RemoveArg(Args, A), B);

    /// <summary/>
    internal static string RemoveArgsSix( string Args, string A, string B, string C, string D, string E, string F ) => RemoveArg(RemoveArg(RemoveArg(RemoveArg(RemoveArg(RemoveArg(Args, A), B), C), D), E), F);

    /// <summary/>
    internal static void RunGitProcessSync( string Command, string UserArgs, IList<string> ImpliedArgs ) {
        foreach ( string ImpliedArg in ImpliedArgs ) {
            UserArgs = UserArgs.Replace($" {ImpliedArg}", string.Empty).Replace(ImpliedArg, string.Empty);
        }
        if ( !string.IsNullOrEmpty(Command) ) {
            UserArgs = UserArgs.Replace($" {Command}", string.Empty).Replace($"{Command}", string.Empty);
        }

        string Args = $"{Command} {UserArgs} {string.Join(' ', ImpliedArgs)}".Trim(' ');

        Process P = new Process {
            StartInfo = new ProcessStartInfo("git", Args) {
                RedirectStandardError = true//,
                //RedirectStandardOutput = true
            }
        };
        P.ErrorDataReceived += ( _, E ) => WriteLine($"ERR: {E.Data}");
        //P.OutputDataReceived += ( _, E ) => WriteLine($"OUT: {E.Data}");
        _ = P.Start();
        //WriteLine("Git execution has concluded.");
        //string Out = P.StandardOutput.ReadToEnd();
        string Err = P.StandardError.ReadToEnd();

        bool CheckForErrorAndRetry( string ErrorPrefix ) { //Returns true if an error is found
            if ( !string.IsNullOrEmpty(Err) && Err.StartsWith(ErrorPrefix) ) {
                string Arg = Err[ErrorPrefix.Length..];
                Arg = Arg[..Arg.IndexOf('\n')].Trim('"').Trim('\'').Trim('`');

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach ( string Implied in ImpliedArgs ) {
                    if ( Implied == Arg || Implied.Contains(Arg) ) { //<-- check if contains in case git returns 'argument' instead of '--argument' or '-a' (git error message formatting is inconsistent)
                        WriteLine((null, null, "Command does not support the implied '"), (ConsoleColor.Yellow, null, Implied), (null, null, "' argument; "), (ConsoleColor.Gray, null, "retrying..."));
                        //WriteLine($"Command does not support implied '{Arg}' argument; retrying...");

                        _ = ImpliedArgs.Remove(Implied);
                        RunGitProcessSync(Command, UserArgs, ImpliedArgs);
                        return true;
                    }
                }
                return true;
            }
            return false;
        }

        if ( CheckForErrorAndRetry("unknown option: ") ) { return; }
        if ( CheckForErrorAndRetry("error: unknown option ") ) { return; }
        if ( CheckForErrorAndRetry(@"fatal: unrecognized argument: ") ) { return; }
        P.WaitForExit();
    }

    /// <summary/>
    internal static void RunGitProcessSync( string Command, params string[] ImpliedArgsArr ) => RunGitProcessSync(Command, string.Join(' ', ArgsToPass).Trim(' '), ImpliedArgs: new List<string>(ImpliedArgsArr));

    /// <summary/>
    internal static void RunDecoratedLog() => RunGitProcessSync("log", "--oneline", "--graph", @"--color", "--all", "--decorate");

    /// <summary/>
    internal static void RunVerboseCommand() => RunGitProcessSync(string.Empty, "--verbose", "--dry-run");

    /// <summary/>
    internal static void NoParamsSpecified() {
        WriteLine((null, null, "No parameters were specified; "), (ConsoleColor.Gray, null, "Displaying help..."));
        Process.Start("git", "--help").WaitForExit();
    }

    /// <summary/>
    internal static void WriteLine( params (ConsoleColor? FG, ConsoleColor? BG, string Text)[] Segments ) {
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.Write("[LGit] ");

        foreach ( (ConsoleColor? FG, ConsoleColor? BG, string Text) in Segments ) {
            Console.ResetColor();
            if ( FG is { } Foreground ) { Console.ForegroundColor = Foreground; }
            if ( BG is { } Background ) { Console.BackgroundColor = Background; }
            Console.Write(Text);
        }
        Console.ResetColor();
        Console.Write("\r\n");
    }

    /// <summary/>
    internal static void WriteLine( string Message, ConsoleColor? FG = null, ConsoleColor? BG = null ) {
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.Write("[LGit] ");

        Console.ResetColor();
        if ( FG is { } Foreground ) { Console.ForegroundColor = Foreground; }
        if ( BG is { } Background ) { Console.BackgroundColor = Background; }
        Console.Write($"{Message}\r\n");
        if ( FG is not null || BG is not null ) {
            Console.ResetColor();
        }
    }
}
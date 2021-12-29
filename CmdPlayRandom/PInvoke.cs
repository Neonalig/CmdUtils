using System.Runtime.InteropServices;

namespace CmdPlayRandom;

/// <summary />
internal static class PInvoke {
    /// <summary />
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool SetConsoleOutputCP( uint WCodePageID );

    /// <summary />
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool SetConsoleCP( uint WCodePageID );
}

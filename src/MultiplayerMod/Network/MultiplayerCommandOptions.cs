using System;

namespace MultiplayerMod.Network;

[Flags]
public enum MultiplayerCommandOptions {

    /// <summary>
    /// Default command behavior.
    /// </summary>
    None = 0,

    /// <summary>
    /// A command will not be sent to host client.
    /// </summary>
    SkipHost = 1

}

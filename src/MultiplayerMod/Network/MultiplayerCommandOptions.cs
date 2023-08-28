using System;

namespace MultiplayerMod.Network;

[Flags]
public enum MultiplayerCommandOptions {

    /// <summary>
    /// Default command behavior.
    /// </summary>
    None = 0,

    /// <summary>
    /// A command will be executed on the server and will not be forwarded to other clients.
    /// </summary>
    ExecuteOnServer = 1,

    /// <summary>
    /// A command will not be sent to host client.
    /// </summary>
    SkipHost = 2

}

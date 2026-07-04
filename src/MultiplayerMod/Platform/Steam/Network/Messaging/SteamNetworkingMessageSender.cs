using System;
using System.Reflection;
using System.Runtime.InteropServices;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Network;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

/// <summary>
/// Sends already-serialized network messages on a specific Steam priority lane.
///
/// Steam's flat <c>SendMessageToConnection</c> cannot set a lane (it always uses lane 0), so lane-aware
/// sends must go through <c>ISteamNetworkingSockets::SendMessages</c>, which takes an array of messages
/// allocated by <c>AllocateMessage</c>. Steamworks.NET's managed <c>SendMessages</c> overload marshals a
/// contiguous struct array where the native ABI expects an array of pointers, so it is unusable here; we
/// P/Invoke the flat entry point directly with an <see cref="IntPtr"/>[] of allocated messages instead.
///
/// The interface instance pointer lives behind an internal Steamworks.NET accessor, resolved once via
/// reflection. If it can't be resolved the sender reports <see cref="Available"/> = false and the
/// transport falls back to plain <c>SendMessageToConnection</c> (which still honours the per-lane
/// reliability flag — only the cross-lane prioritisation is lost).
/// </summary>
public class SteamNetworkingMessageSender {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamNetworkingMessageSender>();

    private readonly IntPtr socketsInterface;
    private bool warnedSendFailure;

    public bool Available => socketsInterface != IntPtr.Zero;

    private SteamNetworkingMessageSender(IntPtr socketsInterface) {
        this.socketsInterface = socketsInterface;
    }

    public static SteamNetworkingMessageSender ForClient() =>
        new(ResolveInterface("Steamworks.CSteamAPIContext"));

    public static SteamNetworkingMessageSender ForServer() =>
        new(ResolveInterface("Steamworks.CSteamGameServerAPIContext"));

    /// <summary>
    /// Send one message on the given lane with the given Steam send flags. Allocates a Steam-owned
    /// message buffer, copies the serialized payload in, tags it with the lane/flags, and hands it to
    /// <c>SendMessages</c> (which takes ownership and frees the buffer once transmitted).
    ///
    /// Returns <c>true</c> only when Steam definitively accepted the message for delivery. On any failure
    /// (allocation failed, or <c>SendMessages</c> reported a negative result) it returns <c>false</c> and
    /// nothing was queued - Steam frees a rejected message itself - so the caller can safely fall back to
    /// the flat <c>SendMessageToConnection</c> without risking a duplicate send.
    /// </summary>
    public unsafe bool Send(INetworkMessageHandle handle, HSteamNetConnection connection, int sendFlags, NetworkLane lane) {
        var size = (int) handle.Size;
        var pMessage = SteamNetworkingUtils.AllocateMessage(size);
        if (pMessage == IntPtr.Zero) {
            WarnOnce("AllocateMessage returned null");
            return false;
        }

        var message = SteamNetworkingMessage_t.FromIntPtr(pMessage);
        // AllocateMessage(size) already allocated m_pData of `size` bytes and set m_pfnFreeData; copy the
        // serialized payload into that Steam-owned buffer.
        Buffer.MemoryCopy(handle.Pointer.ToPointer(), message.m_pData.ToPointer(), size, size);
        message.m_cbSize = size;
        message.m_conn = connection;
        message.m_nFlags = sendFlags;
        message.m_idxLane = (ushort) lane;
        Marshal.StructureToPtr(message, pMessage, false);

        // SendMessages writes, per message, the assigned message number (> 0) on success or the negative
        // EResult on failure. Checking it turns a silent native drop (which manifests as lost gameplay
        // commands -> teleporting duplicants + desync) into a graceful fall back to the flat API.
        var results = new long[1];
        SendMessages(socketsInterface, 1, new[] { pMessage }, results);
        if (results[0] > 0)
            return true;

        WarnOnce($"SendMessages reported {results[0]}");
        return false;
    }

    // Failure here means every lane send is falling back to the flat API (gameplay still flows, only
    // cross-lane prioritisation is lost). Log it once so it is visible without spamming every send.
    private void WarnOnce(string reason) {
        if (warnedSendFailure)
            return;
        warnedSendFailure = true;
        log.Warning($"Priority-lane send failed ({reason}); falling back to flat SendMessageToConnection for all sends");
    }

    private static IntPtr ResolveInterface(string contextTypeName) {
        try {
            var type = typeof(HSteamNetConnection).Assembly.GetType(contextTypeName);
            var method = type?.GetMethod(
                "GetSteamNetworkingSockets",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
            );
            if (method == null) {
                log.Warning($"Could not resolve {contextTypeName}.GetSteamNetworkingSockets; priority lanes disabled");
                return IntPtr.Zero;
            }
            var pointer = (IntPtr) method.Invoke(null, null);
            if (pointer == IntPtr.Zero)
                log.Warning("Steam networking sockets interface is null; priority lanes disabled");
            return pointer;
        } catch (Exception e) {
            log.Warning($"Failed to resolve Steam networking sockets interface ({e.Message}); priority lanes disabled");
            return IntPtr.Zero;
        }
    }

    // Flat entry point for ISteamNetworkingSockets::SendMessages, bypassing the broken managed overload.
    // pMessages is an array of messages previously allocated by AllocateMessage; the pointer array maps
    // to the native `SteamNetworkingMessage_t *const *`.
    [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamNetworkingSockets_SendMessages", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SendMessages(
        IntPtr self,
        int messageCount,
        IntPtr[] pMessages,
        long[] outMessageNumberOrResult
    );

}

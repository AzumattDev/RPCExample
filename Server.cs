using HarmonyLib;

namespace RPCExample
{
    [HarmonyPatch]
    public class Server
    {
        /// <summary>
        ///     The code that you wish to run on the server. Sometimes only the server can get information
        ///     like getting things from the Admin List or server files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pkg"></param>
        public static void RPC_RequestWhatever(long sender, ZPackage pkg)
        {
            if (pkg != null && pkg.Size() > 0)
            {
                // Check that our Package is not null, and if it isn't check that it isn't empty.
                var
                    peer = ZNet.instance
                        .GetPeer(sender); // Get the Peer from the sender, to later check the SteamID against our Adminlist.
                if (peer != null)
                {
                    // Confirm the peer exists
                    var peerSteamID = peer.m_rpc.GetSocket().GetHostName(); // Get the SteamID from peer.
                    var consumedItem = pkg.ReadString(); // Read the user's consumed item.
                    RPCExample.RpcExampleLogger.LogMessage(
                        $"Peer: {peerSteamID} has consumed {Localization.instance.Localize(consumedItem)}");
                    ZPackage newPkg = new();
                    newPkg.Write($"Peer: {peerSteamID} has consumed {Localization.instance.Localize(consumedItem)}");
                    ZRoutedRpc.instance.InvokeRoutedRPC(sender, "LogToChat",
                        newPkg); // send information to client so they can see it in chat.
                }
                else
                {
                    ZPackage newPkg = new(); // Create a new ZPackage.
                    newPkg.Write("Peer was null, Server Patch"); // Tell them what's going on.
                    ZRoutedRpc.instance.InvokeRoutedRPC(sender, "BadRequestMsg", newPkg); // Send the error message.
                }
            }
        }

        /// <summary>
        ///     The event on the server should be blank. This is a "mock" of the client function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pkg"></param>
        public static void RPC_EventWhatever(long sender, ZPackage pkg)
        {
        }
    }
}
using HarmonyLib;

namespace RPCExample
{
    [HarmonyPatch]
    public class Client
    {
        /// <summary>
        ///     The code you wish to have executed on the client AFTER the server responds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pkg"></param>
        public static void RPC_EventWhatever(long sender, ZPackage pkg)
        {
            if (sender == ZRoutedRpc.instance.GetServerPeerID() && pkg != null && pkg.Size() > 0)
            {
                // Confirm our Server is sending the RPC

                string consumedItem = pkg.ReadString();

                if (consumedItem != "") // Make sure it isn't empty
                    Chat.instance.AddString("Server", consumedItem,
                        Talker.Type.Normal); // Add our server announcement to the Client's chat instance
            }
        }

        /// <summary>
        ///     The request on the client should be blank. This is a "mock" of the server function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pkg"></param>
        public static void RPC_RequestWhatever(long sender, ZPackage pkg)
        {
        }


        /// <summary>
        ///     Send chat message letting the user know something went wrong
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pkg"></param>
        public static void RPC_BadRequestMsg(long sender, ZPackage pkg)
        {
            if (sender == ZRoutedRpc.instance.GetServerPeerID() && pkg != null && pkg.Size() > 0)
            {
                // Confirm our Server is sending the RPC
                string msg = pkg.ReadString(); // Get Our Msg
                if (msg != "") // Make sure it isn't empty
                    Chat.instance.AddString("Server", "<color=\"red\">" + msg + "</color>",
                        Talker.Type.Normal); // Add to chat with red color because it's an error
            }
        }

        /// <summary>
        ///     Send chat message informing user of output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pkg"></param>
        public static void RPC_LogToChat(long sender, ZPackage pkg)
        {
            if (sender == ZRoutedRpc.instance.GetServerPeerID() && pkg != null && pkg.Size() > 0)
            {
                // Confirm our Server is sending the RPC
                string msg = pkg.ReadString(); // Get Our Msg
                if (msg != "") // Make sure it isn't empty
                    Chat.instance.AddString("Server", "<color=\"green\">" + msg + "</color>",
                        Talker.Type.Normal); // Add to chat with red color because it's an error
            }
        }
    }
}
using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace RPCExample
{
    [BepInPlugin(PluginId, PluginName, Version)]
    public class RPCExample : BaseUnityPlugin
    {
        private const string Version = "1.0.0";
        private const string PluginId = "azumatt.RPCExample";
        public const string Author = "Azumatt";
        private const string PluginName = "Consume";
        public static readonly ManualLogSource RpcExampleLogger = BepInEx.Logging.Logger.CreateLogSource("Consume");


        private Harmony _harmony;


        public RPCExample(Harmony harmony)
        {
            _harmony = harmony;
        }

        private void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginId);
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }


        /* Server */

        [HarmonyPatch(typeof(Game), "Start")]
        public static class GameStartPatch
        {
            private static void Prefix()
            {
                if (ZNet.instance.IsServer())
                {
                    ZRoutedRpc.instance.Register("Request_Whatever",
                        new Action<long, ZPackage>(Server.RPC_RequestWhatever)); // Our Server Handler
                    ZRoutedRpc.instance.Register("Event_Whatever",
                        new Action<long, ZPackage>(Server.RPC_EventWhatever)); // Our Mock Client Function
                }
            }
        }


        /* Client */
        [HarmonyPatch(typeof(Game), "Start")]
        public static class GameStartPatchClient
        {
            private static void Prefix()
            {
                if (!ZNet.instance.IsServer())
                {
                    ZRoutedRpc.instance.Register("Request_Whatever",
                        new Action<long, ZPackage>(Client.RPC_RequestWhatever)); // Our Mock Server Handler
                    ZRoutedRpc.instance.Register("Event_Whatever",
                        new Action<long, ZPackage>(Client.RPC_EventWhatever)); // Our Client Function
                    ZRoutedRpc.instance.Register("LogToChat", new Action<long, ZPackage>(Client.RPC_LogToChat));
                    ZRoutedRpc.instance.Register("BadRequestMsg",
                        new Action<long, ZPackage>(Client.RPC_BadRequestMsg)); // Our Error Handler
                }
            }
        }


        /* Player consume Item (Invoke Patch) */
        [HarmonyPatch(typeof(Player), nameof(Player.ConsumeItem))]
        public static class ConsumeShit
        {
            public static void Postfix(Inventory inventory, ItemDrop.ItemData item, Player __instance)
            {
                if (__instance == null || item?.m_shared == null || !__instance.m_nview.IsValid()) return;
                var potion = new ZPackage(); // create new ZPackage for sending to server

                potion.Write(item.m_shared.m_name); // write the information that we want to the package
                var localizedString = Localization.instance.Localize(item.m_shared.m_name); // Get localized string.
                RpcExampleLogger.LogInfo(localizedString);

                // Send contents of consumed item over RPC to server
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "Request_Whatever", potion);
            }
        }
    }
}
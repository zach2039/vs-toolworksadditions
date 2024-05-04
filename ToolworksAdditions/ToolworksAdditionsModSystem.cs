using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

using ToolworksAdditions.ModNetwork;

namespace ToolworksAdditions
{
    public class ToolworksAdditionsModSystem : ModSystem
    {
        private IServerNetworkChannel serverChannel;
        private ICoreAPI api;

        public override void StartPre(ICoreAPI api)
        {
            string cfgFileName = "ToolworksAdditions.json";

            try 
            {
                ToolworksAdditionsConfig cfgFromDisk;
                if ((cfgFromDisk = api.LoadModConfig<ToolworksAdditionsConfig>(cfgFileName)) == null)
                {
                    api.StoreModConfig(ToolworksAdditionsConfig.Loaded, cfgFileName);
                }
                else
                {
                    ToolworksAdditionsConfig.Loaded = cfgFromDisk;
                }
            } 
            catch 
            {
                api.StoreModConfig(ToolworksAdditionsConfig.Loaded, cfgFileName);
            }

            base.StartPre(api);
        }

        public override void Start(ICoreAPI api)
        {
            this.api = api;

            base.Start(api);

            api.Logger.Notification("Loaded Toolworks Additions!");
        }

        private void OnPlayerJoin(IServerPlayer player)
        {
            // Send connecting players config settings
            this.serverChannel.SendPacket(
                new SyncConfigClientPacket {
                    
                }, player);
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            sapi.Event.PlayerJoin += this.OnPlayerJoin; 
            
            // Create server channel for config data sync
            this.serverChannel = sapi.Network.RegisterChannel("toolworksadditions")
                .RegisterMessageType<SyncConfigClientPacket>()
                .SetMessageHandler<SyncConfigClientPacket>((player, packet) => {});
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            // Sync config settings with clients
            capi.Network.RegisterChannel("toolworksadditions")
                .RegisterMessageType<SyncConfigClientPacket>()
                .SetMessageHandler<SyncConfigClientPacket>(p => {
                    this.Mod.Logger.Event("Received config settings from server");
                   
                });
        }
        
        public override void Dispose()
        {
            if (this.api is ICoreServerAPI sapi)
            {
                sapi.Event.PlayerJoin -= this.OnPlayerJoin;
            }
        }
    }
}

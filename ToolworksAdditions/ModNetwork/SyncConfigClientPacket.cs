using ProtoBuf;

namespace ToolworksAdditions.ModNetwork
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SyncConfigClientPacket
    {
        public bool PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart;

        public bool PatchToolworksCollectibleBehaviorProspectingPrintProbeResults;

        public bool ApplyToolDurabilityConfigToToolParts;
        
        public bool ApplyToolDurabilityConfigToToolHeads;
    }
}
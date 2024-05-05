using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolworksAdditions
{
    public class ToolworksAdditionsConfig
    {
        public static ToolworksAdditionsConfig Loaded { get; set; } = new ToolworksAdditionsConfig();

        public bool PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart { get; set; } = true;

        public bool ApplyToolDurabilityConfigToParts { get; set; } = true;
    }
}

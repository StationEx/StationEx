namespace StationEx.Runtime.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using StationEx.Runtime.Adapters;
    using StationEx.Sdk.Entities;

    internal static class PlayerIntegration
    {
        [Integration(IntegrationMode.After, "Assembly-CSharp.dll", "Assets.Scripts.Objects.Entities.Human", "System.Void Assets.Scripts.Objects.Entities.Human::OnStateChanged()")]
        public static void PlayerStateChanged([InstanceParameterBinding] HumanAdapter instance)
        {
            // Example use case
        }
    }
}

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
        [Integration(IntegrationType.After, "Assembly-CSharp.dll", "Assets.Scripts.Objects.Entities.Human.OnStateChanged")]
        public static void PlayerStateChanged([InstanceParameterBinding] HumanAdapter instance)
        {
            // Example use case
        }

        [Integration(IntegrationType.After, "Assembly-CSharp.dll", "Assets.Scripts.Objects.Entities.Human.CreateCharacter")]
        public static void PlayerCreated([ReturnParameterBinding(false)] HumanAdapter result)
        {
            // Example use case
        }

        [Integration(IntegrationType.After, "Assembly-CSharp.dll", "Assets.Scripts.Objects.Entities.Human.StateChangedToNotAlive")]
        public static void PlayerDestroyed([InstanceParameterBinding] HumanAdapter instance)
        {
            // Example usecase
        }
    }
}

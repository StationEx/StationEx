namespace StationEx.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using StationEx.Sdk;
    using StationEx.Sdk.Collections;
    using StationEx.Sdk.Managers;

    internal sealed class FeatureContext : IFeatureContext
    {
        private readonly IPlayerCollection players;
        private readonly IPlayerManager playerManager;

        public IPlayerCollection Players
        {
            get
            {
                return this.players;
            }
        }

        public IPlayerManager PlayerManager
        {
            get
            {
                return this.playerManager;
            }
        }

        internal FeatureContext(IPlayerCollection players, IPlayerManager playerManager)
        {
            if (players is null)
            {
                throw new ArgumentNullException(nameof(players));
            }

            if (playerManager is null)
            {
                throw new ArgumentNullException(nameof(playerManager));
            }

            this.players = players;
            this.playerManager = playerManager;
        }
    }
}

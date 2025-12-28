namespace StationEx.Runtime.Entities
{
    using StationEx.Sdk.Entities;

    internal sealed class Player : IPlayer
    {
        private readonly long id;
        private readonly ulong steamId;
        private readonly string name;

        public long Id
        {
            get
            {
                return this.id;
            }
        }

        public ulong SteamId
        {
            get
            {
                return this.steamId;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public Player(long id, ulong steamId, string name)
        {
            this.id = id;
            this.steamId = steamId;
            this.name = name;
        }
    }
}

namespace StationEx.Sdk.Managers
{
    using System;
    using StationEx.Sdk.Entities;

    public interface IPlayerManager
    {
        Action<IPlayer> PlayerCreated
        {
            get;
            set;
        }

        Action<IPlayer> PlayerDestroyed
        {
            get;
            set;
        }
    }
}

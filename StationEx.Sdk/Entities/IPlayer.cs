namespace StationEx.Sdk.Entities
{
    public interface IPlayer : IEntity
    {
        ulong SteamId
        {
            get;
        }
    }
}

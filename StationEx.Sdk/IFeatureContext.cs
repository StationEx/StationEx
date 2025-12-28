namespace StationEx.Sdk
{
    using StationEx.Sdk.Collections;
    using StationEx.Sdk.Managers;

    public interface IFeatureContext
    {
        IPlayerCollection Players
        {
            get;
        }

        IPlayerManager PlayerManager
        {
            get;
        }
    }
}

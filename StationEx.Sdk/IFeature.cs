namespace StationEx.Sdk
{
    using System;

    public interface IFeature
    {
        void Load(IFeatureContext context);

        void Unload();
    }
}

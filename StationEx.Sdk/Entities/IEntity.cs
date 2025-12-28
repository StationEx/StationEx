namespace StationEx.Sdk.Entities
{
    using StationEx.Sdk.Math;

    public interface IEntity
    {
        /// <summary>
        /// The unique identifier for this entity.
        /// </summary>
        long Id
        {
            get;
        }

        /// <summary>
        /// The name of this entity.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// The position of this entity.
        /// </summary>
        Vector3 Position
        {
            get;
        }
    }
}

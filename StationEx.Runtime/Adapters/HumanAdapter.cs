namespace StationEx.Runtime.Adapters
{
    using StationEx.Runtime.Integration;

    [TypeAdapter("Assembly-CSharp.dll", "Assets.Scripts.Objects.Entities.Human")]
    internal sealed class HumanAdapter
    {
        [PropertyBinding("ReferenceId")]
        public long ReferenceId
        {
            get;
        }

        [PropertyBinding("OwnerClientId")]
        public ulong OwnerClientId
        {
            get;
        }

        [PropertyBinding("name")]
        public string Name
        {
            get;
        }

        [FieldBinding("_oldState")]
        public EntityStateAdapter PreviousState
        {
            get;
        }

        [PropertyBinding("State")]
        public EntityStateAdapter CurrentState
        {
            get;
        }
    }
}

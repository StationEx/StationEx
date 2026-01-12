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
            set;
        }

        [PropertyBinding("OwnerClientId")]
        public ulong OwnerClientId
        {
            get;
            set;
        }

        [PropertyBinding("name")]
        public string Name
        {
            get;
            set;
        }

        [FieldBinding("_oldState")]
        public EntityStateAdapter PreviousState
        {
            get;
            set;
        }

        [PropertyBinding("State")]
        public EntityStateAdapter CurrentState
        {
            get;
            set;
        }
    }
}

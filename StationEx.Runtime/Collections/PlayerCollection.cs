namespace StationEx.Runtime.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using StationEx.Sdk.Collections;
    using StationEx.Sdk.Entities;

    internal sealed class PlayerCollection : IPlayerCollection
    {
        public PlayerCollection()
        {

        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<IPlayer> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

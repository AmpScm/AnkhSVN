using System;

namespace Ankh.VS
{
    public struct ImageReference
    {
        public ImageReference(Guid guid, int id)
        {
            Guid = guid;
            Id = id;
        }

        /// <summary>The GUID of the moniker.</summary>
        public Guid Guid;
        /// <summary>The ID of the moniker.</summary>
        public int Id;
    }
}

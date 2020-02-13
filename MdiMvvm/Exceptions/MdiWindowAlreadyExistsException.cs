using System;

namespace MdiMvvm.Exceptions
{
    public class MdiWindowAlreadyExistsException : Exception
    {
        public Guid MdiWindowId { get; private set; }
        public string ContainerTitile { get; private set; }

        public MdiWindowAlreadyExistsException(Guid guid, string containerTitle) 
            : base($"Window with GUID '{guid}' already exists at Collection '{containerTitle}'")
        {
            MdiWindowId = guid;
            ContainerTitile = containerTitle;
        }
    }
}

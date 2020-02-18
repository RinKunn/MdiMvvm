using System;

namespace MdiMvvm.Exceptions
{
    public class MdiWindowNotFoundedException : Exception
    {
        public Guid MdiWindowId { get; private set; }
        public string ContainerTitile { get; private set; }

        public MdiWindowNotFoundedException(Guid guid, string containerTitle)
            : base($"Window with GUID '{guid}' not founded at Collection '{containerTitle}'")
        {
            MdiWindowId = guid;
            ContainerTitile = containerTitle;
        }
    }
}

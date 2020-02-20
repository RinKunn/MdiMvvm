using System;
using System.Collections.Generic;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public class ResumeStoreContext
    {
        public string Id { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public List<ContainersStoreContext> ContainerContextCollection { get; set; }

        public ResumeStoreContext() { }

        internal ResumeStoreContext(string author)
        {
            CreatedDateTime = DateTime.Now;
            AuthorName = author;
            Id = $"{AuthorName}_{CreatedDateTime.ToString("yyyyMMddHHmmss")}";
            ContainerContextCollection = new List<ContainersStoreContext>();
        }
    }
}

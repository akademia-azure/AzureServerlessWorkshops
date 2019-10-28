using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Todos
{
    public class TodoEntity : TableEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public string Title { get; set; }
        public bool Checked { get; set; }
    }
}

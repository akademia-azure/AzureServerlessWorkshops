using System;

namespace Todos
{
    public class Todo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public string Title { get; set; }
        public bool Checked { get; set; }
    }
}

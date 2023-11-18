using Microsoft.EntityFrameworkCore;
using ToDoAPI.DAL.DataModels;

namespace ToDoAPI.DAL.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<ToDoItem> TodoItems { get; set; } = null!;
    }
}

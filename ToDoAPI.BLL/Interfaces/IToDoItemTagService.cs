using ToDoAPI.BLL.Models;

namespace ToDoAPI.BLL.Interfaces
{
    public interface IToDoItemTagService
    {
        Task<IEnumerable<ToDoItemTag>?> GetToDoItemTagByToDoItemId(long id);
        Task<ToDoItemTag?> GetToDoItemTagById(long id, string? accessBy);
        Task<bool> CreateToDoItemTag(ToDoItemTag create);
        Task<bool?> UpdateToDoItemTag(ToDoItemTag update, string? accessBy);
        Task<bool?> DeleteToDoItemTag(long id, string? accessBy);
    }
}

using ToDoAPI.DAL.DataModels;

namespace ToDoAPI.DAL.Interfaces
{
    public interface IToDoItemTagRepository
    {
        Task<IEnumerable<ToDoItemTag>> GetToDoItemTagByToDoItemId(long toDoId);
        Task<ToDoItemTag?> GetToDoItemTagById(long id, string? accessBy);
        Task<long> CreateToDoItemTag(ToDoItemTag tag);
        Task<bool> UpdateToDoItemTag(ToDoItemTag update);
        Task<bool> DeleteToDoItemTag(long id);
    }
}

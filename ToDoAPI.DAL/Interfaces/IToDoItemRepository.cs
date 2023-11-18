using ToDoAPI.DAL.DataModels;

namespace ToDoAPI.DAL.Interfaces
{
    public interface IToDoItemRepository
    {
        Task<IEnumerable<ToDoItem>> GetToDoItems(string? name, string? description, int[] statuses,
            DateTime dueDateFrom, DateTime dueDateTo, List<KeyValuePair<string, string>>? sortingList, string? accessBy);
        Task<ToDoItem?> GetToDoItemById(long id, string? accessBy);
        Task<long> CreateToDoItem(ToDoItem create);
        Task<bool> CreateToDoItemAndItemTag(ToDoItem create, List<ToDoItemTag> toDoItemTagList);
        Task<bool> UpdateToDoItem(ToDoItem update);
        Task<bool> DeleteToDoItem(long id);
        Task<bool> DeleteToDoItemAndTags(long id);
    }
}

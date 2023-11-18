using ToDoAPI.BLL.Models;

namespace ToDoAPI.BLL.Interfaces
{
    public interface IToDoItemWithTagFacade
    {
        Task<IEnumerable<ToDoItemWithTags>?> GetToDoItemsWithTags(string? name, string? description,
            int[] statuses, DateTime dueDateFrom, DateTime dueDateTo, List<KeyValuePair<string, string>> sortingList,
            string? accessBy);
        Task<ToDoItemWithTags?> GetToDoItemsWithTagsById(long id, string? accessBy);
        Task<bool> CreateToDoItemWithTags(ToDoItem toDoItem, List<ToDoItemTag> toDoItemTagList);
        Task<bool> DeleteToDoItemWithTags(long id, string? accessBy);
    }
}

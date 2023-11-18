using Microsoft.AspNetCore.Http.HttpResults;
using ToDoAPI.BLL.Models;

namespace ToDoAPI.BLL.Interfaces
{
    public interface IToDoItemService
    {
        Task<IEnumerable<ToDoItem>?> GetToDoItems(string? name, string? description, int[] statuses,
            DateTime dueDateFrom, DateTime dueDateTo, List<KeyValuePair<string, string>> sortingList, 
            string? accessBy);
        Task<ToDoItem?> GetToDoItemById(long id, string? accessBy);
        Task<long?> CreateToDoItem(ToDoItem create);
        Task<bool> CreateToDoItemWithTags(ToDoItem create, List<ToDoItemTag> toDoItemTagList);
        Task<bool?> UpdateToDoItem(UpdateToDoItem update, string? accessBy);
        Task<bool> DeleteToDoItem(long id);
        Task<bool> DeleteToDoItemAndTags(long id);
    }
}

using ToDoAPI.BLL.Converters;
using ToDoAPI.BLL.Interfaces;
using ToDoAPI.BLL.Models;
using ToDoAPI.DAL.Interfaces;

namespace ToDoAPI.BLL.Services
{
    public class ToDoItemService : IToDoItemService
    {
        private readonly IToDoItemRepository _toDoItemRepository;

        public ToDoItemService(IToDoItemRepository toDoItemRepository)
        {
            _toDoItemRepository = toDoItemRepository;
        }

        public async Task<IEnumerable<ToDoItem>?> GetToDoItems(string? name, string? description, int[] statuses,
            DateTime dueDateFrom, DateTime dueDateTo, List<KeyValuePair<string, string>> sortingList, string? accessBy)
        {
            try
            {
                var result = await _toDoItemRepository.GetToDoItems(name, description, statuses, dueDateFrom, 
                    dueDateTo, sortingList, accessBy);
                return result.Select(ToDoItemConverter.Convert);
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<ToDoItem?> GetToDoItemById(long id, string? accessBy)
        {
            try
            {
                var result = await _toDoItemRepository.GetToDoItemById(id, accessBy);
                return result != null ? ToDoItemConverter.Convert(result) : null;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<long?> CreateToDoItem(ToDoItem create)
        {
            try
            {
                var result = await _toDoItemRepository.CreateToDoItem(ToDoItemConverter.Convert(create));
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<bool?> UpdateToDoItem(UpdateToDoItem update, string? accessBy)
        {
            try
            {
                var currentToDoItem = await _toDoItemRepository.GetToDoItemById(update.Id, accessBy);
                if (currentToDoItem == null)
                    return false;

                var updateToDo = new DAL.DataModels.ToDoItem
                {
                    Id = currentToDoItem.Id,
                    Name = !string.IsNullOrWhiteSpace(update.Name) ? update.Name : currentToDoItem.Name,
                    Description = !string.IsNullOrWhiteSpace(update.Description) ? update.Description : currentToDoItem.Description,
                    DueDate = update.DueDate.HasValue ? update.DueDate.Value : currentToDoItem.DueDate,
                    Status = update.Status.HasValue ? update.Status.Value : currentToDoItem.Status,
                    Priority = update.Priority,
                    UpdatedBy = update.UpdatedBy
                };

                var result = await _toDoItemRepository.UpdateToDoItem(updateToDo);
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<bool> DeleteToDoItem(long id)
        {
            try
            {
                var result = await _toDoItemRepository.DeleteToDoItem(id);
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return false;
            }
        }

        public async Task<bool> CreateToDoItemWithTags(ToDoItem create, List<ToDoItemTag> toDoItemTagList)
        {
            try
            {
                var result = await _toDoItemRepository.CreateToDoItemAndItemTag(ToDoItemConverter.Convert(create),
                    ToDoItemTagConverter.Convert(toDoItemTagList));
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return false;
            }
        }

        public async Task<bool> DeleteToDoItemAndTags(long id)
        {
            try
            {
                var result = await _toDoItemRepository.DeleteToDoItemAndTags(id);
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return false;
            }
        }
    }
}

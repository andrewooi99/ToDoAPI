using Microsoft.VisualBasic;
using ToDoAPI.BLL.Converters;
using ToDoAPI.BLL.Interfaces;
using ToDoAPI.BLL.Models;

namespace ToDoAPI.BLL.Facades
{
    public class ToDoItemWithTagFacade : IToDoItemWithTagFacade
    {
        private readonly IToDoItemService _toDoItemService;
        private readonly IToDoItemTagService _toDoItemTagService;

        public ToDoItemWithTagFacade(IToDoItemService toDoItemService, IToDoItemTagService toDoItemTagService)
        {
            _toDoItemService = toDoItemService;
            _toDoItemTagService = toDoItemTagService;
        }

        public async Task<IEnumerable<ToDoItemWithTags>?> GetToDoItemsWithTags(string? name, string? description,
            int[] statuses, DateTime dueDateFrom, DateTime dueDateTo, List<KeyValuePair<string, string>> sortingList,
            string? accessBy)
        {
            try
            {
                List<ToDoItemWithTags> result = new();
                var toDoItemResult = await _toDoItemService.GetToDoItems(name, description, statuses, dueDateFrom,
                    dueDateTo, sortingList, accessBy);
                if (toDoItemResult != null && toDoItemResult.Any())
                {
                    foreach (var toDoItem in toDoItemResult)
                    {
                        var toDoItemTagResult = await _toDoItemTagService.GetToDoItemTagByToDoItemId(toDoItem.Id);
                        result.Add(ToDoItemWithTagConverter.Convert(toDoItem, toDoItemTagResult));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<ToDoItemWithTags?> GetToDoItemsWithTagsById(long id, string? accessBy)
        {
            try
            {
                ToDoItemWithTags? result = null;
                var toDoItem = await _toDoItemService.GetToDoItemById(id, accessBy);
                if (toDoItem != null)
                {
                    var toDoItemTagResult = await _toDoItemTagService.GetToDoItemTagByToDoItemId(toDoItem.Id);
                    result = ToDoItemWithTagConverter.Convert(toDoItem, toDoItemTagResult);
                }

                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<bool> CreateToDoItemWithTags(ToDoItem toDoItem, List<ToDoItemTag> toDoItemTagList)
        {
            try
            {
                var result = await _toDoItemService.CreateToDoItemWithTags(toDoItem, toDoItemTagList);
                if (result)
                    return true;
                else
                {
                    throw new Exception("CreateToDoItemWithTags failed");
                }
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return false;
            }
        }

        public async Task<bool> DeleteToDoItemWithTags(long id, string? accessBy)
        {
            try
            {
                var toDoItem = await _toDoItemService.GetToDoItemById(id, accessBy);
                if (toDoItem == null)
                    return true;

                var result = await _toDoItemService.DeleteToDoItemAndTags(id);
                if (result)
                {
                    return true;
                }
                else
                {
                    throw new Exception("DeleteToDoItemWithTags failed");
                }
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return false;
            }
        }
    }
}


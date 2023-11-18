using Microsoft.AspNetCore.Http.HttpResults;
using ToDoAPI.BLL.Converters;
using ToDoAPI.BLL.Interfaces;
using ToDoAPI.BLL.Models;
using ToDoAPI.DAL.Interfaces;

namespace ToDoAPI.BLL.Services
{
    public class ToDoItemTagService : IToDoItemTagService
    {
        private readonly IToDoItemTagRepository _toDoItemTagRepository;
        public ToDoItemTagService(IToDoItemTagRepository toDoItemTagRepository)
        {
            _toDoItemTagRepository = toDoItemTagRepository;
        }

        public async Task<IEnumerable<ToDoItemTag>?> GetToDoItemTagByToDoItemId(long id)
        {
            try
            {
                var result = await _toDoItemTagRepository.GetToDoItemTagByToDoItemId(id);
                return result.Select(ToDoItemTagConverter.Convert);
            }
            catch (Exception ex) 
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<ToDoItemTag?> GetToDoItemTagById(long id, string? accessBy)
        {
            try
            {
                var result = await _toDoItemTagRepository.GetToDoItemTagById(id, accessBy);
                return result != null ? ToDoItemTagConverter.Convert(result) : null;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<bool> CreateToDoItemTag(ToDoItemTag create)
        {
            try
            {
                var result = await _toDoItemTagRepository.CreateToDoItemTag(ToDoItemTagConverter.Convert(create));
                return result > 0;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return false;
            }
        }

        public async Task<bool?> UpdateToDoItemTag(ToDoItemTag update, string? accessBy)
        {
            try
            {
                var currentToDoItemTag = await _toDoItemTagRepository.GetToDoItemTagById(update.Id, accessBy);
                if (currentToDoItemTag == null)
                    return false;

                var result = await _toDoItemTagRepository.UpdateToDoItemTag(ToDoItemTagConverter.Convert(update));
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }

        public async Task<bool?> DeleteToDoItemTag(long id, string? accessBy)
        {
            try
            {
                var currentToDoItemTag = await _toDoItemTagRepository.GetToDoItemTagById(id, accessBy);
                if (currentToDoItemTag == null)
                    return false;

                var result = await _toDoItemTagRepository.DeleteToDoItemTag(id);
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                return null;
            }
        }
    }
}

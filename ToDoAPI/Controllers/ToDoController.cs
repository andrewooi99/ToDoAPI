using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoAPI.BLL.Interfaces;
using ToDoAPI.Core.Constants;
using ToDoAPI.Models;

namespace ToDoAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoItemWithTagFacade _toDoItemWithTagFacade;
        private readonly IToDoItemService _toDoItemService;
        private readonly IToDoItemTagService _toDoItemTagService;

        public ToDoController(IToDoItemWithTagFacade toDoItemWithTagFacade, IToDoItemService toDoItemService, IToDoItemTagService toDoItemTagService)
        {
            _toDoItemWithTagFacade = toDoItemWithTagFacade;
            _toDoItemService = toDoItemService;
            _toDoItemTagService = toDoItemTagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetToDoItemWithTags([FromQuery] GetToDoItemFilters filters)
        {
            try
            {
                if (filters.IsValid())
                {
                    var currentUser = HttpContext.User;
                    string accessBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
                    var role = currentUser.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                    var getResult = await _toDoItemWithTagFacade.GetToDoItemsWithTags(filters.Name, filters.Description,
                        filters.Status, Convert.ToDateTime(filters.DueDateFrom), Convert.ToDateTime(filters.DueDateTo),
                        filters.SortingList, role != UserRoles.Admin ? accessBy : null);
                    if (getResult == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response { Status = StatusMessage.Error, Message = ResponseMessage.InternalServerError });
                    }

                    return Ok(getResult);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = StatusMessage.Error, Message = filters.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetToDoItemWithTagsById([FromQuery] long? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var currentUser = HttpContext.User;
                    string accessBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
                    var role = currentUser.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                    var getResult = await _toDoItemWithTagFacade.GetToDoItemsWithTagsById(id.Value, role != UserRoles.Admin ? accessBy : null);
                    if (getResult == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new Response { Status = StatusMessage.Error, Message = ResponseMessage.ToDoItemDNE });
                    }

                    return Ok(getResult);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.IdIsMandatory });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetToDoItemTagsById([FromQuery] long? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var currentUser = HttpContext.User;
                    string accessBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
                    var role = currentUser.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                    var getResult = await _toDoItemTagService.GetToDoItemTagById(id.Value, role != UserRoles.Admin ? accessBy : null);
                    if (getResult == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new Response { Status = StatusMessage.Error, Message = ResponseMessage.ToDoItemTagDNE });
                    }

                    return Ok(getResult);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.IdIsMandatory });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateToDoItem(CreateToDoItemWithTags create)
        {
            try
            {
                if (create.IsValid())
                {
                    var currentUser = HttpContext.User;
                    create.CreatedBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

                    var createResult = await _toDoItemWithTagFacade.CreateToDoItemWithTags(create.GetToDoItem(), create.GeToDoItemTagList());
                    if (createResult)
                        return Ok(new Response { Status = StatusMessage.Success, Message = ResponseMessage.CreateToDoItemSuccessful });

                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.CreateToDoItemFailed });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = StatusMessage.Error, Message = create.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateToDoItem(UpdateToDoItem update)
        {
            try
            {
                if (update.IsValid())
                {
                    var currentUser = HttpContext.User;
                    update.UpdatedBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
                    var role = currentUser.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                    var getResult = await _toDoItemService.UpdateToDoItem(update.GetUpdateToDoItem(), role != UserRoles.Admin ? update.UpdatedBy : null);
                    if (getResult.HasValue)
                    {
                        if (getResult.Value)
                        {
                            return Ok(new Response { Status = StatusMessage.Success, Message = ResponseMessage.UpdateToDoItemSuccessful });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new Response { Status = StatusMessage.Error, Message = ResponseMessage.ToDoItemDNE });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response { Status = StatusMessage.Error, Message = ResponseMessage.UpdateToDoItemFailed });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = StatusMessage.Error, Message = update.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteToDoItem([FromQuery] long? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var currentUser = HttpContext.User;
                    var accessBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

                    // No need check accessBy for Admin
                    var deleteResult = await _toDoItemWithTagFacade.DeleteToDoItemWithTags(id.Value, null);
                    if (deleteResult)
                    {
                        return Ok(new Response { Status = StatusMessage.Success, Message = ResponseMessage.DeletionToDoItemSuccesful });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new Response { Status = StatusMessage.Error, Message = ResponseMessage.DeletionToDoItemFailed });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.IdIsMandatory });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateToDoItemTag(CreateToDoItemTag create)
        {
            try
            {
                var currentUser = HttpContext.User;
                create.CreatedBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

                var createResult = await _toDoItemTagService.CreateToDoItemTag(create.GetToDoItemTag());

                if (createResult)
                    return Ok(new Response { Status = StatusMessage.Success, Message = ResponseMessage.CreateToDoItemTagSuccessful });

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusMessage.Error, Message = ResponseMessage.CreateToDoItemTagFailed });
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateToDoItemTag(UpdateToDoItemTag update)
        {
            try
            {
                var currentUser = HttpContext.User;
                update.UpdatedBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
                var role = currentUser.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                var getResult = await _toDoItemTagService.UpdateToDoItemTag(update.GetToDoItemTag(), role != UserRoles.Admin ? update.UpdatedBy : null);
                if (getResult.HasValue)
                {
                    if (getResult.Value)
                    {
                        return Ok(new Response { Status = StatusMessage.Success, Message = ResponseMessage.UpdateToDoItemTagSuccessful });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new Response { Status = StatusMessage.Error, Message = ResponseMessage.ToDoItemTagDNE });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UpdateToDoItemTagFailed });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteToDoItemTag([FromQuery] long? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var currentUser = HttpContext.User;
                    var accessBy = currentUser.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

                    // No need check accessBy for Admin
                    var deleteResult = await _toDoItemTagService.DeleteToDoItemTag(id.Value, null);
                    if (deleteResult.HasValue)
                    {
                        if (deleteResult.Value)
                        {
                            return Ok(new Response { Status = StatusMessage.Success, Message = ResponseMessage.DeletionToDoItemTagSuccesful });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new Response { Status = StatusMessage.Error, Message = ResponseMessage.ToDoItemTagDNE });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new Response { Status = StatusMessage.Error, Message = ResponseMessage.DeletionToDoItemTagFailed });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.IdIsMandatory });
                }
            }
            catch (Exception ex)
            {
                // TODO Logging

                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = StatusMessage.Error, Message = ResponseMessage.UnknownError });
            }
        }
    }
}

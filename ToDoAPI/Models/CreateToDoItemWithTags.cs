using System.ComponentModel.DataAnnotations;
using ToDoAPI.BLL.Models;
using ToDoAPI.Core.Enums;

namespace ToDoAPI.Models
{
    public class CreateToDoItemWithTags
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "DueDate is required")]
        public string? DueDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int? Status { get; set; }

        public int? Priority { get; set; }

        public string[]? SharedBy { get; set; }

        public string? CreatedBy { get; set; }

        public string? ErrorMessage { get; set; }

        public List<ToDoItemTag>? ToDoItemTagList { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(DueDate))
            {
                ErrorMessage = "DueDate is mandatory";
                return false;
            }

            if (!DateTime.TryParse(DueDate, out _))
            {
                ErrorMessage = "DueDate format is invalid";
                return false;
            }

            if (!EnumList.ToDoItemStatusList.Contains(Status.Value))
            {
                ErrorMessage = "Status is invalid";
                return false;
            }

            return true;
        }

        public ToDoItem GetToDoItem()
        {
            return new ToDoItem
            {
                Name = Name,
                Description = Description,
                DueDate = Convert.ToDateTime(DueDate),
                Status = Status.Value,
                Priority = Priority,
                SharedBy = SharedBy != null && SharedBy.Length > 0 ? string.Join(',', SharedBy) : null,
                CreatedBy = CreatedBy
            };
        }

        public List<BLL.Models.ToDoItemTag> GeToDoItemTagList()
        {
            var result = new List<BLL.Models.ToDoItemTag>();
            if (ToDoItemTagList != null)
            {
                result.AddRange(ToDoItemTagList.Select(x => new BLL.Models.ToDoItemTag
                {
                    TagKey = x.TagKey,
                    TagValue = x.TagValue,
                    CreatedBy = CreatedBy
                }));
            }

            return result;
        }
    }
}

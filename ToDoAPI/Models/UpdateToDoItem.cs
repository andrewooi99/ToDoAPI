using System.ComponentModel.DataAnnotations;
using ToDoAPI.BLL.Models;
using ToDoAPI.Core.Enums;

namespace ToDoAPI.Models
{
    public class UpdateToDoItem
    {
        [Required(ErrorMessage = "Id is mandatory")]
        public long? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? DueDate { get; set; }

        public int? Status { get; set; }

        public int? Priority { get; set; }

        public string[]? SharedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public string? ErrorMessage { get; set; }

        public bool IsValid()
        {
            if (!string.IsNullOrWhiteSpace(DueDate) && !DateTime.TryParse(DueDate, out _))
            {
                ErrorMessage = "DueDate format is invalid";
                return false;
            }

            if (Status.HasValue && !EnumList.ToDoItemStatusList.Contains(Status.Value))
            {
                ErrorMessage = "Status is invalid";
                return false;
            }

            return true;
        }

        public BLL.Models.UpdateToDoItem GetUpdateToDoItem()
        {
            return new BLL.Models.UpdateToDoItem
            {
                Id = Id.Value,
                Name = Name,
                Description = Description,
                DueDate = !string.IsNullOrWhiteSpace(DueDate) ? Convert.ToDateTime(DueDate) : null,
                Status = Status ?? 0,
                Priority = Priority,
                SharedBy = SharedBy != null && SharedBy.Length > 0 ? string.Join(',', SharedBy) : null,
                UpdatedBy = UpdatedBy
            };
        }
    }
}

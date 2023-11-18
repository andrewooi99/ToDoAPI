using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models
{
    public class CreateToDoItemTag : ToDoItemTag
    {
        [Required(ErrorMessage = "ToDoId is required")]
        public long? ToDoId { get; set; }

        public string? CreatedBy { get; set; }

        public BLL.Models.ToDoItemTag GetToDoItemTag()
        {
            return new BLL.Models.ToDoItemTag
            {
                ToDoItemId = ToDoId.Value,
                TagKey = TagKey,
                TagValue = TagValue,
                CreatedBy = CreatedBy
            };
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models
{
    public class UpdateToDoItemTag
    {
        [Required(ErrorMessage = "Id is required")]
        public long? Id { get; set; }

        [Required(ErrorMessage = "TagKey is required")]
        [MaxLength(50, ErrorMessage = "Max length of TagKey is 50")]
        public string? TagKey { get; set; }

        [MaxLength(200, ErrorMessage = "Max length of TagValue is 200")]
        public string? TagValue { get; set; }

        public string? UpdatedBy { get; set; }

        public BLL.Models.ToDoItemTag GetToDoItemTag()
        {
            return new BLL.Models.ToDoItemTag
            {
                Id = Id.Value,
                TagKey = TagKey,
                TagValue = TagValue,
                UpdatedBy = UpdatedBy
            };
        }
    }
}

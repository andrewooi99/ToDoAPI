using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models
{
    public class ToDoItemTag
    {
        [Required(ErrorMessage = "TagKey is required")]
        [MaxLength(50, ErrorMessage = "Max length of TagKey is 50")]
        public string? TagKey { get; set; }

        [MaxLength(200, ErrorMessage = "Max length of TagValue is 200")]
        public string? TagValue { get; set; }
    }
}

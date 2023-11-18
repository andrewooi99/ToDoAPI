using ToDoAPI.BLL.Models;

namespace ToDoAPI.BLL.Converters
{
    public static class ToDoItemWithTagConverter
    {
        public static ToDoItemWithTags Convert(ToDoItem item, IEnumerable<ToDoItemTag>? tags)
        {
            var result = new ToDoItemWithTags
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                DueDate = item.DueDate,
                Status = item.Status,
                Priority = item.Priority,
                CreatedAt = item.CreatedAt,
                CreatedBy = item.CreatedBy,
                UpdatedAt = item.UpdatedAt,
                UpdatedBy = item.UpdatedBy
            };

            if (tags != null && tags.Count() > 0)
            {
                foreach(var tag in tags)
                {
                    var newTag = new ToDoItemTag
                    {
                        Id = tag.Id,
                        ToDoItemId = result.Id,
                        TagKey = tag.TagKey,
                        TagValue = tag.TagValue,
                        CreatedAt = item.CreatedAt,
                        CreatedBy = item.CreatedBy,
                        UpdatedAt = item.UpdatedAt,
                        UpdatedBy = item.UpdatedBy

                    };
                    result.ToDoItemTagList.Add(newTag);
                }
            }

            return result;
        }
    }
}

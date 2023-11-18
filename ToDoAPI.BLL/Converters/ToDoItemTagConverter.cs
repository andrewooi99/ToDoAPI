using ToDoAPI.BLL.Models;

namespace ToDoAPI.BLL.Converters
{
    public static class ToDoItemTagConverter
    {
        public static ToDoItemTag Convert(DAL.DataModels.ToDoItemTag data)
        {
            return new ToDoItemTag
            {
                Id = data.Id,
                ToDoItemId = data.ToDoItemId,
                TagKey = data.TagKey,
                TagValue = data.TagValue,
                CreatedAt = data.CreatedAt,
                CreatedBy = data.CreatedBy,
                UpdatedAt = data.UpdatedAt,
                UpdatedBy = data.UpdatedBy
            };
        }

        public static DAL.DataModels.ToDoItemTag Convert(ToDoItemTag data)
        {
            return new DAL.DataModels.ToDoItemTag
            {
                Id = data.Id,
                ToDoItemId = data.ToDoItemId,
                TagKey = data.TagKey,
                TagValue = data.TagValue,
                CreatedAt = data.CreatedAt,
                CreatedBy = data.CreatedBy,
                UpdatedAt = data.UpdatedAt,
                UpdatedBy = data.UpdatedBy
            };
        }

        public static List<DAL.DataModels.ToDoItemTag> Convert(List<ToDoItemTag> data)
        {
            return data.Select(x => Convert(x)).ToList();
        }
    }
}

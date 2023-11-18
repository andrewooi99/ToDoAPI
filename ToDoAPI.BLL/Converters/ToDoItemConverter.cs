using ToDoAPI.BLL.Models;

namespace ToDoAPI.BLL.Converters
{
    public static class ToDoItemConverter
    {
        public static ToDoItem Convert(DAL.DataModels.ToDoItem data)
        {
            return new ToDoItem
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                DueDate = data.DueDate,
                Status = data.Status,
                Priority = data.Priority,
                CreatedAt = data.CreatedAt,
                CreatedBy = data.CreatedBy,
                UpdatedAt = data.UpdatedAt,
                UpdatedBy = data.UpdatedBy
            };
        }

        public static DAL.DataModels.ToDoItem Convert(ToDoItem data)
        {
            return new DAL.DataModels.ToDoItem
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                DueDate = data.DueDate,
                Status = data.Status,
                Priority = data.Priority,
                CreatedAt = data.CreatedAt,
                CreatedBy = data.CreatedBy,
                UpdatedAt = data.UpdatedAt,
                UpdatedBy = data.UpdatedBy
            };
        }
    }
}

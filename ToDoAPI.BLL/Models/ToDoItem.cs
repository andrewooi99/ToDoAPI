namespace ToDoAPI.BLL.Models
{
    public class ToDoItem : DataTimeBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public int? Priority { get; set; }
    }
}

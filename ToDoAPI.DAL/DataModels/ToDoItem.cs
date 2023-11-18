namespace ToDoAPI.DAL.DataModels
{
    public class ToDoItem : DateTimeBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public int? Priority { get; set; }
    }
}

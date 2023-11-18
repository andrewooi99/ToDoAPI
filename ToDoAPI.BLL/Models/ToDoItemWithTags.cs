namespace ToDoAPI.BLL.Models
{
    public class ToDoItemWithTags : DataTimeBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public int? Priority { get; set; }

        public List<ToDoItemTag> ToDoItemTagList { get; set; }

        public ToDoItemWithTags() 
        {
            ToDoItemTagList = new List<ToDoItemTag>();
        }
    }
}

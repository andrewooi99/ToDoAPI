namespace ToDoAPI.BLL.Models
{
    public class ToDoItemTag : DataTimeBase
    {
        public long Id { get; set; }
        public long ToDoItemId { get; set; }
        public string? TagKey { get; set; }
        public string? TagValue { get; set; }
    }
}

namespace ToDoAPI.DAL.DataModels
{
    public class ToDoItemTag : DateTimeBase
    {
        public long Id { get; set; }
        public long ToDoItemId { get; set; }
        public string? TagKey { get; set; }
        public string? TagValue { get; set; }
    }
}

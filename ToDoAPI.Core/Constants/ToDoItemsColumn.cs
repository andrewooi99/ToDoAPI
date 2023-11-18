namespace ToDoAPI.Core.Constants
{
    public static class ToDoItemsColumn
    {
        public const string Name = "Name";
        public const string Description = "Description";
        public const string DueDate = "DueDate";
        public const string Status = "Status";

        public static List<string> ToDoItemsColumnList = new List<string> { Name, Description, DueDate, Status };
    }
}

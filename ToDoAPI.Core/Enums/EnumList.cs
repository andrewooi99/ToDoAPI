namespace ToDoAPI.Core.Enums
{
    public static class EnumList
    {

        public static List<int> ToDoItemStatusList = new() { (int)ToDoItemStatus.NotStarted, (int)ToDoItemStatus.InProgress, (int)ToDoItemStatus.Completed };
    }
}

namespace ToDoAPI.Core.Constants
{
    public static class ResponseMessage
    {
        public const string UnknownError = "Unknown Error";
        public const string InternalServerError = "Internal Server Error";
        public const string UserAleadyExists = "User Already Exists!";
        public const string RegistrationFailed = "Registration Failed! Please Check User Details and Try Again.";
        public const string RegistrationSuccessful = "Registration Successful!";

        // ToDoItem
        public const string CreateToDoItemSuccessful = "Create ToDo Successful";
        public const string CreateToDoItemFailed = "Create ToDo Failed!";
        public const string UpdateToDoItemSuccessful = "Update ToDo Successful";
        public const string ToDoItemDNE = "ToDo Does Not Exists!";
        public const string UpdateToDoItemFailed = "Update ToDo Failed!";
        public const string DeletionToDoItemSuccesful = "Delete ToDo Successful";
        public const string DeletionToDoItemFailed = "Delete ToDo Failed!";

        // ToDoItemTag
        public const string CreateToDoItemTagSuccessful = "Create ToDo Tag Successful";
        public const string CreateToDoItemTagFailed = "Create ToDo Tag Failed!";
        public const string UpdateToDoItemTagSuccessful = "Update ToDo Tag Successful";
        public const string ToDoItemTagDNE = "ToDo Tag Does Not Exists!";
        public const string UpdateToDoItemTagFailed = "Update ToDo Tag Failed!";
        public const string DeletionToDoItemTagSuccesful = "Delete ToDo Tag Successful";
        public const string DeletionToDoItemTagFailed = "Delete ToDo Tag Failed!";

        public const string IdIsMandatory = "Id Is Mandatory";
    }
}

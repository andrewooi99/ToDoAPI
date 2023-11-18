using System.ComponentModel.DataAnnotations;
using ToDoAPI.Core.Constants;

namespace ToDoAPI.Models
{
    public class GetToDoItemFilters
    {
        public long? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int[]? Status { get; set; }

        [Required(ErrorMessage = "DueDateFrom is required")]
        public string? DueDateFrom { get; set; }

        [Required(ErrorMessage = "DueDateTo is required")]
        public string? DueDateTo { get; set; }

        public string[]? Sorting { get; set; }

        public List<KeyValuePair<string, string>>? SortingList { get; set; }

        public string? ErrorMessage { get; set; }

        public GetToDoItemFilters()
        {
            ErrorMessage = ResponseMessage.UnknownError;
        }

        public bool IsValid()
        {
            if (Status == null || Status.Length <= 0)
            {
                ErrorMessage = "Status is Mandatory";
                return false;
            }

            if (string.IsNullOrWhiteSpace(DueDateFrom))
            {
                ErrorMessage = "DueDateFrom is Mandatory";
                return false;
            }

            if (!DateTime.TryParse(DueDateFrom, out DateTime parseDateFrom))
            {
                ErrorMessage = "DueDateFrom format is invalid";
                return false;
            }

            if (string.IsNullOrWhiteSpace(DueDateTo))
            {
                ErrorMessage = "DueDateTo is Mandatory";
                return false;
            }

            if (!DateTime.TryParse(DueDateFrom, out DateTime parseDateTo))
            {
                ErrorMessage = "DueDateTo format is invalid";
                return false;
            }

            if (parseDateFrom > parseDateTo)
            {
                ErrorMessage = "Invalid DueDateFrom and DueDateTo range";
                return false;
            }

            if (Sorting != null && Sorting.Length > 0)
            {
                SortingList = new List<KeyValuePair<string, string>>();

                foreach(var sort in Sorting)
                {
                    if (!sort.Contains("|"))
                    {
                        ErrorMessage = "Invalid Sorting Value";
                        return false;
                    }

                    var splitSort = sort.Split('|');
                    if (!ToDoItemsColumn.ToDoItemsColumnList.Contains(splitSort[0], StringComparer.OrdinalIgnoreCase))
                    {
                        ErrorMessage = "Invalid sorting column";
                        return false;
                    }

                    if (!SortingOrder.SortingOrderList.Contains(splitSort[1], StringComparer.OrdinalIgnoreCase))
                    {
                        ErrorMessage = "Invalid sorting order";
                        return false;
                    }

                    SortingList.Add(new KeyValuePair<string, string>(splitSort[0], splitSort[1]));
                }               
            }

            return true;
        }
    }
}

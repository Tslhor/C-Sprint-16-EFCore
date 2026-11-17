namespace ShoppingSystem.ViewModels
{
    public class PaginationViewModel
    {
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}

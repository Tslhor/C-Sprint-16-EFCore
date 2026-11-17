namespace ShoppingSystem.ViewModels
{
    public class PagedListViewModel<T> : PaginationViewModel
    {
        public List<T> Items { get; set; } = new List<T>();
    }
}

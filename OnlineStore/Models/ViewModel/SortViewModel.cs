namespace OnlineStore.Models.ViewModel;

public enum SortState
{
    NameAsc,
    NameDesc,
    RateAsc,
    RateDesc,
    PriceAsc,
    PriceDesc,
    CategoryAsc,
    CategoryDesc
}

public class SortViewModel
{
    public SortViewModel(SortState sortOrder)
    {
        NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
        RateSort = sortOrder == SortState.RateAsc ? SortState.RateDesc : SortState.RateAsc;
        PriceSort = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.CategoryAsc;
        CategorySort = sortOrder == SortState.CategoryAsc ? SortState.CategoryDesc : SortState.CategoryAsc;
        Current = sortOrder;
    }

    public SortState NameSort { get; }
    public SortState RateSort { get; }
    public SortState PriceSort { get; }
    public SortState CategorySort { get; }
    public SortState Current { get; }
}
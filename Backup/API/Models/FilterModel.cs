using Data.Utilities;

namespace API.Models
{
    public class FilterModel
    {
        public int? Page { get; set; } = 0;
        public int? PageSize { get; set; } = 20;
        public string[] Keywords { get; set; }


        public virtual SortSpecification[] SortSpecifications { get; set; }
            = new [] {
                new SortSpecification("Id", SortDirection.Ascending)
            };
    }
}
using Data.Utilities;

namespace API.Models.FilterModels
{
    public class PopulationFilterModel : FilterModel
    {
        public PopulationFilterType Type { get; set; } = PopulationFilterType.Keywords;
        public string HawkId { get; set; }

        public enum PopulationFilterType
        {
            Keywords = 0x1,
            Username = 0x2
        }

        public virtual SortSpecification[] SortSpecifications { get; set; }
            = new[] { new SortSpecification("Username", SortDirection.Ascending) };
    }
}
namespace Data.Utilities
{
    public class SortSpecification
    {
        public string Property { get; set; }
        public SortDirection Direction { get; set; }
        public SortSpecification(string property, SortDirection direction)
        {
            this.Property = property;
            this.Direction = direction;
        }

        public override string ToString()
        {
            return $"{Property??"null"}:{Direction}";
        }
    }

    public enum SortDirection
    {
        Ascending = 0x1,
        Descending = 0x2
    }
}
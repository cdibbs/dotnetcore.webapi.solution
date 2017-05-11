namespace API.Models.FilterModels
{
    public enum Operator
    {
        ApiGreaterThan = 0x1,
        ApiLessThan = 0x2,
        ApiEquals = 0x3,
        ApiContains = 0x4,
        ApiBetween = 0x5,
    }
}
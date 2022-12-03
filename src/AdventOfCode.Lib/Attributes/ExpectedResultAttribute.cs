namespace AdventOfCode.Lib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ExpectedResultAttribute : Attribute
{
    public object Expected { get; }
    public object? Sample { get; set; }

    public ExpectedResultAttribute(object expected)
    {
        Expected = expected;
    }
}
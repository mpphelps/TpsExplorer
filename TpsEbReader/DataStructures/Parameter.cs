namespace TpsEbReader;

public class Parameter
{
    public Parameter(string field, string value)
    {
        Field = field;
        Value = value;
    }

    public string Field { get; set; }
    public string Value { get; set; }
}
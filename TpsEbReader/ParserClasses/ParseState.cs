namespace TpsEbReader;

public enum ParseState
{
    StartOfEntity,
    EntityType,
    EntityName,
    EntityParameters,
    EndOfEntity,
    SkipSystemEntity
}
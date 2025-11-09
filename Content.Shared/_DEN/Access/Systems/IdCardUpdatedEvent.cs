namespace Content.Shared._DEN.Access.Systems;

public sealed class IdCardNameUpdatedEvent(string? newName) : EntityEventArgs
{
    public string? NewName { get; set; } = newName;
}

public sealed class IdCardJobTitleUpdatedEvent(string? newJobTitle) : EntityEventArgs
{
    public string? NewJobTitle { get; set; } = newJobTitle;
}

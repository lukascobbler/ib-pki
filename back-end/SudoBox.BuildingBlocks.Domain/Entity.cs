namespace SudoBox.BuildingBlocks.Domain;

public class Entity
{
    public Guid Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Id.Equals(entity.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
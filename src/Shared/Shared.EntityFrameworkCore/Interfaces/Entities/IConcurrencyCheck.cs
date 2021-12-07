namespace Shared.Abstraction.Interfaces.Entities
{
    public interface IConcurrencyCheck
    {
        byte[] Timestamp { get; set; }
    }
}
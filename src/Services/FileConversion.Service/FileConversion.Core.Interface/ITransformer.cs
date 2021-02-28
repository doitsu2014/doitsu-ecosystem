using FileConversion.Abstraction;

namespace FileConversion.Core.Interface
{
    public interface ITransformer
    {
        StreamType Type { get; }

        byte[] Transform(byte[] content);
    }
}

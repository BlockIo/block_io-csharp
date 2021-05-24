
namespace BlockIoLib
{
    public class BlockIoResponse <T>
    {
        public string Status { get; set; }
        public T Data { get; set; }
    }
}

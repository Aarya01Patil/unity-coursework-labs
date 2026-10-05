namespace SoulSucker.World
{
    public interface ILinkedToggleTarget
    {
        string LinkId { get; }
        void SetLinkedActive(bool active);
    }
}

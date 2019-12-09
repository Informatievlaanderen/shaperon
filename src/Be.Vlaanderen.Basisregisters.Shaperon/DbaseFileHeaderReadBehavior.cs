namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public class DbaseFileHeaderReadBehavior
    {
        public static readonly DbaseFileHeaderReadBehavior Default = new DbaseFileHeaderReadBehavior(false);

        public DbaseFileHeaderReadBehavior(bool ignoreFieldOffset)
        {
            IgnoreFieldOffset = ignoreFieldOffset;
        }

        public bool IgnoreFieldOffset { get; }
    }
}

namespace SpriteAnimations.Editor
{
    public interface ITickProvider
    {
        event TickEvent Tick;
    }

    public interface IFPSProvider
    {
        int FPS { get; }
        event FPSChangedEvent FPSChanged;
    }
}
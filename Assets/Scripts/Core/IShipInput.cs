namespace Asteroids.Scripts.Core
{
    public interface IShipInput
    {
        float Turn { get; }
        float Thrust { get; }
        bool Fire { get; }
        bool AnyStart { get; }
    }
}
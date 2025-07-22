namespace David6.ShooterCore.TickSystem
{
    public interface IDLateTickable
    {
        void LateTick(float deltaTime);
    }
}
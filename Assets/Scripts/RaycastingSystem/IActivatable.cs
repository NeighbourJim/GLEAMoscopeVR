namespace GLEAMoscopeVR.Interaction
{
    public interface IActivatable
    {
        bool IsActivated { get; }

        bool CanActivate();
        void Activate();
        void Deactivate();
    }
}
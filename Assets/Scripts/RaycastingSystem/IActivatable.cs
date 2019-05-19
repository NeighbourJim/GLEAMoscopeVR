namespace GLEAMoscopeVR.Interaction
{
    public interface IActivatable
    {
        float ActivationTime { get; }
        bool IsActivated { get; }
        bool CanActivate();
        void Activate();
        void Deactivate();
    }
}
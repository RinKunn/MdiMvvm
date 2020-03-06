namespace MdiMvvm.Interfaces
{
    public interface INotifiable
    {
        bool IsSuccess { get; }
        string NotificationMessage { get; }
    }
}

namespace QTCAD.API.Commands
{
    public interface ICommand
    {
        string Id { get; }

        string DisplayName { get; }

        string Description { get; }

        void Execute();
    }
}

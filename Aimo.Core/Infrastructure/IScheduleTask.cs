namespace Aimo.Core.Infrastructure;

public partial interface IBackgroundTask
{
    string SystemType { get; }
    string SystemName { get; }
}
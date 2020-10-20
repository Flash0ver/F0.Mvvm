namespace F0.Windows.Input
{
	public interface IBoundedCommand : IAsyncCommand
	{
		int CurrentCount { get; }
		int MaxCount { get; }
	}

	public interface IBoundedCommand<T> : IAsyncCommand<T>
	{
		int CurrentCount { get; }
		int MaxCount { get; }
	}
}

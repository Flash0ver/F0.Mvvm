namespace F0.Windows.Input
{
	public interface IBoundedCommandSlim : IAsyncCommandSlim
	{
		int CurrentCount { get; }
		int MaxCount { get; }
	}

	public interface IBoundedCommandSlim<T> : IAsyncCommandSlim<T>
	{
		int CurrentCount { get; }
		int MaxCount { get; }
	}
}

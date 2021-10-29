#if !HAS_TASKCOMPLETIONSOURCE
namespace System.Threading.Tasks
{
	internal class TaskCompletionSource
	{
		private readonly TaskCompletionSource<object?> tcs;

		public TaskCompletionSource()
		{
			tcs = new TaskCompletionSource<object?>();
		}

		public Task Task => tcs.Task;

		public void SetCanceled()
		{
			tcs.SetCanceled();
		}

		public void SetException(Exception exception)
		{
			tcs.SetException(exception);
		}

		public void SetResult()
		{
			tcs.SetResult(null);
		}
	}
}
#endif

using System;
using System.Threading.Tasks;

namespace F0.Windows.Input
{
	internal sealed class AsyncRelayCommandSlim : AsyncCommandSlimBase
	{
		private readonly Func<ValueTask> onExecute;
		private readonly Func<bool>? onCanExecute;

		internal AsyncRelayCommandSlim(Func<ValueTask> onExecute)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		}

		internal AsyncRelayCommandSlim(Func<ValueTask> onExecute, Func<bool> onCanExecute)
			: this(onExecute)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		protected override bool OnCanExecute()
		{
			return onCanExecute is null || onCanExecute();
		}

		protected override ValueTask OnExecuteAsync()
		{
			return onExecute();
		}
	}

	internal sealed class AsyncRelayCommandSlim<T> : AsyncCommandSlimBase<T> where T : notnull
	{
		private readonly Func<T, ValueTask> onExecute;
		private readonly Predicate<T>? onCanExecute;

		internal AsyncRelayCommandSlim(Func<T, ValueTask> onExecute)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		}

		internal AsyncRelayCommandSlim(Func<T, ValueTask> onExecute, Predicate<T> onCanExecute)
			: this(onExecute)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		protected override bool OnCanExecute(T parameter)
		{
			return onCanExecute is null || onCanExecute(parameter);
		}

		protected override ValueTask OnExecuteAsync(T parameter)
		{
			return onExecute(parameter);
		}
	}
}

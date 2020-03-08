using System;
using System.Threading.Tasks;
using F0.Windows.Input;

namespace F0.Tests.Shared
{
	internal sealed class AsyncTestCommandSlim : AsyncCommandSlimBase
	{
		private readonly Action onCanExecute;
		private readonly Func<ValueTask> onExecute;

		private bool isEnabled;
		public bool IsEnabled
		{
			get => isEnabled;
			set
			{
				if (value != isEnabled)
				{
					isEnabled = value;
					RaiseCanExecuteChanged();
				}
			}
		}

		internal AsyncTestCommandSlim()
		{
			isEnabled = true;
		}

		internal AsyncTestCommandSlim(Action onCanExecute, Func<ValueTask> onExecute)
			: this()
		{
			this.onCanExecute = onCanExecute;
			this.onExecute = onExecute;
		}

		protected override bool OnCanExecute()
		{
			onCanExecute?.Invoke();
			return IsEnabled;
		}

		protected override ValueTask OnExecuteAsync()
		{
			return onExecute?.Invoke() ?? default;
		}
	}

	internal sealed class AsyncTestCommandSlim<T> : AsyncCommandSlimBase<T>
	{
		private readonly Action<T> onCanExecute;
		private readonly Func<T, ValueTask> onExecute;

		private bool isEnabled;
		public bool IsEnabled
		{
			get => isEnabled;
			set
			{
				if (value != isEnabled)
				{
					isEnabled = value;
					RaiseCanExecuteChanged();
				}
			}
		}

		internal AsyncTestCommandSlim()
		{
			isEnabled = true;
		}

		internal AsyncTestCommandSlim(Action<T> onCanExecute, Func<T, ValueTask> onExecute)
			: this()
		{
			this.onCanExecute = onCanExecute;
			this.onExecute = onExecute;
		}

		protected override bool OnCanExecute(T parameter)
		{
			onCanExecute?.Invoke(parameter);
			return IsEnabled;
		}

		protected override ValueTask OnExecuteAsync(T parameter)
		{
			return onExecute?.Invoke(parameter) ?? default;
		}
	}
}

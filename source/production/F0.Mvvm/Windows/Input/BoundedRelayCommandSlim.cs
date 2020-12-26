using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace F0.Windows.Input
{
	internal sealed class BoundedRelayCommandSlim : AsyncCommandSlimBase, IBoundedCommandSlim, INotifyPropertyChanged
	{
		private readonly Func<ValueTask> onExecute;
		private readonly Func<bool> onCanExecute;
		private int currentCount;

		internal BoundedRelayCommandSlim(Func<ValueTask> onExecute)
			: this(onExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommandSlim(Func<ValueTask> onExecute, Func<bool> onCanExecute)
			: this(onExecute, onCanExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommandSlim(Func<ValueTask> onExecute, int maxCount)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));

			if (maxCount < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, "[1,int.MaxValue]");
			}

			MaxCount = maxCount;
		}

		internal BoundedRelayCommandSlim(Func<ValueTask> onExecute, Func<bool> onCanExecute, int maxCount)
			: this(onExecute, maxCount)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public int CurrentCount => currentCount;
		public int MaxCount { get; }

		protected override bool OnCanExecute()
		{
			return currentCount < MaxCount && (onCanExecute is null || onCanExecute());
		}

		protected override async ValueTask OnExecuteAsync()
		{
			int incremented = Interlocked.Increment(ref currentCount);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCount)));
			if (incremented == MaxCount)
			{
				RaiseCanExecuteChanged();
			}
			try
			{
				await onExecute();
			}
			finally
			{
				int decremented = Interlocked.Decrement(ref currentCount);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCount)));
				if (decremented == MaxCount - 1)
				{
					RaiseCanExecuteChanged();
				}
			}
		}
	}

	internal sealed class BoundedRelayCommandSlim<T> : AsyncCommandSlimBase<T>, IBoundedCommandSlim<T>, INotifyPropertyChanged
	{
		private readonly Func<T, ValueTask> onExecute;
		private readonly Predicate<T> onCanExecute;
		private int currentCount;

		internal BoundedRelayCommandSlim(Func<T, ValueTask> onExecute)
			: this(onExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommandSlim(Func<T, ValueTask> onExecute, Predicate<T> onCanExecute)
			: this(onExecute, onCanExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommandSlim(Func<T, ValueTask> onExecute, int maxCount)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));

			if (maxCount < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, "[1,int.MaxValue]");
			}

			MaxCount = maxCount;
		}

		internal BoundedRelayCommandSlim(Func<T, ValueTask> onExecute, Predicate<T> onCanExecute, int maxCount)
			: this(onExecute, maxCount)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public int CurrentCount => currentCount;
		public int MaxCount { get; }

		protected override bool OnCanExecute(T parameter)
		{
			return currentCount < MaxCount && (onCanExecute is null || onCanExecute(parameter));
		}

		protected override async ValueTask OnExecuteAsync(T parameter)
		{
			int incremented = Interlocked.Increment(ref currentCount);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCount)));
			if (incremented == MaxCount)
			{
				RaiseCanExecuteChanged();
			}
			try
			{
				await onExecute(parameter);
			}
			finally
			{
				int decremented = Interlocked.Decrement(ref currentCount);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCount)));
				if (decremented == MaxCount - 1)
				{
					RaiseCanExecuteChanged();
				}
			}
		}
	}
}

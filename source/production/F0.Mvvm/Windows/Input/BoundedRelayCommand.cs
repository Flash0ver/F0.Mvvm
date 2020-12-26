using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace F0.Windows.Input
{
	internal sealed class BoundedRelayCommand : AsyncCommandBase, IBoundedCommand, INotifyPropertyChanged
	{
		private readonly Func<Task> onExecute;
		private readonly Func<bool> onCanExecute;
		private int currentCount;

		internal BoundedRelayCommand(Func<Task> onExecute)
			: this(onExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommand(Func<Task> onExecute, Func<bool> onCanExecute)
			: this(onExecute, onCanExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommand(Func<Task> onExecute, int maxCount)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));

			if (maxCount <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, "(0,int.MaxValue]");
			}

			MaxCount = maxCount;
		}

		internal BoundedRelayCommand(Func<Task> onExecute, Func<bool> onCanExecute, int maxCount)
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

		protected override async Task OnExecuteAsync()
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
				if (decremented + 1 == MaxCount)
				{
					RaiseCanExecuteChanged();
				}
			}
		}
	}

	internal sealed class BoundedRelayCommand<T> : AsyncCommandBase<T>, IBoundedCommand<T>, INotifyPropertyChanged
	{
		private readonly Func<T, Task> onExecute;
		private readonly Predicate<T> onCanExecute;
		private int currentCount;

		internal BoundedRelayCommand(Func<T, Task> onExecute)
			: this(onExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommand(Func<T, Task> onExecute, Predicate<T> onCanExecute)
			: this(onExecute, onCanExecute, Int32.MaxValue)
		{
		}

		internal BoundedRelayCommand(Func<T, Task> onExecute, int maxCount)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));

			if (maxCount <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, "(0,int.MaxValue]");
			}

			MaxCount = maxCount;
		}

		internal BoundedRelayCommand(Func<T, Task> onExecute, Predicate<T> onCanExecute, int maxCount)
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

		protected override async Task OnExecuteAsync(T parameter)
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
				if (decremented + 1 == MaxCount)
				{
					RaiseCanExecuteChanged();
				}
			}
		}
	}
}

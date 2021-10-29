using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class BoundedRelayCommandSlimTests
	{
		[Fact]
		public void DoNotCreateCommandWithNullDelegates()
		{
			Assert.Throws<ArgumentNullException>("onExecute", () => new BoundedRelayCommandSlim(null!));
			Assert.Throws<ArgumentNullException>("onExecute", () => new BoundedRelayCommandSlim<int>(null!));

			Assert.Throws<ArgumentNullException>("onCanExecute", () => new BoundedRelayCommandSlim(() => default, null!));
			Assert.Throws<ArgumentNullException>("onCanExecute", () => new BoundedRelayCommandSlim<int>(_ => default, null!));

			Assert.Throws<ArgumentNullException>("onExecute", () => new BoundedRelayCommandSlim(null!, () => true));
			Assert.Throws<ArgumentNullException>("onExecute", () => new BoundedRelayCommandSlim<int>(null!, _ => true));
		}

		[Theory]
		[InlineData(Int32.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public void DoNotCreateCommandWithMaximumCountLessThanOne(int maxCount)
		{
			Func<ValueTask> onExecute = () => default;
			Func<string, ValueTask> onExecuteT = _ => default;

			Func<bool> onCanExecute = () => false;
			Predicate<string> onCanExecuteT = _ => false;

			Assert.Throws<ArgumentOutOfRangeException>("maxCount", () => new BoundedRelayCommandSlim(onExecute, maxCount));
			Assert.Throws<ArgumentOutOfRangeException>("maxCount", () => new BoundedRelayCommandSlim<string>(onExecuteT, maxCount));

			Assert.Throws<ArgumentOutOfRangeException>("maxCount", () => new BoundedRelayCommandSlim(onExecute, onCanExecute, maxCount));
			Assert.Throws<ArgumentOutOfRangeException>("maxCount", () => new BoundedRelayCommandSlim<string>(onExecuteT, onCanExecuteT, maxCount));
		}

		[Fact]
		public void TheMaximumCountArgumentMustBeAPositiveNumber()
		{
			Assert.Equal(1, new BoundedRelayCommandSlim(() => default, 1).MaxCount);
			Assert.Equal(1, new BoundedRelayCommandSlim<string>(_ => default, 1).MaxCount);

			Assert.Equal(1, new BoundedRelayCommandSlim(() => default, () => true, 1).MaxCount);
			Assert.Equal(1, new BoundedRelayCommandSlim<string>(_ => default, _ => true, 1).MaxCount);
		}

		[Fact]
		public void TheDefaultMaximumCountIsTheLargestPossibleInteger()
		{
			const int noMaximum = Int32.MaxValue;

			Assert.Equal(noMaximum, new BoundedRelayCommandSlim(() => default).MaxCount);
			Assert.Equal(noMaximum, new BoundedRelayCommandSlim<string>(_ => default).MaxCount);

			Assert.Equal(noMaximum, new BoundedRelayCommandSlim(() => default, () => true).MaxCount);
			Assert.Equal(noMaximum, new BoundedRelayCommandSlim<string>(_ => default, _ => true).MaxCount);
		}

		[Fact]
		public void TheDefaultReturnValueForTheCanExecuteMethodIsTrue()
		{
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => default);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => default);

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public void TheExecutionStatusLogicDeterminesWhetherTheBoundedRelayCommandSlimCanExecuteInItsCurrentState()
		{
			bool canExecute = false;
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => default, () => canExecute);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => default, _ => canExecute);

			Assert.False(command.CanExecute());
			Assert.False(commandT.CanExecute("F0"));

			canExecute = true;

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public async Task ExecuteTheBoundedRelayCommandSlimOnTheCurrentCommandTarget()
		{
			int number = 0;
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => { number++; return default; });
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => { number--; return default; });

			Assert.Equal(0, number);
			await command.ExecuteAsync();
			Assert.Equal(1, number);
			await commandT.ExecuteAsync("F0");
			Assert.Equal(0, number);
		}

		[Fact]
		public async Task InContrastToTheNonGenericCommandWhichDoesNotSupportDataToBePassed_DataIsUsedByTheStronglyTypedCommand()
		{
			int number = 0;
			IBoundedCommandSlim<int> commandT = new BoundedRelayCommandSlim<int>(addend => { number += addend; return default; }, integer => integer % 2 == 0);

			Assert.False(commandT.CanExecute(1));
			Assert.True(commandT.CanExecute(2));

			Assert.Equal(0, number);
			await commandT.ExecuteAsync(1);
			Assert.Equal(1, number);
			await commandT.ExecuteAsync(2);
			Assert.Equal(3, number);
			await commandT.ExecuteAsync(3);
			Assert.Equal(6, number);
			await commandT.ExecuteAsync(-9);
			Assert.Equal(-3, number);
		}

		[Fact]
		public async Task TheCommandCannotBeExecutedIfTheCurrentCountOfOperationsExceedTheMaximumCount()
		{
			TaskCompletionSource tcs = new();
			TaskCompletionSource tcsT = new();

			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(tcs.Task), 1);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(tcsT.Task), 1);

			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);
			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));

			ValueTask operation = command.ExecuteAsync();
			ValueTask operationT = commandT.ExecuteAsync("F0");

			Assert.Equal(1, command.CurrentCount);
			Assert.Equal(1, commandT.CurrentCount);
			Assert.False(command.CanExecute());
			Assert.False(commandT.CanExecute("F0"));

			tcs.SetResult();
			tcsT.SetResult();
			await operation;
			await operationT;

			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);
			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public async Task CommandCanAlwaysBeInvokedDirectly_RegardlessWhetherTheCommandCanBeExecutedOrNot()
		{
			int number = 0;
			bool canExecute = false;
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => { number++; return default; }, () => canExecute);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => { number--; return default; }, _ => canExecute);

			Assert.False(command.CanExecute());
			await command.ExecuteAsync();
			Assert.Equal(1, number);
			Assert.False(commandT.CanExecute("F0"));
			await commandT.ExecuteAsync("F0");
			Assert.Equal(0, number);

			canExecute = true;

			Assert.True(command.CanExecute());
			await command.ExecuteAsync();
			Assert.Equal(1, number);
			Assert.True(commandT.CanExecute("F0"));
			await commandT.ExecuteAsync("F0");
			Assert.Equal(0, number);
		}

		[Fact]
		public async Task CommandCanAlwaysBeInvokedDirectly_RegardlessWhetherTheMaximumCountIsExceededOrNot()
		{
			int number = 0;
			int numberT = 0;
			TaskCompletionSource tcs = new();
			TaskCompletionSource tcsT = new();
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => { number++; return new ValueTask(tcs.Task); }, 1);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => { numberT++; return new ValueTask(tcsT.Task); }, 1);

			Assert.Equal(0, number);
			Assert.Equal(0, numberT);
			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);
			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));

			ValueTask operationOne = command.ExecuteAsync();
			ValueTask operationOneT = commandT.ExecuteAsync("F0");

			Assert.Equal(1, number);
			Assert.Equal(1, numberT);
			Assert.Equal(1, command.CurrentCount);
			Assert.Equal(1, commandT.CurrentCount);
			Assert.False(command.CanExecute());
			Assert.False(commandT.CanExecute("F0"));

			ValueTask operationTwo = command.ExecuteAsync();
			ValueTask operationTwoT = commandT.ExecuteAsync("F0");

			Assert.Equal(2, number);
			Assert.Equal(2, numberT);
			Assert.Equal(2, command.CurrentCount);
			Assert.Equal(2, commandT.CurrentCount);
			Assert.False(command.CanExecute());
			Assert.False(commandT.CanExecute("F0"));

			tcs.SetResult();
			tcsT.SetResult();
			await operationOne;
			await operationOneT;
			await operationTwo;
			await operationTwoT;

			Assert.Equal(2, number);
			Assert.Equal(2, numberT);
			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);
			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public async Task WhenInvokedOperationIsCanceled_ThenCurrentCountIsStillDecrementedAndExceptionIsThrown()
		{
			TaskCompletionSource tcs = new();
			TaskCompletionSource tcsT = new();

			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(tcs.Task));
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(tcsT.Task));

			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);

			ValueTask operation = command.ExecuteAsync();
			ValueTask operationT = commandT.ExecuteAsync("F0");

			Assert.Equal(1, command.CurrentCount);
			Assert.Equal(1, commandT.CurrentCount);

			tcs.SetCanceled();
			tcsT.SetCanceled();
			await Assert.ThrowsAsync<TaskCanceledException>(() => operation.AsTask());
			await Assert.ThrowsAsync<TaskCanceledException>(() => operationT.AsTask());

			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);
		}

		[Fact]
		public async Task WhenInvokedOperationIsFaulted_ThenCurrentCountIsStillDecrementedAndExceptionIsThrown()
		{
			TaskCompletionSource tcs = new();
			TaskCompletionSource tcsT = new();

			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(tcs.Task));
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(tcsT.Task));

			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);

			ValueTask operation = command.ExecuteAsync();
			ValueTask operationT = commandT.ExecuteAsync("F0");

			Assert.Equal(1, command.CurrentCount);
			Assert.Equal(1, commandT.CurrentCount);

			tcs.SetException(new InvalidOperationException());
			tcsT.SetException(new NotSupportedException());
			await Assert.ThrowsAsync<InvalidOperationException>(() => operation.AsTask());
			await Assert.ThrowsAsync<NotSupportedException>(() => operationT.AsTask());

			Assert.Equal(0, command.CurrentCount);
			Assert.Equal(0, commandT.CurrentCount);
		}

		[Fact]
		public async Task NotifyClientsThatCurrentCountHasChanged()
		{
			TaskCompletionSource tcs = new();
			TaskCompletionSource tcsT = new();
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(tcs.Task));
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(tcsT.Task));

			List<object?> senders = new();
			List<object?> sendersT = new();
			List<string?> propertyNames = new();
			List<string?> propertyNamesT = new();
			List<int> counts = new();
			List<int> countsT = new();

			void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
			{
				senders.Add(sender);
				propertyNames.Add(e.PropertyName);
				counts.Add(command.CurrentCount);
			}
			void OnPropertyChangedT(object? sender, PropertyChangedEventArgs e)
			{
				sendersT.Add(sender);
				propertyNamesT.Add(e.PropertyName);
				countsT.Add(commandT.CurrentCount);
			}

			((INotifyPropertyChanged)command).PropertyChanged += OnPropertyChanged;
			((INotifyPropertyChanged)commandT).PropertyChanged += OnPropertyChangedT;

			ValueTask operation = command.ExecuteAsync();
			ValueTask operationT = commandT.ExecuteAsync("F0");

			Assert.Equal(new object[] { command }, senders);
			Assert.Equal(new object[] { commandT }, sendersT);
			Assert.Equal(new string[] { nameof(IBoundedCommandSlim.CurrentCount) }, propertyNames);
			Assert.Equal(new string[] { nameof(IBoundedCommandSlim<string>.CurrentCount) }, propertyNamesT);
			Assert.Equal(new int[] { 1 }, counts);
			Assert.Equal(new int[] { 1 }, countsT);

			tcs.SetResult();
			tcsT.SetResult();
			await operation;
			await operationT;

			Assert.Equal(new object[] { command, command }, senders);
			Assert.Equal(new object[] { commandT, commandT }, sendersT);
			Assert.Equal(new string[] { nameof(IBoundedCommandSlim.CurrentCount), nameof(IBoundedCommandSlim.CurrentCount) }, propertyNames);
			Assert.Equal(new string[] { nameof(IBoundedCommandSlim<string>.CurrentCount), nameof(IBoundedCommandSlim<string>.CurrentCount) }, propertyNamesT);
			Assert.Equal(new int[] { 1, 0 }, counts);
			Assert.Equal(new int[] { 1, 0 }, countsT);

			((INotifyPropertyChanged)command).PropertyChanged -= OnPropertyChanged;
			((INotifyPropertyChanged)commandT).PropertyChanged -= OnPropertyChangedT;

			await command.ExecuteAsync();
			await commandT.ExecuteAsync("F0");

			Assert.Equal(new object[] { command, command }, senders);
			Assert.Equal(new object[] { commandT, commandT }, sendersT);
			Assert.Equal(new string[] { nameof(IBoundedCommandSlim.CurrentCount), nameof(IBoundedCommandSlim.CurrentCount) }, propertyNames);
			Assert.Equal(new string[] { nameof(IBoundedCommandSlim<string>.CurrentCount), nameof(IBoundedCommandSlim<string>.CurrentCount) }, propertyNamesT);
			Assert.Equal(new int[] { 1, 0 }, counts);
			Assert.Equal(new int[] { 1, 0 }, countsT);
		}

		[Fact]
		public async Task NotifyClientsThatCurrentCountHasChanged_Canceled()
		{
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(Task.FromCanceled(new CancellationToken(true))));
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(Task.FromCanceled(new CancellationToken(true))));

			int onPropertyChanged = 0;
			int onPropertyChangedT = 0;
			((INotifyPropertyChanged)command).PropertyChanged += (sender, e) => onPropertyChanged++;
			((INotifyPropertyChanged)commandT).PropertyChanged += (sender, e) => onPropertyChangedT++;

			await Assert.ThrowsAsync<TaskCanceledException>(() => command.ExecuteAsync().AsTask());
			await Assert.ThrowsAsync<TaskCanceledException>(() => commandT.ExecuteAsync("F0").AsTask());

			Assert.Equal(2, onPropertyChanged);
			Assert.Equal(2, onPropertyChangedT);
		}

		[Fact]
		public async Task NotifyClientsThatCurrentCountHasChanged_Faulted()
		{
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(Task.FromException(new InvalidOperationException())));
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(Task.FromException(new NotSupportedException())));

			int onPropertyChanged = 0;
			int onPropertyChangedT = 0;
			((INotifyPropertyChanged)command).PropertyChanged += (sender, e) => onPropertyChanged++;
			((INotifyPropertyChanged)commandT).PropertyChanged += (sender, e) => onPropertyChangedT++;

			await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync().AsTask());
			await Assert.ThrowsAsync<NotSupportedException>(() => commandT.ExecuteAsync("F0").AsTask());

			Assert.Equal(2, onPropertyChanged);
			Assert.Equal(2, onPropertyChangedT);
		}

		[Fact]
		public async Task RaiseCanExecuteChangedEvent()
		{
			TaskCompletionSource tcs = new();
			TaskCompletionSource tcsT = new();
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(tcs.Task), 1);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(tcsT.Task), 1);

			List<object?> senders = new();
			List<object?> sendersT = new();
			List<EventArgs> eventArgs = new();
			List<EventArgs> eventArgsT = new();
			List<bool> canExecute = new();
			List<bool> canExecuteT = new();

			void OnCanExecuteChanged(object? sender, EventArgs e)
			{
				senders.Add(sender);
				eventArgs.Add(e);
				canExecute.Add(command.CanExecute());
			}
			void OnCanExecuteChangedT(object? sender, EventArgs e)
			{
				sendersT.Add(sender);
				eventArgsT.Add(e);
				canExecuteT.Add(commandT.CanExecute("F0"));
			}

			command.CanExecuteChanged += OnCanExecuteChanged;
			commandT.CanExecuteChanged += OnCanExecuteChangedT;

			ValueTask operation = command.ExecuteAsync();
			ValueTask operationT = commandT.ExecuteAsync("F0");

			Assert.Equal(new object[] { command }, senders);
			Assert.Equal(new object[] { commandT }, sendersT);
			Assert.Equal(new EventArgs[] { EventArgs.Empty }, eventArgs);
			Assert.Equal(new EventArgs[] { EventArgs.Empty }, eventArgsT);
			Assert.Equal(new bool[] { false }, canExecute);
			Assert.Equal(new bool[] { false }, canExecuteT);

			tcs.SetResult();
			tcsT.SetResult();
			await operation;
			await operationT;

			Assert.Equal(new object[] { command, command }, senders);
			Assert.Equal(new object[] { commandT, commandT }, sendersT);
			Assert.Equal(new EventArgs[] { EventArgs.Empty, EventArgs.Empty }, eventArgs);
			Assert.Equal(new EventArgs[] { EventArgs.Empty, EventArgs.Empty }, eventArgsT);
			Assert.Equal(new bool[] { false, true }, canExecute);
			Assert.Equal(new bool[] { false, true }, canExecuteT);

			command.CanExecuteChanged -= OnCanExecuteChanged;
			commandT.CanExecuteChanged -= OnCanExecuteChangedT;

			await command.ExecuteAsync();
			await commandT.ExecuteAsync("F0");

			Assert.Equal(new object[] { command, command }, senders);
			Assert.Equal(new object[] { commandT, commandT }, sendersT);
			Assert.Equal(new EventArgs[] { EventArgs.Empty, EventArgs.Empty }, eventArgs);
			Assert.Equal(new EventArgs[] { EventArgs.Empty, EventArgs.Empty }, eventArgsT);
			Assert.Equal(new bool[] { false, true }, canExecute);
			Assert.Equal(new bool[] { false, true }, canExecuteT);
		}

		[Fact]
		public async Task RaiseCanExecuteChangedEvent_WhenChangesToTheCurrentCountOccurThatAffectWhetherOrNotTheCommandShouldExecute()
		{
			TaskCompletionSource tcs = new();
			TaskCompletionSource tcsT = new();
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(tcs.Task), 2);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(tcsT.Task), 2);

			List<object?> senders = new();
			List<object?> sendersT = new();
			List<EventArgs> eventArgs = new();
			List<EventArgs> eventArgsT = new();
			List<bool> canExecute = new();
			List<bool> canExecuteT = new();

			void OnCanExecuteChanged(object? sender, EventArgs e)
			{
				senders.Add(sender);
				eventArgs.Add(e);
				canExecute.Add(command.CanExecute());
			}
			void OnCanExecuteChangedT(object? sender, EventArgs e)
			{
				sendersT.Add(sender);
				eventArgsT.Add(e);
				canExecuteT.Add(commandT.CanExecute("F0"));
			}

			command.CanExecuteChanged += OnCanExecuteChanged;
			commandT.CanExecuteChanged += OnCanExecuteChangedT;

			ValueTask operation1 = command.ExecuteAsync();
			ValueTask operation1T = commandT.ExecuteAsync("F0");

			Assert.Equal(new object[] { }, senders);
			Assert.Equal(new object[] { }, sendersT);
			Assert.Equal(new EventArgs[] { }, eventArgs);
			Assert.Equal(new EventArgs[] { }, eventArgsT);
			Assert.Equal(new bool[] { }, canExecute);
			Assert.Equal(new bool[] { }, canExecuteT);

			ValueTask operation2 = command.ExecuteAsync();
			ValueTask operation2T = commandT.ExecuteAsync("F0");

			Assert.Equal(new object[] { command }, senders);
			Assert.Equal(new object[] { commandT }, sendersT);
			Assert.Equal(new EventArgs[] { EventArgs.Empty }, eventArgs);
			Assert.Equal(new EventArgs[] { EventArgs.Empty }, eventArgsT);
			Assert.Equal(new bool[] { false }, canExecute);
			Assert.Equal(new bool[] { false }, canExecuteT);

			ValueTask operation3 = command.ExecuteAsync();
			ValueTask operation3T = commandT.ExecuteAsync("F0");

			Assert.Equal(new object[] { command }, senders);
			Assert.Equal(new object[] { commandT }, sendersT);
			Assert.Equal(new EventArgs[] { EventArgs.Empty }, eventArgs);
			Assert.Equal(new EventArgs[] { EventArgs.Empty }, eventArgsT);
			Assert.Equal(new bool[] { false }, canExecute);
			Assert.Equal(new bool[] { false }, canExecuteT);

			tcs.SetResult();
			tcsT.SetResult();
			await operation1;
			await operation1T;
			await operation2;
			await operation2T;
			await operation3;
			await operation3T;

			Assert.Equal(new object[] { command, command }, senders);
			Assert.Equal(new object[] { commandT, commandT }, sendersT);
			Assert.Equal(new EventArgs[] { EventArgs.Empty, EventArgs.Empty }, eventArgs);
			Assert.Equal(new EventArgs[] { EventArgs.Empty, EventArgs.Empty }, eventArgsT);
			Assert.Equal(new bool[] { false, true }, canExecute);
			Assert.Equal(new bool[] { false, true }, canExecuteT);
		}

		[Fact]
		public async Task RaiseCanExecuteChangedEvent_Canceled()
		{
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(Task.FromCanceled(new CancellationToken(true))), 1);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(Task.FromCanceled(new CancellationToken(true))), 1);

			int onCanExecuteChanged = 0;
			int onCanExecuteChangedT = 0;
			command.CanExecuteChanged += (sender, e) => onCanExecuteChanged++;
			commandT.CanExecuteChanged += (sender, e) => onCanExecuteChangedT++;

			await Assert.ThrowsAsync<TaskCanceledException>(() => command.ExecuteAsync().AsTask());
			await Assert.ThrowsAsync<TaskCanceledException>(() => commandT.ExecuteAsync("F0").AsTask());

			Assert.Equal(2, onCanExecuteChanged);
			Assert.Equal(2, onCanExecuteChangedT);
		}

		[Fact]
		public async Task RaiseCanExecuteChangedEvent_Faulted()
		{
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => new ValueTask(Task.FromException(new InvalidOperationException())), 1);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(_ => new ValueTask(Task.FromException(new NotSupportedException())), 1);

			int onCanExecuteChanged = 0;
			int onCanExecuteChangedT = 0;
			command.CanExecuteChanged += (sender, e) => onCanExecuteChanged++;
			commandT.CanExecuteChanged += (sender, e) => onCanExecuteChangedT++;

			await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync().AsTask());
			await Assert.ThrowsAsync<NotSupportedException>(() => commandT.ExecuteAsync("F0").AsTask());

			Assert.Equal(2, onCanExecuteChanged);
			Assert.Equal(2, onCanExecuteChangedT);
		}

		[Fact]
		public async Task OrderOfExecutionForEvents()
		{
			string text = "";
			string textT = "";

			IBoundedCommandSlim command = new BoundedRelayCommandSlim(() => { text += "240,"; return new ValueTask(); }, 1);
			IBoundedCommandSlim<string> commandT = new BoundedRelayCommandSlim<string>(parameter => { textT += parameter + ","; return new ValueTask(); }, 1);

			((INotifyPropertyChanged)command).PropertyChanged += (sender, e) => text += command.CurrentCount + ",";
			((INotifyPropertyChanged)commandT).PropertyChanged += (sender, e) => textT += commandT.CurrentCount + ",";
			command.CanExecuteChanged += (sender, e) => text += command.CanExecute() + ",";
			commandT.CanExecuteChanged += (sender, e) => textT += commandT.CanExecute("F0") + ",";

			await command.ExecuteAsync();
			await commandT.ExecuteAsync("F_0");

			Assert.Equal("1,False,240,0,True,", text);
			Assert.Equal("1,False,F_0,0,True,", textT);
		}
	}
}

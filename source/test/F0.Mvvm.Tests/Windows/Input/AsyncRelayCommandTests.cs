using System;
using System.Threading.Tasks;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class AsyncRelayCommandTests
	{
		[Fact]
		public void DoNotCreateCommandWithNullDelegates()
		{
			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommand(null));
			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommand<int>(null));

			Assert.Throws<ArgumentNullException>("onCanExecute", () => new AsyncRelayCommand(() => Task.CompletedTask, null));
			Assert.Throws<ArgumentNullException>("onCanExecute", () => new AsyncRelayCommand<int>(_ => Task.CompletedTask, null));

			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommand(null, () => true));
			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommand<int>(null, _ => true));
		}

		[Fact]
		public void TheDefaultReturnValueForTheCanExecuteMethodIsTrue()
		{
			IAsyncCommand command = new AsyncRelayCommand(() => Task.CompletedTask);
			IAsyncCommand<string> commandT = new AsyncRelayCommand<string>(_ => Task.CompletedTask);

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public void TheExecutionStatusLogicDeterminesWhetherTheAsyncRelayCommandCanExecuteInItsCurrentState()
		{
			bool canExecute = false;
			IAsyncCommand command = new AsyncRelayCommand(() => Task.CompletedTask, () => canExecute);
			IAsyncCommand<string> commandT = new AsyncRelayCommand<string>(_ => Task.CompletedTask, _ => canExecute);

			Assert.False(command.CanExecute());
			Assert.False(commandT.CanExecute("F0"));

			canExecute = true;

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public async Task ExecuteTheAsyncRelayCommandOnTheCurrentCommandTarget()
		{
			int number = 0;
			IAsyncCommand command = new AsyncRelayCommand(() => { number++; return Task.CompletedTask; });
			IAsyncCommand<string> commandT = new AsyncRelayCommand<string>(_ => { number--; return Task.CompletedTask; });

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
			IAsyncCommand<int> commandT = new AsyncRelayCommand<int>(addend => { number += addend; return Task.CompletedTask; }, integer => integer % 2 == 0);

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
		public async Task CommandCanAlwaysBeInvokedDirectly_RegardlessWhetherTheCommandCanBeExecutedOrNot()
		{
			int number = 0;
			bool canExecute = false;
			IAsyncCommand command = new AsyncRelayCommand(() => { number++; return Task.CompletedTask; }, () => canExecute);
			IAsyncCommand<string> commandT = new AsyncRelayCommand<string>(_ => { number--; return Task.CompletedTask; }, _ => canExecute);

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
	}
}

using System;
using System.Threading.Tasks;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class AsyncRelayCommandSlimTests
	{
		[Fact]
		public void DoNotCreateCommandWithNullDelegates()
		{
			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommandSlim(null));
			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommandSlim<int>(null));

			Assert.Throws<ArgumentNullException>("onCanExecute", () => new AsyncRelayCommandSlim(() => default, null));
			Assert.Throws<ArgumentNullException>("onCanExecute", () => new AsyncRelayCommandSlim<int>(_ => default, null));

			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommandSlim(null, () => true));
			Assert.Throws<ArgumentNullException>("onExecute", () => new AsyncRelayCommandSlim<int>(null, _ => true));
		}

		[Fact]
		public void TheDefaultReturnValueForTheCanExecuteMethodIsTrue()
		{
			IAsyncCommandSlim command = new AsyncRelayCommandSlim(() => default);
			IAsyncCommandSlim<string> commandT = new AsyncRelayCommandSlim<string>(_ => default);

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public void TheExecutionStatusLogicDeterminesWhetherTheAsyncRelayCommandSlimCanExecuteInItsCurrentState()
		{
			bool canExecute = false;
			IAsyncCommandSlim command = new AsyncRelayCommandSlim(() => default, () => canExecute);
			IAsyncCommandSlim<string> commandT = new AsyncRelayCommandSlim<string>(_ => default, _ => canExecute);

			Assert.False(command.CanExecute());
			Assert.False(commandT.CanExecute("F0"));

			canExecute = true;

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public async Task ExecuteTheAsyncRelayCommandSlimOnTheCurrentCommandTarget()
		{
			int number = 0;
			IAsyncCommandSlim command = new AsyncRelayCommandSlim(() => { number++; return default; });
			IAsyncCommandSlim<string> commandT = new AsyncRelayCommandSlim<string>(_ => { number--; return default; });

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
			IAsyncCommandSlim<int> commandT = new AsyncRelayCommandSlim<int>(addend => { number += addend; return default; }, integer => integer % 2 == 0);

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
			IAsyncCommandSlim command = new AsyncRelayCommandSlim(() => { number++; return default; }, () => canExecute);
			IAsyncCommandSlim<string> commandT = new AsyncRelayCommandSlim<string>(_ => { number--; return default; }, _ => canExecute);

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

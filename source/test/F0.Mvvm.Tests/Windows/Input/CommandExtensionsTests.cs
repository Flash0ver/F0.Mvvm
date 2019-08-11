using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class CommandExtensionsTests
	{
		[Fact]
		public void ExecuteTheCommandOnlyIfTheCommandCanBeExecuted_ReturnTrueIfTheCommandWasExecuted_OtherwiseFalse()
		{
			int executeCount = 0;
			var command = new TestCommand(() => { }, () => executeCount++);

			Assert.True(command.CanExecute());
			Assert.True(command.TryExecute());
			Assert.Equal(1, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
			Assert.False(command.TryExecute());
			Assert.Equal(1, executeCount);
		}

		[Fact]
		public void Return_TrueIfTheCommandHasBeenExecuted_FalseIfTheCommandHasNotBeenExecuted()
		{
			int executeCount = 0;
			var command = new TestCommand<int>(_ => { }, integer => executeCount += integer);

			Assert.True(command.CanExecute(9));
			Assert.True(command.TryExecute(9));
			Assert.Equal(9, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute(9));
			Assert.False(command.TryExecute(9));
			Assert.Equal(9, executeCount);
		}
	}
}

using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class CommandTests
	{
		[Fact]
		public void Create_Synchronous_NoCommandParameter_DefaultState()
		{
			IInputCommand command = Command.Create(() => { });
			Assert.IsType<RelayCommand>(command);
			Assert.True(command.CanExecute());
		}

		[Fact]
		public void Create_Synchronous_NoCommandParameter_MutableState()
		{
			IInputCommand command = Command.Create(() => { }, () => false);
			Assert.IsType<RelayCommand>(command);
			Assert.False(command.CanExecute());
		}

		[Fact]
		public void Create_Synchronous_StronglyTypedCommandParameter_DefaultState()
		{
			IInputCommand<int> command = Command.Create<int>(_ => { });
			Assert.IsType<RelayCommand<int>>(command);
			Assert.True(command.CanExecute(240));
		}

		[Fact]
		public void Create_Synchronous_StronglyTypedCommandParameter_MutableState()
		{
			IInputCommand<int> command = Command.Create<int>(_ => { }, _ => false);
			Assert.IsType<RelayCommand<int>>(command);
			Assert.False(command.CanExecute(240));
		}
	}
}

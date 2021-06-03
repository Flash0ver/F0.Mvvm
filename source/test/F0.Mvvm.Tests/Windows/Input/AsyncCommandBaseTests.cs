using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using F0.Tests.Shared;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class AsyncCommandBaseTests
	{
		[Fact]
		public async Task AsyncCommandBase_FrameworkMethodsMustBeCalledWithoutData()
		{
			ICommand command = new AsyncTestCommand();

			bool canExecute = command.CanExecute(null);
			Assert.True(canExecute);
			Assert.Throws<ArgumentException>("parameter", () => command.CanExecute(240));

			command.Execute(null);
			await Assert.ThrowsAsync<ArgumentException>("parameter", () => ((AsyncCommandBase)command).InternalExecuteAsync(240));
		}

		[Fact]
		public async Task AsyncCommandBase_T_FrameworkMethodsMustBeCalledWithData_ValueType()
		{
			ICommand command = new AsyncTestCommand<int>();

			bool canExecute = command.CanExecute(240);
			Assert.True(canExecute);
			Assert.Throws<NullReferenceException>(() => command.CanExecute(null));

			command.Execute(240);
			await Assert.ThrowsAsync<NullReferenceException>(() => ((AsyncCommandBase<int>)command).InternalExecuteAsync(null));
		}

		[Fact]
		public async Task AsyncCommandBase_T_FrameworkMethodsMustBeCalledWithData_ReferenceType()
		{
			ICommand command = new AsyncTestCommand<string>();

			bool canExecute = command.CanExecute("240");
			Assert.True(canExecute);
			Assert.Throws<ArgumentNullException>("parameter", () => command.CanExecute(null));

			command.Execute("240");
			await Assert.ThrowsAsync<ArgumentNullException>("parameter", () => ((AsyncCommandBase<string>)command).InternalExecuteAsync(null));
		}

		[Fact]
		public async Task AsyncCommandBase_T_NonGenericFrameworkMethodsMustBeCalledWithDataOfTypeCompatibleWithGenericTypeParameter()
		{
			ICommand command = new AsyncTestCommand<float>();

			Assert.Throws<InvalidCastException>(() => command.CanExecute(240.0));
			await Assert.ThrowsAsync<InvalidCastException>(() => ((AsyncCommandBase<float>)command).InternalExecuteAsync(240.0));
		}

		[Fact]
		public void AsyncCommandBase_FrameworkMethodsCallIntoParameterlessImplementations()
		{
			string text = null;
			ICommand command = new AsyncTestCommand(() => text = nameof(ICommand.CanExecute), () => { text = nameof(ICommand.Execute); return Task.CompletedTask; });

			bool canExecute = command.CanExecute(null);
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute(null);
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void AsyncCommandBase_T_FrameworkMethodsCallIntoStronglyTypedImplementations()
		{
			string text = null;
			ICommand command = new AsyncTestCommand<string>(t => text = t, t => { text = t; return Task.CompletedTask; });

			bool canExecute = command.CanExecute(nameof(ICommand.CanExecute));
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute(nameof(ICommand.Execute));
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void DiscourageTheUseOfWeaklyTypedMethods()
		{
			CheckThatTheWeaklyTypedCommandMethodsAreObsolete(typeof(IAsyncCommand));
			CheckThatTheWeaklyTypedCommandMethodsAreObsolete(typeof(IAsyncCommand<>));

			static void CheckThatTheWeaklyTypedCommandMethodsAreObsolete(Type type)
			{
				MethodInfo canExecute = type.GetMethod(nameof(ICommand.CanExecute), new Type[] { typeof(object) });
				MethodInfo execute = type.GetMethod(nameof(ICommand.Execute), new Type[] { typeof(object) });

				Assert.NotNull(canExecute);
				Assert.NotNull(execute);

				CheckThatMethodIsObsoleteWithError(canExecute);
				CheckThatMethodIsObsoleteWithError(execute);
			}

			static void CheckThatMethodIsObsoleteWithError(MethodInfo methodInfo)
			{
				ObsoleteAttribute attribute = methodInfo.GetCustomAttribute<ObsoleteAttribute>();

				Assert.NotNull(attribute);
				Assert.True(attribute.IsError);
			}
		}

		[Fact]
		public async Task AsyncCommandBase_EncourageTheUseOfOverloadedMethodsWhichTakeNoParameter()
		{
			string text = null;
			IAsyncCommand command = new AsyncTestCommand(() => text = nameof(IAsyncCommand.CanExecute), async () => { await Task.Yield(); text = nameof(IAsyncCommand.Execute); });

			bool canExecute = command.CanExecute();
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			await command.ExecuteAsync();
			Assert.Equal("Execute", text);
		}

		[Fact]
		public async Task AsyncCommandBase_T_EncourageTheUseOfOverloadedMethodsWhichTakeGenericParameter()
		{
			string text = null;
			IAsyncCommand<string> command = new AsyncTestCommand<string>(t => text = t, async t => { await Task.Yield(); text = t; });

			bool canExecute = command.CanExecute(nameof(IAsyncCommand<string>.CanExecute));
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			await command.ExecuteAsync(nameof(IAsyncCommand<string>.Execute));
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void AsyncCommandBase_StateOfTheCommandCanChange()
		{
			AsyncTestCommand command = new();

			Assert.True(command.IsEnabled);
			Assert.True(command.CanExecute());

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
		}

		[Fact]
		public void AsyncCommandBase_T_StateOfTheCommandCanChange()
		{
			AsyncTestCommand<string> command = new();

			Assert.True(command.IsEnabled);
			Assert.True(command.CanExecute("F0"));

			command.IsEnabled = false;
			Assert.False(command.CanExecute("F0"));
		}

		[Fact]
		public void AsyncCommandBase_RaiseTheCanExecuteChangedEventToIndicateThatTheReturnValueOfTheCanExecuteMethodHasChanged()
		{
			string text = null;
			IAsyncCommand command = new AsyncTestCommand();
			command.CanExecuteChanged += (sender, e) => text = $"{sender.GetType()} | {e.GetType()}";

			Assert.Null(text);
			command.RaiseCanExecuteChanged();
			Assert.Equal($"{typeof(AsyncTestCommand)} | {typeof(EventArgs)}", text);
		}

		[Fact]
		public void AsyncCommandBase_T_RaiseCanExecuteChangedNeedsToBeCalledWheneverCanExecuteIsExpectedToReturnADifferentValue()
		{
			string text = null;
			IAsyncCommand<string> command = new AsyncTestCommand<string>();
			command.CanExecuteChanged += (sender, e) => text = $"{sender.GetType()} | {e.GetType()}";

			Assert.Null(text);
			command.RaiseCanExecuteChanged();
			Assert.Equal($"{typeof(AsyncTestCommand<string>)} | {typeof(EventArgs)}", text);
		}
	}
}

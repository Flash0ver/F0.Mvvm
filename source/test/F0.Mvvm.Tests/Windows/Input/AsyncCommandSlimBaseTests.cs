using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using F0.Tests.Shared;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class AsyncCommandSlimBaseTests
	{
		[Fact]
		public async Task AsyncCommandSlimBase_FrameworkMethodsMustBeCalledWithoutData()
		{
			ICommand command = new AsyncTestCommandSlim();

			bool canExecute = command.CanExecute(null);
			Assert.True(canExecute);
			Assert.Throws<ArgumentException>("parameter", () => command.CanExecute(240));

			command.Execute(null);
			await Assert.ThrowsAsync<ArgumentException>("parameter", () => ((AsyncCommandSlimBase)command).InternalExecuteAsync(240).AsTask());
		}

		[Fact]
		public async Task AsyncCommandSlimBase_T_FrameworkMethodsMustBeCalledWithData_ValueType()
		{
			ICommand command = new AsyncTestCommandSlim<int>();

			bool canExecute = command.CanExecute(240);
			Assert.True(canExecute);
			Assert.Throws<NullReferenceException>(() => command.CanExecute(null));

			command.Execute(240);
			await Assert.ThrowsAsync<NullReferenceException>(() => ((AsyncCommandSlimBase<int>)command).InternalExecuteAsync(null).AsTask());
		}

		[Fact]
		public async Task AsyncCommandSlimBase_T_FrameworkMethodsMustBeCalledWithData_ReferenceType()
		{
			ICommand command = new AsyncTestCommandSlim<string>();

			bool canExecute = command.CanExecute("240");
			Assert.True(canExecute);
			Assert.Throws<ArgumentNullException>("parameter", () => command.CanExecute(null));

			command.Execute("240");
			await Assert.ThrowsAsync<ArgumentNullException>("parameter", () => ((AsyncCommandSlimBase<string>)command).InternalExecuteAsync(null).AsTask());
		}

		[Fact]
		public async Task AsyncCommandSlimBase_T_NonGenericFrameworkMethodsMustBeCalledWithDataOfTypeCompatibleWithGenericTypeParameter()
		{
			ICommand command = new AsyncTestCommandSlim<float>();

			Assert.Throws<InvalidCastException>(() => command.CanExecute(240.0));
			await Assert.ThrowsAsync<InvalidCastException>(() => ((AsyncCommandSlimBase<float>)command).InternalExecuteAsync(240.0).AsTask());
		}

		[Fact]
		public void AsyncCommandSlimBase_FrameworkMethodsCallIntoParameterlessImplementations()
		{
			string text = null;
			ICommand command = new AsyncTestCommandSlim(() => text = nameof(ICommand.CanExecute), () => { text = nameof(ICommand.Execute); return default; });

			bool canExecute = command.CanExecute(null);
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute(null);
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void AsyncCommandSlimBase_T_FrameworkMethodsCallIntoStronglyTypedImplementations()
		{
			string text = null;
			ICommand command = new AsyncTestCommandSlim<string>(t => text = t, t => { text = t; return default; });

			bool canExecute = command.CanExecute(nameof(ICommand.CanExecute));
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute(nameof(ICommand.Execute));
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void DiscourageTheUseOfWeaklyTypedMethods()
		{
			CheckThatTheWeaklyTypedCommandMethodsAreObsolete(typeof(IAsyncCommandSlim));
			CheckThatTheWeaklyTypedCommandMethodsAreObsolete(typeof(IAsyncCommandSlim<>));

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
		public async Task AsyncCommandSlimBase_EncourageTheUseOfOverloadedMethodsWhichTakeNoParameter()
		{
			string text = null;
			IAsyncCommandSlim command = new AsyncTestCommandSlim(() => text = nameof(IAsyncCommandSlim.CanExecute), async () => { await Task.Yield(); text = nameof(IAsyncCommandSlim.Execute); });

			bool canExecute = command.CanExecute();
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			await command.ExecuteAsync();
			Assert.Equal("Execute", text);
		}

		[Fact]
		public async Task AsyncCommandSlimBase_T_EncourageTheUseOfOverloadedMethodsWhichTakeGenericParameter()
		{
			string text = null;
			IAsyncCommandSlim<string> command = new AsyncTestCommandSlim<string>(t => text = t, async t => { await Task.Yield(); text = t; });

			bool canExecute = command.CanExecute(nameof(IAsyncCommandSlim<string>.CanExecute));
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			await command.ExecuteAsync(nameof(IAsyncCommandSlim<string>.Execute));
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void AsyncCommandSlimBase_StateOfTheCommandCanChange()
		{
			AsyncTestCommandSlim command = new();

			Assert.True(command.IsEnabled);
			Assert.True(command.CanExecute());

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
		}

		[Fact]
		public void AsyncCommandSlimBase_T_StateOfTheCommandCanChange()
		{
			AsyncTestCommandSlim<string> command = new();

			Assert.True(command.IsEnabled);
			Assert.True(command.CanExecute("F0"));

			command.IsEnabled = false;
			Assert.False(command.CanExecute("F0"));
		}

		[Fact]
		public void AsyncCommandSlimBase_RaiseTheCanExecuteChangedEventToIndicateThatTheReturnValueOfTheCanExecuteMethodHasChanged()
		{
			string text = null;
			IAsyncCommandSlim command = new AsyncTestCommandSlim();
			command.CanExecuteChanged += (sender, e) => text = $"{sender.GetType()} | {e.GetType()}";

			Assert.Null(text);
			command.RaiseCanExecuteChanged();
			Assert.Equal($"{typeof(AsyncTestCommandSlim)} | {typeof(EventArgs)}", text);
		}

		[Fact]
		public void AsyncCommandSlimBase_T_RaiseCanExecuteChangedNeedsToBeCalledWheneverCanExecuteIsExpectedToReturnADifferentValue()
		{
			string text = null;
			IAsyncCommandSlim<string> command = new AsyncTestCommandSlim<string>();
			command.CanExecuteChanged += (sender, e) => text = $"{sender.GetType()} | {e.GetType()}";

			Assert.Null(text);
			command.RaiseCanExecuteChanged();
			Assert.Equal($"{typeof(AsyncTestCommandSlim<string>)} | {typeof(EventArgs)}", text);
		}
	}
}

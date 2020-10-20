# F0.Mvvm
CHANGELOG

## vNext
- Added implementation of `Windows.Input.IBoundedCommand`, a parameterless and asynchronous `System.Windows.Input.ICommand` abstraction, which limits the number of concurrent operations executing.
- Added implementation of `Windows.Input.IBoundedCommand<T>`, a strongly typed and asynchronous `System.Windows.Input.ICommand` abstraction, which limits the number of concurrent operations executing.
- Added implementation of `Windows.Input.IBoundedCommandSlim`, a parameterless and allocation-free asynchronous `System.Windows.Input.ICommand` abstraction, which limits the number of concurrent operations executing.
- Added implementation of `Windows.Input.IBoundedCommandSlim<T>`, a strongly typed and allocation-free asynchronous `System.Windows.Input.ICommand` abstraction, which limits the number of concurrent operations executing.

## v0.6.0 (2019-12-31)
- Added target framework: `.NET Standard 2.1`.

## v0.5.0 (2019-10-20)
- Added implementation of `Windows.Input.IAsyncCommandSlim`, a parameterless and allocation-free asynchronous `System.Windows.Input.ICommand` abstraction.
- Added implementation of `Windows.Input.IAsyncCommandSlim<T>`, a strongly typed and allocation-free asynchronous `System.Windows.Input.ICommand` abstraction.

## v0.4.0 (2019-07-31)
- Added implementation of `Windows.Input.IAsyncCommand`, a parameterless and asynchronous `System.Windows.Input.ICommand` abstraction.
- Added implementation of `Windows.Input.IAsyncCommand<T>`, a strongly typed and asynchronous `System.Windows.Input.ICommand` abstraction.

## v0.3.0 (2019-04-30)
- Changed target framework from `.NET Standard 1.0` to `.NET Standard 2.0`.

## v0.2.0 (2018-12-21)
- Added implementation of `Windows.Input.IInputCommand`, a parameterless `System.Windows.Input.ICommand` abstraction.
- Added implementation of `Windows.Input.IInputCommand<T>`, a strongly typed `System.Windows.Input.ICommand` abstraction.

## v0.1.0 (2018-09-18)
- Added `ComponentModel.ViewModel` implementing Property Change Notification.

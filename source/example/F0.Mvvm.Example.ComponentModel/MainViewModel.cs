using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using F0.ComponentModel;

namespace F0.Mvvm.Example.ComponentModel
{
	internal class MainViewModel : ViewModel, IDisposable
	{
		public string UpTime { get; private set; }

		public IReadOnlyList<BindingSource<int, TimeSpan>> TimeIntervalComponents { get; }

		private readonly Stopwatch stopwatch;
		private readonly Timer timer;

		public MainViewModel()
		{
			TimeIntervalComponents = new ObservableCollection<BindingSource<int, TimeSpan>>()
			{
				new BindingSource<int, TimeSpan>("Milliseconds", time => time.Milliseconds),
				new BindingSource<int, TimeSpan>("Seconds", time => time.Seconds),
				new BindingSource<int, TimeSpan>("Minutes", time => time.Minutes),
				new BindingSource<int, TimeSpan>("Hours", time => time.Hours),
				new BindingSource<int, TimeSpan>("Days", time => time.Days)
			};
			stopwatch = Stopwatch.StartNew();
			timer = new Timer(OnTimerCallback, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
		}

		private void OnTimerCallback(object state)
		{
			TimeSpan elapsed = stopwatch.Elapsed;

			foreach (BindingSource<int, TimeSpan> timeIntervalComponent in TimeIntervalComponents)
			{
				timeIntervalComponent.Mutate(elapsed);
			}

			UpTime = elapsed.ToString(@"d\:hh\:mm\:ss");
			RaisePropertyChangedEvent(() => UpTime);
		}

		void IDisposable.Dispose()
		{
			timer.Dispose();
			stopwatch.Stop();
		}
	}
}

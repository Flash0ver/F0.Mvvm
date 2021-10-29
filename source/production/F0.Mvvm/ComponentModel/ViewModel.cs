using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace F0.ComponentModel
{
	public abstract class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected ViewModel()
		{
		}

		protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(backingField, value))
			{
				backingField = value;
				RaisePropertyChangedEvent(propertyName);
			}
		}

		protected bool TrySetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(backingField, value))
			{
				return false;
			}
			else
			{
				backingField = value;
				RaisePropertyChangedEvent(propertyName);
				return true;
			}
		}

		protected virtual void RaisePropertyChangedEvent<T>(Expression<Func<T>> propertyExpression)
		{
			string propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
			RaisePropertyChangedEvent(propertyName);
		}

		private void RaisePropertyChangedEvent(string? propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

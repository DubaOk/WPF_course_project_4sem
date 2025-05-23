﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Coach_search.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Cleanup()
        {
            // Базовая реализация пуста, может быть переопределена в наследниках
        }
    }
}
﻿using System;
using Yaga.Utils;

namespace Yaga.Binding.Observable
{
    internal class ConvertedObservable<T> : Utils.IObservable<T>
    {
        private readonly Func<T> _dataAccessor;

        public ConvertedObservable(Func<T> dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }

        private event Action<T> OnDataChange;
        public void Perform(T data) => OnDataChange?.Invoke(data);

        public T Data => _dataAccessor();

        public IDisposable Subscribe(Action<T> action)
        {
            OnDataChange += action;
            return new Reflector(() => OnDataChange -= action);
        }
    }
}
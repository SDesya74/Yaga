﻿using System;

namespace Yaga.Binding.Observable
{
    public class BindObservable<T>
    {
        protected readonly Binding.BindingContext Context;
        private readonly Utils.IObservable<T> _observable;
        private readonly Func<T> _dataAccessor;
        protected Action OnDispose;

        public T Data => _dataAccessor();

        public BindObservable(Binding.BindingContext context, Utils.IObservable<T> observable, Action onDispose = default)
        {
            Context = context;
            _observable = observable;
            OnDispose = onDispose;
            _dataAccessor = () => observable.Data;
        }

        public IBindAccessor To(IView<T> view)
        {
            var accessor = new BindAccessor(() =>
            {
                if (!view.IsInstanced)
                    view.Create();

                if (!view.IsOpened)
                    view.Open();

                UiBootstrap.Instance.Set(view, _dataAccessor());
            }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        private ConvertedObservable<T1> GetConverted<T1>(Func<T, T1> converter)
        {
            var converted = new ConvertedObservable<T1>(() => converter(_dataAccessor()));
            var reflector = _observable.Subscribe(e => converted.Perform(converter(e)));
            OnDispose += reflector.Dispose;
            return converted;
        }

        public BindObservable<T1> As<T1>(Func<T, T1> converter) 
            => new BindObservable<T1>(Context, GetConverted(converter));
        
        public BindStringObservable As(Func<T, string> converter) => 
            new BindStringObservable(Context, GetConverted(converter));
    }
}
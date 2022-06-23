namespace AsyncAE
{
    using Internal;

    /// <summary>
    /// Class for handling effect
    /// </summary>
    public sealed class EffectHandler : IDisposable, IAsyncDisposable
    {
        private readonly AlgebraicContext _holder;

        private readonly Dictionary<Type, Delegate> _funcMap = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, Delegate> _asyncFuncMap = new Dictionary<Type, Delegate>();

        public EffectHandler()
        {
            _holder = AlgebraicEffectsManager
                    .Instance.Value.GetContextHolder();
            _holder.AddHandler(this);
        }

        public bool HasHandler(Type type, bool enableAsync)
        {
            if (_funcMap.Keys
                .Any(t => t.IsAssignableFrom(type)))
            {
                return true;
            }
            return enableAsync &&
                   _asyncFuncMap.Keys
                       .Any(t => t.IsAssignableFrom(type));
        }

        public EffectHandler Handle<T, TRet>(Func<T, TRet> handler)
            where T : Perform<T, TRet>
        {
            _funcMap.Add(typeof(T), handler);
            return this;
        }

        public EffectHandler HandleAsync<T, TRet>(Func<T, ValueTask<TRet>> asyncHandler)
            where T : Perform<T, TRet>
        {
            _asyncFuncMap.Add(typeof(T), asyncHandler);
            return this;
        }

        internal Func<T, TRet>? TryGetHandler<T, TRet>()
            where T : Perform<T, TRet>
        {
            var type = typeof(T);
            if (_funcMap
                    .FirstOrDefault(
                        (pair) => pair.Key.IsAssignableFrom(type)
                                  && pair.Value is Func<T, TRet>)
                    .Value is Func<T, TRet> handler)
            {
                return handler;
            }
            return default;
        }

        internal Func<T, ValueTask<TRet>>? TryGetHandlerAsync<T, TRet>()
            where T : Perform<T, TRet>
        {
            var type = typeof(T);
            if (_asyncFuncMap
                    .FirstOrDefault(
                        (pair) => pair.Key.IsAssignableFrom(type)
                                  && pair.Value is Func<T, ValueTask<TRet>>)
                    .Value is Func<T, ValueTask<TRet>> handler)
            {
                return handler;
            }
            return default;
        }

        void IDisposable.Dispose()
        {
            DisposeInternal();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            // TODO: 非同期実行中の後始末
            // await this._holder.Wait();
            DisposeInternal();
        }

        private void DisposeInternal()
        {
            _holder.RemoveHandler(this);
            _asyncFuncMap.Clear();
            _funcMap.Clear();
        }
    }
}
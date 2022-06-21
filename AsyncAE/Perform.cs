using System.Diagnostics;

namespace AsyncAE
{
    using AsyncAE.Internal;

    /// <summary>
    /// Abstract class for provides perform logic.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRet"></typeparam>
    public abstract class Perform<T, TRet>
           where T : Perform<T, TRet>
    {
        /// <summary>
        /// Retrieves message from perform.
        /// </summary>
        /// <value>Any type object.</value>
        public object? Message { get; }

        private readonly AlgebraicContext _holder;

        private readonly Func<Exception> _exceptionFactory;

        protected Perform(object? param, Func<Exception>? exceptionFactory = null)
        {
            Message = param;
            _exceptionFactory = exceptionFactory
                    ?? new Func<Exception>(() => new NotImplementedException($"{GetType().Name} is Uncaught."));
            _holder = AlgebraicEffectsManager
                .Instance.Value.GetContextHolder();
        }

        public static implicit operator TRet(Perform<T, TRet> it) => it.Do();

        public static implicit operator ValueTask<TRet>(Perform<T, TRet> it) => it.DoAsync();

        private TRet Do()
        {
            var effect = _holder.GetHandler(typeof(T), false);
            if (effect == null)
            {
                throw _exceptionFactory();
            }
            var perform = (this as T)!;
            var handler = effect.TryGetHandler<T, TRet>();
            Debug.Assert(handler != null);
            return handler.Invoke(perform);
        }

        private ValueTask<TRet> DoAsync()
        {
            var effect = _holder.GetHandler(typeof(T), true);
            if (effect == null)
            {
                throw _exceptionFactory();
            }
            var perform = (this as T)!;
            var handler = effect.TryGetHandlerAsync<T, TRet>();
            Debug.Assert(handler != null);
            return handler.Invoke(perform);
        }
    }
}
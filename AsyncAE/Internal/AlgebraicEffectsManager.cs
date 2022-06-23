namespace AsyncAE.Internal
{
    using System.Diagnostics;

    /// <summary>
    /// Manager class for AlgebraicEffects.
    /// </summary>
    internal sealed class AlgebraicEffectsManager
    {
        /// <summary>
        /// Context class which contains corresponded handlers.
        /// </summary>
        internal sealed class CallContext
        {
            private readonly Stack<EffectHandler> _stack
                = new Stack<EffectHandler>();

            public EffectHandler? GetHandler(Type type, bool enableAsync)
            {
                return _stack
                    .FirstOrDefault((handler) => handler.HasHandler(type, enableAsync));
            }

            public void AddHandler(EffectHandler handler)
            {
                _stack.Push(handler);
            }

            public void RemoveHandler(EffectHandler handler)
            {
                if (_stack.TryPop(out var h))
                {
                    Debug.Assert(h == handler);
                }
            }
        }

        public static Lazy<AlgebraicEffectsManager> Instance { get; }

        private readonly AsyncLocal<CallContext> _holder = new AsyncLocal<CallContext>();

        static AlgebraicEffectsManager()
        {
            Instance = new Lazy<AlgebraicEffectsManager>(
                () => new AlgebraicEffectsManager(), isThreadSafe: true
            );
        }

        /// <summary>
        /// Gets or creates an call context.
        /// </summary>
        /// <returns>ContextHolder</returns>
        internal CallContext GetContext()
        {
            lock (this)
            {
                _holder.Value ??= new CallContext();
                return _holder.Value;
            }
        }
    }
}
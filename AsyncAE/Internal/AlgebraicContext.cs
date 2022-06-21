namespace AsyncAE.Internal
{
    using System.Diagnostics;

    /// <summary>
    /// Context class which contains corresponded handlers.
    /// </summary>
    internal sealed class AlgebraicContext
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
}
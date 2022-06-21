namespace AsyncAE.Internal
{
    /// <summary>
    /// Manager class for AlgebraicEffects.
    /// </summary>
    internal sealed class AlgebraicEffectsManager
    {
        public static Lazy<AlgebraicEffectsManager> Instance { get; }

        private readonly AsyncLocal<AlgebraicContext> _holder = new AsyncLocal<AlgebraicContext>();

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
        internal AlgebraicContext GetContextHolder()
        {
            lock (this)
            {
                _holder.Value ??= new AlgebraicContext();
                return _holder.Value;
            }
        }
    }
}
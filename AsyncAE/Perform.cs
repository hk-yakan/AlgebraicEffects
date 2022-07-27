// -----------------------------------------------------------------------------
// Copyright (c) yakan_k 2022-2022.  All Rights Reserved.
// Licensed under the MIT license.
// See License.txt in the project root for license information.
// -----------------------------------------------------------------------------
// PROJECT : AsyncAE
// FILE : Perform.cs

namespace AsyncAE
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Internal;

    /// <summary>
    ///     Abstract class for provides perform logic.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRet"></typeparam>
    public abstract class Perform<T, TRet>
        where T : Perform<T, TRet>
    {
        private readonly AlgebraicEffectsManager.CallContext _context;

        private readonly Func<Exception> _exceptionFactory;

        protected Perform(object? param, Func<Exception>? exceptionFactory = null)
        {
            Message = param;
            _exceptionFactory = exceptionFactory
                                ?? new Func<Exception>(() =>
                                    new NotImplementedException($"{GetType().Name} is Uncaught."));
            _context = AlgebraicEffectsManager
                .Instance.Value.GetContext();
        }

        /// <summary>
        ///     Retrieves message from perform.
        /// </summary>
        /// <value>Any type object.</value>
        public object? Message { get; }

        public static implicit operator TRet(Perform<T, TRet> it)
        {
            return it.Do();
        }

        public static implicit operator ValueTask<TRet>(Perform<T, TRet> it)
        {
            return it.DoAsync();
        }

        private TRet Do()
        {
            var effect = _context.GetHandler(typeof(T), false);
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
            var effect = _context.GetHandler(typeof(T), true);
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
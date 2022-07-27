// -----------------------------------------------------------------------------
// Copyright (c) yakan_k 2022-2022.  All Rights Reserved.
// Licensed under the MIT license.
// See License.txt in the project root for license information.
// -----------------------------------------------------------------------------
// PROJECT : AsyncAE.Tests
// FILE : PerformAsyncTest.cs

namespace AsyncAE.Tests
{
    using System;
    using System.Threading.Tasks;

    public class PerformAsyncTest
    {
        private static string CallStrPerformFunc()
        {
            return new StrPerform(null);
        }

        private static int CallIntPerformFunc()
        {
            return new IntPerform(null);
        }

        private static ValueTask<string> CallStrPerformFuncAsync()
        {
            return new StrPerform(null);
        }

        private static ValueTask<int> CallIntPerformFuncAsync()
        {
            return new IntPerform(null);
        }

        [Fact]
        public async void HandleStrPerformAsync()
        {
            const string expected = "Call it!";
            await using (new EffectHandler()
                             .Handle((StrPerform _) => expected))
            {
                await Task.Delay(10);
                var ret = CallStrPerformFunc();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public async void HandleIntPerformAsync()
        {
            const int expected = 100;
            await using (new EffectHandler()
                             .Handle((IntPerform _) => expected))
            {
                await Task.Delay(10);
                var ret = CallIntPerformFunc();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public async void HandleStrPerformAsyncWithAsyncHandler()
        {
            const string expected = "Call it!";
            await using (new EffectHandler()
                             .HandleAsync(async (StrPerform _) =>
                             {
                                 await Task.Delay(10);
                                 return expected;
                             }))
            {
                await Task.Delay(10);
                var ret = await CallStrPerformFuncAsync();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public async void HandlePerformAsyncOnParallel()
        {
            const string expected1 = "Call it on thread 1";
            const string expected2 = "Call it on thread 2";
            async Task<string> Thread1()
            {
                await using (new EffectHandler().HandleAsync(async (StrPerform _) =>
                             {
                                 await Task.Delay(10);
                                 return expected1;
                             }))
                {
                    await Task.Delay(10);
                    return await CallStrPerformFuncAsync();
                }
            }

            async Task<string> Thread2()
            {
                await using (new EffectHandler().HandleAsync(async (StrPerform _) =>
                             {
                                 await Task.Delay(10);
                                 return expected2;
                             }))
                {
                    await Task.Delay(10);
                    return await CallStrPerformFuncAsync();
                }
            }

            var results = await Task.WhenAll(Task.Run(Thread1), Task.Run(Thread2));
            Assert.Equal(expected1, results[0]);
            Assert.Equal(expected2, results[1]);
        }

        [Fact]
        public async void HandlePerformAsyncOnParallelThrowException()
        {
            ValueTask<int> NoCall1(IntPerform _) => throw new NotSupportedException();
            ValueTask<string> NoCall2(StrPerform _) => throw new NotSupportedException();

            async Task<Exception> Thread1()
            {
                await using (new EffectHandler().HandleAsync(
                                 (Func<IntPerform, ValueTask<int>>)NoCall1))
                {
                    await Task.Delay(10);
                    return Assert.Throws<NotImplementedException>(() => CallStrPerformFuncAsync());
                }
            }

            async Task<Exception> Thread2()
            {
                await using (new EffectHandler().HandleAsync(
                                 (Func<StrPerform, ValueTask<string>>)NoCall2))
                {
                    await Task.Delay(10);
                    return Assert.Throws<NotImplementedException>(() => CallIntPerformFuncAsync());
                }
            }

            var messages = await Task.WhenAll(
                Task.Run(Thread1),
                Task.Run(Thread2)
            );
            Assert.Equal("StrPerform is Uncaught.", messages[0].Message);
            Assert.Equal("IntPerform is Uncaught.", messages[1].Message);
        }
    }
}
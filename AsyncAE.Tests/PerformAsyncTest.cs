namespace AsyncAE.Tests;


using System.Diagnostics;

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
        const string Expected = "Call it!";
        await using (new EffectHandler()
                .Handle((StrPerform p) => Expected))
        {
            await Task.Delay(10);
            var ret = CallStrPerformFunc();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public async void HandleIntPerformAsync()
    {
        const int Expected = 100;
        await using (new EffectHandler()
                .Handle((IntPerform p) => Expected))
        {
            await Task.Delay(10);
            var ret = CallIntPerformFunc();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public async void HandleStrPerformAsyncWithAsyncHandler()
    {
        const string Expected = "Call it!";
        await using (new EffectHandler()
                .HandleAsync(async (StrPerform p) =>
                {
                    await Task.Delay(10);
                    return Expected;
                }))
        {
            await Task.Delay(10);
            var ret = await CallStrPerformFuncAsync();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public async void HandlePerformAsyncOnParallel()
    {
        const string Expected1 = "Call it on thread 1";
        const string Expected2 = "Call it on thread 2";
        async Task<string> Thread1()
        {
            await using (new EffectHandler().HandleAsync(async (StrPerform p) =>
                         {
                             await Task.Delay(10);
                             return Expected1;
                         }))
            {
                await Task.Delay(10);
                return await CallStrPerformFuncAsync();
            }
        }

        async Task<string> Thread2()
        {
            await using (new EffectHandler().HandleAsync(async (StrPerform p) =>
                         {
                             await Task.Delay(10);
                             return Expected2;
                         }))
            {
                await Task.Delay(10);
                return await CallStrPerformFuncAsync();
            }
        }

        var results = await Task.WhenAll(Task.Run(Thread1), Task.Run(Thread2));
        Assert.Equal(Expected1, results[0]);
        Assert.Equal(Expected2, results[1]);
    }

    [Fact]
    public async void HandlePerformAsyncOnParallelThrowException()
    {
        async Task<Exception> Thread1()
        {
            Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
            const int Unexpected = 10;
            await using (new EffectHandler().HandleAsync(async (IntPerform p) =>
                         {
                             await Task.Delay(10);
                             throw new NotSupportedException();
                             return Unexpected;
                         }))
            {
                await Task.Delay(10);
                return Assert.Throws<NotImplementedException>(() => CallStrPerformFuncAsync());
            }
        }

        async Task<Exception> Thread2()
        {
            Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
            const string Unexpected = "Call it on thread 2";
            await using (new EffectHandler().HandleAsync(async (StrPerform p) =>
                         {
                             await Task.Delay(10);
                             throw new NotSupportedException();
                             return Unexpected;
                         }))
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
namespace AsyncAE.Tests;


public class PerformTest
{
    private static string CallStrPerformFunc()
    {
        return new StrPerform(null);
    }

    private static int CallIntPerformFunc()
    {
        return new IntPerform(null);
    }

    [Fact]
    public void CreateStrPerformAndThrowException()
    {
        void Action() => CallStrPerformFunc();

        var exception = Assert.Throws<NotImplementedException>((Action)Action);
        Assert.Equal("StrPerform is Uncaught.", exception.Message);
    }

    [Fact]
    public void CreateEffectHandler()
    {
        var handler = new EffectHandler();
    }

    [Fact]
    public void HandleEffectHandler()
    {
        var handler = new EffectHandler();

        var ret = handler.Handle((StrPerform p) => "");
        Assert.Equal(handler, ret);
    }

    [Fact]
    public void HandleStrPerform()
    {
        const string Expected = "Call it!";
        using (new EffectHandler()
                .Handle((StrPerform p) => Expected))
        {
            var ret = CallStrPerformFunc();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleIntPerform()
    {
        const string Unexpected = "Unexpected!";
        const int Expected = 100;
        using (new EffectHandler()
                .Handle((StrPerform p) => Unexpected)
                .Handle((IntPerform p) => Expected))
        {
            var ret = CallIntPerformFunc();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandlePerformStackInside()
    {
        const string Unexpected = "Unexpected!";
        var isCalled = false;
        using (new EffectHandler()
                .Handle((StrPerform p) =>
                {
                    isCalled = true;
                    return Unexpected;
                }))
        {
            HandleStrPerform();
        }
        Assert.False(isCalled, "Outside Handler is called.");
    }

    [Fact]
    public void HandlePerformStackOutside()
    {
        const int Unexpected = 100;
        string InsideFunc()
        {
            using (new EffectHandler()
                .Handle((IntPerform p) => Unexpected))
            {
                return CallStrPerformFunc();
            }
        }

        const string Expected = "Call It!";
        using (new EffectHandler()
                .Handle((StrPerform p) => Expected))
        {
            var ret = InsideFunc();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void NoHandlePerformWithException()
    {
        const string Unexpected = "Unexpected!";
        using (new EffectHandler()
                .Handle((StrPerform p) => Unexpected))
        {
            void Action() => CallIntPerformFunc();
            var exception = Assert.Throws<NotImplementedException>((Action)Action);
            Assert.Equal("IntPerform is Uncaught.", exception.Message);

        }
    }
}
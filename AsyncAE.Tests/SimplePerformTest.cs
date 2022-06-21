namespace AsyncAE.Tests;


public class SimplePerformTest
{
    [Fact]
    public void HandleStrPerform()
    {
        string Func() => new StrPerform();
        const string Expected = "Call it!";
        using (new EffectHandler()
                .Handle((StrPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleStrPerformWithNull()
    {
        string Func() => new StrPerform(null);
        const string Expected = "Call it!";
        using (new EffectHandler()
                   .Handle((StrPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleIntPerform()
    {
        int Func() => new IntPerform();
        const int Expected = 10;
        using (new EffectHandler()
                   .Handle((IntPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleIntPerformWithNull()
    {
        int Func() => new IntPerform(null);
        const int Expected = 10;
        using (new EffectHandler()
                   .Handle((IntPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleFloatPerform()
    {
        float Func() => new FloatPerform();
        const float Expected = 10.0f;
        using (new EffectHandler()
                   .Handle((FloatPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleFloatPerformWithNull()
    {
        float Func() => new FloatPerform(null);
        const float Expected = 10.0f;
        using (new EffectHandler()
                   .Handle((FloatPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleBoolPerform()
    {
        bool Func() => new BoolPerform();
        const bool Expected = true;
        using (new EffectHandler()
                   .Handle((BoolPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }

    [Fact]
    public void HandleBoolPerformWithNull()
    {
        bool Func() => new BoolPerform(null);
        const bool Expected = true;
        using (new EffectHandler()
                   .Handle((BoolPerform p) => Expected))
        {
            var ret = Func();
            Assert.Equal(Expected, ret);
        }
    }
}
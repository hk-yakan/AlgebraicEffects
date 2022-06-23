namespace AsyncAE.Tests
{
    public class SimplePerformTest
    {
        [Fact]
        public void HandleStrPerform()
        {
            string Func() => new StrPerform();
            const string expected = "Call it!";
            using (new EffectHandler()
                       .Handle((StrPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleStrPerformWithNull()
        {
            string Func() => new StrPerform(null);
            const string expected = "Call it!";
            using (new EffectHandler()
                       .Handle((StrPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleIntPerform()
        {
            int Func() => new IntPerform();
            const int expected = 10;
            using (new EffectHandler()
                       .Handle((IntPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleIntPerformWithNull()
        {
            int Func() => new IntPerform(null);
            const int expected = 10;
            using (new EffectHandler()
                       .Handle((IntPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleFloatPerform()
        {
            float Func() => new FloatPerform();
            const float expected = 10.0f;
            using (new EffectHandler()
                       .Handle((FloatPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleFloatPerformWithNull()
        {
            float Func() => new FloatPerform(null);
            const float expected = 10.0f;
            using (new EffectHandler()
                       .Handle((FloatPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleBoolPerform()
        {
            bool Func() => new BoolPerform();
            const bool expected = true;
            using (new EffectHandler()
                       .Handle((BoolPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleBoolPerformWithNull()
        {
            bool Func() => new BoolPerform(null);
            const bool expected = true;
            using (new EffectHandler()
                       .Handle((BoolPerform _) => expected))
            {
                var ret = Func();
                Assert.Equal(expected, ret);
            }
        }
    }
}
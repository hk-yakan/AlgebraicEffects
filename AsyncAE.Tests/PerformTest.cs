namespace AsyncAE.Tests
{
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
            Assert.NotNull(handler);
        }

        [Fact]
        public void HandleEffectHandler()
        {
            var handler = new EffectHandler();

            var ret = handler.Handle((StrPerform _) => "");
            Assert.Equal(handler, ret);
        }

        [Fact]
        public void HandleStrPerform()
        {
            const string expected = "Call it!";
            using (new EffectHandler()
                       .Handle((StrPerform _) => expected))
            {
                var ret = CallStrPerformFunc();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleStrPerformWithMessage()
        {
            const string expected = "Call it!";
            string PerformFunc() => new StrPerform(expected);

            using (new EffectHandler()
                       .Handle<StrPerform, string>(p => (p.Message as string)!))
            {
                var ret = PerformFunc();
                Assert.Equal(expected, ret);
            }
        }


        [Fact]
        public void HandleIntPerform()
        {
            const string unexpected = "Unexpected!";
            const int expected = 100;
            using (new EffectHandler()
                       .Handle((StrPerform _) => unexpected)
                       .Handle((IntPerform _) => expected))
            {
                var ret = CallIntPerformFunc();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void HandleNestedPerform()
        {
            const int unexpected = 200;
            const int expected = 100;
            using (new EffectHandler()
                       .Handle((IntPerform _) => unexpected))
            {
                using (new EffectHandler()
                           .Handle((IntPerform _) => expected))
                {
                    var ret = CallIntPerformFunc();
                    Assert.Equal(expected, ret);
                }
            }
        }

        [Fact]
        public void HandlePerformStackInside()
        {
            int NoCall(IntPerform _) => throw new NotSupportedException();
            using (new EffectHandler().Handle<IntPerform, int>(NoCall))
            {
                HandleStrPerform();
            }
        }

        [Fact]
        public void HandlePerformStackOutside()
        {
            int NoCall(IntPerform _) => throw new NotSupportedException();
            string InsideFunc()
            {
                using (new EffectHandler()
                           .Handle<IntPerform, int>(NoCall))
                {
                    return CallStrPerformFunc();
                }
            }

            const string expected = "Call It!";
            using (new EffectHandler()
                       .Handle((StrPerform _) => expected))
            {
                var ret = InsideFunc();
                Assert.Equal(expected, ret);
            }
        }

        [Fact]
        public void NoHandlePerformWithException()
        {
            const string unexpected = "Unexpected!";
            using (new EffectHandler()
                       .Handle((StrPerform _) => unexpected))
            {
                void Action() => CallIntPerformFunc();
                var exception = Assert.Throws<NotImplementedException>((Action)Action);
                Assert.Equal("IntPerform is Uncaught.", exception.Message);

            }
        }

        [Fact]
        public void HandlePerformRemove()
        {
            const int expected = 100;
            using (new EffectHandler()
                       .Handle((IntPerform _) => expected))
            {
                var ret = CallIntPerformFunc();
                Assert.Equal(expected, ret);
            }

            using (new EffectHandler())
            {
                void Action() => CallIntPerformFunc();
                var exception = Assert.Throws<NotImplementedException>((Action)Action);
                Assert.Equal("IntPerform is Uncaught.", exception.Message);
            }
        }
    }
}
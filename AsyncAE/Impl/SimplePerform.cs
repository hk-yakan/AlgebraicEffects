namespace AsyncAE.Impl
{
    public class StrPerform : Perform<StrPerform, string>
    {
        public StrPerform(object? param) : base(param) { }
        public StrPerform() : this(null) { }
    }

    public class BoolPerform : Perform<BoolPerform, bool>
    {
        public BoolPerform(object? param) : base(param) { }
        public BoolPerform() : this(null) { }
    }

    public class IntPerform : Perform<IntPerform, int>
    {
        public IntPerform(object? param) : base(param) { }
        public IntPerform() : this(null) { }
    }

    public class FloatPerform : Perform<FloatPerform, float>
    {
        public FloatPerform(object? param) : base(param) { }
        public FloatPerform() : this(null) { }
    }
}
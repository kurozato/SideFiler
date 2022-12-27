namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class AssertExtension
    {
        public static void Is<T>(this T actual, T expect, string message = "")
            => Assert.AreEqual(actual, expect, message);

        public static void IsNull<T>(this T value, string message = "")
            => Assert.IsNull(value, message);

        public static void IsNotNull<T>(this T value, string message = "")
            => Assert.IsNotNull(value, message);

        public static void IsInstanceOf<TExpect>(this object actual, string message = "")
            => Assert.IsInstanceOfType(actual, typeof(TExpect), message);
    }
}
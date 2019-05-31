using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using C3;

namespace C3UnitTests
{
    /// <summary>
    /// Tests for utility functions.
    /// Different from TestUtils, which are utilities for tests!
    /// </summary>
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void TestContainsDuplicates()
        {
            Assert.IsFalse(new string[] { "cat", "dog", "mouse" }.ContainsDuplicates());
            Assert.IsTrue(new string[] { "cat", "dog", "mouse", "dog" }.ContainsDuplicates());

            Assert.IsFalse(new int[] {3, 5, 7 }.ContainsDuplicates());
            Assert.IsTrue(new int[] { 3, 3, 5, 7 }.ContainsDuplicates());

            Assert.IsFalse(new Tuple<string, int>[] { Tuple.Create("cat", 7), Tuple.Create("dog", 5) }.ContainsDuplicates());
            Assert.IsTrue(new Tuple<string, int>[] { Tuple.Create("cat", 7), Tuple.Create("dog", 5), Tuple.Create("mouse", -1), Tuple.Create("cat", 7) }.ContainsDuplicates());
        }
    }
}

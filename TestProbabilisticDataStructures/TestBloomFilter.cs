﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProbabilisticDataStructures;
using System.Text;
using System.Collections.Generic;

namespace TestProbabilisticDataStructures
{
    [TestClass]
    public class TestBloomFilter
    {
        /// <summary>
        /// Ensures that Capacity() returns the number of bits, m, in the Bloom filter.
        /// </summary>
        [TestMethod]
        public void TestBloomCapacity()
        {
            var f = new BloomFilter(100, 0.1);
            var capacity = f.Capacity();

            Assert.AreEqual(480u, capacity);
        }

        /// <summary>
        /// Ensures that K() returns the number of hash functions in the Bloom Filter.
        /// </summary>
        [TestMethod]
        public void TestBloomK()
        {
            var f = new BloomFilter(100, 0.1);
            var k = f.K();

            Assert.AreEqual(4u, k);
        }

        /// <summary>
        /// Ensures that Count returns the number of items added to the filter.
        /// </summary>
        [TestMethod]
        public void TestBloomCount()
        {
            var f = new BloomFilter(100, 0.1);
            for (uint i = 0; i < 10; i++)
            {
                f.Add(Encoding.ASCII.GetBytes(i.ToString()));
            }

            var count = f.Count;
            Assert.AreEqual(10u, count);
        }

        /// <summary>
        /// Ensures that EstimatedFillRatio returns the correct approximation.
        /// </summary>
        [TestMethod]
        public void TestBloomEstimatedFillRatio()
        {
            var f = new BloomFilter(100, 0.5);
            for (uint i = 0; i < 100; i++)
            {
                f.Add(Encoding.ASCII.GetBytes(i.ToString()));
            }

            var ratio = f.EstimatedFillRatio();
            if (ratio > 0.5)
            {
                Assert.Fail("Expected less than or equal to 0.5, got {0}", ratio);
            }
        }

        /// <summary>
        /// Ensures that FillRatio returns the ratio of set bits.
        /// </summary>
        [TestMethod]
        public void TestBloomFillRatio()
        {
            var f = new BloomFilter(100, 0.1);
            f.Add(Encoding.ASCII.GetBytes("a"));
            f.Add(Encoding.ASCII.GetBytes("b"));
            f.Add(Encoding.ASCII.GetBytes("c"));

            var ratio = f.FillRatio();
            Assert.AreEqual(0.025, ratio);
        }

        /// <summary>
        /// Ensures that Test, Add, and TestAndAdd behave correctly.
        /// </summary>
        [TestMethod]
        public void TestBloomTestAndAdd()
        {
            var f = new BloomFilter(100, 0.01);

            // 'a' is not in the filter.
            if (f.Test(Encoding.ASCII.GetBytes("a")))
            {
                Assert.Fail("'a' should not be a member");
            }

            var addedF = f.Add(Encoding.ASCII.GetBytes("a"));
            Assert.AreSame(f, addedF, "Returned BloomFilter should be the same instance");

            // 'a' is now in the filter.
            if (!f.Test(Encoding.ASCII.GetBytes("a")))
            {
                Assert.Fail("'a' should be a member");
            }

            // 'b' is not in the filter.
            if (f.TestAndAdd(Encoding.ASCII.GetBytes("b")))
            {
                Assert.Fail("'b' should not be a member");
            }

            // 'a' is still in the filter.
            if (!f.Test(Encoding.ASCII.GetBytes("a")))
            {
                Assert.Fail("'a' should be a member");
            }

            // 'b' is now in the filter.
            if (!f.Test(Encoding.ASCII.GetBytes("b")))
            {
                Assert.Fail("'b' should be a member");
            }

            // 'c' is not in the filter.
            if (f.Test(Encoding.ASCII.GetBytes("c")))
            {
                Assert.Fail("'c' should not be a member");
            }

            for (int i = 0; i < 1000000; i++)
            {
                f.TestAndAdd(Encoding.ASCII.GetBytes(i.ToString()));
            }

            // 'x' should be a false positive.
            if (!f.Test(Encoding.ASCII.GetBytes("x")))
            {
                Assert.Fail("'x' should be a member");
            }
        }

        /// <summary>
        /// Ensures that Reset sets every bit to zero.
        /// </summary>
        [TestMethod]
        public void TestBloomReset()
        {
            var f = new BloomFilter(100, 0.1);
            for (int i = 0; i < 1000; i++)
            {
                f.Add(Encoding.ASCII.GetBytes(i.ToString()));
            }

            var resetF = f.Reset();
            Assert.AreSame(f, resetF, "Returned BloomFilter should be the same instance");

            for (uint i = 0; i < f.Buckets.Count; i++)
            {
                if (f.Buckets.Get(i) != 0)
                {
                    Assert.Fail("Expected all bits to be unset");
                }
            }
        }

        [TestMethod]
        public void BenchmarkBloomAdd()
        {
            var n = 100000;
            var f = new BloomFilter(100000, 0.1);
            var data = new byte[n][];
            //var data = new byte[(count * bucketSize + 7) / 8];
            for (int i = 0; i < n; i++)
            {
                data[i] = Encoding.ASCII.GetBytes(i.ToString());
            }

            for (int i = 0; i < n; i++)
            {
                f.Add(data[i]);
            }
        }

        [TestMethod]
        public void BenchmarkBloomTest()
        {
            var n = 100000;
            var f = new BloomFilter(100000, 0.1);
            var data = new byte[n][];
            //var data = new byte[(count * bucketSize + 7) / 8];
            for (int i = 0; i < n; i++)
            {
                data[i] = Encoding.ASCII.GetBytes(i.ToString());
            }

            for (int i = 0; i < n; i++)
            {
                f.Test(data[i]);
            }
        }

        [TestMethod]
        public void BenchmarkBloomTestAndAdd()
        {
            var n = 100000;
            var f = new BloomFilter(100000, 0.1);
            var data = new byte[n][];
            //var data = new byte[(count * bucketSize + 7) / 8];
            for (int i = 0; i < n; i++)
            {
                data[i] = Encoding.ASCII.GetBytes(i.ToString());
            }

            for (int i = 0; i < n; i++)
            {
                f.TestAndAdd(data[i]);
            }
        }
    }
}
﻿using System;
using System.Linq;
using System.Security.Cryptography;

namespace ProbabilisticDataStructures
{
    public static class Utils
    {
        /// <summary>
        /// Calculates the optimal Bloom filter size, m, based on the number of items and
        /// the desired rate of false positives.
        /// </summary>
        /// <param name="n">Number of items.</param>
        /// <param name="fpRate">Desired false positive rate.</param>
        /// <returns>The optimal BloomFilter size, m.</returns>
        public static uint OptimalM(uint n, double fpRate)
        {
            var optimalM = Math.Ceiling((double)n / ((Math.Log(Defaults.FILL_RATIO) *
                Math.Log(1 - Defaults.FILL_RATIO)) / Math.Abs(Math.Log(fpRate))));
            return Convert.ToUInt32(optimalM);
        }

        /// <summary>
        /// Calculates the optimal Bloom filter size, m, based on the number of items and
        /// the desired rate of false positives.
        /// </summary>
        /// <param name="n">Number of items.</param>
        /// <param name="fpRate">Desired false positive rate.</param>
        /// <returns>The optimal BloomFilter size, m.</returns>
        public static ulong OptimalM64(ulong n, double fpRate)
        {
            var optimalM = Math.Ceiling((double)n / ((Math.Log(Defaults.FILL_RATIO) *
                Math.Log(1 - Defaults.FILL_RATIO)) / Math.Abs(Math.Log(fpRate))));
            return Convert.ToUInt64(optimalM);
        }

        /// <summary>
        /// Calculates the optimal number of hash functions to use for a Bloom filter
        /// based on the desired rate of false positives.
        /// </summary>
        /// <param name="fpRate">Desired false positive rate.</param>
        /// <returns>The optimal number of hash functions, k.</returns>
        public static uint OptimalK(double fpRate)
        {
            var optimalK = Math.Ceiling(Math.Log(1 / fpRate, 2));
            return Convert.ToUInt32(optimalK);
        }

        /// <summary>
        /// Returns the upper and lower base hash values from which the k hashes are
        /// derived.
        /// </summary>
        /// <param name="data">The data bytes to hash.</param>
        /// <param name="algorithm">The hashing algorithm to use.</param>
        /// <returns>A HashKernel</returns>
        public static HashKernelReturnValue HashKernel(byte[] data, HashAlgorithm algorithm)
        {
            var hash = new Hash(algorithm);
            hash.ComputeHash(data);
            var sum = hash.Sum();
            return HashKernelReturnValue.Create(
                ToBigEndianUInt32(sum.Skip(4).Take(4).ToArray()),
                ToBigEndianUInt32(sum.Take(4).ToArray())
                );
        }

        /// <summary>
        /// Returns the upper and lower base hash values from which the k hashes are
        /// derived.
        /// </summary>
        /// <param name="data">The data bytes to hash.</param>
        /// <param name="algorithm">The hashing algorithm to use.</param>
        /// <returns>A HashKernel</returns>
        public static HashKernel128ReturnValue HashKernel128(byte[] data, HashAlgorithm algorithm)
        {
            var hash = new Hash128(algorithm);
            var sum = hash.ComputeHashAndSum(data);
            return HashKernel128ReturnValue.Create(
                ToBigEndianUInt64(sum, 8),
                ToBigEndianUInt64(sum, 0)
                );
        }

        public static uint ToBigEndianUInt32(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            uint i = BitConverter.ToUInt32(bytes, 0);
            return i;
        }

        public static ulong ToBigEndianUInt64(byte[] bytes, int offset)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes, offset, 8);

            ulong i = BitConverter.ToUInt64(bytes, offset);
            return i;
        }
    }

    public struct HashKernelReturnValue
    {
        public uint UpperBaseHash { get; private set; }
        public uint LowerBaseHash { get; private set; }

        public static HashKernelReturnValue Create(uint lowerBaseHash, uint upperBaseHash)
        {
            return new HashKernelReturnValue
            {
                UpperBaseHash = upperBaseHash,
                LowerBaseHash = lowerBaseHash
            };
        }
    }

    public struct HashKernel128ReturnValue
    {
        public ulong UpperBaseHash { get; private set; }
        public ulong LowerBaseHash { get; private set; }
        public static HashKernel128ReturnValue Create(ulong lowerBaseHash, ulong upperBaseHash)
        {
            return new HashKernel128ReturnValue
            {
                UpperBaseHash = upperBaseHash,
                LowerBaseHash = lowerBaseHash,
            };
        }
    }
}

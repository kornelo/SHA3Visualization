﻿namespace System.Security.Cryptography
{
    using System.Runtime;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Computes the <see cref="T:System.Security.Cryptography.SHA3" /> hash for the input data.
    /// </summary>
    [ComVisible(true)]
    public abstract class SHA3 : HashAlgorithm
    {
        internal const int KeccakB = 1600;

        internal const int KeccakLaneSizeInBits = 8 * 8;

        internal const int KeccakNumberOfRounds = 24;

        internal readonly ulong[] RoundConstants;

        internal byte[] buffer;

        internal int buffLength;

        // protected new byte[] HashValue;
        // protected new int HashSizeValue;
        internal int keccakR;

        internal ulong[] state;

        static SHA3()
        {
            CryptoConfig.AddAlgorithm(
                typeof(SHA3Managed),
                "SHA3",
                "SHA3Managed",
                "SHA-3",
                "System.Security.Cryptography.SHA3");
            CryptoConfig.AddOID("0.9.2.0", "SHA3", "SHA3Managed", "SHA-3", "System.Security.Cryptography.SHA3");
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="T:System.Security.Cryptography.SHA3" />.
        /// </summary>
        protected SHA3()
        {
            this.HashSizeValue = 512;
        }

        /// <summary>
        /// </summary>
        /// <param name="hashBitLength"></param>
        protected SHA3(int hashBitLength)
        {
            if (hashBitLength != 224 && hashBitLength != 256 && hashBitLength != 384 && hashBitLength != 512)
                throw new ArgumentException("hashBitLength must be 224, 256, 384, or 512", "hashBitLength");
            this.Initialize();
            this.HashSizeValue = hashBitLength;
            switch (hashBitLength)
            {
                case 224:
                    this.KeccakR = 1152;
                    break;
                case 256:
                    this.KeccakR = 1088;
                    break;
                case 384:
                    this.KeccakR = 832;
                    break;
                case 512:
                    this.KeccakR = 576;
                    break;
            }
            this.RoundConstants = new[]
                                      {
                                          0x0000000000000001UL, 0x0000000000008082UL, 0x800000000000808aUL,
                                          0x8000000080008000UL, 0x000000000000808bUL, 0x0000000080000001UL,
                                          0x8000000080008081UL, 0x8000000000008009UL, 0x000000000000008aUL,
                                          0x0000000000000088UL, 0x0000000080008009UL, 0x000000008000000aUL,
                                          0x000000008000808bUL, 0x800000000000008bUL, 0x8000000000008089UL,
                                          0x8000000000008003UL, 0x8000000000008002UL, 0x8000000000000080UL,
                                          0x000000000000800aUL, 0x800000008000000aUL, 0x8000000080008081UL,
                                          0x8000000000008080UL, 0x0000000080000001UL, 0x8000000080008008UL
                                      };
        }

        /// <summary>
        /// </summary>
        public override bool CanReuseTransform => true;

        /// <summary>
        /// </summary>
        public override byte[] Hash => this.HashValue;

        /// <summary>
        /// </summary>
        public int HashByteLength => this.HashSizeValue / 8;

        /// <summary>
        /// </summary>
        public override int HashSize => this.HashSizeValue;

        /// <summary>
        /// </summary>
        public int SizeInBytes => this.KeccakR / 8;

        internal int KeccakR
        {
            get => this.keccakR;
            set => this.keccakR = value;
        }

        /// <summary>Creates an instance of the default implementation of <see cref="T:System.Security.Cryptography.SHA3" />.</summary>
        /// <returns>A new instance of <see cref="T:System.Security.Cryptography.SHA3" />.</returns>
        /// <exception cref="T:System.Reflection.TargetInvocationException">
        ///     The algorithm was used with Federal Information
        ///     Processing Standards (FIPS) mode enabled, but is not FIPS compatible.
        /// </exception>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static new SHA3 Create()
        {
            return Create("System.Security.Cryptography.SHA3");
        }

        /// <summary>Creates an instance of a specified implementation of <see cref="T:System.Security.Cryptography.SHA3" />.</summary>
        /// <returns>A new instance of <see cref="T:System.Security.Cryptography.SHA3" /> using the specified implementation.</returns>
        /// <param name="hashName">
        ///     The name of the specific implementation of <see cref="T:System.Security.Cryptography.SHA3" /> to
        ///     be used.
        /// </param>
        /// <exception cref="T:System.Reflection.TargetInvocationException">
        ///     The algorithm described by the
        ///     <paramref name="hashName" /> parameter was used with Federal Information Processing Standards (FIPS) mode enabled,
        ///     but is not FIPS compatible.
        /// </exception>
        public static new SHA3 Create(string hashName)
        {
            return (SHA3)CryptoConfig.CreateFromName(hashName);
        }

        /// <summary>
        /// </summary>
        public override void Initialize()
        {
            this.buffLength = 0;
            this.state = new ulong[5 * 5]; // 1600 bits
            this.HashValue = null;
        }

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected void AddToBuffer(byte[] array, ref int offset, ref int count)
        {
            var amount = Math.Min(count, this.buffer.Length - this.buffLength);
            Buffer.BlockCopy(array, offset, this.buffer, this.buffLength, amount);
            offset += amount;
            this.buffLength += amount;
            count -= amount;
        }

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="ibStart"></param>
        /// <param name="cbSize"></param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (ibStart < 0) throw new ArgumentOutOfRangeException("ibStart");
            if (cbSize > array.Length) throw new ArgumentOutOfRangeException("cbSize");
            if (ibStart + cbSize > array.Length) throw new ArgumentOutOfRangeException("ibStart or cbSize");
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal()
        {
            return this.Hash;
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected ulong ROL(ulong a, int offset)
        {
            return (a << (offset % KeccakLaneSizeInBits))
                   ^ (a >> (KeccakLaneSizeInBits - offset % KeccakLaneSizeInBits));
        }
    }
}
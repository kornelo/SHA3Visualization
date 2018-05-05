using System;
using System.Security.Cryptography;

namespace SHA3Visualization.SHA3
{

    public sealed class SHA3 : HashAlgorithm {

	    private readonly Sha3Permutation _permutation;

	    private Bitstring _hash = new Bitstring();

        public SHA3(int size)
	        : base()
	    {
	        switch (size)
	        {
	            case 224:
	                _permutation = Sha3Permutation.Sha3_224();
	                break;
	            case 256:
	                _permutation = Sha3Permutation.Sha3_256();
	                break;
	            case 384:
	                _permutation = Sha3Permutation.Sha3_384();
	                break;
	            case 512:
	            default:
	                _permutation = Sha3Permutation.Sha3_512();
	                break;
	        }
	    }

	    public static SHA3 Create(int size)
	    {
	        return new SHA3(size);
	    }

	    protected override void HashCore(byte[] array, int ibStart, int cbSize)
	    {
	        byte[] data = new byte[cbSize];
	        Array.Copy(array, ibStart, data, 0, cbSize);
	        _hash.Append(data);
	    }

	    protected override byte[] HashFinal()
	    {
	        _hash = new Bitstring(_permutation.Process(_hash.Bytes, _permutation.Width));
	        return _hash?.Bytes ?? new byte[0];
	    }

	    public override void Initialize()
	    {
	        _hash = new Bitstring();
	    }
    }
}

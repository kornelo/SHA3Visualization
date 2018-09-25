namespace System.Security.Cryptography
{
    using System.Runtime.InteropServices;
    using System.Text;

    using SHA3Visualization;

    /// <summary>
    ///     Computes the <see cref="T:System.Security.Cryptography.SHA3" /> hash algorithm for the input data using the managed
    ///     library.
    /// </summary>
    [ComVisible(true)]
    public class SHA3Managed : SHA3
    {
        /// <summary>
        /// </summary>
        public SHA3Managed()
            : base(512)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="hashBitLength"></param>
        public SHA3Managed(int hashBitLength)
            : base(hashBitLength)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="ibStart"></param>
        /// <param name="cbSize"></param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            base.HashCore(array, ibStart, cbSize);
            if (cbSize == 0) return;
            var sizeInBytes = this.SizeInBytes;
            if (this.buffer == null) this.buffer = new byte[sizeInBytes];
            var stride = sizeInBytes >> 3;
            var utemps = new ulong[stride];
            if (this.buffLength == sizeInBytes) throw new Exception("Unexpected error, the internal buffer is full");
            this.AddToBuffer(array, ref ibStart, ref cbSize);
            if (this.buffLength == sizeInBytes)
            {
                // buffer full
                Buffer.BlockCopy(this.buffer, 0, utemps, 0, sizeInBytes);
                this.KeccakF(utemps, stride);
                this.buffLength = 0;
            }

            for (; cbSize >= sizeInBytes; cbSize -= sizeInBytes, ibStart += sizeInBytes)
            {
                Buffer.BlockCopy(array, ibStart, utemps, 0, sizeInBytes);
                this.KeccakF(utemps, stride);
            }

            if (cbSize > 0)
            {
                // some left over
                Buffer.BlockCopy(array, ibStart, this.buffer, this.buffLength, cbSize);
                this.buffLength += cbSize;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal()
        {
            var sizeInBytes = this.SizeInBytes;
            var outb = new byte[this.HashByteLength];

            // padding
            if (this.buffer == null) this.buffer = new byte[sizeInBytes];
            else Array.Clear(this.buffer, this.buffLength, sizeInBytes - this.buffLength);
            this.buffer[this.buffLength++] = 1;
            this.buffer[sizeInBytes - 1] |= 0x80;
            var stride = sizeInBytes >> 3;
            var utemps = new ulong[stride];
            Buffer.BlockCopy(this.buffer, 0, utemps, 0, sizeInBytes);
            this.KeccakF(utemps, stride);
            Buffer.BlockCopy(this.state, 0, outb, 0, this.HashByteLength);
            return outb;
        }

        private void KeccakF(ulong[] inb, int laneCount)
        {
            while (--laneCount >= 0) this.state[laneCount] ^= inb[laneCount];
            ulong Aba, Abe, Abi, Abo, Abu;
            ulong Aga, Age, Agi, Ago, Agu;
            ulong Aka, Ake, Aki, Ako, Aku;
            ulong Ama, Ame, Ami, Amo, Amu;
            ulong Asa, Ase, Asi, Aso, Asu;
            ulong BCa, BCe, BCi, BCo, BCu;
            ulong Da, De, Di, Do, Du;
            ulong Eba, Ebe, Ebi, Ebo, Ebu;
            ulong Ega, Ege, Egi, Ego, Egu;
            ulong Eka, Eke, Eki, Eko, Eku;
            ulong Ema, Eme, Emi, Emo, Emu;
            ulong Esa, Ese, Esi, Eso, Esu;
            var round = laneCount;

            // copyFromState(A, state)
            Aba = this.state[0];
            Abe = this.state[1];
            Abi = this.state[2];
            Abo = this.state[3];
            Abu = this.state[4];
            Aga = this.state[5];
            Age = this.state[6];
            Agi = this.state[7];
            Ago = this.state[8];
            Agu = this.state[9];
            Aka = this.state[10];
            Ake = this.state[11];
            Aki = this.state[12];
            Ako = this.state[13];
            Aku = this.state[14];
            Ama = this.state[15];
            Ame = this.state[16];
            Ami = this.state[17];
            Amo = this.state[18];
            Amu = this.state[19];
            Asa = this.state[20];
            Ase = this.state[21];
            Asi = this.state[22];
            Aso = this.state[23];
            Asu = this.state[24];

            

            for (round = 0; round < KeccakNumberOfRounds; round += 2)
            {
                // prepareTheta
                BCa = Aba ^ Aga ^ Aka ^ Ama ^ Asa;
                BCe = Abe ^ Age ^ Ake ^ Ame ^ Ase;
                BCi = Abi ^ Agi ^ Aki ^ Ami ^ Asi;
                BCo = Abo ^ Ago ^ Ako ^ Amo ^ Aso;
                BCu = Abu ^ Agu ^ Aku ^ Amu ^ Asu;

                // thetaRhoPiChiIotaPrepareTheta(round  , A, E)
                Da = BCu ^ this.ROL(BCe, 1);
                De = BCa ^ this.ROL(BCi, 1);
                Di = BCe ^ this.ROL(BCo, 1);
                Do = BCi ^ this.ROL(BCu, 1);
                Du = BCo ^ this.ROL(BCa, 1);

                Aba ^= Da;
                BCa = Aba;
                Age ^= De;
                BCe = this.ROL(Age, 44);
                Aki ^= Di;
                BCi = this.ROL(Aki, 43);
                Amo ^= Do;
                BCo = this.ROL(Amo, 21);
                Asu ^= Du;
                BCu = this.ROL(Asu, 14);
                Eba = BCa ^ (~BCe & BCi);
                Eba ^= this.RoundConstants[round];
                Ebe = BCe ^ (~BCi & BCo);
                Ebi = BCi ^ (~BCo & BCu);
                Ebo = BCo ^ (~BCu & BCa);
                Ebu = BCu ^ (~BCa & BCe);

                Abo ^= Do;
                BCa = this.ROL(Abo, 28);
                Agu ^= Du;
                BCe = this.ROL(Agu, 20);
                Aka ^= Da;
                BCi = this.ROL(Aka, 3);
                Ame ^= De;
                BCo = this.ROL(Ame, 45);
                Asi ^= Di;
                BCu = this.ROL(Asi, 61);
                Ega = BCa ^ (~BCe & BCi);
                Ege = BCe ^ (~BCi & BCo);
                Egi = BCi ^ (~BCo & BCu);
                Ego = BCo ^ (~BCu & BCa);
                Egu = BCu ^ (~BCa & BCe);

                Abe ^= De;
                BCa = this.ROL(Abe, 1);
                Agi ^= Di;
                BCe = this.ROL(Agi, 6);
                Ako ^= Do;
                BCi = this.ROL(Ako, 25);
                Amu ^= Du;
                BCo = this.ROL(Amu, 8);
                Asa ^= Da;
                BCu = this.ROL(Asa, 18);
                Eka = BCa ^ (~BCe & BCi);
                Eke = BCe ^ (~BCi & BCo);
                Eki = BCi ^ (~BCo & BCu);
                Eko = BCo ^ (~BCu & BCa);
                Eku = BCu ^ (~BCa & BCe);

                Abu ^= Du;
                BCa = this.ROL(Abu, 27);
                Aga ^= Da;
                BCe = this.ROL(Aga, 36);
                Ake ^= De;
                BCi = this.ROL(Ake, 10);
                Ami ^= Di;
                BCo = this.ROL(Ami, 15);
                Aso ^= Do;
                BCu = this.ROL(Aso, 56);
                Ema = BCa ^ (~BCe & BCi);
                Eme = BCe ^ (~BCi & BCo);
                Emi = BCi ^ (~BCo & BCu);
                Emo = BCo ^ (~BCu & BCa);
                Emu = BCu ^ (~BCa & BCe);

                Abi ^= Di;
                BCa = this.ROL(Abi, 62);
                Ago ^= Do;
                BCe = this.ROL(Ago, 55);
                Aku ^= Du;
                BCi = this.ROL(Aku, 39);
                Ama ^= Da;
                BCo = this.ROL(Ama, 41);
                Ase ^= De;
                BCu = this.ROL(Ase, 2);
                Esa = BCa ^ (~BCe & BCi);
                Ese = BCe ^ (~BCi & BCo);
                Esi = BCi ^ (~BCo & BCu);
                Eso = BCo ^ (~BCu & BCa);
                Esu = BCu ^ (~BCa & BCe);

                // prepareTheta
                BCa = Eba ^ Ega ^ Eka ^ Ema ^ Esa;
                BCe = Ebe ^ Ege ^ Eke ^ Eme ^ Ese;
                BCi = Ebi ^ Egi ^ Eki ^ Emi ^ Esi;
                BCo = Ebo ^ Ego ^ Eko ^ Emo ^ Eso;
                BCu = Ebu ^ Egu ^ Eku ^ Emu ^ Esu;

                // thetaRhoPiChiIotaPrepareTheta(round+1, E, A)
                Da = BCu ^ this.ROL(BCe, 1);
                De = BCa ^ this.ROL(BCi, 1);
                Di = BCe ^ this.ROL(BCo, 1);
                Do = BCi ^ this.ROL(BCu, 1);
                Du = BCo ^ this.ROL(BCa, 1);

                Eba ^= Da;
                BCa = Eba;
                Ege ^= De;
                BCe = this.ROL(Ege, 44);
                Eki ^= Di;
                BCi = this.ROL(Eki, 43);
                Emo ^= Do;
                BCo = this.ROL(Emo, 21);
                Esu ^= Du;
                BCu = this.ROL(Esu, 14);
                Aba = BCa ^ (~BCe & BCi);
                Aba ^= this.RoundConstants[round + 1];
                Abe = BCe ^ (~BCi & BCo);
                Abi = BCi ^ (~BCo & BCu);
                Abo = BCo ^ (~BCu & BCa);
                Abu = BCu ^ (~BCa & BCe);

                Ebo ^= Do;
                BCa = this.ROL(Ebo, 28);
                Egu ^= Du;
                BCe = this.ROL(Egu, 20);
                Eka ^= Da;
                BCi = this.ROL(Eka, 3);
                Eme ^= De;
                BCo = this.ROL(Eme, 45);
                Esi ^= Di;
                BCu = this.ROL(Esi, 61);
                Aga = BCa ^ (~BCe & BCi);
                Age = BCe ^ (~BCi & BCo);
                Agi = BCi ^ (~BCo & BCu);
                Ago = BCo ^ (~BCu & BCa);
                Agu = BCu ^ (~BCa & BCe);

                Ebe ^= De;
                BCa = this.ROL(Ebe, 1);
                Egi ^= Di;
                BCe = this.ROL(Egi, 6);
                Eko ^= Do;
                BCi = this.ROL(Eko, 25);
                Emu ^= Du;
                BCo = this.ROL(Emu, 8);
                Esa ^= Da;
                BCu = this.ROL(Esa, 18);
                Aka = BCa ^ (~BCe & BCi);
                Ake = BCe ^ (~BCi & BCo);
                Aki = BCi ^ (~BCo & BCu);
                Ako = BCo ^ (~BCu & BCa);
                Aku = BCu ^ (~BCa & BCe);

                Ebu ^= Du;
                BCa = this.ROL(Ebu, 27);
                Ega ^= Da;
                BCe = this.ROL(Ega, 36);
                Eke ^= De;
                BCi = this.ROL(Eke, 10);
                Emi ^= Di;
                BCo = this.ROL(Emi, 15);
                Eso ^= Do;
                BCu = this.ROL(Eso, 56);
                Ama = BCa ^ (~BCe & BCi);
                Ame = BCe ^ (~BCi & BCo);
                Ami = BCi ^ (~BCo & BCu);
                Amo = BCo ^ (~BCu & BCa);
                Amu = BCu ^ (~BCa & BCe);

                Ebi ^= Di;
                BCa = this.ROL(Ebi, 62);
                Ego ^= Do;
                BCe = this.ROL(Ego, 55);
                Eku ^= Du;
                BCi = this.ROL(Eku, 39);
                Ema ^= Da;
                BCo = this.ROL(Ema, 41);
                Ese ^= De;
                BCu = this.ROL(Ese, 2);
                Asa = BCa ^ (~BCe & BCi);
                Ase = BCe ^ (~BCi & BCo);
                Asi = BCi ^ (~BCo & BCu);
                Aso = BCo ^ (~BCu & BCa);
                Asu = BCu ^ (~BCa & BCe);
            }

            // copyToState(state, A)
            this.state[0] = Aba;
            this.state[1] = Abe;
            this.state[2] = Abi;
            this.state[3] = Abo;
            this.state[4] = Abu;
            this.state[5] = Aga;
            this.state[6] = Age;
            this.state[7] = Agi;
            this.state[8] = Ago;
            this.state[9] = Agu;
            this.state[10] = Aka;
            this.state[11] = Ake;
            this.state[12] = Aki;
            this.state[13] = Ako;
            this.state[14] = Aku;
            this.state[15] = Ama;
            this.state[16] = Ame;
            this.state[17] = Ami;
            this.state[18] = Amo;
            this.state[19] = Amu;
            this.state[20] = Asa;
            this.state[21] = Ase;
            this.state[22] = Asi;
            this.state[23] = Aso;
            this.state[24] = Asu;
        }

    }

}
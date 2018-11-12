using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHA3Visualization.SHA3
{
    class SHA3Manual
    {

        internal const int KeccakB = 1600;

        internal const int KeccakLaneSizeInBits = 8 * 8;

        internal const int KeccakNumberOfRounds = 24;

        internal byte[] buffer;

        internal int buffLength;

        // protected new byte[] HashValue;
        // protected new int HashSizeValue;
        internal int keccakR;

        internal ulong[] state;

        internal readonly ulong[] RoundConstants = new[]
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

        internal readonly int[] OffsetsOfRho = new[] { 0, 36, 3, 105, 210, 1, 300, 10, 45, 66, 190, 6, 171, 15, 253, 28, 55, 153, 21, 120, 91, 276, 231, 136, 78 };
        private ulong[] A { get; set; }
        private ulong[] B { get; set; }
        private ulong[] C { get; set; }
        private ulong[] D { get; set; }

        //public override bool CanReuseTransform => true;

        //public override byte[] Hash => this.HashValue;

        public int HashByteLength => 512 / 8;

        public int HashSize => 512;

        public int SizeInBytes => this.KeccakR / 8;

        internal int KeccakR
        {
            get => this.keccakR;
            set => this.keccakR = value;
        }

        private void KeccakF(ulong[] inb, int laneCount)
        {
            while (--laneCount >= 0) A[laneCount] ^= inb[laneCount];
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


            // copyFromA(A, A)
            Aba = A[0];
            Abe = A[1];
            Abi = A[2];
            Abo = A[3];
            Abu = A[4];
            Aga = A[5];
            Age = A[6];
            Agi = A[7];
            Ago = A[8];
            Agu = A[9];
            Aka = A[10];
            Ake = A[11];
            Aki = A[12];
            Ako = A[13];
            Aku = A[14];
            Ama = A[15];
            Ame = A[16];
            Ami = A[17];
            Amo = A[18];
            Amu = A[19];
            Asa = A[20];
            Ase = A[21];
            Asi = A[22];
            Aso = A[23];
            Asu = A[24];

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

                CopyToA(Aba, Abe, Abi, Abo, Abu, Aga, Age, Agi, Ago, Agu, Aka, Ake, Aki, Ako, Aku, Ama, Ame, Ami, Amo, Amu, Asa, Ase, Asi, Aso, Asu);

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


                CopyToA(Aba, Abe, Abi, Abo, Abu, Aga, Age, Agi, Ago, Agu, Aka, Ake, Aki, Ako, Aku, Ama, Ame, Ami, Amo, Amu, Asa, Ase, Asi, Aso, Asu);
            }

            // copyToA(A, A)
            A[0] = Aba;
            A[1] = Abe;
            A[2] = Abi;
            A[3] = Abo;
            A[4] = Abu;
            A[5] = Aga;
            A[6] = Age;
            A[7] = Agi;
            A[8] = Ago;
            A[9] = Agu;
            A[10] = Aka;
            A[11] = Ake;
            A[12] = Aki;
            A[13] = Ako;
            A[14] = Aku;
            A[15] = Ama;
            A[16] = Ame;
            A[17] = Ami;
            A[18] = Amo;
            A[19] = Amu;
            A[20] = Asa;
            A[21] = Ase;
            A[22] = Asi;
            A[23] = Aso;
            A[24] = Asu;

            // CopyToA(Aba, Abe, Abi, Abo, Abu, Aga, Age, Agi, Ago, Agu, Aka, Ake, Aki, Ako, Aku, Ama, Ame, Ami, Amo, Amu, Asa, Ase, Asi, Aso, Asu);

        }

        private void CopyToA(
            ulong Aba, ulong Abe, ulong Abi, ulong Abo, ulong Abu,
            ulong Aga, ulong Age, ulong Agi, ulong Ago, ulong Agu,
            ulong Aka, ulong Ake, ulong Aki, ulong Ako, ulong Aku,
            ulong Ama, ulong Ame, ulong Ami, ulong Amo, ulong Amu,
            ulong Asa, ulong Ase, ulong Asi, ulong Aso, ulong Asu)
        {

            // copyToA(A, A)
            A[0] = Aba;
            A[1] = Abe;
            A[2] = Abi;
            A[3] = Abo;
            A[4] = Abu;
            A[5] = Aga;
            A[6] = Age;
            A[7] = Agi;
            A[8] = Ago;
            A[9] = Agu;
            A[10] = Aka;
            A[11] = Ake;
            A[12] = Aki;
            A[13] = Ako;
            A[14] = Aku;
            A[15] = Ama;
            A[16] = Ame;
            A[17] = Ami;
            A[18] = Amo;
            A[19] = Amu;
            A[20] = Asa;
            A[21] = Ase;
            A[22] = Asi;
            A[23] = Aso;
            A[24] = Asu;

            //CubeHandler.A = new List<ulong>();

            //foreach (var A in A)
            //{
            //    CubeHandler.A.Add(A);
            //}


            Buffer.BlockCopy(A, 0, A = new ulong[this.HashByteLength], 0, this.HashByteLength);

            CubeHandler.State = new List<ulong>();

            foreach (var state in A)
            {
                CubeHandler.State.Add(state);
            }
        }

        private void Theta(ulong[] A)
        {
            // prepareTheta
            C[0] = A[0] ^ A[5] ^ A[10] ^ A[15] ^ A[20];
            C[1] = A[1] ^ A[6] ^ A[11] ^ A[16] ^ A[21];
            C[2] = A[2] ^ A[7] ^ A[12] ^ A[17] ^ A[22];
            C[3] = A[3] ^ A[8] ^ A[13] ^ A[18] ^ A[23];
            C[4] = A[4] ^ A[9] ^ A[14] ^ A[19] ^ A[24];


            // thetaRhoPiChiIotaPrepareTheta(round  , A, E)
            D[0] = C[4] ^ this.ROL(C[1], 1);
            D[1] = C[0] ^ this.ROL(C[2], 1);
            D[2] = C[1] ^ this.ROL(C[3], 1);
            D[3] = C[2] ^ this.ROL(C[4], 1);
            D[4] = C[3] ^ this.ROL(C[0], 1);

            for (var i = 0; i < 5; i++)
            {
                A[i] ^= D[i];
                A[i+5] ^= D[i];
                A[i+10] ^= D[i];
                A[i+15] ^= D[i];
                A[i+20] ^= D[i];
            }
        }

        private void Rho(ulong[] A)
        {
            for (var i = 0; i < 25; i++) this.A[i] = ROL(A[i], this.OffsetsOfRho[i]);
        }

        private void Pi(ulong[] A)
        {
            for (var x = 0; x < 5; x++)
                for(var y=0; y<5; y++)
                    B[(y*5)+(x+3*y)%5] = A[(x*5)+y];
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

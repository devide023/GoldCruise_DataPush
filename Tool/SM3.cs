using System;
using Org.BouncyCastle.Utilities.Encoders;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Generators;
using System.Collections.Generic;

namespace GoldCruise_DataPush.SM
{
    public class SM2
    {
        public static SM2 Instance
        {
            get
            {
                return new SM2();
            }

        }
        public static SM2 InstanceTest
        {
            get
            {
                return new SM2();
            }

        }

        public static readonly string[] sm2_param = {
            "FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF",// p,0
			"FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC",// a,1
			"28E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93",// b,2
			"FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123",// n,3
			"32C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7",// gx,4
			"BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0" // gy,5
	    };

        public string[] ecc_param = sm2_param;

        public readonly BigInteger ecc_p;
        public readonly BigInteger ecc_a;
        public readonly BigInteger ecc_b;
        public readonly BigInteger ecc_n;
        public readonly BigInteger ecc_gx;
        public readonly BigInteger ecc_gy;

        public readonly ECCurve ecc_curve;
        public readonly ECPoint ecc_point_g;

        public readonly ECDomainParameters ecc_bc_spec;

        public readonly ECKeyPairGenerator ecc_key_pair_generator;

        private SM2()
        {
            ecc_param = sm2_param;

            ECFieldElement ecc_gx_fieldelement;
            ECFieldElement ecc_gy_fieldelement;

            ecc_p = new BigInteger(ecc_param[0], 16);
            ecc_a = new BigInteger(ecc_param[1], 16);
            ecc_b = new BigInteger(ecc_param[2], 16);
            ecc_n = new BigInteger(ecc_param[3], 16);
            ecc_gx = new BigInteger(ecc_param[4], 16);
            ecc_gy = new BigInteger(ecc_param[5], 16);


            ecc_gx_fieldelement = new FpFieldElement(ecc_p, ecc_gx);
            ecc_gy_fieldelement = new FpFieldElement(ecc_p, ecc_gy);

            ecc_curve = new FpCurve(ecc_p, ecc_a, ecc_b);
            ecc_point_g = new FpPoint(ecc_curve, ecc_gx_fieldelement, ecc_gy_fieldelement);

            ecc_bc_spec = new ECDomainParameters(ecc_curve, ecc_point_g, ecc_n);

            ECKeyGenerationParameters ecc_ecgenparam;
            ecc_ecgenparam = new ECKeyGenerationParameters(ecc_bc_spec, new SecureRandom());

            ecc_key_pair_generator = new ECKeyPairGenerator();
            ecc_key_pair_generator.Init(ecc_ecgenparam);
        }

        public virtual byte[] Sm2GetZ(byte[] userId, ECPoint userKey)
        {
            SM3Digest sm3 = new SM3Digest();
            byte[] p;
            // userId length
            int len = userId.Length * 8;
            sm3.Update((byte)(len >> 8 & 0x00ff));
            sm3.Update((byte)(len & 0x00ff));

            // userId
            sm3.BlockUpdate(userId, 0, userId.Length);

            // a,b
            p = ecc_a.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            p = ecc_b.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            // gx,gy
            p = ecc_gx.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            p = ecc_gy.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);

            // x,y
            p = userKey.X.ToBigInteger().ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            p = userKey.Y.ToBigInteger().ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);

            // Z
            byte[] md = new byte[sm3.GetDigestSize()];
            sm3.DoFinal(md, 0);

            return md;
        }

    }

    class SM2Utils
    {
        public static void GenerateKeyPair()
        {
            SM2 sm2 = SM2.Instance;
            AsymmetricCipherKeyPair key = sm2.ecc_key_pair_generator.GenerateKeyPair();
            ECPrivateKeyParameters ecpriv = (ECPrivateKeyParameters)key.Private;
            ECPublicKeyParameters ecpub = (ECPublicKeyParameters)key.Public;
            BigInteger privateKey = ecpriv.D;
            ECPoint publicKey = ecpub.Q;

            System.Console.Out.WriteLine("公钥: " + Encoding.Default.GetString(Hex.Encode(publicKey.GetEncoded())).ToUpper());
            System.Console.Out.WriteLine("私钥: " + Encoding.Default.GetString(Hex.Encode(privateKey.ToByteArray())).ToUpper());
        }

        public static String Encrypt(byte[] publicKey, byte[] data)
        {
            if (null == publicKey || publicKey.Length == 0)
            {
                return null;
            }
            if (data == null || data.Length == 0)
            {
                return null;
            }

            byte[] source = new byte[data.Length];
            Array.Copy(data, 0, source, 0, data.Length);

            Cipher cipher = new Cipher();
            SM2 sm2 = SM2.Instance;

            ECPoint userKey = sm2.ecc_curve.DecodePoint(publicKey);

            ECPoint c1 = cipher.Init_enc(sm2, userKey);
            cipher.Encrypt(source);

            byte[] c3 = new byte[32];
            cipher.Dofinal(c3);

            String sc1 = Encoding.Default.GetString(Hex.Encode(c1.GetEncoded()));
            String sc2 = Encoding.Default.GetString(Hex.Encode(source));
            String sc3 = Encoding.Default.GetString(Hex.Encode(c3));

            return (sc1 + sc2 + sc3).ToUpper();
        }

        public static byte[] Decrypt(byte[] privateKey, byte[] encryptedData)
        {
            if (null == privateKey || privateKey.Length == 0)
            {
                return null;
            }
            if (encryptedData == null || encryptedData.Length == 0)
            {
                return null;
            }

            String data = Encoding.Default.GetString(Hex.Encode(encryptedData));

            byte[] c1Bytes = Hex.Decode(Encoding.Default.GetBytes(data.Substring(0, 130)));
            int c2Len = encryptedData.Length - 97;
            byte[] c2 = Hex.Decode(Encoding.Default.GetBytes(data.Substring(130, 2 * c2Len)));
            byte[] c3 = Hex.Decode(Encoding.Default.GetBytes(data.Substring(130 + 2 * c2Len, 64)));

            SM2 sm2 = SM2.Instance;
            BigInteger userD = new BigInteger(1, privateKey);

            ECPoint c1 = sm2.ecc_curve.DecodePoint(c1Bytes);
            Cipher cipher = new Cipher();
            cipher.Init_dec(userD, c1);
            cipher.Decrypt(c2);
            cipher.Dofinal(c3);

            return c2;
        }

        //[STAThread]
        //public static void Main()
        //{
        //    GenerateKeyPair();

        //    String plainText = "ererfeiisgod";
        //    byte[] sourceData = Encoding.Default.GetBytes(plainText);

        //    //下面的秘钥可以使用generateKeyPair()生成的秘钥内容  
        //    // 国密规范正式私钥  
        //    String prik = "3690655E33D5EA3D9A4AE1A1ADD766FDEA045CDEAA43A9206FB8C430CEFE0D94";
        //    // 国密规范正式公钥  
        //    String pubk = "04F6E0C3345AE42B51E06BF50B98834988D54EBC7460FE135A48171BC0629EAE205EEDE253A530608178A98F1E19BB737302813BA39ED3FA3C51639D7A20C7391A";

        //    System.Console.Out.WriteLine("加密: ");
        //    String cipherText = SM2Utils.Encrypt(Hex.Decode(pubk), sourceData);
        //    System.Console.Out.WriteLine(cipherText);
        //    System.Console.Out.WriteLine("解密: ");
        //    plainText = Encoding.Default.GetString(SM2Utils.Decrypt(Hex.Decode(prik), Hex.Decode(cipherText)));
        //    System.Console.Out.WriteLine(plainText);

        //    Console.ReadLine();
        //}
    }

    class SM4
    {
        public const int SM4_ENCRYPT = 1;
        public const int SM4_DECRYPT = 0;

        private long GET_ULONG_BE(byte[] b, int i)
        {
            long n = (long)(b[i] & 0xff) << 24 | (long)((b[i + 1] & 0xff) << 16) | (long)((b[i + 2] & 0xff) << 8) | (long)(b[i + 3] & 0xff) & 0xffffffffL;
            return n;
        }

        private void PUT_ULONG_BE(long n, byte[] b, int i)
        {
            b[i] = (byte)(int)(0xFF & n >> 24);
            b[i + 1] = (byte)(int)(0xFF & n >> 16);
            b[i + 2] = (byte)(int)(0xFF & n >> 8);
            b[i + 3] = (byte)(int)(0xFF & n);
        }

        private long SHL(long x, int n)
        {
            return (x & 0xFFFFFFFF) << n;
        }

        private long ROTL(long x, int n)
        {
            return SHL(x, n) | x >> (32 - n);
        }

        private void SWAP(long[] sk, int i)
        {
            long t = sk[i];
            sk[i] = sk[(31 - i)];
            sk[(31 - i)] = t;
        }

        public byte[] SboxTable = new byte[] { (byte) 0xd6, (byte) 0x90, (byte) 0xe9, (byte) 0xfe,
            (byte) 0xcc, (byte) 0xe1, 0x3d, (byte) 0xb7, 0x16, (byte) 0xb6,
            0x14, (byte) 0xc2, 0x28, (byte) 0xfb, 0x2c, 0x05, 0x2b, 0x67,
            (byte) 0x9a, 0x76, 0x2a, (byte) 0xbe, 0x04, (byte) 0xc3,
            (byte) 0xaa, 0x44, 0x13, 0x26, 0x49, (byte) 0x86, 0x06,
            (byte) 0x99, (byte) 0x9c, 0x42, 0x50, (byte) 0xf4, (byte) 0x91,
            (byte) 0xef, (byte) 0x98, 0x7a, 0x33, 0x54, 0x0b, 0x43,
            (byte) 0xed, (byte) 0xcf, (byte) 0xac, 0x62, (byte) 0xe4,
            (byte) 0xb3, 0x1c, (byte) 0xa9, (byte) 0xc9, 0x08, (byte) 0xe8,
            (byte) 0x95, (byte) 0x80, (byte) 0xdf, (byte) 0x94, (byte) 0xfa,
            0x75, (byte) 0x8f, 0x3f, (byte) 0xa6, 0x47, 0x07, (byte) 0xa7,
            (byte) 0xfc, (byte) 0xf3, 0x73, 0x17, (byte) 0xba, (byte) 0x83,
            0x59, 0x3c, 0x19, (byte) 0xe6, (byte) 0x85, 0x4f, (byte) 0xa8,
            0x68, 0x6b, (byte) 0x81, (byte) 0xb2, 0x71, 0x64, (byte) 0xda,
            (byte) 0x8b, (byte) 0xf8, (byte) 0xeb, 0x0f, 0x4b, 0x70, 0x56,
            (byte) 0x9d, 0x35, 0x1e, 0x24, 0x0e, 0x5e, 0x63, 0x58, (byte) 0xd1,
            (byte) 0xa2, 0x25, 0x22, 0x7c, 0x3b, 0x01, 0x21, 0x78, (byte) 0x87,
            (byte) 0xd4, 0x00, 0x46, 0x57, (byte) 0x9f, (byte) 0xd3, 0x27,
            0x52, 0x4c, 0x36, 0x02, (byte) 0xe7, (byte) 0xa0, (byte) 0xc4,
            (byte) 0xc8, (byte) 0x9e, (byte) 0xea, (byte) 0xbf, (byte) 0x8a,
            (byte) 0xd2, 0x40, (byte) 0xc7, 0x38, (byte) 0xb5, (byte) 0xa3,
            (byte) 0xf7, (byte) 0xf2, (byte) 0xce, (byte) 0xf9, 0x61, 0x15,
            (byte) 0xa1, (byte) 0xe0, (byte) 0xae, 0x5d, (byte) 0xa4,
            (byte) 0x9b, 0x34, 0x1a, 0x55, (byte) 0xad, (byte) 0x93, 0x32,
            0x30, (byte) 0xf5, (byte) 0x8c, (byte) 0xb1, (byte) 0xe3, 0x1d,
            (byte) 0xf6, (byte) 0xe2, 0x2e, (byte) 0x82, 0x66, (byte) 0xca,
            0x60, (byte) 0xc0, 0x29, 0x23, (byte) 0xab, 0x0d, 0x53, 0x4e, 0x6f,
            (byte) 0xd5, (byte) 0xdb, 0x37, 0x45, (byte) 0xde, (byte) 0xfd,
            (byte) 0x8e, 0x2f, 0x03, (byte) 0xff, 0x6a, 0x72, 0x6d, 0x6c, 0x5b,
            0x51, (byte) 0x8d, 0x1b, (byte) 0xaf, (byte) 0x92, (byte) 0xbb,
            (byte) 0xdd, (byte) 0xbc, 0x7f, 0x11, (byte) 0xd9, 0x5c, 0x41,
            0x1f, 0x10, 0x5a, (byte) 0xd8, 0x0a, (byte) 0xc1, 0x31,
            (byte) 0x88, (byte) 0xa5, (byte) 0xcd, 0x7b, (byte) 0xbd, 0x2d,
            0x74, (byte) 0xd0, 0x12, (byte) 0xb8, (byte) 0xe5, (byte) 0xb4,
            (byte) 0xb0, (byte) 0x89, 0x69, (byte) 0x97, 0x4a, 0x0c,
            (byte) 0x96, 0x77, 0x7e, 0x65, (byte) 0xb9, (byte) 0xf1, 0x09,
            (byte) 0xc5, 0x6e, (byte) 0xc6, (byte) 0x84, 0x18, (byte) 0xf0,
            0x7d, (byte) 0xec, 0x3a, (byte) 0xdc, 0x4d, 0x20, 0x79,
            (byte) 0xee, 0x5f, 0x3e, (byte) 0xd7, (byte) 0xcb, 0x39, 0x48 };

        public uint[] FK = { 0xa3b1bac6, 0x56aa3350, 0x677d9197, 0xb27022dc };

        public uint[] CK = { 0x00070e15,0x1c232a31,0x383f464d,0x545b6269,
                                        0x70777e85,0x8c939aa1,0xa8afb6bd,0xc4cbd2d9,
                                        0xe0e7eef5,0xfc030a11,0x181f262d,0x343b4249,
                                        0x50575e65,0x6c737a81,0x888f969d,0xa4abb2b9,
                                        0xc0c7ced5,0xdce3eaf1,0xf8ff060d,0x141b2229,
                                        0x30373e45,0x4c535a61,0x686f767d,0x848b9299,
                                        0xa0a7aeb5,0xbcc3cad1,0xd8dfe6ed,0xf4fb0209,
                                        0x10171e25,0x2c333a41,0x484f565d,0x646b7279 };

        private byte sm4Sbox(byte inch)
        {
            int i = inch & 0xFF;
            byte retVal = SboxTable[i];
            return retVal;
        }

        private long sm4Lt(long ka)
        {
            long bb = 0L;
            long c = 0L;
            byte[] a = new byte[4];
            byte[] b = new byte[4];
            PUT_ULONG_BE(ka, a, 0);
            b[0] = sm4Sbox(a[0]);
            b[1] = sm4Sbox(a[1]);
            b[2] = sm4Sbox(a[2]);
            b[3] = sm4Sbox(a[3]);
            bb = GET_ULONG_BE(b, 0);
            c = bb ^ ROTL(bb, 2) ^ ROTL(bb, 10) ^ ROTL(bb, 18) ^ ROTL(bb, 24);
            return c;
        }

        private long sm4F(long x0, long x1, long x2, long x3, long rk)
        {
            return x0 ^ sm4Lt(x1 ^ x2 ^ x3 ^ rk);
        }

        private long sm4CalciRK(long ka)
        {
            long bb = 0L;
            long rk = 0L;
            byte[] a = new byte[4];
            byte[] b = new byte[4];
            PUT_ULONG_BE(ka, a, 0);
            b[0] = sm4Sbox(a[0]);
            b[1] = sm4Sbox(a[1]);
            b[2] = sm4Sbox(a[2]);
            b[3] = sm4Sbox(a[3]);
            bb = GET_ULONG_BE(b, 0);
            rk = bb ^ ROTL(bb, 13) ^ ROTL(bb, 23);
            return rk;
        }

        private void sm4_setkey(long[] SK, byte[] key)
        {
            long[] MK = new long[4];
            long[] k = new long[36];
            int i = 0;
            MK[0] = GET_ULONG_BE(key, 0);
            MK[1] = GET_ULONG_BE(key, 4);
            MK[2] = GET_ULONG_BE(key, 8);
            MK[3] = GET_ULONG_BE(key, 12);
            k[0] = MK[0] ^ (long)FK[0];
            k[1] = MK[1] ^ (long)FK[1];
            k[2] = MK[2] ^ (long)FK[2];
            k[3] = MK[3] ^ (long)FK[3];
            for (; i < 32; i++)
            {
                k[(i + 4)] = (k[i] ^ sm4CalciRK(k[(i + 1)] ^ k[(i + 2)] ^ k[(i + 3)] ^ (long)CK[i]));
                SK[i] = k[(i + 4)];
            }
        }

        private void sm4_one_round(long[] sk, byte[] input, byte[] output)
        {
            int i = 0;
            long[] ulbuf = new long[36];
            ulbuf[0] = GET_ULONG_BE(input, 0);
            ulbuf[1] = GET_ULONG_BE(input, 4);
            ulbuf[2] = GET_ULONG_BE(input, 8);
            ulbuf[3] = GET_ULONG_BE(input, 12);
            while (i < 32)
            {
                ulbuf[(i + 4)] = sm4F(ulbuf[i], ulbuf[(i + 1)], ulbuf[(i + 2)], ulbuf[(i + 3)], sk[i]);
                i++;
            }
            PUT_ULONG_BE(ulbuf[35], output, 0);
            PUT_ULONG_BE(ulbuf[34], output, 4);
            PUT_ULONG_BE(ulbuf[33], output, 8);
            PUT_ULONG_BE(ulbuf[32], output, 12);
        }

        private byte[] padding(byte[] input, int mode)
        {
            if (input == null)
            {
                return null;
            }

            byte[] ret = (byte[])null;
            if (mode == SM4_ENCRYPT)
            {
                int p = 16 - input.Length % 16;
                ret = new byte[input.Length + p];
                Array.Copy(input, 0, ret, 0, input.Length);
                for (int i = 0; i < p; i++)
                {
                    ret[input.Length + i] = (byte)p;
                }
            }
            else
            {
                int p = input[input.Length - 1];
                ret = new byte[input.Length - p];
                Array.Copy(input, 0, ret, 0, input.Length - p);
            }
            return ret;
        }

        public void sm4_setkey_enc(SM4_Context ctx, byte[] key)
        {
            ctx.mode = SM4_ENCRYPT;
            sm4_setkey(ctx.sk, key);
        }

        public void sm4_setkey_dec(SM4_Context ctx, byte[] key)
        {
            int i = 0;
            ctx.mode = SM4_DECRYPT;
            sm4_setkey(ctx.sk, key);
            for (i = 0; i < 16; i++)
            {
                SWAP(ctx.sk, i);
            }
        }

        public byte[] sm4_crypt_ecb(SM4_Context ctx, byte[] input)
        {
            if ((ctx.isPadding) && (ctx.mode == SM4_ENCRYPT))
            {
                input = padding(input, SM4_ENCRYPT);
            }

            int length = input.Length;
            byte[] bins = new byte[length];
            Array.Copy(input, 0, bins, 0, length);
            byte[] bous = new byte[length];
            for (int i = 0; length > 0; length -= 16, i++)
            {
                byte[] inBytes = new byte[16];
                byte[] outBytes = new byte[16];
                Array.Copy(bins, i * 16, inBytes, 0, length > 16 ? 16 : length);
                sm4_one_round(ctx.sk, inBytes, outBytes);
                Array.Copy(outBytes, 0, bous, i * 16, length > 16 ? 16 : length);
            }

            if (ctx.isPadding && ctx.mode == SM4_DECRYPT)
            {
                bous = padding(bous, SM4_DECRYPT);
            }
            return bous;
        }

        public byte[] sm4_crypt_cbc(SM4_Context ctx, byte[] iv, byte[] input)
        {
            if (ctx.isPadding && ctx.mode == SM4_ENCRYPT)
            {
                input = padding(input, SM4_ENCRYPT);
            }

            int i = 0;
            int length = input.Length;
            byte[] bins = new byte[length];
            Array.Copy(input, 0, bins, 0, length);
            byte[] bous = null;
            List<byte> bousList = new List<byte>();
            if (ctx.mode == SM4_ENCRYPT)
            {
                for (int j = 0; length > 0; length -= 16, j++)
                {
                    byte[] inBytes = new byte[16];
                    byte[] outBytes = new byte[16];
                    byte[] out1 = new byte[16];

                    Array.Copy(bins, i * 16, inBytes, 0, length > 16 ? 16 : length);
                    for (i = 0; i < 16; i++)
                    {
                        outBytes[i] = ((byte)(inBytes[i] ^ iv[i]));
                    }
                    sm4_one_round(ctx.sk, outBytes, out1);
                    Array.Copy(out1, 0, iv, 0, 16);
                    for (int k = 0; k < 16; k++)
                    {
                        bousList.Add(out1[k]);
                    }
                }
            }
            else
            {
                byte[] temp = new byte[16];
                for (int j = 0; length > 0; length -= 16, j++)
                {
                    byte[] inBytes = new byte[16];
                    byte[] outBytes = new byte[16];
                    byte[] out1 = new byte[16];

                    Array.Copy(bins, i * 16, inBytes, 0, length > 16 ? 16 : length);
                    Array.Copy(inBytes, 0, temp, 0, 16);
                    sm4_one_round(ctx.sk, inBytes, outBytes);
                    for (i = 0; i < 16; i++)
                    {
                        out1[i] = ((byte)(outBytes[i] ^ iv[i]));
                    }
                    Array.Copy(temp, 0, iv, 0, 16);
                    for (int k = 0; k < 16; k++)
                    {
                        bousList.Add(out1[k]);
                    }
                }

            }

            if (ctx.isPadding && ctx.mode == SM4_DECRYPT)
            {
                bous = padding(bousList.ToArray(), SM4_DECRYPT);
                return bous;
            }
            else
            {
                return bousList.ToArray();
            }
        }
    }

    class SM4_Context
    {
        public int mode;

        public long[] sk;

        public bool isPadding;

        public SM4_Context()
        {
            this.mode = 1;
            this.isPadding = true;
            this.sk = new long[32];
        }
    }

    class SM4Utils
    {
        public String secretKey = "";
        public String iv = "";
        public bool hexString = false;
 
        public String Encrypt_ECB(String plainText)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_ENCRYPT;
 
            byte[] keyBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(secretKey);
            }
 
            SM4 sm4 = new SM4();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_ecb(ctx, Encoding.Default.GetBytes(plainText));
 
            String cipherText = Encoding.Default.GetString(Hex.Encode(encrypted));
            return cipherText;  
        }
 
        public String Decrypt_ECB(String cipherText)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_DECRYPT;
 
            byte[] keyBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(secretKey);
            }
 
            SM4 sm4 = new SM4();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] decrypted = sm4.sm4_crypt_ecb(ctx, Hex.Decode(cipherText));
            return Encoding.Default.GetString(decrypted);
        }
        public String Encrypt_CBC(String plainText)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_ENCRYPT;
 
            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
                ivBytes = Hex.Decode(iv);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(secretKey);
                ivBytes = Encoding.Default.GetBytes(iv);
            }
 
            SM4 sm4 = new SM4();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Encoding.Default.GetBytes(plainText));
 
            String cipherText = Encoding.Default.GetString(Hex.Encode(encrypted));
            return cipherText;
        }
 
        public String Decrypt_CBC(String cipherText)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_DECRYPT;
 
            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
                ivBytes = Hex.Decode(iv);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(secretKey);
                ivBytes = Encoding.Default.GetBytes(iv);
            }
 
            SM4 sm4 = new SM4();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] decrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Hex.Decode(cipherText));
            return Encoding.Default.GetString(decrypted);
        }
 
        //[STAThread]
        //public static void Main()
        //{
        //    String plainText = "ererfeiisgod";  
          
        //    SM4Utils sm4 = new SM4Utils();  
        //    sm4.secretKey = "JeF8U9wHFOMfs2Y8";  
        //    sm4.hexString = false;  
          
        //    System.Console.Out.WriteLine("ECB模式");  
        //    String cipherText = sm4.Encrypt_ECB(plainText);  
        //    System.Console.Out.WriteLine("密文: " + cipherText);  
        //    System.Console.Out.WriteLine("");  
          
        //    plainText = sm4.Decrypt_ECB(cipherText);  
        //    System.Console.Out.WriteLine("明文: " + plainText);  
        //    System.Console.Out.WriteLine("");  
          
        //    System.Console.Out.WriteLine("CBC模式");  
        //    sm4.iv = "UISwD9fW6cFh9SNS";  
        //    cipherText = sm4.Encrypt_CBC(plainText);  
        //    System.Console.Out.WriteLine("密文: " + cipherText);  
        //    System.Console.Out.WriteLine("");  
          
        //    plainText = sm4.Decrypt_CBC(cipherText);  
        //    System.Console.Out.WriteLine("明文: " + plainText);
 
        //    Console.ReadLine();
        //}
    }


    public abstract class GeneralDigest : IDigest
    {
        private const int BYTE_LENGTH = 64;

        private byte[] xBuf;
        private int xBufOff;

        private long byteCount;

        internal GeneralDigest()
        {
            xBuf = new byte[4];
        }

        internal GeneralDigest(GeneralDigest t)
        {
            xBuf = new byte[t.xBuf.Length];
            Array.Copy(t.xBuf, 0, xBuf, 0, t.xBuf.Length);

            xBufOff = t.xBufOff;
            byteCount = t.byteCount;
        }

        public void Update(byte input)
        {
            xBuf[xBufOff++] = input;

            if (xBufOff == xBuf.Length)
            {
                ProcessWord(xBuf, 0);
                xBufOff = 0;
            }

            byteCount++;
        }

        public void BlockUpdate(
            byte[] input,
            int inOff,
            int length)
        {
            //
            // fill the current word
            //
            while ((xBufOff != 0) && (length > 0))
            {
                Update(input[inOff]);
                inOff++;
                length--;
            }

            //
            // process whole words.
            //
            while (length > xBuf.Length)
            {
                ProcessWord(input, inOff);

                inOff += xBuf.Length;
                length -= xBuf.Length;
                byteCount += xBuf.Length;
            }

            //
            // load in the remainder.
            //
            while (length > 0)
            {
                Update(input[inOff]);

                inOff++;
                length--;
            }
        }

        public void Finish()
        {
            long bitLength = (byteCount << 3);

            //
            // add the pad bytes.
            //
            Update(unchecked((byte)128));

            while (xBufOff != 0) Update(unchecked((byte)0));
            ProcessLength(bitLength);
            ProcessBlock();
        }

        public virtual void Reset()
        {
            byteCount = 0;
            xBufOff = 0;
            Array.Clear(xBuf, 0, xBuf.Length);
        }

        public int GetByteLength()
        {
            return BYTE_LENGTH;
        }

        internal abstract void ProcessWord(byte[] input, int inOff);
        internal abstract void ProcessLength(long bitLength);
        internal abstract void ProcessBlock();
        public abstract string AlgorithmName { get; }
        public abstract int GetDigestSize();
        public abstract int DoFinal(byte[] output, int outOff);
    }

    public class SupportClass
    {
        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static int URShift(int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            else
                return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static int URShift(int number, long bits)
        {
            return URShift(number, (int)bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static long URShift(long number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            else
                return (number >> bits) + (2L << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static long URShift(long number, long bits)
        {
            return URShift(number, (int)bits);
        }


    }

    public class SM3Digest : GeneralDigest
    {
        public override string AlgorithmName
        {
            get
            {
                return "SM3";
            }

        }
        public override int GetDigestSize()
        {
            return DIGEST_LENGTH;
        }

        private const int DIGEST_LENGTH = 32;

        private static readonly int[] v0 = new int[] { 0x7380166f, 0x4914b2b9, 0x172442d7, unchecked((int)0xda8a0600), unchecked((int)0xa96f30bc), 0x163138aa, unchecked((int)0xe38dee4d), unchecked((int)0xb0fb0e4e) };

        private int[] v = new int[8];
        private int[] v_ = new int[8];

        private static readonly int[] X0 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private int[] X = new int[68];
        private int xOff;

        private int T_00_15 = 0x79cc4519;
        private int T_16_63 = 0x7a879d8a;

        public SM3Digest()
        {
            Reset();
        }

        public SM3Digest(SM3Digest t) : base(t)
        {

            Array.Copy(t.X, 0, X, 0, t.X.Length);
            xOff = t.xOff;

            Array.Copy(t.v, 0, v, 0, t.v.Length);
        }

        public override void Reset()
        {
            base.Reset();

            Array.Copy(v0, 0, v, 0, v0.Length);

            xOff = 0;
            Array.Copy(X0, 0, X, 0, X0.Length);
        }

        internal override void ProcessBlock()
        {
            int i;

            int[] ww = X;
            int[] ww_ = new int[64];

            for (i = 16; i < 68; i++)
            {
                ww[i] = P1(ww[i - 16] ^ ww[i - 9] ^ (ROTATE(ww[i - 3], 15))) ^ (ROTATE(ww[i - 13], 7)) ^ ww[i - 6];
            }

            for (i = 0; i < 64; i++)
            {
                ww_[i] = ww[i] ^ ww[i + 4];
            }

            int[] vv = v;
            int[] vv_ = v_;

            Array.Copy(vv, 0, vv_, 0, v0.Length);

            int SS1, SS2, TT1, TT2, aaa;
            for (i = 0; i < 16; i++)
            {
                aaa = ROTATE(vv_[0], 12);
                SS1 = aaa + vv_[4] + ROTATE(T_00_15, i);
                SS1 = ROTATE(SS1, 7);
                SS2 = SS1 ^ aaa;

                TT1 = FF_00_15(vv_[0], vv_[1], vv_[2]) + vv_[3] + SS2 + ww_[i];
                TT2 = GG_00_15(vv_[4], vv_[5], vv_[6]) + vv_[7] + SS1 + ww[i];
                vv_[3] = vv_[2];
                vv_[2] = ROTATE(vv_[1], 9);
                vv_[1] = vv_[0];
                vv_[0] = TT1;
                vv_[7] = vv_[6];
                vv_[6] = ROTATE(vv_[5], 19);
                vv_[5] = vv_[4];
                vv_[4] = P0(TT2);
            }
            for (i = 16; i < 64; i++)
            {
                aaa = ROTATE(vv_[0], 12);
                SS1 = aaa + vv_[4] + ROTATE(T_16_63, i);
                SS1 = ROTATE(SS1, 7);
                SS2 = SS1 ^ aaa;

                TT1 = FF_16_63(vv_[0], vv_[1], vv_[2]) + vv_[3] + SS2 + ww_[i];
                TT2 = GG_16_63(vv_[4], vv_[5], vv_[6]) + vv_[7] + SS1 + ww[i];
                vv_[3] = vv_[2];
                vv_[2] = ROTATE(vv_[1], 9);
                vv_[1] = vv_[0];
                vv_[0] = TT1;
                vv_[7] = vv_[6];
                vv_[6] = ROTATE(vv_[5], 19);
                vv_[5] = vv_[4];
                vv_[4] = P0(TT2);
            }
            for (i = 0; i < 8; i++)
            {
                vv[i] ^= vv_[i];
            }

            // Reset
            xOff = 0;
            Array.Copy(X0, 0, X, 0, X0.Length);
        }

        internal override void ProcessWord(byte[] in_Renamed, int inOff)
        {
            int n = in_Renamed[inOff] << 24;
            n |= (in_Renamed[++inOff] & 0xff) << 16;
            n |= (in_Renamed[++inOff] & 0xff) << 8;
            n |= (in_Renamed[++inOff] & 0xff);
            X[xOff] = n;

            if (++xOff == 16)
            {
                ProcessBlock();
            }
        }

        internal override void ProcessLength(long bitLength)
        {
            if (xOff > 14)
            {
                ProcessBlock();
            }

            X[14] = (int)(SupportClass.URShift(bitLength, 32));
            X[15] = (int)(bitLength & unchecked((int)0xffffffff));
        }

        public static void IntToBigEndian(int n, byte[] bs, int off)
        {
            bs[off] = (byte)(SupportClass.URShift(n, 24));
            bs[++off] = (byte)(SupportClass.URShift(n, 16));
            bs[++off] = (byte)(SupportClass.URShift(n, 8));
            bs[++off] = (byte)(n);
        }

        public override int DoFinal(byte[] out_Renamed, int outOff)
        {
            Finish();

            for (int i = 0; i < 8; i++)
            {
                IntToBigEndian(v[i], out_Renamed, outOff + i * 4);
            }

            Reset();

            return DIGEST_LENGTH;
        }

        private int ROTATE(int x, int n)
        {
            return (x << n) | (SupportClass.URShift(x, (32 - n)));
        }

        private int P0(int X)
        {
            return ((X) ^ ROTATE((X), 9) ^ ROTATE((X), 17));
        }

        private int P1(int X)
        {
            return ((X) ^ ROTATE((X), 15) ^ ROTATE((X), 23));
        }

        private int FF_00_15(int X, int Y, int Z)
        {
            return (X ^ Y ^ Z);
        }

        private int FF_16_63(int X, int Y, int Z)
        {
            return ((X & Y) | (X & Z) | (Y & Z));
        }

        private int GG_00_15(int X, int Y, int Z)
        {
            return (X ^ Y ^ Z);
        }

        private int GG_16_63(int X, int Y, int Z)
        {
            return ((X & Y) | (~X & Z));
        }

        //[STAThread]
        //public static void  Main()
        //{
        //    byte[] md = new byte[32];
        //    byte[] msg1 = Encoding.Default.GetBytes("ererfeiisgod");
        //    SM3Digest sm3 = new SM3Digest();
        //    sm3.BlockUpdate(msg1, 0, msg1.Length);
        //    sm3.DoFinal(md, 0);
        //    System.String s = new UTF8Encoding().GetString(Hex.Encode(md));
        //    System.Console.Out.WriteLine(s.ToUpper());

        //    Console.ReadLine();
        //}
    }

    public class Cipher
    {
        private int ct = 1;
        private ECPoint p2;
        private SM3Digest sm3keybase;
        private SM3Digest sm3c3;


        private byte[] key = new byte[32];
        private byte keyOff = 0;


        public Cipher()
        {
        }


        private void Reset()
        {
            sm3keybase = new SM3Digest();
            sm3c3 = new SM3Digest();


            byte[] p;


            p = p2.X.ToBigInteger().ToByteArray();
            sm3keybase.BlockUpdate(p, 0, p.Length);
            sm3c3.BlockUpdate(p, 0, p.Length);


            p = p2.Y.ToBigInteger().ToByteArray();
            sm3keybase.BlockUpdate(p, 0, p.Length);


            ct = 1;
            NextKey();
        }


        private void NextKey()
        {
            SM3Digest sm3keycur = new SM3Digest(sm3keybase);
            sm3keycur.Update((byte)(ct >> 24 & 0x00ff));
            sm3keycur.Update((byte)(ct >> 16 & 0x00ff));
            sm3keycur.Update((byte)(ct >> 8 & 0x00ff));
            sm3keycur.Update((byte)(ct & 0x00ff));
            sm3keycur.DoFinal(key, 0);
            keyOff = 0;
            ct++;
        }


        public virtual ECPoint Init_enc(SM2 sm2, ECPoint userKey)
        {
            BigInteger k = null;
            ECPoint c1 = null;


            AsymmetricCipherKeyPair key = sm2.ecc_key_pair_generator.GenerateKeyPair();
            ECPrivateKeyParameters ecpriv = (ECPrivateKeyParameters)key.Private;
            ECPublicKeyParameters ecpub = (ECPublicKeyParameters)key.Public;
            k = ecpriv.D;
            c1 = ecpub.Q;


            p2 = userKey.Multiply(k);
            Reset();


            return c1;
        }


        public virtual void Encrypt(byte[] data)
        {
            sm3c3.BlockUpdate(data, 0, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                if (keyOff == key.Length)
                    NextKey();


                data[i] ^= key[keyOff++];
            }
        }


        public virtual void Init_dec(BigInteger userD, ECPoint c1)
        {
            p2 = c1.Multiply(userD);
            Reset();
        }


        public virtual void Decrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (keyOff == key.Length)
                    NextKey();


                data[i] ^= key[keyOff++];
            }
            sm3c3.BlockUpdate(data, 0, data.Length);
        }


        public virtual void Dofinal(byte[] c3)
        {
            byte[] p = p2.Y.ToBigInteger().ToByteArray();
            sm3c3.BlockUpdate(p, 0, p.Length);
            sm3c3.DoFinal(c3, 0);
            Reset();
        }
    }

}
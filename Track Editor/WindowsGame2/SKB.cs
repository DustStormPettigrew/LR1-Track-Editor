namespace WindowsGame2
{
    using LibLR1.Exceptions;
    using LibLR1.IO;
    using LibLR1.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SKB
    {
        private const byte ID_GRADIENTBLOCK = 0x27;
        private const byte ID_GRADIENTS = 0x2c;
        private const byte PROPERTY_DEFAULTGRADIENT = 0x2d;
        private const byte PROPERTY_UNKNOWNFLOAT = 0x2e;
        public Dictionary<string, SKB_Gradient> Gradients = new Dictionary<string, SKB_Gradient>();
        public string Default;
        public float? Unknownfloat;

        public SKB(string path)
        {
            LRBinaryReader objA = BinaryFileHelper.Decompress(path);
            try
            {
                while (true)
                {
                    if (objA.BaseStream.Position >= objA.BaseStream.Length)
                    {
                        break;
                    }
                    byte num = objA.ReadByte();
                    byte num2 = num;
                    switch (num2)
                    {
                        case 0x2c:
                        {
                            this.Gradients = objA.ReadArrayBlock<KeyValuePair<string, SKB_Gradient>>(new Func<LRBinaryReader, KeyValuePair<string, SKB_Gradient>>(SKB.ReadGradientblock)).ToDictionary<KeyValuePair<string, SKB_Gradient>, string, SKB_Gradient>(x => x.Key, x => x.Value);
                            continue;
                        }
                        case 0x2d:
                        {
                            this.Default = objA.ReadStringWithHeader();
                            continue;
                        }
                        case 0x2e:
                        {
                            this.Unknownfloat = new float?(objA.ReadFloatWithHeader());
                            continue;
                        }
                    }
                    throw new UnexpectedBlockException(num, objA.BaseStream.Position - 1L);
                }
            }
            finally
            {
                if (!ReferenceEquals(objA, null))
                {
                    objA.Dispose();
                }
            }
        }

        private static KeyValuePair<string, SKB_Gradient> ReadGradientblock(LRBinaryReader r)
        {
            r.Expect((byte) 0x27);
            r.Expect(Token.LeftBracket);
            r.ReadIntWithHeader();
            r.Expect(Token.RightBracket);
            string key = r.ReadStringWithHeader();
            r.Expect(Token.LeftCurly);
            r.Expect((byte) 0x27);
            r.Expect(Token.RightCurly);
            return new KeyValuePair<string, SKB_Gradient>(key, r.ReadStruct<SKB_Gradient>(new Func<LRBinaryReader, SKB_Gradient>(SKB_Gradient.FromStream)));
        }

        public void Save(LRBinaryWriter writer)
        {
            if ((this.Gradients != null) && (this.Gradients.Count != 0))
            {
                writer.WriteByte(0x2c);
                writer.WriteArrayBlock<KeyValuePair<string, SKB_Gradient>>(new Action<LRBinaryWriter, KeyValuePair<string, SKB_Gradient>>(SKB.WriteGradientBlock), this.Gradients.ToArray<KeyValuePair<string, SKB_Gradient>>());
            }
            writer.WriteByte(0x2d);
            writer.WriteStringWithHeader(this.Default);
            if (this.Unknownfloat != null)
            {
                writer.WriteByte(0x2e);
                writer.WriteFloatWithHeader(this.Unknownfloat.Value);
            }
        }

        public void Save(string path)
        {
            LRBinaryWriter writer = new LRBinaryWriter(File.OpenWrite(path), true);
            try
            {
                this.Save(writer);
            }
            finally
            {
                if (!ReferenceEquals(writer, null))
                {
                    writer.Dispose();
                }
            }
        }

        private static void WriteGradientBlock(LRBinaryWriter w, KeyValuePair<string, SKB_Gradient> gradient)
        {
            w.WriteByte(0x27);
            w.WriteToken(Token.LeftBracket);
            w.WriteIntWithHeader(1);
            w.WriteToken(Token.RightBracket);
            w.WriteStringWithHeader(gradient.Key);
            w.WriteToken(Token.LeftCurly);
            w.WriteByte(0x27);
            w.WriteStruct<SKB_Gradient>(new Action<LRBinaryWriter, SKB_Gradient>(SKB_Gradient.Write), gradient.Value);
            w.WriteToken(Token.RightCurly);
        }
    }
}


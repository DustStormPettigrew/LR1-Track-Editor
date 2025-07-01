namespace WindowsGame2
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class Material
    {
        public Texture2D texture;
        public Color diffusecolor = Color.White;
        public Color ambientcolor = Color.White;
        public byte alpha = 0xff;
        public bool semitransparent = false;
    }
}


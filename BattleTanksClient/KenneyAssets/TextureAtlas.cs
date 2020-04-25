using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BattleTanksCommon.KenneyAssets
{
    [XmlRoot("TextureAtlas")]
    public class TextureAtlasData
    {
        [XmlAttribute(AttributeName = "imagePath")]
        public string ImagePath { get; set; }
        [XmlElement("SubTexture")]
        public SubTexture[] SubTextures { get; set; }

        public static TextureAtlas CreateFromFile(ContentManager content, string filepath)
        {
            if (!File.Exists(filepath))
                throw new ArgumentException("Invalid filepath provided!");

            using (var file = new StreamReader(filepath))
            {
                var reader = new XmlSerializer(typeof(TextureAtlasData));
                var textureAtlasData = (TextureAtlasData)reader.Deserialize(file);

                var sheetPath = textureAtlasData.ImagePath.Replace(".png", "");
                var sheet = content.Load<Texture2D>(sheetPath);
                var atlas = new TextureAtlas(sheetPath, sheet);

                foreach (var subTexture in textureAtlasData.SubTextures)
                    atlas.CreateRegion(subTexture.Name.Replace(".png", ""), subTexture.X, subTexture.Y, subTexture.Width, subTexture.Height);

                return atlas;
            }
        }
    }

    public class SubTexture
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }
        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }
        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }
        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }
    }
}

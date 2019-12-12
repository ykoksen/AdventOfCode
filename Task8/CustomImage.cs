using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Task8
{
    public class CustomImage
    {
        public CustomImage(int height, int width)
        {
            Width = width;
            Height = height;
            Layers = new List<ImageLayer>();
        }

        public int Width { get; }
        public int Height { get; }

        public List<ImageLayer> Layers { get; }

        public void Parse(List<int> pixels)
        {
            for (int i = 0; i < pixels.Count/(Width*Height); i++)
            {
                var imageLayer = new ImageLayer(this);
                imageLayer.InsertLayer(pixels.GetRange(i*Width*Height, Width*Height));
                Layers.Add(imageLayer);
            }
        }

        public ImageLayer FinalLayer()
        {
            var result = new ImageLayer(this);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    foreach (var layer in Layers)
                    {
                        var color = layer.Lines[y][x];
                        result.Lines[y][x] = color;
                        if (color != 2)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }

    public class ImageLayer
    {
        public CustomImage Owner { get; }

        public int[][] Lines { get; }

        public ImageLayer(CustomImage owner)
        {
            Owner = owner;
            Lines = new int[owner.Height][];
            for (var index = 0; index < Lines.Length; index++)
            {
                Lines[index] = new int[owner.Width];
            }
        }

        public void InsertLayer(IEnumerable<int> imageLayer)
        {
            int counter = 0;

            foreach (var pixel in imageLayer)
            {
                Lines[counter / Owner.Width][counter % Owner.Width] = pixel;
                counter++;
            }
        }

        public Color[][] ToColorArray()
        {
            Color[][] back = new Color[Lines.Length][];
            for (var index = 0; index < Owner.Height; index++)
            {
                back[index] = new Color[Owner.Width];
            }

            for (int y = 0; y < Owner.Height; y++)
            {
                for (int x = 0; x < Owner.Width; x++)
                {
                    back[y][x] = GetColor(Lines[y][x]);
                }
            }

            return back;
        }

        private Color GetColor(int i)
        {
            return i switch
            {
                0 => Color.Black,
                1 => Color.White,
                2 => Color.Transparent,
                _ => throw new ArgumentOutOfRangeException(nameof(i))
            };
        }

        public Bitmap GetPicture()
        {
            Bitmap map = new Bitmap(Owner.Width, Owner.Height);

            var colorArray = ToColorArray();

            for (int y = 0; y < Owner.Height; y++)
            {
                for (int x = 0; x < Owner.Width; x++)
                {
                    map.SetPixel(x, y, colorArray[y][x]);
                }
            }

            return map;
        }
    }
}

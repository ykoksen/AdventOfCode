using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Task8
{
    public class Image
    {
        public Image(int height, int width)
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
    }

    public class ImageLayer
    {
        public Image Owner { get; }

        public int[][] Lines { get; }

        public ImageLayer(Image owner)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OCRProject.ImageProcessing
{
    public class ShiftImage
    {
        public Bitmap Apply(Bitmap image, int shiftX, int shiftY)
        {
            Bitmap shiftedBitmap = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(shiftedBitmap))
            {
                g.Clear(Color.White);
                g.DrawImage(image, new Rectangle(shiftX, shiftY, image.Width, image.Height));
            }
            return shiftedBitmap;
        }
    }
}

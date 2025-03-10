using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace OCRProject.TesseractProcessor
{
    public class ImageDisplayer
    {
        public static void ShowImage(Bitmap image, string title)
        {
            using (var form = new Form())
            {
                form.Text = title;
                form.ClientSize = new Size(image.Width, image.Height);

                var pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = image,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                form.Controls.Add(pictureBox);
                form.Load += async (sender, e) =>
                {
                    await Task.Delay(3000); // Wait for 3 seconds
                    form.Close(); // Close the form automatically
                };

                Application.Run(form);
            }
        }
    }
}

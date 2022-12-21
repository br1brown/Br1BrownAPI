using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Br1BrownAPI.Img
{
    public static class ManageImage
    {
        public static System.Drawing.Image AddMargins(System.Drawing.Image image, float topMargin, float bottomMargin, Color Sfondo)
        {

            int topMarginSize = Convert.ToInt32(topMargin);
            int bottomMarginSize = Convert.ToInt32(bottomMargin);

            // Crea una nuova immagine con i margini aggiunti
            Bitmap newImage = new Bitmap(image.Width, image.Height + topMarginSize + bottomMarginSize);

            // Disegna l'immagine originale nella nuova immagine, ignorando i margini superiori e inferiori
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, topMarginSize, image.Width, image.Height);
            }

            // Disegna il margine superiore nero nella nuova immagine
            for (int y = 0; y < topMarginSize; y++)
            {
                for (int x = 0; x < newImage.Width; x++)
                {
                    newImage.SetPixel(x, y, Sfondo);
                }
            }

            // Disegna il margine inferiore nero nella nuova immagine
            for (int y = newImage.Height - bottomMarginSize; y < newImage.Height; y++)
            {
                for (int x = 0; x < newImage.Width; x++)
                {
                    newImage.SetPixel(x, y, Sfondo);
                }
            }

            return newImage;
        }
    }
}

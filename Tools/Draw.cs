using System.Drawing;
using System.Drawing.Text;

namespace InazumaElevenSaveEditor.Tools
{
    public class Draw
    {
        public static Image DrawString(Image image, string text, int x, int y)
        {
            Bitmap bmp = new Bitmap(image);

            Graphics graphics = Graphics.FromImage(bmp);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.DrawString(text, new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.Black), new Point(x, y));

            return bmp;
        }

        public static Bitmap DrawImage(Image image, int x, int y, Image imagePath)
        {
            Bitmap bmp = new Bitmap(image);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(imagePath, new Point(x, y));
            }
            return bmp;
        }
    }
}

/*
 *
 *  Método   : Printer Engine
 *             Suporte de Imagens
 * 
 *  por      : Renato Igleziaz
 * 
 *  em       : 10/out de 2012
 * 
 */

using System;
using System.Drawing;

namespace PrintingCore
{
    public class PrintPrimitiveImage : IPrintPrimitive
    {
        private Image i;
        private string align_ = "left";

        public PrintPrimitiveImage(string bmpPath, string align = "left")
        {
            i = Image.FromFile(bmpPath);
            align_ = align;
        }

        public float CalculateHeight(PrintEngine engine, Graphics graphics)
        {
            return (i.Height / i.VerticalResolution) * 100;
        }

        public void Draw(PrintEngine engine, float yPos, Graphics graphics, Rectangle elementBounds)
        {
            int pos = 0;
            int tamanhoimagem = ((i.Width / (int)i.HorizontalResolution) * 100);

            if (align_ == "left")
                pos = elementBounds.Left;
            else if (align_ == "center")
                pos = (elementBounds.Width / 2) - (tamanhoimagem / 2);
            else if (align_ == "right")
            {
                pos = elementBounds.Width - tamanhoimagem;
                pos -= 30;
            }

            graphics.DrawImage(i, pos, elementBounds.Y);
        }
    }
}

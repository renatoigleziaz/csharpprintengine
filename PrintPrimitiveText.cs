/*
 *
 *  Método   : Printer Engine
 *             Impressão de Texto, colunas
 * 
 *  por      : Renato Igleziaz
 * 
 *  em       : 10/out de 2012
 * 
 */

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace PrintingCore
{
    public class PrintPrimitiveText : IPrintPrimitive 
    {
        public Row row;
        public Font Fonte;
        private string type = "";
        private bool cuttext = false;

        public PrintPrimitiveText(Row RowPrint) 
        {
            row = RowPrint;

            string fontname = RowPrint.Item(0, "font");
            int size = int.Parse(RowPrint.Item(0, "size"));
            int bold = int.Parse(RowPrint.Item(0, "bold"));
            type = RowPrint.Item(0, "type");

            if (bold == 0)
                Fonte = new Font(fontname, size); 
            else
                Fonte = new Font(fontname, size, FontStyle.Bold);

            if (RowPrint.Item(0, "cuttext") == "True")
                cuttext = true;
        } 

        public float CalculateHeight(PrintEngine engine, Graphics graphics) 
        { 
            // retorna o height correto baseado na fonte

            // inicialmente era usando somente
            // return Fonte.GetHeight(graphics); 
            // mas como estamos usando impressão
            // baseada colunas, temos que ler
            // todas e verificar qual é a fonte
            // que tem maior altura para determinar
            // a altura da linha.

            // 19-07
            // nova correção que calcula se no espaço 
            // de impressão vai caber o texto.
            // caso não caiba, calcula o espaço
            // necessário e alerta a engine 
            // a nova altura necessária para impressão.

            float mHeight = 0;
            float cHeight = 0;
            Font mFont;
            string fontname = "";
            int size = 0;
            int bold = 0;
            int percent = 0;
            float width = 0;
            float calcWidth = 0;
            string text = "";

            for (int x = 0; x < row.lst.Items.Count; x++)
            {
                fontname = row.Item(x, "font");
                size = int.Parse(row.Item(x, "size"));
                bold = int.Parse(row.Item(x, "bold"));

                if (bold == 0)
                    mFont = new Font(fontname, size);
                else
                    mFont = new Font(fontname, size, FontStyle.Bold);

                cHeight = mFont.GetHeight(graphics);
                percent = int.Parse(row.Item(x, "percent"));
                width = (engine.MarginBoundsWidth * percent) / 100;
                text = row.Item(x, "text");
                calcWidth = graphics.MeasureString(text, mFont).Width;

                // so entra aq se tiver a medida de comprimento da pagina
                if (engine.MarginBoundsWidth > 0)
                {
                    if (calcWidth > width)
                    {
                        // neste caso o texto não cabe dentro do espaço destinado
                        // vamos calcular a altura que deve ocupar
                        cHeight = cHeight * ((calcWidth / width) + 1);
                    }
                }

                if (mHeight == 0)
                {
                    // primeira vez
                    mHeight = cHeight;
                }
                else
                {
                    // se a altura da fonte atual for maior que a anterior
                    if (cHeight > mHeight)
                        mHeight = cHeight;
                }
            }

            mFont = null;            

            // retorna a altura da maior fonte na linha
            // +3 ajuda a dar uma distância melhor
            // entre as linhas de texto no caso de uma
            // impressão com codigo de barras.
            return mHeight;
        }

        public float CalculateWidth(string input, Graphics graphics, Font font)
        {
            // retorna o comprimento do gráfico
            input = input.Replace(" ", "X");
            return graphics.MeasureString(input, font).Width;
        }

        private string TextCheckSize(Graphics graphics, Font font, string text, int maxwidth)
        {
            if (!cuttext)
            {
                return text;
            }
            else
            {
                int textwidth = (int)CalculateWidth(text, graphics, font); 

                if (textwidth > maxwidth)
                {
                    for (; ; )
                    {
                        textwidth = (int)CalculateWidth(text, graphics, font); 

                        if (textwidth <= maxwidth)
                            break;

                        if (text.Length > 1)
                        {
                            try
                            {
                                text = text.Substring(0, text.Length - 1);
                            }
                            catch
                            {
                                return text;
                            }
                        }
                        else
                            break;
                    }

                    return text;
                }
                else
                {
                    return text;
                }
            }
        }

        public void Draw(PrintEngine engine, float yPos, Graphics graphics, Rectangle elementBounds)
        {
            // vars
            StringFormat drawFormat;
            Brush forecolor;
            Brush caneta;
            int r = 0;
            int g = 0;
            int b = 0;
            int bk_r = 0;
            int bk_g = 0;
            int bk_b = 0;
            Color rgb;
            Rectangle rect = elementBounds;
            rect.Y = (int)yPos;
            Rectangle background = elementBounds;
            Rectangle rectTemp = elementBounds;
            int percent = 0;
            int lastWidth = 0;
            string fontname = "";
            int size = 0;
            int bold = 0;
            string imagepathfile = "";
            Image i;
            
            if (row.lst.Items.Count == 1)
            {
                // string format
                drawFormat = new StringFormat();
                drawFormat.FormatFlags = StringFormatFlags.NoClip;
                drawFormat.LineAlignment = StringAlignment.Near;

                if (row.Item(0, "align") == "left")
                    drawFormat.Alignment = StringAlignment.Near;
                else if (row.Item(0, "align") == "center")
                    drawFormat.Alignment = StringAlignment.Center;
                else
                    drawFormat.Alignment = StringAlignment.Far;

                r = int.Parse(row.Item(0, "colorr"));
                g = int.Parse(row.Item(0, "colorg"));
                b = int.Parse(row.Item(0, "colorb"));
                rgb = Color.FromArgb(r, g, b);
                forecolor = new SolidBrush(rgb);

                fontname = row.Item(0, "font");
                size = int.Parse(row.Item(0, "size"));
                bold = int.Parse(row.Item(0, "bold"));

                if (bold == 0)
                    Fonte = new Font(fontname, size);
                else
                    Fonte = new Font(fontname, size, FontStyle.Bold);

                if (type == "italic" && bold == 0)
                    Fonte = new Font(fontname, size, FontStyle.Italic);
                else if (type == "italic" && bold == 1)
                    Fonte = new Font(fontname, size, FontStyle.Italic | FontStyle.Bold);

                if (type == "normal:gradientgraywhitecolor")
                {
                    caneta = new LinearGradientBrush(new Point(0, 10),
                                                     new Point(rectTemp.Width + 22, 10),
                                                     Color.Gray,    
                                                     Color.White);

                    rectTemp.Height = (int)Fonte.GetHeight();
                    graphics.FillRectangle(caneta, rectTemp);

                    // texto
                    graphics.DrawString(TextCheckSize(graphics, Fonte, engine.ReplaceTokens(row.Item(0, "text")), rect.Width),
                                        Fonte,
                                        forecolor,
                                        rect,
                                        drawFormat);
                }
                else
                {
                    // desenha um fundo com cor
                    if (row.backgroundcolor)
                    {
                        bk_r = row.bk_r;
                        bk_g = row.bk_g;
                        bk_b = row.bk_b;

                        caneta = new SolidBrush(Color.FromArgb(bk_r, bk_g, bk_b));
                        graphics.FillRectangle(caneta, elementBounds);
                    }

                    // texto
                    graphics.DrawString(TextCheckSize(graphics, Fonte, engine.ReplaceTokens(row.Item(0, "text")), rect.Width),
                                        Fonte,
                                        forecolor,
                                        rect,
                                        drawFormat);
                }
            }
            else
            {
                // várias colunas

                for (int x = 0; x < row.lst.Items.Count; x++)
                {
                    // calcula o rect
                    percent = int.Parse(row.Item(x, "percent"));
                    rect = elementBounds;
                    if (x != 0)
                        rect.X = lastWidth + 22;
                    rect.Y = (int)yPos;
                    rect.Width = (rect.Width * percent) / 100;
                    lastWidth += rect.Width;

                    // configura
                    drawFormat = new StringFormat();
                    drawFormat.FormatFlags = StringFormatFlags.NoClip;
                    drawFormat.LineAlignment = StringAlignment.Near;

                    if (row.Item(x, "align") == "left")
                        drawFormat.Alignment = StringAlignment.Near;
                    else if (row.Item(x, "align") == "center")
                        drawFormat.Alignment = StringAlignment.Center;
                    else
                        drawFormat.Alignment = StringAlignment.Far;

                    r = int.Parse(row.Item(x, "colorr"));
                    g = int.Parse(row.Item(x, "colorg"));
                    b = int.Parse(row.Item(x, "colorb"));
                    rgb = Color.FromArgb(r, g, b);
                    forecolor = new SolidBrush(rgb);

                    fontname = row.Item(x, "font");
                    size = int.Parse(row.Item(x, "size"));
                    bold = int.Parse(row.Item(x, "bold"));

                    if (bold == 0)
                        Fonte = new Font(fontname, size);
                    else
                        Fonte = new Font(fontname, size, FontStyle.Bold);

                    if (row.Item(x, "type") == "italic" && bold == 0)
                        Fonte = new Font(fontname, size, FontStyle.Italic);
                    else if (row.Item(x, "type") == "italic" && bold == 1)
                        Fonte = new Font(fontname, size, FontStyle.Italic | FontStyle.Bold);

                    if (row.backgroundcolor)
                    {
                        bk_r = row.bk_r;
                        bk_g = row.bk_g;
                        bk_b = row.bk_b;
                        background = rect;
                        background.Height = (int)Fonte.GetHeight();

                        if (x != 0)
                            background.Width -= 2;

                        caneta = new SolidBrush(Color.FromArgb(bk_r, bk_g, bk_b));
                        graphics.FillRectangle(caneta, background);
                    }

                    if (row.Item(x, "type") == "normal:backgroundgray")
                    {
                        background = rect;
                        background.Height = (int)Fonte.GetHeight();

                        if (x != 0)
                            background.Width -= 2;

                        caneta = new SolidBrush(Color.FromArgb(230, 230, 242));
                        graphics.FillRectangle(caneta, background);
                    }

                    if (row.Item(x, "type").IndexOf("image=") != -1)
                    {
                        // imprimir uma imagem
                        imagepathfile = row.Item(x, "type").Substring(6);
                        i = Image.FromFile(imagepathfile);
                        graphics.DrawImage(i, rect.X, rect.Y + 15);
                    }
                    else
                    {
                        if (row.Item(x, "cuttext") == "True")
                            cuttext = true;

                        // imprime
                        graphics.DrawString(TextCheckSize(graphics, Fonte, engine.ReplaceTokens(row.Item(x, "text")), rect.Width),
                                            Fonte,
                                            forecolor,
                                            rect,
                                            drawFormat);

                        cuttext = false;
                    }
                }
            }

            if (row.circle)
            {
                caneta = new SolidBrush(Color.FromArgb(118, 78, 173));
                graphics.FillEllipse(caneta, new Rectangle(elementBounds.X + 2, (int)yPos + 2, 10, (int)Fonte.GetHeight() - 4));
            }

            drawFormat = null;
            forecolor = null;
            caneta = null;
        }
    }
}

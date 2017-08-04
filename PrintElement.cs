/*
 *
 *  Método   : Printer Engine
 *             Gerencia Elementos de Impressão da Engine
 * 
 *  por      : Renato Igleziaz
 * 
 *  em       : 10/out de 2012
 * 
 */

using System; 
using System.Collections; 
using System.Drawing; 
using System.Drawing.Printing;

namespace PrintingCore
{
    public class PrintElement
    {
        // members... 
        private ArrayList _printPrimitives = new ArrayList(); 
        private IPrintable _printObject;
        private bool _newpage = false;

        public PrintElement(IPrintable printObject) 
        { 
          _printObject = printObject; 
        }

        public void AddImage(string bmpPath, string align = "left")
        {
            AddPrimitive(new PrintPrimitiveImage(bmpPath, align));
        }

        public void AddText(Row rowInput) 
        {  
            AddPrimitive(new PrintPrimitiveText(rowInput)); 
        } 

        // AddPrimitive - add a primitive to the list... 
        public void AddPrimitive(IPrintPrimitive primitive) 
        { 
            _printPrimitives.Add(primitive); 
        } 

        public void AddHorizontalRule()
        {
            AddPrimitive(new PrintPrimitiveRule());
        } 

        public void AddBlankLine() 
        {
            Row rowBlanckLine = new Row();
            rowBlanckLine.AddCol(100, "blankline", "left", "Calibri", 10, false, " ", 0, 0, 0);
            AddText(rowBlanckLine); 
        } 

        public void AddHeader(Row buf) 
        {
            AddText(buf); 
            AddHorizontalRule(); 
        }

        public void NewPage()
        {
            AddHorizontalRule();
            _newpage = true;
        }

        public bool isNewPage()
        {
            return _newpage;
        }

        public float CalculateHeight(PrintEngine engine, Graphics graphics) 
        { 
            // loop through the print height... 
            float height = 0; 

            foreach(IPrintPrimitive primitive in _printPrimitives) 
            { 
                // get the height... 
                height += primitive.CalculateHeight(engine, graphics); 
            } 

            // return the height... 
            return height; 
        } 

        // Draw - draw the element on a graphics object... 
        public void Draw(PrintEngine engine, float yPos, Graphics graphics, Rectangle pageBounds) 
        { 
            // where... 
            float height = CalculateHeight(engine, graphics); 
            Rectangle elementBounds = new Rectangle(pageBounds.Left, (int)yPos, pageBounds.Right - pageBounds.Left, (int)height); 

            // now, tell the primitives to print themselves... 
            foreach(IPrintPrimitive primitive in _printPrimitives) 
            { 
                // render it... 
                primitive.Draw(engine, yPos, graphics, elementBounds); 

                // move to the next line... 
                yPos += primitive.CalculateHeight(engine, graphics); 
            } 
        } 
    }
}

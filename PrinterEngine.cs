/*
 *
 *  Método   : Printer Engine
 *             Main Class
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
using System.Windows.Forms;

namespace PrintingCore
{
    public class PrintEngine : PrintDocument
    {
        private ArrayList _printObjects = new ArrayList(); 
        public Margins margin = new Margins(20, 50, 20, 20);
        public const string FontName = "Calibri";
        public const int FontSize = 8;
        public Font PrintFont = new Font(FontName, FontSize);
        public Brush PrintBrush = Brushes.Black;
        public PrintElement Header; 
        public PrintElement Footer;
        public bool LandScape = false;
        private ArrayList _printElements; 
        private int _printIndex = 0; 
        private int _pageNum = 0;
        public bool endPrint = false;
        public float MarginBoundsWidth = 0;

        public PrintEngine(bool landscape = false)
        {
            PageSettings settings = DefaultPageSettings;            
            settings.Margins = margin;

            if (landscape)
                settings.Landscape = true;

            LandScape = settings.Landscape;
            DefaultPageSettings = settings;
        }

        public void AddPrintObject(IPrintable printObject) 
        { 
            _printObjects.Add(printObject); 
        } 

        public void ShowPreview() 
        { 
            // now, show the print dialog... 
            PrintPreviewDialog dialog = new PrintPreviewDialog();
            dialog.Document = this;
            dialog.PrintPreviewControl.Zoom = 1.5;
            ((Form)dialog).Icon = PrinterEngine.Properties.Resources.labgen;
            ((Form)dialog).Size = new Size(800, 600);
            ((Form)dialog).StartPosition = FormStartPosition.CenterScreen;
            ((Form)dialog).WindowState = FormWindowState.Maximized;
            ((Form)dialog).Text = "Labgen - Visualização";

            // show the dialog... 
            dialog.ShowDialog(); 
        }

        public PrintPreviewControl GetPreview()
        {
            PrintPreviewControl dialog = new PrintPreviewControl();
            dialog.Document = this;
            return dialog;
        }

        public PrintDocument GetObjectPrinter()
        {
            return this;
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            // reset... 
            _printElements = new ArrayList();
            _pageNum = 0;
            _printIndex = 0;

            // go through the objects in the list and create print elements for each one... 
            foreach (IPrintable printObject in _printObjects)
            {
                // create an element... 
                PrintElement element = new PrintElement(printObject);
                _printElements.Add(element);

                // tell it to print... 
                printObject.Print(element);
            }
        }

        protected override void OnEndPrint(PrintEventArgs e)
        {
            endPrint = true;
            base.OnEndPrint(e);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            // adjust the page number... 
            _pageNum++; 

            // now, render the header element... 
            float headerHeight = Header.CalculateHeight(this, e.Graphics); 
            Header.Draw(this, e.MarginBounds.Top, e.Graphics, e.MarginBounds); 

            // also, we need to calculate the footer height... 
            // Drawing the footer is a similar deal, except this has to appear at the bottom of the page: 
            float footerHeight = Footer.CalculateHeight(this, e.Graphics); 
            Footer.Draw(this, e.MarginBounds.Bottom - footerHeight, e.Graphics, e.MarginBounds);

            // now we know the header and footer, we can adjust the page bounds... 
            Rectangle pageBounds = new Rectangle(e.MarginBounds.Left,
                                                (int)(e.MarginBounds.Top + headerHeight), 
                                                e.MarginBounds.Width, 
                                                (int)(e.MarginBounds.Height - footerHeight - headerHeight));
            float yPos = pageBounds.Top; 

            // ok, now we need to loop through the elements... 
            bool morePages = false; 
            int elementsOnPage = 0;

            // registra a largura da página para calcular 
            // a altura do bloco de impressão
            MarginBoundsWidth = e.MarginBounds.Width;

            while (_printIndex < _printElements.Count)
            {
                // get the element... 
                PrintElement element = (PrintElement)_printElements[_printIndex];

                if (element.isNewPage())
                {
                    morePages = true;
                    _printIndex++;
                    elementsOnPage++; 
                    break;
                }

                // how tall is the primitive? 
                float height = element.CalculateHeight(this, e.Graphics);

                // will it fit on the page? 
                if (yPos + height > pageBounds.Bottom)
                {
                    // we don't want to do this if we're the first thing on the page... 
                    if (elementsOnPage != 0)
                    {
                        morePages = true;
                        break;
                    }
                }

                // now draw the element... 
                element.Draw(this, yPos, e.Graphics, pageBounds);

                // move the ypos... 
                yPos += height;
                //yPos = e.Graphics.ClipBounds.Bottom;

                // next... 
                _printIndex++;
                elementsOnPage++; 
            }

            // do we have more pages? 
            e.HasMorePages = morePages; 
        }
 
        public String ReplaceTokens(String buf) 
        { 
            // replace... 
            buf = buf.Replace("[pagenum]", _pageNum.ToString()); 

            // return... 
            return buf; 
        }

        public void ShowPageSettings()
        {
            PageSetupDialog setup = new PageSetupDialog();
            PageSettings settings = DefaultPageSettings;
            setup.PageSettings = settings;

            // display the dialog and, 
            if (setup.ShowDialog() == DialogResult.OK)
                DefaultPageSettings = setup.PageSettings;
        }

        public void ShowPrintDialog(PrintDialog overridedialog = null, string documentname = "")
        {
            // create and show... 

            // se nenhuma impressora for chamada
            if (overridedialog == null)
            {
                PrintDialog dialog = new PrintDialog();
                dialog.PrinterSettings = PrinterSettings;
                dialog.Document = this;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // save the changes... 
                    PrinterSettings = dialog.PrinterSettings;
                    // do the printing... 
                    Print();
                }
            }
            else
            {
                // aqui caso a impressora seja escolhida
                // antes da impressão ser gerada.
                // caso haja várias impressões em sequencia.
                PrinterSettings = overridedialog.PrinterSettings;
                overridedialog.Document = this;
                
                // verifica se precisa mudar o nome do arquivo de impressão
                if (documentname.Trim() != "")
                    DocumentName = documentname;

                // re-define qualquer dialogo de impressão
                this.PrintController = new System.Drawing.Printing.StandardPrintController();

                // do the printing...                 
                Print();
            }
        } 
    }
}

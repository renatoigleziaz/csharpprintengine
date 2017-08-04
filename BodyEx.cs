/*
 *
 *  Método   : Printer Engine
 *             Arquivo 'Body' de Exemplo 
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
    public class BodyEx : IPrintable
    {
        public Row rowsbody = new Row();

        public void Print(PrintElement element)
        {
            // printa o corpo do relatório
            element.AddText(rowsbody);
        }
    }
}

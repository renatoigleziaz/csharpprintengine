/*
 *
 *  Método   : Printer Engine
 *             IPrintPrimitive Interface Macro
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
  public interface IPrintPrimitive 
  { 
    // CalculateHeight - work out how tall the primitive is... 
    float CalculateHeight(PrintEngine engine, Graphics graphics); 

    // Print - tell the primitive to physically draw itself... 
    void Draw(PrintEngine engine, float yPos, Graphics graphics, Rectangle elementBounds); 
  } 
} 

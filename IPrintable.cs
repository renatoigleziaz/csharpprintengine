/*
 *
 *  Método   : Printer Engine
 *             IPrintable Interface Macro
 * 
 *  por      : Renato Igleziaz
 * 
 *  em       : 10/out de 2012
 * 
 */

using System;

namespace PrintingCore 
{ 
  public interface IPrintable 
  { 
    // Print 
    void Print(PrintElement element); 
  } 
} 

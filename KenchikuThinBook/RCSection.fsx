

#I @"bin\Debug\"
#r "SvgDrawer.dll"
#r "KenchikuThinBook.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"
#load "RCSectionHelper.fs"

open System
open System.Windows
open SvgDrawer
open KTB
open RCSectionHelper

let model : SvgDrawer.Model = 
    { BlockSize   = 10.0 
      RowCount    = 30 
      ColumnCount = 30
      Position    = (0, 0) 
      StrokeColor = Color.Black
      StrokeWidth = 2.0
      Shapes      = [] }

let section = 
    { Position    = (10, 10) 
      CountOfBarX = 4 
      CountOfBarY = 4 }

let draw () = 
    section.Draw (model)
    |> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\RCSection.bmp"

draw ()
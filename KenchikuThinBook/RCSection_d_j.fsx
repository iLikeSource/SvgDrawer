
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
    { Position    = (12, 10) 
      CountOfBarX = 4 
      CountOfBarY = 4 }

let drawScaleLine_d (model : SvgDrawer.Model) = 
    let (ox, oy) = section.Position
    let upperY = oy - section.Height / 2 + 2
    let lowerY = oy + section.Height / 2
    model
    |> Action.Move(2, upperY).On
    |> Line.To(4, upperY)
    |> Action.Move(2, lowerY).On
    |> Line.To(4, lowerY)
    |> Action.Move(3, upperY).On
    |> Line.To(3, lowerY)
    |> Action.Move(1, (lowerY + upperY) / 2).On
    |> Text.At ("d", 0, 0)
    |> Attr.FontSize(16.0).With

let drawScaleLine_j (model : SvgDrawer.Model) = 
    let (ox, oy) = section.Position
    let upperY = oy - section.Height / 2 + 2
    let lowerY = oy + section.Height / 2 - 2
    model
    |> Action.Move(8 + section.Width, upperY).On
    |> Line.To(10 + section.Width, upperY)
    |> Action.Move(8 + section.Width, lowerY).On
    |> Line.To(10 + section.Width, lowerY)
    |> Action.Move(9 + section.Width, upperY).On
    |> Line.To(9 + section.Width, lowerY)
    |> Action.Move(10 + section.Width, (lowerY + upperY) / 2).On
    |> Text.At ("j", 0, 0)
    |> Attr.FontSize(16.0).With

let draw () = 
    section.Draw (model)
    |> drawScaleLine_d 
    |> drawScaleLine_j
    |> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\RCSection.bmp"

draw ()



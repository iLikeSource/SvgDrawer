
#I @"bin\Debug\"
#r "SvgDrawer.dll"
#r "KenchikuThinBook.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"

open System.Windows
open SvgDrawer
open KTB

let model : SvgDrawer.Model = 
    { BlockSize   = 10.0 
      RowCount    = 20 
      ColumnCount = 20
      Position    = (0, 0) 
      StrokeColor = Color.Black
      StrokeWidth = 2.0
      Shapes      = [] }
      
let draw () = 
    model
    |> Action.Move(10, 10).On
    |> Rectangle.On
    |> Attr.Width(5.0).With
    |> Attr.Height(5.0).With
    |> Attr.StrokeWidth(2.0).With
    |> Attr.StrokeColor("gray").With
    |> Attr.FillColor("white").With
    |> Action.Move(8, 10).On
    |> Line.To(12, 10)
    |> Attr.StrokeWidth(5.0).With
    |> Action.Move(10, 8).On
    |> Line.To(10, 12)
    |> Attr.StrokeWidth(5.0).With
    |> Action.Move(4, 10).On
    |> Line.To(16, 10)
    |> Action.Move(10, 4).On
    |> Line.To(10, 16)
    |> Action.Move(12, 2).On
    |> Line.To(14, 2)
    |> Attr.StrokeWidth(5.0).With
    |> Action.Move(16, 2).On
    |> Text.At ("剛域", 0, 0)
    |> Attr.FontSize(10.0).With
    |> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\RigidZone.bmp"

           
draw ()

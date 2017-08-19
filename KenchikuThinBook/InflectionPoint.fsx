

#I @"bin\Debug\"
#r "SvgDrawer.dll"
#r "KenchikuThinBook.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"

open System.Windows
open SvgDrawer
open KTB

let model : SvgDrawer.Model = 
    { BlockSize   = 20.0 
      RowCount    = 10 
      ColumnCount = 20
      Position    = (0, 0) 
      StrokeColor = Color.Black
      StrokeWidth = 2.0
      Shapes      = [] }

let draw () = 
    model
    |> Action.Move(6, 2).On
    |> Line.To(6, 8)
    |> Attr.StrokeWidth(3.0).With
    |> Attr.StrokeColor("blue").WithDefault
    |> Line.To(2, 8)
    |> Line.To(6, 2)
    |> Action.Move(16, 2).On
    |> Attr.StrokeColor("black").WithDefault
    |> Line.To(16, 8)
    |> Attr.StrokeWidth(3.0).With
    |> Attr.StrokeColor("blue").WithDefault
    |> Line.To(14, 8)
    |> Line.To(18, 2)
    |> Line.To(16, 2)
    |> Action.Move(6, 8).On
    |> Text.At ("片持柱", 0, 1)
    |> Attr.FontSize(16.0).With
    |> Action.Move(16, 8).On
    |> Text.At ("逆対称", 0, 1)
    |> Attr.FontSize(16.0).With
    |> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\InflectionPoint.bmp"

           
draw ()



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
      Shapes      = [] }

let draw () = 
    model
    |> Action.Move(6, 10).On
    |> Line.To(6, 16)
    |> Attr.StrokeWidth(3.0).With
    |> Attr.StrokeColor("blue").WithDefault
    |> Line.To(4, 16)
    |> Line.To(6, 10)
    |> Action.Move(16, 10).On
    |> Attr.StrokeColor("black").WithDefault
    |> Line.To(16, 16)
    |> Attr.StrokeWidth(3.0).With
    |> Attr.StrokeColor("blue").WithDefault
    |> Line.To(15, 16)
    |> Line.To(17, 10)
    |> Line.To(16, 10)
    |> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\InflectionPoint.bmp"

           
draw ()

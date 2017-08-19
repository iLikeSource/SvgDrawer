
#I @"bin\Debug\"
#r "SvgDrawer.dll"
#r "KenchikuThinBook.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"

open System
open System.Windows
open SvgDrawer
open KTB

let model : SvgDrawer.Model = 
    { BlockSize   = 10.0 
      RowCount    = 60 
      ColumnCount = 40
      Position    = (0, 0) 
      StrokeColor = Color.Black
      StrokeWidth = 2.0
      Shapes      = [] }

let drawSlab (minX, minY, maxX, maxY) (model : SvgDrawer.Model) =
    let xc = (minX + maxX) / 2
    let yc = (minY + maxY) / 2
    let w  = maxX - minX
    let h  = maxY - minY 
    model
    |> Action.Move(xc, yc).On
    |> Rectangle.On
    |> Attr.Width(float w).With
    |> Attr.Height(float h).With
    |> Attr.StrokeWidth(4.0).With

let drawLoadDivideLine (minX, minY, maxX, maxY) (model : SvgDrawer.Model) = 
    let y      = (minY + maxY) / 2    
    let offset = y - minY
    let leftX  = minX + offset
    let rightX = maxX - offset 
    model
    |> Action.Move(minX, minY).On
    |> Line.To(leftX, y)
    |> Attr.StrokeColor("blue").With
    |> Action.Move(minX, maxY).On
    |> Line.To(leftX, y)
    |> Attr.StrokeColor("blue").With
    |> Action.Move(maxX, minY).On
    |> Line.To(rightX, y)
    |> Attr.StrokeColor("blue").With
    |> Action.Move(maxX, maxY).On
    |> Line.To(rightX, y)
    |> Attr.StrokeColor("blue").With
    |> Action.Move(leftX, y).On
    |> Line.To(rightX, y)
    |> Attr.StrokeColor("blue").With

let drawMinimumAreaLoad (x, y) (model : SvgDrawer.Model) = 
    model
    |> Action.Move(x, y).On
    |> Rectangle.On
    |> Attr.Width(1.0).With
    |> Attr.Height(1.0).With
    |> Attr.FillColor("red").With
    |> Action.Move(x + 12, y).On
    |> Text.At ("微小面積の単位荷重", 0, 0)
    |> Attr.FontSize(12.0).With

let drawMinimumAreaLoadArrow (x0, y0, x1, y1) (model : SvgDrawer.Model) = 
    model
    |> Action.Move(x0, y0).On
    |> Line.ArrowTo(x1, y1)
    |> Attr.StrokeColor("red").With

let drawColumns (minX, minY, maxX, maxY) (model : SvgDrawer.Model) =
    [ (minX, maxY); (maxX, maxY); (maxX, minY); (minX, minY) ]
    |> List.map (fun (x, y) -> 
        Action.Move(x, y).On >> Rectangle.On
                             >> Attr.Width(4.0).With
                             >> Attr.Height(4.0).With
    )
    |> List.fold (fun model f -> f (model)) model

let drawGirder (x0, y0, x1, y1, offset) (model : SvgDrawer.Model) =
    model
    |> Action.Move(x0, y0).On
    |> Line.To(x1, y1) |> Attr.StrokeWidth(4.0).With
    |> Action.Move(x0, y0).On
    |> Line.To(x0 + offset, y0 - offset) |> Attr.StrokeColor("blue").With
    |> Line.To(x1 - offset, y0 - offset) |> Attr.StrokeColor("blue").With
    |> Line.To(x1, y0)                   |> Attr.StrokeColor("blue").With
    |> Action.Move((x0 + x1) / 2, y0 - offset).On
    |> Text.At("CMoQ荷重", 0, -1)
    |> Attr.FontSize(14.0).With
    

let minX =  4 
let maxX = 36 
let minY =  4 
let maxY = 24
    
model 
|> drawSlab (minX, minY, maxX, maxY)
|> drawLoadDivideLine (minX, minY, maxX, maxY)
|> drawMinimumAreaLoad (16, 20)
|> drawMinimumAreaLoadArrow (16, 20, 16, maxY - 1)
|> drawColumns (minX, minY, maxX, maxY)
|> drawGirder  (minX, maxY + 20, maxX, maxY + 20, 10)
|> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\SlabLoad.bmp"


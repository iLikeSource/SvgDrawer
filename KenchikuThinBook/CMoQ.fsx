
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
      RowCount    = 20 
      ColumnCount = 60
      Position    = (0, 0) 
      StrokeColor = Color.Black
      StrokeWidth = 2.0
      Shapes      = [] }

let drawLeftBoundary (x) (model) = 
    model
    |> Action.Move(x, 8).On  |> Line.To(x    , 12)
    |> Action.Move(x, 8).On  |> Line.To(x - 1,  9)
    |> Action.Move(x, 9).On  |> Line.To(x - 1, 10)
    |> Action.Move(x, 10).On |> Line.To(x - 1, 11)
    |> Action.Move(x, 11).On |> Line.To(x - 1, 12)

let drawRightBoundary (x) (model) = 
    model
    |> Action.Move(x,  8).On |> Line.To(x    , 12)
    |> Action.Move(x,  9).On |> Line.To(x + 1,  8)
    |> Action.Move(x, 10).On |> Line.To(x + 1,  9)
    |> Action.Move(x, 11).On |> Line.To(x + 1, 10)
    |> Action.Move(x, 12).On |> Line.To(x + 1, 11)

let drawBeam (x1, x2) (model) = 
    model
    |> Action.Move(x1,  10).On |> Line.To(x2, 10)

let drawNode (x) (model) = 
    model
    |> Action.Move(x, 10).On 
    |> Circle.On
    |> Attr.Radius(0.5).With
    |> Attr.FillColor("black").With
        

let drawDistributedLoad (x1, x2) (model) = 
    model
    |> Action.Move((x1 + x2) / 2, 9).On
    |> Rectangle.On
    |> Attr.Width(15.0).With
    |> Attr.Height(2.0).With
    |> Attr.StrokeWidth(1.0).With
    |> Attr.FillColor("gray").With

let drawNodeMoment (x, y) (model) = 
    let r     = model.BlockSize * 2.0
    let ox    = SvgHelper.coordinate model x 
    let oy    = SvgHelper.coordinate model y 
    let angle = 45.0 
    let x1    = ox + r * System.Math.Cos (angle / 180.0 * System.Math.PI)
    let y1    = oy + r * System.Math.Sin (angle / 180.0 * System.Math.PI) 
    let x2    = ox + r * System.Math.Cos (angle / 180.0 * System.Math.PI)
    let y2    = oy - r * System.Math.Sin (angle / 180.0 * System.Math.PI) 
    model
    |> Path.On(Printf.sprintf "M %.0f,%.0f A %.0f,%.0f %.1f 1,1 %.0f,%.0f" 
                              x1 
                              y1 
                              r 
                              r 
                              angle
                              x2 
                              y2)    

let (x1, x2) = (5, 21) 
let (x3, x4) = (30, 46) 
model
|> drawLeftBoundary(x1)
|> drawRightBoundary(x2)
|> drawBeam(x1, x2)
|> drawDistributedLoad(x1, x2)
|> drawBeam(x3, x4)
|> drawNode(x3)
|> drawNode(x4)
|> drawNodeMoment(x3, 10)
|> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\CMoQ.bmp"
|> SvgHelper.draw @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\CMoQ.svg"

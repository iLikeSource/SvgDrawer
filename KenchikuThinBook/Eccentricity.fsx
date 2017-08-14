
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
      StrokeWidth = 1.0
      Shapes      = [] }

let xs = [ 2; 6; 10; 14 ]
let ys = [ 2; 6 ]

let drawPlan (xs, ys) (model : SvgDrawer.Model) =
    let startX = xs |> List.head
    let endX   = xs |> List.rev |> List.head 
    let startY = ys |> List.head
    let endY   = ys |> List.rev |> List.head 
    let drawXLine y model = 
        model |> Action.Move(startX, y).On |> Line.To(endX, y)
    let drawYLine x model = 
        model |> Action.Move(x, startY).On |> Line.To(x, endY)
    [ ys |> List.map (fun y -> drawXLine y)
      xs |> List.map (fun x -> drawYLine x) ]
    |> List.concat
    |> List.fold (fun dst f -> f (dst)) model


let drawColumn (xs, ys) (model : SvgDrawer.Model) = 
    xs |> List.collect (fun x ->
        ys |> List.map (fun y ->
            Action.Move(x, y).On >> Rectangle.On
                                 >> Attr.Width(1.0).With
                                 >> Attr.Height(1.0).With
                                 >> Attr.StrokeWidth(2.0).With
                                 >> Attr.StrokeColor("black").With
                                 >> Attr.FillColor("white").With
        )
    ) 
    |> List.fold (fun dst f -> f (dst)) model

let drawWall (xs, ys) y (model : SvgDrawer.Model) = 
    let startX = xs |> List.head
    let endX   = xs |> List.rev |> List.head
    model
    |> Action.Move(startX, y).On 
    |> Line.To(endX, y)        
    |> Attr.StrokeWidth(3.0).With

let drawMovedRectangle (xs, ys) (angle, offsetRx, offsetRy) (model : SvgDrawer.Model) = 
    let startX = xs |> List.head
    let endX   = xs |> List.rev |> List.head 
    let startY = ys |> List.head
    let endY   = ys |> List.rev |> List.head
    let x = (startX + endX) / 2
    let y = (startY + endY) / 2
    let blockSize = model.BlockSize
    model
    |> Action.Move(x + 2, y).On 
    |> Rectangle.On
    |> Attr.Width(float (endX - startX)).With
    |> Attr.Height(float (endY - startY)).With
    |> Attr.StrokeWidth(1.0).With
    |> Attr.StrokeColor("blue").With
    |> Attr.FillColor("transparent").With
    |> Attr.RotateCenter(float (x + offsetRx) * blockSize, float (y + offsetRy) * blockSize ).With
    |> Attr.Angle(angle).With

let ys' = ys |> List.map (fun y -> y + 10)

model
|> drawPlan (xs, ys)
|> drawWall (xs, ys) 2
|> drawColumn (xs, ys)
|> drawMovedRectangle (xs, ys) (0.0, 0, 0)
|> drawPlan (xs, ys')
|> drawWall (xs, ys') 12
|> drawColumn (xs, ys')
|> drawMovedRectangle (xs, ys') (-8.0, 0, -2)
|> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\Eccentricity.Plan.bmp"
|> SvgHelper.draw @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\Eccentricity.Plan.svg"


#I @"bin\Debug\"
#r "SvgDrawer.dll"
#r "KenchikuThinBook.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"

open System.Windows
open SvgDrawer
open KTB


let initModel (rowCount, clmCount) : SvgDrawer.Model = 
    { BlockSize   = 10.0 
      RowCount    = rowCount 
      ColumnCount = clmCount
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

let drawTitle (x, y, title) (model) = 
    model
    |> Action.Move(x, y).On
    |> Text.At (title, 0, 0)    
    |> Attr.FontSize(10.0).With

let xs' = xs |> List.map (fun y -> y + 20)
let ys' = ys |> List.map (fun y -> y + 10)

initModel (15, 40)
|> drawPlan (xs, ys)
|> drawWall (xs, ys) 2
|> drawColumn (xs, ys)
|> drawMovedRectangle (xs, ys) (0.0, 0, 0)
|> drawTitle (10, 8, "平面フレーム")
|> drawPlan (xs', ys)
|> drawWall (xs', ys) 2
|> drawColumn (xs', ys)
|> drawMovedRectangle (xs', ys) (-8.0, 0, -2)
|> drawTitle (30, 8, "立体フレーム")
|> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\Eccentricity.Plan.bmp"
|> SvgHelper.draw @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\Eccentricity.Plan.svg"

let drawWallRect (pairs : (int * int) array) (model : SvgDrawer.Model) = 
    let (x0, y0) = pairs.[0]
    let tl = Array.sub pairs 1 3
    let toF (v) = SvgHelper.coordinate model v
    let data =
        tl
        |> Array.fold (fun dst (x, y) -> Printf.sprintf "%s L %.0f %.0f " dst (toF x) (toF y)) 
                      (Printf.sprintf "M %.0f %.0f " (toF x0) (toF y0))   
        |> Printf.sprintf "%s z"
    model
    |> Path.On(data)
    |> Attr.FillColor("gray").With 

let drawDeformationFrame (offsetX, offsetY, magnify) (model : SvgDrawer.Model) = 
    model
    |> Action.Move(offsetX + 2, offsetY + 16).On
    |> Line.To(offsetX + 2 + 4 * magnify, offsetY + 4)
    |> Action.Move(offsetX + 6, offsetY + 16).On
    |> Line.To(offsetX + 6 + 4 * magnify, offsetY + 4)
    |> Action.Move(offsetX + 10, offsetY + 16).On
    |> Line.To(offsetX + 10 + 4 * magnify, offsetY + 4)

    |> Action.Move(offsetX + 2 + 4 * magnify, offsetY + 4).On
    |> Line.To(offsetX + 10 + 4 * magnify, offsetY + 4)
    |> Action.Move(offsetX + 2 + 3 * magnify, offsetY + 7).On
    |> Line.To(offsetX + 10 + 3 * magnify, offsetY + 7)
    |> Action.Move(offsetX + 2 + 2 * magnify, offsetY + 10).On
    |> Line.To(offsetX + 10 + 2 * magnify, offsetY + 10)
    |> Action.Move(offsetX + 2 + 1 * magnify, offsetY + 13).On
    |> Line.To(offsetX + 10 + 1 * magnify, offsetY + 13)
    |> Action.Move(offsetX + 2, offsetY + 16).On
    |> Line.To(offsetX + 10, offsetY + 16)


initModel (40, 40)
|> drawWallRect [| (2, 16); (10, 16); (14, 4); (6, 4) |]
|> drawDeformationFrame (0, 0, 1)
|> drawDeformationFrame (0, 20, 1) 
|> Action.Move(14, 2).On 
|> Line.To(14, 36)
|> Attr.StrokeColor("blue").With
|> drawTitle (14, 38, "平面フレーム(ねじれ考慮なし)") 

|> drawWallRect [| (22, 16); (30, 16); (34, 4); (26, 4) |]
|> drawDeformationFrame (20, 0, 1)
|> drawDeformationFrame (20, 20, 2) 
|> Action.Move(34, 2).On 
|> Line.To(34, 36)
|> Attr.StrokeColor("blue").With
|> drawTitle (34, 38, "立体フレーム(ねじれ考慮あり)") 

|> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\Eccentricity.Elevation.bmp"

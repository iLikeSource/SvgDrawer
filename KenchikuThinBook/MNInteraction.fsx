

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
      RowCount    = 40 
      ColumnCount = 40
      Position    = (0, 0) 
      StrokeColor = Color.Black
      StrokeWidth = 2.0
      Shapes      = [] }

let ox    = 20
let oy    = 20
let range = 16 

let drawAxis (model) = 
    model
    |> Action.Move(ox,  oy + range).On
    |> Line.ArrowTo(ox, oy - range)
    |> Action.Move(ox - range, oy).On
    |> Line.ArrowTo(ox + range, oy)
    

let drawInteractionS (maxX, maxY) (model) = 
    model
    |> Action.Move(ox, oy - maxY).On
    |> Line.To(ox + maxX, oy)
    |> Attr.StrokeColor("red").With    
    |> Action.Move(ox + maxX, oy).On
    |> Line.To(ox, oy + maxY)    
    |> Attr.StrokeColor("red").With    
    |> Action.Move(ox, oy + maxY).On
    |> Line.To(ox - maxX, oy)    
    |> Attr.StrokeColor("red").With    
    |> Action.Move(ox - maxX, oy).On
    |> Line.To(ox, oy - maxY)    
    |> Attr.StrokeColor("red").With    

let drawInteractionRC (maxX, minY, maxY) (model) = 
    let orgCoordX    = model.BlockSize * float (ox - 1)
    let orgCoordY    = model.BlockSize * float (oy - 1)
    let minCoordX    = model.BlockSize * float (ox - maxX - 1)
    let maxCoordX    = model.BlockSize * float (ox + maxX - 1)
    let minCoordY    = model.BlockSize * float (oy - minY - 1)
    let maxCoordY    = model.BlockSize * float (oy - maxY - 1)
    let centerCoordY = 0.5 * (maxCoordY + minCoordY)

    let source = 
        [| (0.0, - 30.0); (10.0, - 20.0); (20.0, - 10.0); (30.0, 0.0); (40.0, 5.0); (50.0, 10.0); (60.0, 15.0); (65.0, 20.0);
           (55.0, 40.0); (50.0, 55.0); (45.0, 65.0); (35.0, 70.0); (25.0, 75.0); (15.0, 80.0); (5.0, 85.0); (0.0, 90.0)|]
    let pPoints =
        source
        |> Array.map (fun (x, y) -> (orgCoordX + x * 1.5, orgCoordY - y * 1.5)) 
    let nPoints = 
        source
        |> Array.map (fun (x, y) -> (orgCoordX - x * 1.5, orgCoordY - y * 1.5)) 

    [| 0 .. (pPoints.Length - 2) |]
    |> Array.map (fun index ->
        let (px0, py0) = pPoints.[index]
        let (px1, py1) = pPoints.[index + 1]
        let (nx0, ny0) = nPoints.[index]
        let (nx1, ny1) = nPoints.[index + 1]
        Line.DirectTo(px0, py0, px1, py1) 
        >> Attr.StrokeColor("red").With           
        >> Line.DirectTo(nx0, ny0, nx1, ny1) 
        >> Attr.StrokeColor("red").With           
    )
    |> Array.fold (fun model f -> f model) model

    (*
    //  下側
    let lowerDiv = 10
    let lowerFunc =
        [| 0 .. (lowerDiv - 1) |]
        |> Array.map (fun index ->
            let x0 = orgCoordX + float (index    ) * (maxCoordX - orgCoordX) / float (lowerDiv) 
            let y0 = minCoordY + float (index    ) * (centerCoordY - minCoordY) / float (lowerDiv) 
            let x1 = orgCoordX + float (index + 1) * (maxCoordX - orgCoordX) / float (lowerDiv) 
            let y1 = minCoordY + float (index + 1) * (centerCoordY - minCoordY) / float (lowerDiv) 
            Line.DirectTo(x0, y0, x1, y1) >> Attr.StrokeColor("red").With           
        )    
    
    //  上側
    let upperDiv = 10
    let upperFunc =
        [| 0 .. (upperDiv - 1) |]
        |> Array.map (fun index ->
            let x0 = maxCoordX    - float (index    ) * (maxCoordX - orgCoordX) / float (upperDiv) 
            let y0 = centerCoordY + float (index    ) * (maxCoordY - centerCoordY) / float (upperDiv) 
            let x1 = maxCoordX    - float (index + 1) * (maxCoordX - orgCoordX) / float (upperDiv) 
            let y1 = centerCoordY + float (index + 1) * (maxCoordY - centerCoordY) / float (upperDiv) 
            Line.DirectTo(x0, y0, x1, y1) >> Attr.StrokeColor("red").With      
        )    

    [| lowerFunc; upperFunc |]
    |> Array.concat
    |> Array.fold (fun model f -> f model) model
    *)


            

model
|> drawAxis
|> drawInteractionS (10, 10)
|> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\MNI_S.bmp"
|> ignore

model
|> drawAxis
|> drawInteractionRC (10, -4, 10)
|> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\MNI_RC.bmp"
|> ignore
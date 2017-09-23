module RCSectionHelper

open System.Windows
open SvgDrawer
open KTB


type RCSection = 
    { Position           : int * int
      CountOfBarX        : int 
      CountOfBarY        : int }
    with
    member this.Width  = (this.CountOfBarX + 1) * 2
    member this.Height = (this.CountOfBarY + 1) * 2
    member private this.BarCoordinates (ox : int, 
                                        oy : int, 
                                        width : int, 
                                        height : int) = 

        [
            //  左上から左下
            [ 1 .. this.CountOfBarX ] |> List.map (fun y -> (2, y * 2))
            //  左下から右下
            [ 1 .. this.CountOfBarY ] |> List.map (fun x -> (x * 2, this.CountOfBarX * 2))
            //  右上から右下
            [ 1 .. this.CountOfBarX ] |> List.map (fun y -> (this.CountOfBarY * 2, y * 2))
            //  左上から右上
            [ 1 .. this.CountOfBarY ] |> List.map (fun x -> (x * 2, 2))
        ]
        |> List.concat
        |> List.map (fun (x, y) -> (x + (ox - width / 2), y + (oy - height / 2)))
             
    member this.Draw (model : SvgDrawer.Model) = 
        let drawRebarRectangle barCoordinates model =
            List.fold (fun model (x, y) ->
                model
                |> Action.Move(x, y).On
                |> Circle.On
                |> Attr.Radius(0.2).With 
                |> Attr.StrokeColor("black").With
            ) model barCoordinates
        
        let getSize (x) = (x + 1) * 2
        
        let height = this.Width 
        let width  = this.Height

        let (ox, oy) = this.Position
        let barCoordinates = this.BarCoordinates (ox, oy, width, height)
        
        model
        |> Action.Move(ox, oy).On
        |> Rectangle.On
        |> Attr.Width(float width).With    
        |> Attr.Height(float height).With 
        |> drawRebarRectangle barCoordinates
                
                    
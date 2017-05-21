
#I @"bin\Debug\"
#r "Svg.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"
#load "AppModels.fs"
#load "SvgHelper.fs"
open System.Windows
open SvgDrawer

// ここでライブラリ スクリプト コードを定義します

let repeat times (f : int -> Model -> Model) = 
    fun model ->
        [| 1 .. times |]
        |> Array.fold (fun dst i -> f i dst) model     


let model = 
    { BlockSize   = 10.0
      RowCount    = 55 
      ColumnCount = 99  
      Position    = (1, 1) 
      Shapes      = [] }

let centerX      = model.ColumnCount / 2 + 1
let centerY      = model.RowCount / 2 + 1
let topY         = 10 
let bottomY      = 40 
let storySpan    = 10 
let axisOffset   = 10
let momentOffset = 5

let lumpedMass (size) = fun model -> 
    model
    //  質点棒
    |> Action.Move(centerX, topY).On
    |> Line.To(centerX, bottomY)
    |> Attr.StrokeColor("Black").With
    |> Attr.StrokeWidth(3.0).With
    //  質点球
    |> Action.Move(centerX, topY).On
    |> repeat size (fun i x -> 
        x 
        |> Circle.On 
        |> Attr.Radius(1.0).With
        |> Attr.StrokeWidth(2.0).With
        |> Action.RMove(0, storySpan).On
    )
    //  寸法線
    |> Action.Move(centerX + axisOffset, topY).On
    |> Line.To(centerX + axisOffset, bottomY)
    |> repeat (size + 1) (fun i x -> 
        x
        |> Action.Move(centerX + axisOffset, topY + (i - 1) * storySpan).On
        |> Circle.On
        |> Attr.Radius(0.2).With
        |> Attr.FillColor("Black").With
    )
    |> Action.Move(centerX + axisOffset, topY + storySpan / 2).On
    |> repeat size (fun i x -> 
        x
        |> Text.At(Printf.sprintf "H%d" (size - i + 1), 1, 0)
        |> Attr.FontSize(12.0).With
        |> Action.RMove(0, storySpan).On
    )
    

let result = 
    model
    //  質点棒
    |> lumpedMass(3)
    //  モーメント線
    |> Action.Move(centerX, topY).On
    |> Line.To(centerX - 3 * momentOffset, bottomY)
    |> Attr.StrokeWidth(1.0).With
    |> Action.Move(centerX, topY).On
    |> repeat 3 (fun i x -> 
        let lText = Printf.sprintf "M%d" (3 - i)
        let rText  = 
            [| 1 .. i |]
            |> Array.fold (fun (dst:string) index -> 
                let qText = Printf.sprintf "Q%d" (3 - index + 1)
                let hText = Printf.sprintf "H%d" (3 - index + 1)
                let expr  = Printf.sprintf "%s*%s" qText hText
                if dst.Length = 0 then expr
                                  else Printf.sprintf "%s+%s" dst expr
            ) ""
        x
        |> Action.Move(centerX, topY + i * storySpan).On
        |> Line.To(centerX - i * momentOffset, topY + i * storySpan)        
        |> Text.At(Printf.sprintf "%s=%s" lText rText, -1, 0)
        |> Attr.FontSize(12.0).With
    )



result |> SvgHelper.save @"C:\temp\Otm.bmp"

result



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
    { BlockSize   = 20.0
      RowCount    = 15 
      ColumnCount = 25  
      Position    = (1, 1) 
      Shapes      = [] }

let centerX = model.ColumnCount / 2 + 1
let centerY = model.RowCount / 2 + 1

let result = 
    model
    //  質点棒
    |> Action.Move(centerX, 1).On
    |> Line.To(centerX, 7)
    |> Attr.StrokeColor("Black").With
    |> Attr.StrokeWidth(3.0).With
    //  モーメント線
    |> Action.Move(centerX, 1).On
    |> Line.To(centerX - 3, 7)
    |> Attr.StrokeWidth(1.0).With
    |> Action.Move(centerX, 1).On
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
        |> Action.Move(centerX, 1 + i * 2).On
        |> Line.To(centerX - i, 1 + i * 2)        
        |> Text.At(Printf.sprintf "%s=%s" lText rText, 0, 0)
        |> Attr.FontSize(12.0).With
    )
    //  質点球
    |> Action.Move(centerX, 1).On
    |> repeat 3 (fun i x -> 
        x 
        |> Circle.On 
        |> Attr.StrokeWidth(2.0).With
        |> Action.RMove(0, 2).On
    )

result |> SvgHelper.draw @"C:\temp\Otm.svg"

result


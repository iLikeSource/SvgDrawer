
#I @"bin\Debug\"
#r "SvgDrawer.dll"
#r "KenchikuThinBook.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"
#load "LumpedMassHelper.fs"

open System.Windows
open SvgDrawer
open KTB

let model : SvgDrawer.Model = 
    { BlockSize   = 10.0 
      RowCount    =  50 
      ColumnCount = 120
      Position    = (0, 0) 
      StrokeColor = Color.Black
      StrokeWidth = 1.0
      Shapes      = [] }

let result = 
    let config       = LumpedMassHelper.Config.Create ()
    let draw         = LumpedMassHelper.draw (config)
    let centerX      = model.ColumnCount / 2 + 1
    let centerY      = model.RowCount / 2 + 1
    let topY         = config.TopY
    let bottomY      = config.BottomY
    let momentOffset = config.MomentOffset
    let storySpan    = config.StorySpan
    model
    //  質点棒
    |> draw (3)
    //  モーメント線
    |> Action.Move(centerX, topY).On
    |> Line.To(centerX - 3 * momentOffset, bottomY)
    |> Attr.StrokeWidth(1.0).With
    |> Action.Move(centerX, topY).On
    |> LumpedMassHelper.repeat 3 (fun i x -> 
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
        |> Attr.FontSize(16.0).With
    )



result |> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\Otm.bmp"

result


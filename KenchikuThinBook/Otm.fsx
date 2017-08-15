
#I @"bin\Debug\"
#r "SvgDrawer.dll"
#r "KenchikuThinBook.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"
#load "LumpedMassHelper.fs"

open System.Windows
open SvgDrawer
open KTB


let result = 
    let config = LumpedMassHelper.Config.Create ()
    let draw   = LumpedMassHelper.draw (config)
    model
    //  質点棒
    |> draw (3)
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



result |> SvgHelper.save @"C:\Users\So\Documents\Programs\other\kenchiku-thin-book\kenchiku-thin-book\md\html\image\Otm.bmp"

result


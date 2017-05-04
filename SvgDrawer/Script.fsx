// F# の詳細については、http://fsharp.org を参照してください。F# プログラミングのガイダンスについては、
// 'F# チュートリアル' プロジェクトを参照してください。
#I @"bin\Debug\"
#r "Svg.dll"
#r "WindowsBase.dll"
#r "PresentationCore.dll"
#load "AppModels.fs"
#load "SvgHelper.fs"
open System.Windows
open SvgDrawer

// ここでライブラリ スクリプト コードを定義します


let model = 
    { BlockSize   = 20.0
      RowCount    = 10
      ColumnCount = 10 
      Position    = (1, 1) 
      Shapes      = [] }

let result = 
    model
    |> Action.Move(5, 5).On
    |> Line.To(5, 3)
    |> Attr.StrokeColor("Black").With
    |> Circle.On
    |> Attr.StrokeWidth(2.0).With
    |> Action.RMove(-1, 0).On
    |> Rectangle.On
    |> Attr.FillColor("White").With

result |> SvgHelper.draw @"C:\temp\test.svg"

result


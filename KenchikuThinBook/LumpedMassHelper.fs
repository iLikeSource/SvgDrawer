namespace KTB

module LumpedMassHelper = 

    open SvgDrawer

    type Config = 
        { TopY         : int 
          BottomY      : int 
          StorySpan    : int
          AxisOffset   : int
          MomentOffset : int }
        with
        static member Create () = 
            { TopY         = 10 
              BottomY      = 40 
              StorySpan    = 10
              AxisOffset   = 10
              MomentOffset = 10 }

    let repeat times (f : int -> Model -> Model) = 
        fun model ->
            [| 1 .. times |]
            |> Array.fold (fun dst i -> f i dst) model     

    let draw (config : Config) (size) = fun model ->
        let centerX = model.ColumnCount / 2 + 1
        let centerY = model.RowCount / 2 + 1

        model
        //  質点棒
        |> Action.Move(centerX, config.TopY).On
        |> Line.To(centerX, config.BottomY)
        |> Attr.StrokeColor("Black").With
        |> Attr.StrokeWidth(3.0).With
        //  質点球
        |> Action.Move(centerX, config.TopY).On
        |> repeat size (fun i x -> 
            x 
            |> Circle.On 
            |> Attr.Radius(1.0).With
            |> Attr.StrokeWidth(2.0).With
            |> Action.RMove(0, config.StorySpan).On
        )
        //  寸法線
        |> Action.Move(centerX + config.AxisOffset, config.TopY).On
        |> Line.To(centerX + config.AxisOffset, config.BottomY)
        |> repeat (size + 1) (fun i x -> 
            x
            |> Action.Move(centerX + config.AxisOffset, config.TopY + (i - 1) * config.StorySpan).On
            |> Circle.On
            |> Attr.Radius(0.2).With
            |> Attr.FillColor("Black").With
        )
        |> Action.Move(centerX + config.AxisOffset, config.TopY + config.StorySpan / 2).On
        |> repeat size (fun i x -> 
            x
            |> Text.At(Printf.sprintf "H%d" (size - i + 1), 1, 0)
            |> Attr.FontSize(12.0).With
            |> Action.RMove(0, config.StorySpan).On
        )
            
        

namespace SvgDrawer

module SvgHelper =

    open System.Windows
    open Svg

    let typeFaceBase = new Media.Typeface("ＭＳ Ｐゴシック");

    let number x = new SvgUnit (float32 x)

    let numberCollection (xs : float array) = 
        let collection = new SvgUnitCollection ()
        xs |> Array.iter (fun x -> collection.Add (number x))    
        collection        

    let color (color : Color) = 
        new SvgColourServer (System.Drawing.Color.FromArgb (color.R, color.G, color.B)) 

    let drawShape (doc : SvgDocument) (shape : Shape) =
        match shape with
        | Line (line) ->
            let (x0, y0) = line.Start
            let (x1, y1) = line.End
            let svgLine = new SvgLine ()
            svgLine.StartX      <- number x0
            svgLine.StartY      <- number y0
            svgLine.EndX        <- number x1
            svgLine.EndY        <- number y1
            svgLine.StrokeWidth <- number (line.StrokeWidth)
            svgLine.Stroke      <- color (line.StrokeColor)
            doc.Children.Add (svgLine)
        | Circle (circle) ->
            let (x, y) = circle.Position
            let svgCircle = new SvgCircle ()
            svgCircle.CenterX     <- number x
            svgCircle.CenterY     <- number y
            svgCircle.Radius      <- number circle.R
            svgCircle.StrokeWidth <- number (circle.StrokeWidth)
            svgCircle.Stroke      <- color (circle.StrokeColor)
            svgCircle.Fill        <- color (circle.FillColor)
            doc.Children.Add (svgCircle)
        | Rectangle (rectangle) ->
            let (x, y) = rectangle.Position
            let svgRectangle = new SvgRectangle ()
            svgRectangle.X           <- number (x - 0.5 * rectangle.W)
            svgRectangle.Y           <- number (y - 0.5 * rectangle.H)
            svgRectangle.Width       <- number rectangle.W
            svgRectangle.Height      <- number rectangle.H
            svgRectangle.StrokeWidth <- number (rectangle.StrokeWidth)
            svgRectangle.Stroke      <- color (rectangle.StrokeColor)
            svgRectangle.Fill        <- color (rectangle.FillColor)
            doc.Children.Add (svgRectangle)
        | Text (text) -> 
            let (x, y)  = text.Position
            let svgText = new SvgText ()
            let (offsetX, offsetY) = text.Offset
            let (sign, anchor) =
                if      0 < offsetX then ( 1.0, SvgTextAnchor.Start) 
                else if 0 > offsetX then (-1.0, SvgTextAnchor.End)
                else                     ( 1.0, SvgTextAnchor.Middle)
            let font  = 
                new Media.FormattedText (text.Content, 
                                         System.Globalization.CultureInfo.CurrentCulture,
                                         FlowDirection.LeftToRight,
                                         typeFaceBase,
                                         text.FontSize,
                                         Media.Brushes.Transparent)
            let textW = font.Width
            let textH = font.Height
            let textX = x - sign * (0.5 * textW) + float offsetX * textW     
            let textY = y - (0.5 * textH) + float offsetY * textH     
            svgText.X          <- numberCollection [| textX |]
            svgText.Y          <- numberCollection [| textY |]
            svgText.Content    <- text.Content
            svgText.FontSize   <- number text.FontSize
            svgText.TextAnchor <- anchor
            svgText.Color      <- color (text.Color)
            svgText.Fill       <- color (text.Color)

         
    let draw (path : string) (model : Model) = 
        let doc = new SvgDocument ()
        doc.Width  <- number (model.BlockSize * float model.RowCount)        
        doc.Height <- number (model.BlockSize * float model.ColumnCount)
        model.Shapes
        |> List.rev
        |> List.iter (drawShape doc)
        doc.Write (path)


                

    

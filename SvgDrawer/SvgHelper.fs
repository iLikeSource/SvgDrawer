namespace SvgDrawer

module SvgHelper =

    open System.Windows
    open Svg

    let margin = 20.0

    let typeFaceBase = new Media.Typeface("ＭＳ ゴシック");

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
            svgLine.StartX      <- number (margin + x0)
            svgLine.StartY      <- number (margin + y0)
            svgLine.EndX        <- number (margin + x1)
            svgLine.EndY        <- number (margin + y1)
            svgLine.StrokeWidth <- number (line.StrokeWidth)
            svgLine.Stroke      <- color (line.StrokeColor)
            doc.Children.Add (svgLine)
        | Circle (circle) ->
            let (x, y) = circle.Position
            let svgCircle = new SvgCircle ()
            svgCircle.CenterX     <- number (margin + x)
            svgCircle.CenterY     <- number (margin + y)
            svgCircle.Radius      <- number circle.R
            svgCircle.StrokeWidth <- number (circle.StrokeWidth)
            svgCircle.Stroke      <- color (circle.StrokeColor)
            svgCircle.Fill        <- color (circle.FillColor)
            doc.Children.Add (svgCircle)
        | Rectangle (rectangle) ->
            let (x, y) = rectangle.Position
            let svgRectangle = new SvgRectangle ()
            let path = new SvgPath ()
            svgRectangle.X           <- number (margin + x - 0.5 * rectangle.W)
            svgRectangle.Y           <- number (margin + y - 0.5 * rectangle.H)
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
            let textX = margin + x - sign * (0.5 * textW) + float offsetX * textW     
            let textY = margin + y - (0.5 * textH) + float offsetY * textH     
            svgText.X          <- numberCollection [| textX |]
            svgText.Y          <- numberCollection [| textY |]
            svgText.Content    <- text.Content
            svgText.FontSize   <- number text.FontSize
            svgText.TextAnchor <- anchor
            svgText.Color      <- color (text.Color)
            svgText.Fill       <- color (text.Color)
            svgText.Nodes.Add (new SvgContentNode (Content = text.Content))
            doc.Children.Add (svgText)
        | Path (path) ->
            let svgPath = new SvgPath ()
            svgPath.PathData    <- SvgPathBuilder.Parse(path.Data)
            svgPath.StrokeWidth <- number (path.StrokeWidth)
            svgPath.Stroke      <- color (path.StrokeColor)
            svgPath.Fill        <- color (path.FillColor)
            doc.Children.Add (svgPath)
            
    let build (model : Model) = 
        let doc = new SvgDocument ()
        doc.Width  <- number (margin + model.BlockSize * float model.ColumnCount)        
        doc.Height <- number (margin + model.BlockSize * float model.RowCount)
        model.Shapes
        |> List.rev
        |> List.iter (drawShape doc)
        doc
         
    let draw (path : string) (model : Model) = 
        let doc = build (model) 
        doc.Write (path)
    
    let save (path : string) (model : Model) = 
        let doc = build (model) 
        doc.Draw().Save(path)

                

    

namespace SvgDrawer

module SvgHelper =

    open System
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
        new SvgColourServer (System.Drawing.Color.FromArgb (color.A, color.R, color.G, color.B)) 


    let coordinate (model : Model) (index : int) = 
        margin + model.BlockSize * float (index - 1)

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
            let (x, y)    = rectangle.Position
            let coordX    = margin + x
            let coordY    = margin + y
            let (rx, ry)  = rectangle.RotateCenter 
            let transform = new Transforms.SvgRotate (float32 rectangle.Angle) 
            transform.CenterX <- float32 rx            
            transform.CenterY <- float32 ry            

            let svgRectangle = new SvgRectangle ()
            let path         = new SvgPath ()
            svgRectangle.X           <- number (coordX - 0.5 * rectangle.W)
            svgRectangle.Y           <- number (coordY - 0.5 * rectangle.H)
            svgRectangle.Width       <- number rectangle.W
            svgRectangle.Height      <- number rectangle.H
            svgRectangle.StrokeWidth <- number (rectangle.StrokeWidth)
            svgRectangle.Stroke      <- color (rectangle.StrokeColor)
            svgRectangle.Fill        <- color (rectangle.FillColor)
            svgRectangle.FillOpacity <- float32 (rectangle.FillColor.A / 255)  
            svgRectangle.Transforms.Add (transform)
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
        | Arc (arc) ->
            let svgPath    = new SvgPath ()
            let (ox, oy)   = arc.Position
            let r          = arc.R
            let startAngle = arc.StartAngle
            let endAngle   = arc.EndAngle
            let x1         = margin + ox + r * Math.Cos (startAngle / 180.0 * Math.PI)
            let y1         = margin + oy + r * Math.Sin (startAngle / 180.0 * Math.PI) 
            let x2         = margin + ox + r * Math.Cos (endAngle   / 180.0 * Math.PI)
            let y2         = margin + oy + r * Math.Sin (endAngle   / 180.0 * Math.PI) 
            let clockwise  = if arc.Clockwise then 1 else 0
            let over180    = 
                if arc.Clockwise then
                    if Math.Abs (endAngle - startAngle) >= 180.0 then 1 
                                                                 else 0
                else
                    if Math.Abs (endAngle - startAngle) <  180.0 then 1 
                                                                 else 0
            
            let data = 
                Printf.sprintf "M %.0f,%.0f A %.0f,%.0f %.1f %d,%d %.0f,%.0f" 
                               x1 y1 r r startAngle over180 clockwise x2 y2 
            svgPath.PathData    <- SvgPathBuilder.Parse(data)
            svgPath.StrokeWidth <- number (arc.StrokeWidth)
            svgPath.Stroke      <- color (arc.StrokeColor)
            svgPath.Fill        <- color (arc.FillColor)
            doc.Children.Add (svgPath)
        | Triangle (triangle) ->
            let svgPath    = new SvgPath ()
            let (ox, oy)   = triangle.Position
            let r          = triangle.R
            let angle      = triangle.Angle
            let x1 = margin + ox + r * Math.Cos ((  0.0 + angle) / 180.0 * Math.PI) 
            let y1 = margin + oy + r * Math.Sin ((  0.0 + angle) / 180.0 * Math.PI) 
            let x2 = margin + ox + r * Math.Cos ((120.0 + angle) / 180.0 * Math.PI) 
            let y2 = margin + oy + r * Math.Sin ((120.0 + angle) / 180.0 * Math.PI) 
            let x3 = margin + ox + r * Math.Cos ((240.0 + angle) / 180.0 * Math.PI) 
            let y3 = margin + oy + r * Math.Sin ((240.0 + angle) / 180.0 * Math.PI) 
            let data = 
                Printf.sprintf "M %.0f,%.0f %.0f,%.0f %.0f,%.0f z" 
                               x1 y1 x2 y2 x3 y3
            svgPath.PathData    <- SvgPathBuilder.Parse(data)
            svgPath.StrokeWidth <- number (triangle.StrokeWidth)
            svgPath.Stroke      <- color (triangle.StrokeColor)
            svgPath.Fill        <- color (triangle.FillColor)
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
        model
    
    let save (path : string) (model : Model) = 
        let doc = build (model) 
        doc.Draw().Save(path)
        model

                

    

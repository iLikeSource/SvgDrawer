namespace SvgDrawer

type Color =  
    { R : int 
      G : int
      B : int
      A : int }
    with 
    static member Transparent = { R = 255; G = 255; B = 255; A =   0 }
    static member Black       = { R =   0; G =   0; B =   0; A = 255 }
    static member White       = { R = 255; G = 255; B = 255; A = 255 }
    static member Gray        = { R = 204; G = 204; B = 204; A = 255 }
    static member Red         = { R = 255; G =   0; B =   0; A = 255 }
    static member Green       = { R =   0; G = 255; B =   0; A = 255 }
    static member Blue        = { R =   0; G =   0; B = 255; A = 255 }
    static member FromName (s : string) : Color =
        match s.ToLower () with
        | "transparent" -> Color.Transparent 
        | "black"       -> Color.Black 
        | "white"       -> Color.White 
        | "gray"        -> Color.Gray 
        | "blue"        -> Color.Blue 
        | "red"         -> Color.Red 
        | "green"       -> Color.Green 
        | _             -> failwith "未実装" 



type Action = 
    | Move  of int * int 
    | RMove of int * int 
    with 
    member __.On (model : Model) =
        match __ with
        | Move  (x, y) -> 
            { model with Position = (x, y) } 
        | RMove (x, y) -> 
            let (x0, y0) = model.Position
            { model with Position = (x0 + x, y0 + y) } 

and Attr =
    | Position     of float * float 
    | RotateCenter of float * float 
    | Angle        of float 
    | Width        of float 
    | Height       of float 
    | Radius       of float 
    | StrokeWidth  of float 
    | StrokeColor  of string 
    | FillColor    of string 
    | FontSize     of float 
    with 
    member __.With (model : Model) = 
        match model.Shapes with
        | Line      (line)      :: tl -> { model with Shapes = Line      (line.Attr      __ model) :: tl } 
        | Circle    (circle)    :: tl -> { model with Shapes = Circle    (circle.Attr    __ model) :: tl }  
        | Rectangle (rectangle) :: tl -> { model with Shapes = Rectangle (rectangle.Attr __ model) :: tl }  
        | Text      (text)      :: tl -> { model with Shapes = Text      (text.Attr      __ model) :: tl }   
        | Path      (path)      :: tl -> { model with Shapes = Path      (path.Attr      __ model) :: tl }    
        | Arc       (arc)       :: tl -> { model with Shapes = Arc       (arc.Attr       __ model) :: tl }     
        | Triangle  (triangle)  :: tl -> { model with Shapes = Triangle  (triangle.Attr  __ model) :: tl }     
        | [] -> model
    
    member __.WithDefault (model : Model) = 
        match __ with
        | StrokeColor (color) -> { model with StrokeColor = Color.FromName color }
        | StrokeWidth (width) -> { model with StrokeWidth = width }
        | _                   -> model

and Line =
    { Start       : float * float
      End         : float * float
      StrokeWidth : float
      StrokeColor : Color }
    with 
    static member Default () = 
        { Start       = (0.0, 0.0)
          End         = (0.0, 0.0)
          StrokeWidth = 1.0
          StrokeColor = Color.Black }
    static member To (x, y) (model : Model) = 
        let blockSize = model.BlockSize
        let (startX, startY) = model.Position
        let (endX  , endY  ) = (x, y)
        let shape =
            { Line.Default () with Start       = (float (startX - 1) * blockSize, float (startY - 1) * blockSize) 
                                   End         = (float (endX   - 1) * blockSize, float (endY   - 1) * blockSize) 
                                   StrokeColor = model.StrokeColor 
                                   StrokeWidth = model.StrokeWidth  }
        { model with Position = (x, y)
                     Shapes   = Line (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Line = 
        match attr with
        | StrokeWidth (width) -> { __ with StrokeWidth = width } 
        | StrokeColor (color) -> { __ with StrokeColor = Color.FromName color } 
        | _                 -> __
        

and Circle = 
    { Position    : float * float
      R           : float 
      StrokeWidth : float
      StrokeColor : Color 
      FillColor   : Color }
    with 
    static member Default () = 
        { Position    = (0.0, 0.0)
          R           = 10.0
          StrokeWidth = 1.0
          StrokeColor = Color.Black 
          FillColor   = Color.White }
    static member On (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y) = model.Position
        let shape =
            { Circle.Default () with Position    = (float (x - 1) * blockSize, float (y - 1) * blockSize)  
                                     StrokeColor = model.StrokeColor 
                                     StrokeWidth = model.StrokeWidth }
        { model with  Shapes = Circle (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Circle =
        let blockSize = model.BlockSize 
        match attr with
        | Position    (x, y)   -> { __ with Position = (x, y) }
        | Width       (width)  -> { __ with R = width  * blockSize }
        | Height      (height) -> { __ with R = height * blockSize } 
        | Radius      (radius) -> { __ with R = radius * blockSize }  
        | StrokeWidth (width)  -> { __ with StrokeWidth = width } 
        | StrokeColor (color)  -> { __ with StrokeColor = Color.FromName (color) } 
        | FillColor   (color)  -> { __ with FillColor = Color.FromName (color) } 
        | _                    -> __

and Rectangle = 
    { Position     : float * float 
      W            : float 
      H            : float
      Angle        : float  
      RotateCenter : float * float 
      StrokeWidth  : float
      StrokeColor  : Color 
      FillColor    : Color } 
    with 
    static member Default () = 
        { Position     = (0.0, 0.0)
          W            = 10.0
          H            = 10.0
          Angle        = 0.0
          RotateCenter = (0.0, 0.0)
          StrokeWidth  = 1.0
          StrokeColor  = Color.Black 
          FillColor    = Color.White }
    static member On (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y)    = model.Position
        let coordX    = float (x - 1) * blockSize
        let coordY    = float (y - 1) * blockSize
        let shape =
            { Rectangle.Default () with Position     = (coordX, coordY) 
                                        RotateCenter = (coordX, coordY) 
                                        StrokeColor  = model.StrokeColor 
                                        StrokeWidth  = model.StrokeWidth }
        { model with  Shapes = Rectangle (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Rectangle =
        let blockSize = model.BlockSize 
        match attr with
        | Position     (x, y)   -> { __ with Position = (x, y) }
        | RotateCenter (x, y)   -> { __ with RotateCenter = (x, y) }
        | Angle        (angle)  -> { __ with Angle = angle }
        | Width        (width)  -> { __ with W = width  * blockSize }
        | Height       (height) -> { __ with H = height * blockSize } 
        | StrokeWidth  (width)  -> { __ with StrokeWidth = width } 
        | StrokeColor  (color)  -> { __ with StrokeColor = Color.FromName (color) } 
        | FillColor    (color)  -> { __ with FillColor = Color.FromName (color) } 
        | _                     -> __

and Text = 
    { Position : float * float 
      Offset   : int * int
      Content  : string 
      Color    : Color
      FontSize : float }
    with
    static member Default () = 
        { Position = (0.0, 0.0) 
          Offset   = (0, 0)
          Content  = "" 
          Color    = Color.Black
          FontSize = 8.0 }
    static member On (content : string) (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y) = model.Position
        let shape =
            { Text.Default () with Position = (float (x - 1) * blockSize, float (y - 1) * blockSize) 
                                   Content  = content }
        { model with  Shapes = Text (shape) :: model.Shapes }
    
    static member At (content : string, offsetX : int, offsetY : int) (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y) = model.Position
        let shape =
            { Text.Default () with Position = (float (x) * blockSize, float (y) * blockSize) 
                                   Offset   = (offsetX, offsetY) 
                                   Content  = content }
        { model with  Shapes = Text (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Text =
        let blockSize = model.BlockSize 
        match attr with
        | Position  (x, y)    -> { __ with Position = (x, y) }
        | StrokeColor (color) -> { __ with Color = Color.FromName (color) } 
        | FontSize    (size)  -> { __ with FontSize = size } 
        | _                   -> __

and Path = 
    { Data        : string 
      StrokeColor : Color 
      FillColor   : Color 
      StrokeWidth : float }
    static member Default () = 
        { Data        = ""
          StrokeWidth = 1.0
          StrokeColor = Color.Black 
          FillColor   = Color.Transparent }
    static member On (data : string) (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y)    = model.Position
        let shape =
            { Path.Default () with Data        = data
                                   StrokeColor = model.StrokeColor 
                                   StrokeWidth = model.StrokeWidth }
        { model with  Shapes = Path (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Path =
        let blockSize = model.BlockSize 
        match attr with
        | StrokeWidth (width) -> { __ with StrokeWidth = width } 
        | StrokeColor (color) -> { __ with StrokeColor = Color.FromName (color) } 
        | FillColor (color)   -> { __ with FillColor = Color.FromName (color) } 
        | _                   -> __

and Arc = 
    { Position    : float * float
      StartAngle  : float
      EndAngle    : float
      R           : float
      Clockwise   : bool
      StrokeColor : Color 
      FillColor   : Color 
      StrokeWidth : float }
    static member Default () = 
        { Position    = (0.0, 0.0)
          StartAngle  = 0.0
          EndAngle    = 90.0
          R           = 10.0
          Clockwise   = true
          StrokeWidth = 1.0
          StrokeColor = Color.Black 
          FillColor   = Color.Transparent }
    static member On (r, startAngle, endAngle, clockwise) (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y)    = model.Position
        let shape =
            { Arc.Default () with Position    = (float (x - 1) * blockSize, float (y - 1) * blockSize)  
                                  StartAngle  = startAngle
                                  EndAngle    = endAngle
                                  R           = r
                                  Clockwise   = clockwise
                                  StrokeColor = model.StrokeColor 
                                  StrokeWidth = model.StrokeWidth }
        { model with  Shapes = Arc (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Arc =
        let blockSize = model.BlockSize 
        match attr with
        | Position  (x, y)    -> { __ with Position = (x, y) }
        | StrokeWidth (width) -> { __ with StrokeWidth = width } 
        | StrokeColor (color) -> { __ with StrokeColor = Color.FromName (color) } 
        | FillColor (color)   -> { __ with FillColor = Color.FromName (color) } 
        | Width      (width)  -> { __ with R = width  * blockSize }
        | Height     (height) -> { __ with R = height * blockSize } 
        | Radius     (radius) -> { __ with R = radius * blockSize }  
        | _                   -> __

and Triangle  = 
    { Position    : float * float
      Angle       : float
      R           : float 
      StrokeColor : Color 
      FillColor   : Color 
      StrokeWidth : float }
    static member Default () = 
        { Position    = (0.0, 0.0)
          Angle       = 0.0
          R           = 10.0  
          StrokeWidth = 1.0
          StrokeColor = Color.Black 
          FillColor   = Color.Transparent }
    static member On (r, angle) (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y)    = model.Position
        let shape =
            { Triangle.Default () with Position    = (float (x - 1) * blockSize, float (y - 1) * blockSize)  
                                       Angle       = angle
                                       R           = r
                                       StrokeColor = model.StrokeColor 
                                       StrokeWidth = model.StrokeWidth }
        { model with  Shapes = Triangle (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Triangle =
        let blockSize = model.BlockSize 
        match attr with
        | Position  (x, y)    -> { __ with Position = (x, y) }
        | StrokeWidth (width) -> { __ with StrokeWidth = width } 
        | StrokeColor (color) -> { __ with StrokeColor = Color.FromName (color) } 
        | FillColor (color)   -> { __ with FillColor = Color.FromName (color) } 
        | Width      (width)  -> { __ with R = width  * blockSize }
        | Height     (height) -> { __ with R = height * blockSize } 
        | Radius     (radius) -> { __ with R = radius * blockSize }  
        | _                   -> __



and Shape = 
    | Line      of Line
    | Rectangle of Rectangle
    | Circle    of Circle
    | Text      of Text
    | Path      of Path
    | Arc       of Arc
    | Triangle  of Triangle


and Model = 
    { BlockSize   : float
      RowCount    : int 
      ColumnCount : int
      Position    : int * int
      StrokeColor : Color
      StrokeWidth : float
      Shapes      : Shape list }
    with 
    member __.Shape = List.head __.Shapes



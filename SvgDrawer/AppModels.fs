namespace SvgDrawer

type Color =  
    { R : int 
      G : int
      B : int }
    with 
    static member Black = { R =   0; G =   0; B =   0 }
    static member White = { R = 255; G = 255; B = 255 }
    static member FromName (s : string) : Color =
        match s.ToLower () with
        | "black" -> Color.Black 
        | "white" -> Color.White 
        | _       -> failwith "未実装" 



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
    | Width       of int 
    | Height      of int 
    | Radius      of int 
    | StrokeWidth of float 
    | StrokeColor of string 
    | FillColor   of string 
    | FontSize    of float 
    with 
    member __.With (model : Model) = 
        match model.Shapes with
        | Line (line)           :: tl -> { model with Shapes = Line      (line.Attr      __ model) :: tl } 
        | Circle (circle)       :: tl -> { model with Shapes = Circle    (circle.Attr    __ model) :: tl }  
        | Rectangle (rectangle) :: tl -> { model with Shapes = Rectangle (rectangle.Attr __ model) :: tl }  
        | _ -> failwith "想定外"

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
            { Line.Default () with Start = (float (startX - 1) * blockSize, float (startY - 1) * blockSize) 
                                   End   = (float (endX   - 1) * blockSize, float (endY   - 1) * blockSize) }
        { model with Position = (x, y)
                     Shapes   = Line (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Line = 
        match attr with
        | StrokeWidth (width) -> { __ with StrokeWidth = width } 
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
            { Circle.Default () with Position = (float (x - 1) * blockSize, float (y - 1) * blockSize) }
        { model with  Shapes = Circle (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Circle =
        let blockSize = model.BlockSize 
        match attr with
        | Width     (width)   -> { __ with R = float width  * blockSize }
        | Height    (height)  -> { __ with R = float height * blockSize } 
        | Radius    (radius)  -> { __ with R = float radius * blockSize }  
        | StrokeWidth (width) -> { __ with StrokeWidth = width } 
        | StrokeColor (color) -> { __ with StrokeColor = Color.FromName (color) } 
        | FillColor (color)   -> { __ with FillColor = Color.FromName (color) } 
        | _                   -> __

and Rectangle = 
    { Position    : float * float 
      W           : float 
      H           : float  
      StrokeWidth : float
      StrokeColor : Color 
      FillColor   : Color } 
    with 
    static member Default () = 
        { Position    = (0.0, 0.0)
          W           = 10.0
          H           = 10.0
          StrokeWidth = 1.0
          StrokeColor = Color.Black 
          FillColor   = Color.White }
    static member On (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y) = model.Position
        let shape =
            { Rectangle.Default () with Position = (float (x - 1) * blockSize, float (y - 1) * blockSize) }
        { model with  Shapes = Rectangle (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Rectangle =
        let blockSize = model.BlockSize 
        match attr with
        | Width     (width)   -> { __ with W = float width  * blockSize }
        | Height    (height)  -> { __ with H = float height * blockSize } 
        | StrokeWidth (width) -> { __ with StrokeWidth = width } 
        | StrokeColor (color) -> { __ with StrokeColor = Color.FromName (color) } 
        | FillColor (color)   -> { __ with FillColor = Color.FromName (color) } 
        | _                   -> __

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
    static member On (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y) = model.Position
        let shape =
            { Text.Default () with Position = (float (x - 1) * blockSize, float (y - 1) * blockSize) }
        { model with  Shapes = Text (shape) :: model.Shapes }
    
    static member At (offsetX : int, offsetY : int) (model : Model) = 
        let blockSize = model.BlockSize
        let (x, y) = model.Position
        let shape =
            { Text.Default () with Position = (float (x - 1) * blockSize, float (y - 1) * blockSize) 
                                   Offset   = (offsetX, offsetY) }
        { model with  Shapes = Text (shape) :: model.Shapes }

    member __.Attr (attr : Attr) (model : Model) : Text =
        let blockSize = model.BlockSize 
        match attr with
        | StrokeColor (color) -> { __ with Color = Color.FromName (color) } 
        | FontSize    (size)  -> { __ with FontSize = size } 
        | _                   -> __


and Shape = 
    | Line      of Line
    | Rectangle of Rectangle
    | Circle    of Circle
    | Text      of Text


and Model = 
    { BlockSize   : float
      RowCount    : int 
      ColumnCount : int
      Position    : int * int
      Shapes      : Shape list }
    with 
    member __.Shape = List.head __.Shapes



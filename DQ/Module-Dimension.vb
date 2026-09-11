Imports netDxf
Imports netDxf.Entities
Imports netDxf.Entities.HatchBoundaryPath
Imports netDxf.Header
Imports netDxf.Tables
Imports netDxf.Units

Module Module4
    Sub CreateContinueDimensionsWithLoop_x(ByVal _doc As DxfDocument, ByVal _xCoords() As Double, ByVal _y As Double, ByVal scale_raito As Double)




        Dim doc As DxfDocument = _doc
        Dim dimStyle As New DimensionStyle("MyDimStyle")
        '设置小数位数为 2 位
        dimStyle.LengthPrecision = 2

        doc.DimensionStyles.Add(dimStyle)


        Dim layer As Layer
        If Not doc.Layers.Contains("DJL挡墙水平标注") Then
            layer = New Layer("DJL挡墙水平标注")
            doc.Layers.Add(layer)
        Else
            layer = doc.Layers("DJL挡墙水平标注")
        End If
        ' ✅ 设置图层为紫色
        layer.Color = AciColor.Green



        ' 定义所有点的X坐标
        'Dim xCoords() As Double = {0, 50, 100, 150, 200}
        Dim xCoords() As Double = _xCoords
        Dim y As Double = _y
        'Dim yPos As Double = 10 ' 尺寸线Y位置

        ' 创建第一个标注
        'Dim prevDim As LinearDimension = Nothing
        'MessageBox.Show(xCoords.Length)
        For i As Integer = 0 To xCoords.Length - 2
            Dim startPoint As New Vector2(xCoords(i), y)
            Dim endPoint As New Vector2(xCoords(i + 1), y)
            'Dim line1 As New Line(New Vector2(0, 0), New Vector2(100, 100))
            Dim line_ref As New netDxf.Entities.Line(startPoint, endPoint)
            'Dim line_ref As New netDxf.Entities.Line(endPoint, startPoint)
            Dim currentDim As New LinearDimension(
               line_ref,
             -1 * scale_raito,
               180,
                dimStyle)
            currentDim.Layer = layer
            doc.Entities.Add(currentDim)
        Next



        'Dim startPoint_temp As New Vector2(xCoords(0), y)
        'Dim endPoint_temp As New Vector2(xCoords(xCoords.Length - 1), y)
        ''Dim line1 As New Line(New Vector2(0, 0), New Vector2(100, 100))
        'Dim line_ref_temp As New netDxf.Entities.Line(startPoint_temp, endPoint_temp)

        'Dim currentDim_temp As New LinearDimension(
        '       line_ref_temp,
        '      0.7 * scale_raito,
        '       180,
        '        dimStyle)
        'doc.Entities.Add(currentDim_temp)



        'doc.Save("continue_dimensions_loop.dxf")
    End Sub

    Sub CreateContinueDimensionsWithLoop_y(ByVal _doc As DxfDocument, ByVal _yCoords() As Double, ByVal _x As Double, ByVal scale_raito As Double)
        Dim doc As DxfDocument = _doc
        Dim dimStyle As New DimensionStyle("MyDimStyle")
        '设置小数位数为 2 位
        dimStyle.LengthPrecision = 2
        doc.DimensionStyles.Add(dimStyle)



        Dim layer As Layer
        If Not doc.Layers.Contains("DJL挡墙竖向标注") Then
            layer = New Layer("DJL挡墙竖向标注")
            doc.Layers.Add(layer)
        Else
            layer = doc.Layers("DJL挡墙竖向标注")
        End If
        ' ✅ 设置图层为紫色
        layer.Color = AciColor.Green


        ' 定义所有点的X坐标
        'Dim xCoords() As Double = {0, 50, 100, 150, 200}
        Dim yCoords() As Double = _yCoords
        Dim x As Double = _x
        'Dim yPos As Double = 10 ' 尺寸线Y位置

        ' 创建第一个标注
        'Dim prevDim As LinearDimension = Nothing

        For i As Integer = 0 To yCoords.Length - 2
            Dim startPoint As New Vector2(x, yCoords(i))
            Dim endPoint As New Vector2(x, yCoords(i + 1))
            'Dim line1 As New Line(New Vector2(0, 0), New Vector2(100, 100))
            Dim line_ref As New netDxf.Entities.Line(startPoint, endPoint)
            Dim currentDim As New LinearDimension(
               line_ref,
             0.5 * scale_raito,
               -270,
                dimStyle)
            currentDim.Layer = layer
            doc.Entities.Add(currentDim)
        Next

        'Dim startPoint_temp As New Vector2(x, yCoords(0))
        'Dim endPoint_temp As New Vector2(x, yCoords(yCoords.Length - 1))
        ''Dim line1 As New Line(New Vector2(0, 0), New Vector2(100, 100))
        'Dim line_ref_temp As New netDxf.Entities.Line(startPoint_temp, endPoint_temp)

        'Dim currentDim_temp As New LinearDimension(
        '       line_ref_temp,
        '      0.9 * scale_raito,
        '        -270,
        '        dimStyle)
        'doc.Entities.Add(currentDim_temp)



    End Sub

    Sub CreateAngleDimension(ByVal _doc As DxfDocument, ByVal _center As Vector2, ByVal _startPoint As Vector2, ByVal _endPoint As Vector2)
        Dim doc As DxfDocument = _doc
        'Dim dimStyle As New DimensionStyle("MyDimStyle")
        'doc.DimensionStyles.Add(dimStyle)


        ' 定义起始点和终止点（相对于圆心）
        'Dim center As  Vector2(150, 100) ' 3点钟方向
        'Dim startPoint As New Vector2(150, 100) ' 3点钟方向
        'Dim endPoint As New Vector2(100, 150)   ' 12点钟方向
        Dim center As Vector2 = _center ' 
        Dim startPoint As Vector2 = _startPoint ' 
        Dim endPoint As Vector2 = _endPoint   ' 

        Dim line_start As New netDxf.Entities.Line(center, startPoint)
        Dim line_end As New netDxf.Entities.Line(center, endPoint)
        doc.Entities.Add(line_start)
        doc.Entities.Add(line_end)

        ' 创建角度标注
        Dim angularDim As New Angular3PointDimension(
        center,
        startPoint,
      endPoint,
        5) ' 

        doc.Entities.Add(angularDim)


    End Sub

    Sub CreateAngleDimension2(ByVal _doc As DxfDocument, ByVal _center As Vector2, ByVal _startPoint As Vector2, ByVal _endPoint As Vector2)
        Dim doc As DxfDocument = _doc
        'Dim dimStyle As New DimensionStyle("MyDimStyle")
        'doc.DimensionStyles.Add(dimStyle)


        ' 定义起始点和终止点（相对于圆心）
        'Dim center As  Vector2(150, 100) ' 3点钟方向
        'Dim startPoint As New Vector2(150, 100) ' 3点钟方向
        'Dim endPoint As New Vector2(100, 150)   ' 12点钟方向
        Dim center As Vector2 = _center ' 
        Dim startPoint As Vector2 = _startPoint ' 
        Dim endPoint As Vector2 = _endPoint

        Dim line_start As New netDxf.Entities.Line(center, startPoint)
        Dim line_end As New netDxf.Entities.Line(center, endPoint)
        doc.Entities.Add(line_start)
        doc.Entities.Add(line_end)

        ' 创建角度标注
        Dim angularDim As New Angular2LineDimension(
        line_end,
      line_start,
        0) ' 

        doc.Entities.Add(angularDim)


    End Sub


End Module

Imports netDxf
Imports netDxf.Collections
Imports netDxf.Entities
Imports netDxf.Entities.HatchBoundaryPath
Imports netDxf.Tables ' 必须引用这个命名空间，才能使用 Layer 类型
Imports System.Data

Module Module_draw_dmx


    ''' <summary>
    ''' 根据 DataTable 中的 XY 坐标，在 DxfDocument 中绘制多段线
    ''' </summary>
    ''' <param name="doc">DxfDocument 文档</param>
    ''' <param name="dt">包含 X、Y 坐标的 DataTable</param>
    ''' <param name="layerName">图层名称</param>
    Public Sub DrawPolylineFromDataTable(
    doc As DxfDocument,
    dt As DataTable,
    layerName As String)

        If doc Is Nothing OrElse dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return
        End If

        ' 1. 创建或获取图层
        Dim layer As Layer
        If Not doc.Layers.Contains(layerName) Then
            layer = New Layer(layerName)
            doc.Layers.Add(layer)
        Else
            layer = doc.Layers(layerName)
        End If
        ' ✅ 设置图层为紫色
        layer.Color = AciColor.Cyan
        ' 2. 构建 Polyline2D 顶点集合
        Dim vertexList As New List(Of Vector2)
        For Each row As DataRow In dt.Rows
            Dim x As Double = Convert.ToDouble(row("X"))
            Dim y As Double = Convert.ToDouble(row("Y"))
            vertexList.Add(New Vector2(x, y))
        Next

        ' 3. 创建多段线（2D）
        Dim polyline As New Polyline2D(vertexList)

        ' 是否闭合（如需闭合，可改为 True）
        polyline.IsClosed = False

        ' 4. 设置图层
        polyline.Layer = layer
        polyline.SetConstantWidth(0.05)
        ' 5. 添加到文档
        doc.Entities.Add(polyline)

    End Sub
End Module

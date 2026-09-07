Imports netDxf
Imports netDxf.Entities.HatchBoundaryPath
Imports netDxf.Tables

Imports netDxf.Collections
Imports netDxf.Entities

Imports System.Data

Module Module_DrawElevations
    Public Sub DrawElevations(
        doc As DxfDocument,
       x As Double,
        y As Double,
        str_display As String)

        'For Each row As DataRow In dt.Rows

        'Dim x = CDbl(row("X"))
        '    Dim y = CDbl(row("Y"))
        '    Dim z = CDbl(row("Z"))
        ''Dim x = x
        ''Dim y = CDbl(row("Y"))
        ''Dim z = CDbl(row("Z"))
        ' 1️⃣ 三角形符号


        ' 1. 创建或获取图层
        Dim layer As Layer
        If Not doc.Layers.Contains("DJL挡墙标高符号") Then
            layer = New Layer("DJL挡墙标高符号")
            doc.Layers.Add(layer)
        Else
            layer = doc.Layers("DJL挡墙标高符号")
        End If
        ' ✅ 设置图层为紫色
        layer.Color = AciColor.Red


        ' 1. 创建或获取图层
        Dim layer2 As Layer
        If Not doc.Layers.Contains("DJL挡墙标高数字") Then
            layer2 = New Layer("DJL挡墙标高数字")
            doc.Layers.Add(layer2)
        Else
            layer2 = doc.Layers("DJL挡墙标高数字")
        End If
        ' ✅ 设置图层为紫色
        layer.Color = AciColor.Red

        Dim num As Double
        If Double.TryParse(str_display, num) Then
            '是数值，num保存转换结果
            ' ========= 1. 三角形符号 =========
            Dim pts As New List(Of Vector2) From {
                New Vector2(x, y),
                New Vector2(x + 0.15, y + 0.2),
                New Vector2(x - 0.15, y + 0.2)
            }

            Dim tri As New Polyline2D(pts)
            tri.IsClosed = True
            tri.Layer = layer
            tri.SetConstantWidth(0.01)
            doc.Entities.Add(tri)
        Else
            '不是数值

            'txt.Alignment = TextAlignment.MiddleCenter; 
        End If


        ' ========= 2. 标高文字 =========
        'Dim txt As New Text(
        '         num_display.ToString("0.00"),
        '        New Vector3(x, y + 0.25, 0),
        '        0.2)
        Dim txt As New Text(
                 str_display,
                New Vector3(x, y + 0.25, 0),
                0.2)

        txt.Layer = layer2
        txt.Alignment = TextAlignment.BottomCenter
        doc.Entities.Add(txt)
        'Next
    End Sub

    Public Sub Drawtext(
    doc As DxfDocument,
   x As Double,
    y As Double,
    str_display As String)

        'For Each row As DataRow In dt.Rows

        'Dim x = CDbl(row("X"))
        '    Dim y = CDbl(row("Y"))
        '    Dim z = CDbl(row("Z"))
        ''Dim x = x
        ''Dim y = CDbl(row("Y"))
        ''Dim z = CDbl(row("Z"))
        ' 1️⃣ 三角形符号


        ' 1. 创建或获取图层
        Dim layer As Layer
        If Not doc.Layers.Contains("DJL挡墙编号") Then
            layer = New Layer("DJL挡墙编号")
            doc.Layers.Add(layer)
        Else
            layer = doc.Layers("DJL挡墙编号")
        End If
        ' ✅ 设置图层为紫色
        layer.Color = AciColor.Red


        '' 1. 创建或获取图层
        'Dim layer2 As Layer
        'If Not doc.Layers.Contains("DJL挡墙标高数字") Then
        '    layer2 = New Layer("DJL挡墙标高数字")
        '    doc.Layers.Add(layer2)
        'Else
        '    layer2 = doc.Layers("DJL挡墙标高数字")
        'End If
        ' ✅ 设置图层为紫色
        'layer.Color = AciColor.Red

        'Dim num As Double
        'If Double.TryParse(str_display, num) Then
        '    '是数值，num保存转换结果
        '    ' ========= 1. 三角形符号 =========
        '    Dim pts As New List(Of Vector2) From {
        '        New Vector2(x, y),
        '        New Vector2(x + 0.15, y + 0.2),
        '        New Vector2(x - 0.15, y + 0.2)
        '    }

        '    Dim tri As New Polyline2D(pts)
        '    tri.IsClosed = True
        '    tri.Layer = layer
        '    tri.SetConstantWidth(0.01)
        '    doc.Entities.Add(tri)
        'Else
        '    '不是数值

        '    'txt.Alignment = TextAlignment.MiddleCenter; 
        'End If


        ' ========= 2. 标高文字 =========
        'Dim txt As New Text(
        '         num_display.ToString("0.00"),
        '        New Vector3(x, y + 0.25, 0),
        '        0.2)
        Dim txt As New Text(
                 str_display,
                New Vector3(x, y + 0.25, 0),
                0.2)

        txt.Layer = layer
        txt.Alignment = TextAlignment.MiddleCenter
        doc.Entities.Add(txt)
        'Next
    End Sub
End Module

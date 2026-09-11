Imports System.IO
Imports netDxf
Imports netDxf.Tables
Imports NPOI.SS.Formula.Functions

Module Module_draw4
    Public Sub Createdqdxf4(ByVal strfilename As String, ByVal dt As DataTable， ByVal datatable_B As DataTable， ByVal datatable_T As DataTable， ByVal dtTemplate As DataTable)
        'Dim doc As New DxfDocument(DxfVersion.AutoCad2018)
        Dim dt_temp As DataTable = dt.Copy
        Dim doc As New DxfDocument()
        ' 1. 创建或获取图层

        Dim layer As Layer
        If Not doc.Layers.Contains("DJL挡墙") Then
            layer = New Layer("DJL挡墙")
            doc.Layers.Add(layer)
        Else
            layer = doc.Layers("DJL挡墙")
        End If
        ' ✅ 设置图层为紫色
        layer.Color = AciColor.Magenta

        For i = 0 To dt_temp.Rows.Count - 1
            Dim x As Double = dt_temp.Rows(i)("x")
            Dim dqdy1 As Double = dt_temp.Rows(i)("dqdy1")
            Dim dqdy2 As Double = dt_temp.Rows(i)("dqdy2")
            Dim dq_‌foundation_height As Double = dt_temp.Rows(i)("dq_‌foundation_height")
            Dim x_next As Double
            Dim dqdy2_next As Double
            Dim dqdy1_next As Double
            If i <> dt_temp.Rows.Count - 1 Then
                x_next = dt_temp.Rows(i + 1)("x")
                dqdy2_next = dt_temp.Rows(i + 1)("dqdy2")
                dqdy1_next = dt_temp.Rows(i + 1)("dqdy1")
            Else
                x_next = dt_temp.Rows(i)("x") + dt_temp.Rows(i)("L")
                dqdy2_next = dqdy2
                dqdy1_next = dqdy1
            End If


            '以下绘制剖分线1
            Dim vertexes_1 As New List(Of Vector2) From {
    New Vector2(x, dqdy2),
    New Vector2(x, dqdy1)
     }
            Dim polyline1 As New netDxf.Entities.Polyline2D(vertexes_1)
            polyline1.Layer = layer
            polyline1.SetConstantWidth(0.05)
            doc.Entities.Add(polyline1)

            '以下绘制剖分线2
            Dim vertexes_1P As New List(Of Vector2) From {
    New Vector2(x_next, dqdy2_next),
    New Vector2(x_next, dqdy1)
     }
            Dim polyline1p As New netDxf.Entities.Polyline2D(vertexes_1P)
            polyline1p.Layer = layer
            polyline1p.SetConstantWidth(0.05)
            doc.Entities.Add(polyline1p)


            '以下绘制顶线
            Dim vertexes_2 As New List(Of Vector2) From {
    New Vector2(x, dqdy2),
    New Vector2(x_next, dqdy2_next)
     }
            '              Dim vertexes_3 As New List(Of Vector2) From {
            '                 New Vector2(x, dqdy1),
            '                 New Vector2(x_next, dqdy1_next)
            '}

            Dim polyline2 As New netDxf.Entities.Polyline2D(vertexes_2)
            polyline2.Layer = layer
            polyline2.SetConstantWidth(0.05)
            doc.Entities.Add(polyline2)

            '以下绘制底线
            Dim vertexes_3 As New List(Of Vector2) From {
  New Vector2(x, dqdy1),
  New Vector2(x_next, dqdy1)
   }

            Dim polyline3 As New netDxf.Entities.Polyline2D(vertexes_3)
            polyline3.Layer = layer
            polyline3.SetConstantWidth(0.05)
            doc.Entities.Add(polyline3)


            '以下绘制最基础线
            'MessageBox.Show(dq_‌foundation_height)
            Dim vertexes_4 As New List(Of Vector2) From {
  New Vector2(x, dqdy1 + dq_‌foundation_height),
  New Vector2(x_next, dqdy1 + dq_‌foundation_height)'我他妈也不知道怎么回事
   }

            Dim polyline4 As New netDxf.Entities.Polyline2D(vertexes_4)
            polyline4.Layer = layer
            polyline4.SetConstantWidth(0.05)
            doc.Entities.Add(polyline4)
            'Call DrawElevations(doc, (x + x_next) * 0.5, dqdy1, dqdy1) '貌似底高程
            'Call DrawElevations(doc, x, dqdy2, dqdy2) '貌似顶高程
            'Call DrawElevations(doc, (x + x_next) * 0.5, (dqdy1 + dqdy2) * 0.5, dtTemplate.Rows(i)("group")) '挡墙编号
            'Call DrawElevations(doc, (x + x_next) * 0.5, dqdy1, dqdy1.ToString("0.00")) '貌似底高程
            'Call DrawElevations(doc, x, dqdy2, dqdy2.ToString("0.00")) '貌似顶高程
            Call DrawElevations(doc, (x + x_next) * 0.5, dqdy1, dqdy1.ToString()) '貌似底高程
            Call DrawElevations(doc, x, dqdy2, dqdy2.ToString()) '貌似顶高程
            Call Drawtext(doc, (x + x_next) * 0.5, (dqdy1 + dqdy2) * 0.5, dt_temp.Rows(i)("group") & "#") '挡墙编号
            'num_display.ToString("0.00")
        Next

        For n = 0 To dtTemplate.Rows.Count - 1
            '以下绘制剖分线
            Dim vertexes_p As New List(Of Vector2) From {
  New Vector2(dtTemplate.Rows(n)("x"), dtTemplate.Rows(n)("dqdy1")),
  New Vector2(dtTemplate.Rows(n)("x"), dtTemplate.Rows(n)("dqdy2"))
   }

            Dim polyline_p As New netDxf.Entities.Polyline2D(vertexes_p)
            polyline_p.Layer = layer
            polyline_p.SetConstantWidth(0.01)
            doc.Entities.Add(polyline_p)
            Call Drawtext(doc, dtTemplate.Rows(n)("x"), (dtTemplate.Rows(n)("dqdy2") + dtTemplate.Rows(n)("dqdy1")) * 0.5, dtTemplate.Rows(n)("ID")) '挡墙编号
        Next

        For j = 0 To dt_temp.Rows.Count - 1
            If j <> dt_temp.Rows.Count - 1 Then
                Dim xCoords() As Double = {dt_temp.Rows(j)("x"), dt_temp.Rows(j + 1)("x")}
                Call CreateContinueDimensionsWithLoop_x(doc, xCoords, Math.Max(dt_temp.Rows(j)("dqdy2"), dt_temp.Rows(j + 1)("dqdy2")), 1)
            Else
                Dim xCoords() As Double = {dt_temp.Rows(j)("x"), dt_temp.Rows(j)("x") + dt_temp.Rows(dt_temp.Rows.Count - 1)("L")}
                Call CreateContinueDimensionsWithLoop_x(doc, xCoords, dt_temp.Rows(j)("dqdy2"), 1)
            End If

        Next
        Dim max_dqdy2 As Double = dt_temp.AsEnumerable().Max(Function(row) row.Field(Of Double)("dqdy2"))
        Dim xCoords_total() As Double = {dt_temp.Rows(0)("x"), dt_temp.Rows(dt_temp.Rows.Count - 1)("x") + dt_temp.Rows(dt_temp.Rows.Count - 1)("L")}
        Call CreateContinueDimensionsWithLoop_x(doc, xCoords_total, max_dqdy2, 2)
        For m = 0 To dt_temp.Rows.Count - 1
            Dim yCoords() As Double = {dt_temp.Rows(m)("dqdy1"), dt_temp.Rows(m)("dqdy2")}
            Call CreateContinueDimensionsWithLoop_y(doc, yCoords, dt_temp.Rows(m)("x"), 1)
            'Call Drawtext(doc, (x + x_next) * 0.5, (dqdy1 + dqdy2) * 0.5, dt_temp.Rows(i)("group") & "#") '挡墙编号
            ''        'num_display.ToString("0.00")
        Next

        Call DrawPolylineFromDataTable(doc, datatable_T, "顶部")
        Call DrawPolylineFromDataTable(doc, datatable_B, "底部")
        ' 定义报告文件夹路径
        Dim reportFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report")

        ' 如果文件夹不存在则创建
        If Not Directory.Exists(reportFolder) Then
            Directory.CreateDirectory(reportFolder)
        End If

        ' 生成带时间戳的文件名
        Dim fileName As String = strfilename & $"_{DateTime.Now:yyyyMMdd_HH_mm_ss}.dxf"
        Dim fullPath As String = Path.Combine(reportFolder, fileName)
        ' 保存DXF文件
        doc.Save(fullPath)

        ' 打开report文件夹
        Try
            Process.Start("explorer.exe", reportFolder)
        Catch ex As Exception
            'Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"无法打开文件夹: {ex.Message}")
        End Try
    End Sub
    'Public Sub Createdqdxf_pfx(ByVal strfilename As String, ByVal dt As DataTable， ByVal datatable_B As DataTable， ByVal datatable_T As DataTable)
    '    'Dim doc As New DxfDocument(DxfVersion.AutoCad2018)
    '    Dim dt_temp As DataTable = dt.Copy
    '    Dim doc As New DxfDocument()
    '    ' 1. 创建或获取图层

    '    Dim layer As Layer
    '    If Not doc.Layers.Contains("DJL挡墙") Then
    '        layer = New Layer("DJL挡墙")
    '        doc.Layers.Add(layer)
    '    Else
    '        layer = doc.Layers("DJL挡墙")
    '    End If
    '    ' ✅ 设置图层为紫色
    '    layer.Color = AciColor.Magenta

    '    For i = 0 To dt_temp.Rows.Count - 1
    '        Dim x As Double = dt_temp.Rows(i)("x")
    '        Dim dqdy1 As Double = dt_temp.Rows(i)("dqdy1")
    '        Dim dqdy2 As Double = dt_temp.Rows(i)("dqdy2")
    '        Dim x_next As Double
    '        Dim dqdy2_next As Double
    '        Dim dqdy1_next As Double
    '        If i <> dt_temp.Rows.Count - 1 Then
    '            x_next = dt_temp.Rows(i + 1)("x")
    '            dqdy2_next = dt_temp.Rows(i + 1)("dqdy2")
    '            dqdy1_next = dt_temp.Rows(i + 1)("dqdy1")
    '        Else
    '            x_next = dt_temp.Rows(i)("x") + dt_temp.Rows(i)("L")
    '            dqdy2_next = dqdy2
    '            dqdy1_next = dqdy1
    '        End If


    '        '以下绘制剖分线1
    '        Dim vertexes_1 As New List(Of Vector2) From {
    'New Vector2(x, dqdy2),
    'New Vector2(x, dqdy1)
    ' }
    '        Dim polyline1 As New netDxf.Entities.Polyline2D(vertexes_1)
    '        polyline1.Layer = layer
    '        polyline1.SetConstantWidth(0.05)
    '        doc.Entities.Add(polyline1)

    '        '以下绘制剖分线2
    '        Dim vertexes_1P As New List(Of Vector2) From {
    'New Vector2(x_next, dqdy2_next),
    'New Vector2(x_next, dqdy1)
    ' }
    '        Dim polyline1p As New netDxf.Entities.Polyline2D(vertexes_1P)
    '        polyline1p.Layer = layer
    '        polyline1p.SetConstantWidth(0.05)
    '        doc.Entities.Add(polyline1p)


    '        '          '以下绘制顶线
    '        '          Dim vertexes_2 As New List(Of Vector2) From {
    '        '  New Vector2(x, dqdy2),
    '        '  New Vector2(x_next, dqdy2_next)
    '        '   }
    '        '          '              Dim vertexes_3 As New List(Of Vector2) From {
    '        '          '                 New Vector2(x, dqdy1),
    '        '          '                 New Vector2(x_next, dqdy1_next)
    '        '          '}

    '        '          Dim polyline2 As New netDxf.Entities.Polyline2D(vertexes_2)
    '        '          polyline2.Layer = layer
    '        '          polyline2.SetConstantWidth(0.05)
    '        '          doc.Entities.Add(polyline2)

    '        '          '以下绘制底线
    '        '          Dim vertexes_3 As New List(Of Vector2) From {
    '        'New Vector2(x, dqdy1),
    '        'New Vector2(x_next, dqdy1)
    '        ' }

    '        '          Dim polyline3 As New netDxf.Entities.Polyline2D(vertexes_3)
    '        '          polyline3.Layer = layer
    '        '          polyline3.SetConstantWidth(0.05)
    '        '          doc.Entities.Add(polyline3)

    '        'Call DrawElevations(doc, (x + x_next) * 0.5, dqdy1, dqdy1) '貌似底高程
    '        'Call DrawElevations(doc, x, dqdy2, dqdy2) '貌似顶高程
    '        'Call DrawElevations(doc, (x + x_next) * 0.5, (dqdy1 + dqdy2) * 0.5, dtTemplate.Rows(i)("group")) '挡墙编号
    '        'Call DrawElevations(doc, (x + x_next) * 0.5, dqdy1, dqdy1.ToString("0.00")) '貌似底高程
    '        'Call DrawElevations(doc, x, dqdy2, dqdy2.ToString("0.00")) '貌似顶高程
    '        'Call Drawtext(doc, (x + x_next) * 0.5, (dqdy1 + dqdy2) * 0.5, dt_temp.Rows(i)("group") & "#") '挡墙编号
    '        'num_display.ToString("0.00")
    '    Next

    '    'For j = 0 To dt_temp.Rows.Count - 1
    '    '    If j <> dt_temp.Rows.Count - 1 Then
    '    '        Dim xCoords() As Double = {dt_temp.Rows(j)("x"), dt_temp.Rows(j + 1)("x")}
    '    '        Call CreateContinueDimensionsWithLoop_x(doc, xCoords, Math.Max(dt_temp.Rows(j)("dqdy2"), dt_temp.Rows(j + 1)("dqdy2")), 1)
    '    '    Else
    '    '        Dim xCoords() As Double = {dt_temp.Rows(j)("x"), dt_temp.Rows(j)("x") + dt_temp.Rows(dt_temp.Rows.Count - 1)("L")}
    '    '        Call CreateContinueDimensionsWithLoop_x(doc, xCoords, dt_temp.Rows(j)("dqdy2"), 1)
    '    '    End If

    '    'Next
    '    'Dim max_dqdy2 As Double = dt_temp.AsEnumerable().Max(Function(row) row.Field(Of Double)("dqdy2"))
    '    'Dim xCoords_total() As Double = {dt_temp.Rows(0)("x"), dt_temp.Rows(dt_temp.Rows.Count - 1)("x") + dt_temp.Rows(dt_temp.Rows.Count - 1)("L")}
    '    'Call CreateContinueDimensionsWithLoop_x(doc, xCoords_total, max_dqdy2, 2)
    '    'For m = 0 To dt_temp.Rows.Count - 1
    '    '    Dim yCoords() As Double = {dt_temp.Rows(m)("dqdy1"), dt_temp.Rows(m)("dqdy2")}
    '    '    Call CreateContinueDimensionsWithLoop_y(doc, yCoords, dt_temp.Rows(m)("x"), 1)
    '    'Next

    '    'Call DrawPolylineFromDataTable(doc, datatable_T, "顶部")
    '    'Call DrawPolylineFromDataTable(doc, datatable_B, "底部")
    '    ' 定义报告文件夹路径
    '    Dim reportFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report")

    '    ' 如果文件夹不存在则创建
    '    If Not Directory.Exists(reportFolder) Then
    '        Directory.CreateDirectory(reportFolder)
    '    End If

    '    ' 生成带时间戳的文件名
    '    Dim fileName As String = strfilename & $"_{DateTime.Now:yyyyMMdd_HH_mm_ss}.dxf"
    '    Dim fullPath As String = Path.Combine(reportFolder, fileName)
    '    ' 保存DXF文件
    '    doc.Save(fullPath)

    '    ' 打开report文件夹
    '    Try
    '        Process.Start("explorer.exe", reportFolder)
    '    Catch ex As Exception
    '        'Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"无法打开文件夹: {ex.Message}")
    '    End Try
    'End Sub
End Module

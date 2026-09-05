Imports System.IO
Imports netDxf
Imports netDxf.Tables
Imports NPOI.SS.Formula.Functions

Module Module_draw4
    Public Sub Createdqdxf4(ByVal strfilename As String, ByVal dt As DataTable， ByVal datatable_B As DataTable， ByVal datatable_T As DataTable)
        'Dim doc As New DxfDocument(DxfVersion.AutoCad2018)
        Dim dtTemplate As DataTable = dt.Copy
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

        For i = 0 To dtTemplate.Rows.Count - 2
            Dim x As Double = dtTemplate.Rows(i)("x")
            Dim x_next As Double = dtTemplate.Rows(i + 1)("x")
            Dim dy1 As Double = dtTemplate.Rows(i)("dy1")
            Dim dy2 As Double = dtTemplate.Rows(i)("dy2")
            Dim dy2_next As Double = dtTemplate.Rows(i + 1)("dy2")
            Dim dy1_next As Double = dtTemplate.Rows(i + 1)("dy1")
            '以下绘制剖分线1
            Dim vertexes_1 As New List(Of Vector2) From {
    New Vector2(x, dy2),
    New Vector2(x, dy1)
     }
            Dim polyline1 As New netDxf.Entities.Polyline2D(vertexes_1)
            polyline1.Layer = layer
            polyline1.SetConstantWidth(0.05)
            doc.Entities.Add(polyline1)

            '以下绘制剖分线2
            Dim vertexes_1P As New List(Of Vector2) From {
    New Vector2(x_next, dy2_next),
    New Vector2(x_next, dy1)
     }
            Dim polyline1p As New netDxf.Entities.Polyline2D(vertexes_1P)
            polyline1p.Layer = layer
            polyline1p.SetConstantWidth(0.05)
            doc.Entities.Add(polyline1p)

            'If i <> dtTemplate.Rows.Count - 1 Then



            'Dim x_next As Double = dtTemplate.Rows(i + 1)("x")


            '以下绘制顶线
            Dim vertexes_2 As New List(Of Vector2) From {
    New Vector2(x, dy2),
    New Vector2(x_next, dy2_next)
     }
            '              Dim vertexes_3 As New List(Of Vector2) From {
            '                 New Vector2(x, dy1),
            '                 New Vector2(x_next, dy1_next)
            '}

            Dim polyline2 As New netDxf.Entities.Polyline2D(vertexes_2)
            polyline2.Layer = layer
            polyline2.SetConstantWidth(0.05)
            doc.Entities.Add(polyline2)

            '以下绘制底线
            Dim vertexes_3 As New List(Of Vector2) From {
  New Vector2(x, dy1),
  New Vector2(x_next, dy1)
   }
            '              Dim vertexes_3 As New List(Of Vector2) From {
            '                 New Vector2(x, dy1),
            '                 New Vector2(x_next, dy1_next)
            '}

            Dim polyline3 As New netDxf.Entities.Polyline2D(vertexes_3)
            polyline3.Layer = layer
            polyline3.SetConstantWidth(0.05)
            doc.Entities.Add(polyline3)


            Call DrawElevations(doc, (x + x_next) * 0.5, dy1, dy1)
            'Call DrawElevations(doc, (x + x_next) * 0.5, （dy2 + dy2_next） * 0.5, dy2)
            Call DrawElevations(doc, x, dy2, dy2)
        Next






        For j = 0 To dtTemplate.Rows.Count - 2
            Dim xCoords() As Double = {dtTemplate.Rows(j)("x"), dtTemplate.Rows(j + 1)("x")}
            Call CreateContinueDimensionsWithLoop_x(doc, xCoords, Math.Max(dtTemplate.Rows(j)("dy2"), dtTemplate.Rows(j + 1)("dy2")), 1)
        Next
        Dim max_dy2 As Double = dtTemplate.AsEnumerable().Max(Function(row) row.Field(Of Double)("dy2"))
        Dim xCoords_total() As Double = {dtTemplate.Rows(0)("x"), dtTemplate.Rows(dtTemplate.Rows.Count - 1)("x")}
        Call CreateContinueDimensionsWithLoop_x(doc, xCoords_total, max_dy2, 2)
        For m = 0 To dtTemplate.Rows.Count - 2
            Dim yCoords() As Double = {dtTemplate.Rows(m)("dy1"), dtTemplate.Rows(m)("dy2")}
            Call CreateContinueDimensionsWithLoop_y(doc, yCoords, dtTemplate.Rows(m)("x"), 1)
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
End Module

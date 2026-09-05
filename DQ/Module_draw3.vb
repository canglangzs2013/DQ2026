Imports System.IO
Imports netDxf
Imports NPOI.SS.Formula.Functions

Module Module_draw3
    Public Sub Createdqdxf3(ByVal strfilename As String)
        'Dim doc As New DxfDocument(DxfVersion.AutoCad2018)
        Dim doc As New DxfDocument()

        For i = 0 To dtTemplate.Rows.Count - 1
            Dim x As Double = dtTemplate.Rows(i)("x")
            Dim dy1 As Double = dtTemplate.Rows(i)("dy1")
            Dim dy2 As Double = dtTemplate.Rows(i)("dy2")
            '以下绘制剖分线
            Dim vertexes_1 As New List(Of Vector2) From {
    New Vector2(x, dy2),
    New Vector2(x, dy1)
     }
            Dim polyline1 As New netDxf.Entities.Polyline2D(vertexes_1)
            doc.Entities.Add(polyline1)


            If i <> dtTemplate.Rows.Count - 1 Then
                '以下绘制顶线
                Dim x_next As Double = dtTemplate.Rows(i + 1)("x")
                Dim dy2_next As Double = dtTemplate.Rows(i + 1)("dy2")
                Dim dy1_next As Double = dtTemplate.Rows(i + 1)("dy1")


                Dim vertexes_2 As New List(Of Vector2) From {
    New Vector2(x, dy2),
    New Vector2(x_next, dy2_next)
     }
                '              Dim vertexes_3 As New List(Of Vector2) From {
                '                 New Vector2(x, dy1),
                '                 New Vector2(x_next, dy1_next)
                '}

                Dim polyline2 As New netDxf.Entities.Polyline2D(vertexes_2)
                doc.Entities.Add(polyline2)

                '以下绘制底线
                Dim n As Integer = Math.Abs((dy2_next - dy1_next) - (dy2 - dy1) / 1) '每1m搞个台阶
                If n > 1 Then

                    Dim x_step As Double = (dtTemplate.Rows(i + 1)("x") - dtTemplate.Rows(i)("x")) / n
                    Dim y_step As Double = ((dy2_next - dy1_next) - (dy2 - dy1)) / (n - 1)
                    'MessageBox.Show(y_step)
                    For k = 0 To n - 1
                        Dim vertexes_x1 As New List(Of Vector2) From {
  New Vector2(x + k * x_step, dy1 - k * y_step),
  New Vector2(x + (k + 1) * x_step, dy1 - k * y_step)
   }
                        Dim polyline_x1 As New netDxf.Entities.Polyline2D(vertexes_x1)
                        doc.Entities.Add(polyline_x1)

                        If k <> n - 1 Then
                            Dim vertexes_y1 As New List(Of Vector2) From {
 New Vector2(x + (k + 1) * x_step, dy1 - （k） * y_step),
 New Vector2(x + (k + 1) * x_step, dy1 - （k + 1） * y_step)
  }
                            Dim polyline_y1 As New netDxf.Entities.Polyline2D(vertexes_y1)
                            doc.Entities.Add(polyline_y1)
                        End If

                    Next
                ElseIf n = 0 Then
                    Dim vertexes_3 As New List(Of Vector2) From {
                       New Vector2(x, dy1),
                       New Vector2(x_next, dy1_next)
      }
                    'MessageBox.Show(dy1)
                    Dim polyline3 As New netDxf.Entities.Polyline2D(vertexes_3)
                    doc.Entities.Add(polyline3)

                ElseIf n = 1 Then
                    Dim vertexes_4 As New List(Of Vector2) From {
                       New Vector2(x, dy1),
                       New Vector2(x + (x_next - x) * 0.5, dy1)
                         }
                    Dim vertexes_5 As New List(Of Vector2) From {
                      New Vector2(x + (x_next - x) * 0.5, dy1_next),
                      New Vector2(x_next, dy1_next)
                        }
                    Dim vertexes_6 As New List(Of Vector2) From {
                      New Vector2(x + (x_next - x) * 0.5, dy1),
                      New Vector2(x + (x_next - x) * 0.5, dy1_next)
                        }
                    Dim polyline4 As New netDxf.Entities.Polyline2D(vertexes_4)
                    Dim polyline5 As New netDxf.Entities.Polyline2D(vertexes_5)
                    Dim polyline6 As New netDxf.Entities.Polyline2D(vertexes_6)
                    doc.Entities.Add(polyline4)
                    doc.Entities.Add(polyline5)
                    doc.Entities.Add(polyline6)
                End If
            End If
        Next



        For j = 0 To dtTemplate.Rows.Count - 2
            Dim xCoords() As Double = {dtTemplate.Rows(j)("x"), dtTemplate.Rows(j + 1)("x")}
            Call CreateContinueDimensionsWithLoop_x(doc, xCoords, Math.Max(dtTemplate.Rows(j)("dy2"), dtTemplate.Rows(j + 1)("dy2")), 1)
        Next
        Dim max_dy2 As Double = dtTemplate.AsEnumerable().Max(Function(row) row.Field(Of Double)("dy2"))
        Dim xCoords_total() As Double = {dtTemplate.Rows(0)("x"), dtTemplate.Rows(dtTemplate.Rows.Count - 1)("x")}
        Call CreateContinueDimensionsWithLoop_x(doc, xCoords_total, max_dy2, 2)
        For m = 0 To dtTemplate.Rows.Count - 1
            Dim yCoords() As Double = {dtTemplate.Rows(m)("dy1"), dtTemplate.Rows(m)("dy2")}
            Call CreateContinueDimensionsWithLoop_y(doc, yCoords, dtTemplate.Rows(m)("x"), 1)
        Next


        ' 定义报告文件夹路径
        Dim reportFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report")

        ' 如果文件夹不存在则创建
        If Not Directory.Exists(reportFolder) Then
            Directory.CreateDirectory(reportFolder)
        End If

        ' 生成带时间戳的文件名
        Dim fileName As String = strfilename & $"飞天挡墙_{DateTime.Now:yyyyMMdd_HHmmss}.dxf"
        Dim fullPath As String = Path.Combine(reportFolder, fileName)
        'Dim fileName2 As String = $"A隧洞工程量_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
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

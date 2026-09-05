Imports System.IO
Imports netDxf
Imports netDxf.Entities
Imports netDxf.Tables

Module Module_draw
    Public Sub Createdqdxf(ByVal strfilename As String)
        'Dim doc As New DxfDocument(DxfVersion.AutoCad2018)
        Dim doc As New DxfDocument()



        '' 1. 读取配置文件
        'Dim json = File.ReadAllText(configPath)
        'Dim config = JObject.Parse(json)

        '' 2. 提取参数
        'Dim width As Double = config("width").Value(Of Double)() * scale_raito
        'Dim height = config("height").Value(Of Double)() * scale_raito
        'Dim angle_top = config("angle_top").Value(Of Double)() '顶拱中心角
        'Dim thickness_2 = config("thickness_2").Value(Of Double)() * scale_raito '二衬
        'Dim thickness_1 = config("thickness_1").Value(Of Double)() * scale_raito '喷砼
        'Dim deformation = config("deformation").Value(Of Double)() * scale_raito

        'Dim thickness_cushion_layer = config("thickness_cushion_layer").Value(Of Double)() * scale_raito '垫层
        'Dim anchor_number = config("anchor_number").Value(Of Double)() '锚杆数量
        'Dim anchor_angle = config("anchor_angle").Value(Of Double)() '锚杆中心角
        'Dim pipe_number = config("pipe_number").Value(Of Double)()
        'Dim pipe_angle = config("pipe_angle").Value(Of Double)() '导管中心角

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

            '以下绘制顶和底线
            If i <> dtTemplate.Rows.Count - 1 Then

                Dim x_next As Double = dtTemplate.Rows(i + 1)("x")
                Dim dy2_next As Double = dtTemplate.Rows(i + 1)("dy2")
                Dim dy1_next As Double = dtTemplate.Rows(i + 1)("dy1")


                Dim vertexes_2 As New List(Of Vector2) From {
    New Vector2(x, dy2),
    New Vector2(x_next, dy2_next)
     }
                Dim vertexes_3 As New List(Of Vector2) From {
                   New Vector2(x, dy1),
                   New Vector2(x_next, dy1_next)
  }

                Dim polyline2 As New netDxf.Entities.Polyline2D(vertexes_2)
                doc.Entities.Add(polyline2)
                Dim polyline3 As New netDxf.Entities.Polyline2D(vertexes_3)
                doc.Entities.Add(polyline3)
            End If
        Next





        '' 定义报告文件夹路径
        'Dim reportFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report")

        '' 如果文件夹不存在则创建
        'If Not Directory.Exists(reportFolder) Then
        '    Directory.CreateDirectory(reportFolder)
        'End If

        '' 生成带时间戳的文件名
        'Dim fileName As String = $"tunnel_{DateTime.Now:yyyyMMdd_HHmmss}.dxf"
        'Dim fullPath As String = Path.Combine(reportFolder, fileName)
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
        'If doc.Save(fullPath) = True Then
        '    MessageBox.Show("True")
        'Else
        '    MessageBox.Show("false")
        'End If




        ''' 生成带时间戳的文件名
        'Dim fileName2 As String = strfilename & $"工程量_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
        ''Dim fileName2 As String = strfilename & $ "_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
        'Dim fullPath2 As String = Path.Combine(reportFolder, fileName2)
        'ExportParamsToExcel(paramData, fullPath2)



        ' 打开report文件夹
        Try
            Process.Start("explorer.exe", reportFolder)
        Catch ex As Exception
            'Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"无法打开文件夹: {ex.Message}")
        End Try
    End Sub
End Module

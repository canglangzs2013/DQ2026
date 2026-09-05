Imports System.IO
Imports NPOI.HPSF

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '初始化dtTemplate
        '创建新的DataTable模板

        'Dim dataset_landslide_AB_point As DataSet = New DataSet
        Dim dtTemplate = New DataTable
        dtTemplate.Columns.Add("ID", Type.GetType("System.String"))
        dtTemplate.Columns.Add("x", Type.GetType("System.Double"))
        dtTemplate.Columns.Add("y1", Type.GetType("System.Double")) '底
        dtTemplate.Columns.Add("y2", Type.GetType("System.Double")) '顶
        dtTemplate.Columns.Add("h", Type.GetType("System.Double")) '高差
        dtTemplate.Columns.Add("plus_dq_height", Type.GetType("System.Double")) '基础埋深
        dtTemplate.Columns.Add("dh", Type.GetType("System.Double")) '设计挡墙高度.刚开始是单个剖面的，后来是分组的
        dtTemplate.Columns.Add("L", Type.GetType("System.Double")) '挡墙分段长度
        dtTemplate.Columns.Add("dy1", Type.GetType("System.Double")) '底
        dtTemplate.Columns.Add("dy2", Type.GetType("System.Double")) '顶
        dtTemplate.Columns.Add("pmdy1", Type.GetType("System.Double")) '底
        dtTemplate.Columns.Add("pmdy2", Type.GetType("System.Double")) '顶
        dtTemplate.Columns.Add("pmdh", Type.GetType("System.Double")) '单个剖面的挡墙高度

        'dtTemplate.Columns.Add("elevation_1", System.Type.GetType("System.Double")) '挡墙底
        'dtTemplate.Columns.Add("elevation_2", System.Type.GetType("System.Double")) '挡墙顶
        dtTemplate.Columns（"y1"）.Caption = "地形底高程"
        dtTemplate.Columns（"y2"）.Caption = "地形顶高程"
        dtTemplate.Columns（"h"）.Caption = "地形高差h"
        dtTemplate.Columns（"plus_dq_height"）.Caption = "挡墙基础埋深"
        dtTemplate.Columns（"dh"）.Caption = "挡墙高度dh"
        dtTemplate.Columns（"L"）.Caption = "挡墙长度L"
        dtTemplate.Columns（"dy1"）.Caption = "挡墙设计底高程dy1"
        dtTemplate.Columns（"dy2"）.Caption = "挡墙设计顶高程dy2"
        'dtTemplate.Columns（"pmdh"）.Caption = "挡墙高度dh"
        Dim datatable_T = New DataTable '注意这是一个仅仅存储滑坡前后缘点的datable。但是其结构和存储整个滑坡坐标的datatable相同，也可以使用其结构
        datatable_T.Columns.Add("ID", Type.GetType("System.String"))
        datatable_T.Columns.Add("x", Type.GetType("System.Double"))
        datatable_T.Columns.Add("y", Type.GetType("System.Double")) '顶

        Dim datatable_B = New DataTable '注意这是一个仅仅存储滑坡前后缘点的datable。但是其结构和存储整个滑坡坐标的datatable相同，也可以使用其结构
        datatable_B.Columns.Add("ID", Type.GetType("System.String"))
        datatable_B.Columns.Add("x", Type.GetType("System.Double"))
        datatable_B.Columns.Add("y", Type.GetType("System.Double")) '顶

        Dim OpenFileDialog1 As New OpenFileDialog
        OpenFileDialog1.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx"
        'OpenFileDialog1.Filter = "CSV files (*.csv)|*.csv"
        OpenFileDialog1.FilterIndex = 1
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then

            'Dim str_filename_temp As String = Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName)
            Dim str_filename_path_temp = OpenFileDialog1.FileName
            Dim str_filename_temp = Path.GetFileNameWithoutExtension(str_filename_path_temp)
            If ExcelHelper.IsFileLocked(str_filename_temp) = False Then
                'dtTemplate = ReadCsvToDataTable（str_filename_temp, False）

                Dim c = New Class_read_excel
                'dtTemplate = c.DoReadExcelDataTable(str_filename_path_temp, 0, dtTemplate).Copy
                datatable_T = c.DoReadExcelDataTable(str_filename_path_temp, 0, datatable_T).Copy
                datatable_B = c.DoReadExcelDataTable(str_filename_path_temp, 1, datatable_B).Copy

                '第一步：准备基础数据,模型划分（各剖面线，含最后一根） dtTemplate
                For i = 0 To datatable_T.Rows.Count - 1
                    Dim dr = dtTemplate.NewRow
                    dr("ID"） = "*"
                    dr("x"） = datatable_T.Rows(i)("x")
                    dr("y1"） = GetYFromX(datatable_B, dr("x"）) '底
                    dr("y2"） = datatable_T.Rows(i)("y") '顶
                    dtTemplate.Rows.Add(dr)
                Next

                Dim minx As Double = datatable_T.Rows(0)("x")
                Dim maxx As Double = datatable_T.Rows(datatable_T.Rows.Count - 1)("x")
                Dim x_current As Double = minx
                Do While x_current < maxx
                    If x_current > minx Then '第一个点的信息上一步已经录入了
                        Dim boolean_effective As Boolean = True
                        For i = 1 To datatable_T.Rows.Count - 2
                            If Math.Abs(x_current - datatable_T.Rows(i)("x")) < min_dq_length Then
                                boolean_effective = False
                                Exit For
                            End If
                        Next
                        If boolean_effective = True Then
                            Dim dr2 = dtTemplate.NewRow
                            dr2("ID"） = "哈哈哈哈"
                            dr2("x"） = x_current
                            dr2("y1"） = GetYFromX(datatable_B, x_current)
                            dr2("y2"） = GetYFromX(datatable_T, x_current)
                            dtTemplate.Rows.Add(dr2)
                        End If
                    End If
                    x_current = x_current + step_cal
                Loop
                ''最后一个剖面线
                'Dim dr999 = dtTemplate.NewRow
                'dr999("ID"） = "哈哈哈哈"
                'dr999("x"） = maxx
                'dr999("y1"） = GetYFromX(datatable_B, maxx)
                'dr999("y2"） = GetYFromX(datatable_T, maxx)
                'dtTemplate.Rows.Add(dr999)

                '排序
                '' 创建 DataView（或直接使用 DefaultView）
                Dim dv As DataView = dtTemplate.DefaultView
                ' 按 "ColumnName" 列升序排序
                dv.Sort = "x ASC"

                ' dv.Sort = "ColumnName DESC"' 或降序
                ' 如果需要获取排序后的 DataTable（新表）
                dtTemplate = dv.ToTable()


                ''清除过短分段
                'For n = 0 To dtTemplate.Rows.Count - 2
                '    If dtTemplate.Rows(n + 1)("x") - dtTemplate.Rows(n)("x") < min_dq_length And dtTemplate.Rows(n + 1)("ID") = "*" And dtTemplate.Rows(n)("ID") <> "*" Then
                '        dtTemplate.Rows(n)("ID") = "delete"
                '        'MessageBox.Show(dtTemplate.Rows(n)("x"))
                '    ElseIf dtTemplate.Rows(n + 1)("x") - dtTemplate.Rows(n)("x") < min_dq_length And dtTemplate.Rows(n)("ID") = "*" And dtTemplate.Rows(n + 1)("ID") <> "*" Then
                '        dtTemplate.Rows(n + 1)("ID") = "delete"
                '        'MessageBox.Show(dtTemplate.Rows(n + 1)("x") - dtTemplate.Rows(n)("x"))
                '    End If
                'Next


                'For f = dtTemplate.Rows.Count - 1 To 0 Step -1
                '    If dtTemplate.Rows(f)("ID").ToString() = "delete" Then
                '        dtTemplate.Rows(f).Delete()
                '        'MessageBox.Show("3333")
                '    End If
                'Next

                'ID重新赋值
                Dim n_temp As Integer = 0

                For j = 0 To dtTemplate.Rows.Count - 1
                    If dtTemplate.Rows(j)("ID").ToString().Trim().Contains("*") = False Then
                        dtTemplate.Rows(j)("ID") = n_temp
                    Else
                        dtTemplate.Rows(j)("ID") = n_temp & "*"
                    End If
                    n_temp = n_temp + 1
                Next
                '准备除了x y1 y2 之外的基础数据
                For m = 0 To dtTemplate.Rows.Count - 1
                    dtTemplate.Rows(m)("h") = dtTemplate.Rows(m)("y2") - dtTemplate.Rows(m)("y1")
                    dtTemplate.Rows(m)("plus_dq_height") = plus_dq_height

                    dtTemplate.Rows(m)("dy2") = dtTemplate.Rows(m)("y2")

                    'dtTemplate.Rows(m)("pmdy1") = dtTemplate.Rows(m)("y2") - dtTemplate.Rows(m)("y1")
                    'Dim h_temp As Double = RoundUpToHalf(dtTemplate.Rows(m)("h") + dtTemplate.Rows(m)("plus_dq_height"))
                    Dim h_temp As Double = dtTemplate.Rows(m)("h") + dtTemplate.Rows(m)("plus_dq_height")
                    dtTemplate.Rows(m)("pmdy1") = dtTemplate.Rows(m)("y2") - h_temp
                    dtTemplate.Rows(m)("pmdy2") = dtTemplate.Rows(m)("y2")
                    dtTemplate.Rows(m)("pmdh") = h_temp

                    If m <> dtTemplate.Rows.Count - 1 Then
                        dtTemplate.Rows(m)("L") = dtTemplate.Rows(m + 1)("x") - dtTemplate.Rows(m)("x")
                    End If
                Next


                '第三步：准备dt（合并相同高度的相邻挡墙） 
                dtTemplate = GroupByThreshold_pmdy1_dy1(dtTemplate, 1).Copy


                Dim dt = dtTemplate.Clone
                Dim current_group As Double = 0
                For x = 0 To dtTemplate.Rows.Count - 1
                    If x = 0 Then
                        Dim dr1 = dt.NewRow
                        dr1("ID"） = dtTemplate.Rows(x)("ID")
                        dr1("x"） = dtTemplate.Rows(x)("x")
                        dr1("y1"） = dtTemplate.Rows(x)("y1")
                        dr1("y2"） = dtTemplate.Rows(x)("y2")
                        dr1("h"） = dtTemplate.Rows(x)("h")
                        dr1("plus_dq_height"） = dtTemplate.Rows(x)("plus_dq_height")
                        'dr1("dh"） = dtTemplate.Rows(x)("dh")
                        'dr1("dy1"） = dtTemplate.Rows(x)("dy1")
                        'dr1("dy2"） = dtTemplate.Rows(x)("dy2")
                        dr1("dy1"） = dtTemplate.Rows(x)("dy1") '分组的时候已经修改过每组的dy1了，修改前是空值
                        dr1("dy2"） = dtTemplate.Rows(x)("pmdy2")
                        dr1("dh"） = dr1("dy2"） - dr1("dy1"）
                        dr1("group"） = dtTemplate.Rows(x)("group")

                        dr1("pmdy1"） = dtTemplate.Rows(x)("pmdy1")
                        dr1("pmdy2"） = dtTemplate.Rows(x)("pmdy2")
                        dr1("pmdh"） = dtTemplate.Rows(x)("pmdh")
                        dt.Rows.Add(dr1)
                        current_group = dtTemplate.Rows(x)("group")
                    ElseIf x < dtTemplate.Rows.Count - 1 And x > 0 Then

                        If dtTemplate.Rows(x)("group") <> current_group Then
                            Dim dr2 = dt.NewRow
                            'dr2("ID"） = x
                            dr2("ID"） = dtTemplate.Rows(x)("ID")
                            dr2("x"） = dtTemplate.Rows(x)("x")
                            dr2("y1"） = dtTemplate.Rows(x)("y1") '分组的时候已经修改过每组的dy1了，修改前是空值
                            dr2("y2"） = dtTemplate.Rows(x)("pmdy2")
                            dr2("h"） = dtTemplate.Rows(x)("h")
                            dr2("plus_dq_height"） = dtTemplate.Rows(x)("plus_dq_height")
                            'dr2("dh"） = dtTemplate.Rows(x)("dh")’错误，这个dh是单剖面的数据，现在每组的底高程确定了，必须重新更新dh
                            dr2("dy1"） = dtTemplate.Rows(x)("dy1") '分组的时候已经修改过每组的dy1了，修改前是空值
                            dr2("dy2"） = dtTemplate.Rows(x)("pmdy2")
                            dr2("dh"） = dr2("dy2"） - dr2("dy1"）
                            dr2("group"） = dtTemplate.Rows(x)("group")

                            dr2("pmdy1"） = dtTemplate.Rows(x)("pmdy1")
                            dr2("pmdy2"） = dtTemplate.Rows(x)("pmdy2")
                            dr2("pmdh"） = dtTemplate.Rows(x)("pmdh")
                            dt.Rows.Add(dr2)
                            current_group = dtTemplate.Rows(x)("group")
                        End If
                    ElseIf x = dtTemplate.Rows.Count - 1 Then
                        Dim dr3 = dt.NewRow
                        'dr3("ID"） = x
                        dr3("ID"） = dtTemplate.Rows(x)("ID")

                        dr3("x"） = dtTemplate.Rows(x)("x")
                        dr3("y1"） = dtTemplate.Rows(x)("y1")
                        dr3("y2"） = dtTemplate.Rows(x)("y2")
                        dr3("h"） = dtTemplate.Rows(x)("h")
                        dr3("plus_dq_height"） = dtTemplate.Rows(x)("plus_dq_height")
                        'dr3("dh"） = dtTemplate.Rows(x)("dh")
                        dr3("dy1"） = dtTemplate.Rows(x)("dy1") '分组的时候已经修改过每组的dy1了，修改前是空值
                        dr3("dy2"） = dtTemplate.Rows(x)("pmdy2")
                        dr3("dh"） = dr3("dy2"） - dr3("dy1"）
                        dr3("L"） = 0 '如果不赋值，后面分类求和会有dbnull
                        dr3("group"） = dtTemplate.Rows(x)("group")

                        dr3("pmdy1"） = dtTemplate.Rows(x)("pmdy1")
                        dr3("pmdy2"） = dtTemplate.Rows(x)("pmdy2")
                        dr3("pmdh"） = dtTemplate.Rows(x)("pmdh")
                        dt.Rows.Add(dr3)

                    End If
                Next

                For k = 0 To dt.Rows.Count - 2
                    Dim min_ele As Double = Double.MinValue
                    If dt.Rows(k)("dy1") > dt.Rows(k + 1)("pmdy1") Then
                        dt.Rows(k)("ID") = dt.Rows(k)("ID") & "#"
                        'dt.Rows(k)("dy1") = dt.Rows(k + 1)("pmdy1")
                        'dt.Rows(k)("dh") = dt.Rows(k)("dy2") - dt.Rows(k)("dy1")
                        min_ele = dt.Rows(k + 1)("pmdy1")
                    Else
                        min_ele = dt.Rows(k)("dy1")
                        'dt.Rows(k)("ID") = dt.Rows(k)("ID")
                    End If
                    'dt.Rows(k)("pmdy1") = min_ele
                    Dim h_temp As Double = RoundUpToHalf(dt.Rows(k)("pmdy2") - min_ele)
                    dt.Rows(k)("dy1") = dt.Rows(k)("dy2") - h_temp
                    dt.Rows(k)("dh") = dt.Rows(k)("dy2") - dt.Rows(k)("dy1")
                Next

                '第四步：再次更新整理dt的L(挡墙长度)
                For t = 0 To dt.Rows.Count - 2
                    dt.Rows(t)("L") = dt.Rows(t + 1)("x") - dt.Rows(t)("x")
                Next

                Createdqdxf4（str_filename_temp, dt, datatable_B, datatable_T）

                If ExcelHelper.IsFileLocked(str_filename_temp) = True Then
                    MessageBox.Show($"保存Excel文件时发生未知错误1，文件被占用: {str_filename_temp}")
                    Exit Sub
                Else
                    'ExportDataTableToExcel(dtTemplate, str_filename_temp)
                    ' 定义报告文件夹路径
                    Dim reportFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "成果输出")
                    str_filename_temp = str_filename_temp & $"_{Date.Now:yyyyMMdd_HH_mm_ss}.xlsx"
                    Dim fullPath = Path.Combine(reportFolder, str_filename_temp)

                    Dim dt_sum = SumByA(dt)
                    Dim dataset_sheet = New DataSet
                    'dataset_sheet.Tables.Add(dtTemplate)
                    dataset_sheet.Tables.Add(dt) '挡墙工程量统计
                    dataset_sheet.Tables.Add(dtTemplate) '挡墙绘图基础数据

                    dataset_sheet.Tables.Add(datatable_T) '挡墙顶部设计线数据
                    dataset_sheet.Tables.Add(datatable_B) '挡墙底部设计线数据
                    dataset_sheet.Tables.Add(dt_sum) '挡墙底部设计线数据
                    Dim arr = New ArrayList
                    arr.Add("挡墙工程量统计")
                    arr.Add("挡墙设计中间数据")
                    arr.Add("挡墙顶部地形线")
                    arr.Add("挡墙底部地形线")
                    arr.Add("挡墙长度分组求和")

                    Export_hp_dataset_to_excel(dataset_sheet, fullPath, arr)

                    Try
                        ' 打开结果文件
                        Process.Start(New ProcessStartInfo(str_filename_temp) With {.UseShellExecute = True})
                    Catch

                    End Try
                End If
            End If
        End If

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class

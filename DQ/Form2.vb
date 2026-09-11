Imports System.IO
Imports NPOI.SS.Formula.Functions

Public Class Form2
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '初始化dtTemplate
        '创建新的DataTable模板

        'Dim dataset_landslide_AB_point As DataSet = New DataSet
        Dim dtTemplate = New DataTable
        dtTemplate.Columns.Add("ID", Type.GetType("System.String"))
        dtTemplate.Columns.Add("‌memo2", Type.GetType("System.String"))
        dtTemplate.Columns.Add("x", Type.GetType("System.Double"))
        dtTemplate.Columns.Add("y1", Type.GetType("System.Double")) '底
        dtTemplate.Columns.Add("y2", Type.GetType("System.Double")) '顶
        dtTemplate.Columns.Add("h", Type.GetType("System.Double")) '高差

        dtTemplate.Columns.Add("pfxdqdy2", Type.GetType("System.Double")) '顶
        dtTemplate.Columns.Add("dq_‌foundation_height", Type.GetType("System.Double")) '基础埋深
        dtTemplate.Columns.Add("pfxdqdh", Type.GetType("System.Double")) '单个剖面的挡墙高度
        dtTemplate.Columns.Add("pfxdqdy1", Type.GetType("System.Double")) '底
        dtTemplate.Columns.Add("min_pfxdqdy1_group", Type.GetType("System.Double")) '同一个分组的最小pfxdqdy1

        dtTemplate.Columns.Add("dqdy2", Type.GetType("System.Double")) '顶
        dtTemplate.Columns.Add("h_plus_f", Type.GetType("System.Double")) '设计挡墙高度.刚开始是单个剖面的，后来是分组的
        dtTemplate.Columns.Add("dqdh", Type.GetType("System.Double")) '设计挡墙高度.刚开始是单个剖面的，后来是分组的
        'dtTemplate.Columns.Add("finnal_pfxdqdy1_group", Type.GetType("System.Double")) '同一个分组的最小pfxdqdy1
        dtTemplate.Columns.Add("dqdy1", Type.GetType("System.Double")) '底
        dtTemplate.Columns.Add("group", Type.GetType("System.Double")) '分组
        dtTemplate.Columns.Add("max_y1_group", Type.GetType("System.Double")) '分组
        dtTemplate.Columns.Add("min_y1_group", Type.GetType("System.Double")) '分组
        'dtTemplate.Columns.Add("group", Type.GetType("System.Double")) '分组
        dtTemplate.Columns.Add("‌memo", Type.GetType("System.String"))
        dtTemplate.Columns.Add("L", Type.GetType("System.Double")) '挡墙分段长度
        dtTemplate.Columns.Add("pd", Type.GetType("System.Double")) '挡墙纵坡坡度


        dtTemplate.Columns（"y1"）.Caption = "地形底高程y1"
        dtTemplate.Columns（"y2"）.Caption = "地形顶高程y2"
        dtTemplate.Columns（"h"）.Caption = "地形高差h"
        dtTemplate.Columns（"dq_‌foundation_height"）.Caption = "挡墙基础埋深"
        dtTemplate.Columns（"dqdh"）.Caption = "挡墙高度dqdh最终值"
        dtTemplate.Columns（"L"）.Caption = "挡墙长度L"
        dtTemplate.Columns（"dqdy1"）.Caption = "挡墙设计底高程dqdy1"
        dtTemplate.Columns（"dqdy2"）.Caption = "挡墙设计顶高程dqdy2"
        dtTemplate.Columns（"min_pfxdqdy1_group"）.Caption = "分组内剖分线最低值min_pfxdqdy1_group"
        'dtTemplate.Columns（"dqdh_pre"）.Caption = "挡墙高度计算值dqdh_pre"
        dtTemplate.Columns（"h_plus_f"）.Caption = "高差+基础埋深"
        dtTemplate.Columns（"group"）.Caption = "挡墙分组group"
        dtTemplate.Columns（"pd"）.Caption = "挡墙基底纵坡"
        dtTemplate.Columns（"memo"）.Caption = "备注"
        'dtTemplate.Columns（"pfxdqdh"）.Caption = "挡墙高度dqdh"
        'dtTemplate.Columns.Add("name", Type.GetType("System.String"))

        Dim datatable_T = New DataTable '顶线
        datatable_T.Columns.Add("ID", Type.GetType("System.String"))
        datatable_T.Columns.Add("x", Type.GetType("System.Double"))
        datatable_T.Columns.Add("y", Type.GetType("System.Double")) '顶

        Dim datatable_B = New DataTable '底线
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
                    dr("ID"） = "*" '标记坡顶线的转折处
                    dr("x"） = datatable_T.Rows(i)("x")
                    dr("y1"） = GetYFromX(datatable_B, dr("x"）) '底
                    dr("y2"） = datatable_T.Rows(i)("y") '顶
                    dtTemplate.Rows.Add(dr)
                Next


                '按步长 step_cal插入剖分线，插入时不能距离已有线太近，阈值为  min_threshold
                Dim minx As Double = datatable_T.Rows(0)("x")
                Dim maxx As Double = datatable_T.Rows(datatable_T.Rows.Count - 1)("x")
                Dim x_current As Double = minx
                Do While x_current < maxx
                    'MessageBox.Show(x_current & "ppp")
                    If x_current > minx Then '第一个点的信息上一步已经录入了
                        Dim boolean_effective As Boolean = True
                        ' 检查当前 x_current 是否与已有的控制点太近
                        For i = 1 To datatable_T.Rows.Count - 1
                            If Math.Abs(x_current - datatable_T.Rows(i)("x")) < min_threshold Then
                                boolean_effective = False
                                Exit For
                            End If
                        Next
                        ' 如果不冲突，则生成新点
                        If boolean_effective = True Then
                            Dim dr2 = dtTemplate.NewRow
                            dr2("ID"） = "哈哈" '后面重新编编号，不影响
                            dr2("x"） = x_current
                            dr2("y1"） = GetYFromX(datatable_B, x_current)
                            dr2("y2"） = GetYFromX(datatable_T, x_current)
                            dtTemplate.Rows.Add(dr2)
                        End If
                    End If

                    'MessageBox.Show(step_cal)
                    x_current = x_current + step_cal
                Loop

                '排序，按x列升序排列
                '' 创建 DataView（或直接使用 DefaultView）
                Dim dv As DataView = dtTemplate.DefaultView
                ' 按 "ColumnName" 列升序排序
                dv.Sort = "x ASC"

                ' dv.Sort = "ColumnName DESC"' 或降序
                ' 如果需要获取排序后的 DataTable（新表）
                dtTemplate = dv.ToTable()

                'ID重新排序赋值
                Dim n_temp As Integer = 0

                For j = 0 To dtTemplate.Rows.Count - 1
                    If dtTemplate.Rows(j)("ID").ToString().Trim().Contains("*") = False Then '标记坡顶线的转折处
                        dtTemplate.Rows(j)("ID") = n_temp
                    Else
                        dtTemplate.Rows(j)("ID") = n_temp & "*"
                    End If
                    n_temp = n_temp + 1
                Next

                '准备除了x y1 y2 之外的基础数据
                For m = 0 To dtTemplate.Rows.Count - 1
                    dtTemplate.Rows(m)("h") = dtTemplate.Rows(m)("y2") - dtTemplate.Rows(m)("y1")
                    'If dtTemplate.Rows(m)("h") <= 1.5 Then
                    '    dtTemplate.Rows(m)("dq_‌foundation_height") = 0.5
                    'Else
                    '    dtTemplate.Rows(m)("dq_‌foundation_height") = 1
                    '    'dtTemplate.Rows(m)("dq_‌foundation_height") = dq_‌foundation_height
                    'End If
                    dtTemplate.Rows(m)("dqdy2") = dtTemplate.Rows(m)("y2") '挡墙设计顶高程，因为是填方边坡，所以是固定值，


                    'Dim h_temp As Double = Math.Ceiling(dtTemplate.Rows(m)("h") + dtTemplate.Rows(m)("dq_‌foundation_height"))
                    Dim h_temp As Double = GetFirstCeilingKey(dtTemplate.Rows(m)("h"))
                    dtTemplate.Rows(m)("dq_‌foundation_height") = dq_Map(h_temp).foundation_height

                    If h_temp < min_dq_height Then h_temp = min_dq_height '限制最小挡墙高度2m

                    dtTemplate.Rows(m)("pfxdqdh") = h_temp '该条分线处挡墙的高度的最小值，(地形高差+基础深度)，将来挡墙高不能比这个小
                    dtTemplate.Rows(m)("pfxdqdy1") = dtTemplate.Rows(m)("y2") - h_temp
                    dtTemplate.Rows(m)("pfxdqdy2") = dtTemplate.Rows(m)("y2")
                    dtTemplate.Rows(m)("h_plus_f") = dtTemplate.Rows(m)("h") + dtTemplate.Rows(m)("dq_‌foundation_height")
                    If m <> dtTemplate.Rows.Count - 1 Then
                        dtTemplate.Rows(m)("L") = dtTemplate.Rows(m + 1)("x") - dtTemplate.Rows(m)("x")
                        dtTemplate.Rows(m)("pd") = （dtTemplate.Rows(m + 1)("y1") - dtTemplate.Rows(m)("y1")） / dtTemplate.Rows(m)("L")
                        If Math.Abs（dtTemplate.Rows(m)("pd")） > 0.05 Then
                            dtTemplate.Rows(m)("memo") = "陡坡sli"
                        End If
                    End If
                Next

                '第三步：准备dt（合并相同高度的相邻挡墙）
                '改为按 pfxdqdh（向上取整后）分组，保证不同挡墙高度(1m/2m/3m)绝不混在同一组
                dtTemplate = GroupByPfxdqdy1RangeFixed(dtTemplate, 1).Copy
                'AdjustSmallGroups(dtTemplate, min_segment_length)

                '根据上一步的分组结果，合并分组，（其实就是新建datatable，并取各剖分线第一组）并更新相关数据
                'Dim dt = dtTemplate.Clone
                Dim dt As DataTable = New DataTable
                dt.Columns.Add("group", Type.GetType("System.Double")) '分组
                dt.Columns.Add("dqdh", Type.GetType("System.Double")) '分组
                dt.Columns.Add("L", Type.GetType("System.Double")) '分组
                dt.Columns.Add("x", Type.GetType("System.Double"))
                dt.Columns.Add("y1", Type.GetType("System.Double"))
                dt.Columns.Add("dqdy1", Type.GetType("System.Double"))
                dt.Columns.Add("dqdy2", Type.GetType("System.Double"))
                dt.Columns.Add("pd", Type.GetType("System.Double"))
                dt.Columns.Add("memo", Type.GetType("System.String"))
                dt.Columns.Add("memo2", Type.GetType("System.String"))
                'dt.Columns（"dqdh"）.Caption = "挡墙高度dqdh"
                'dt.Columns（"L"）.Caption = "挡墙长度L"

                'dt.Columns（"dqdy1"）.Caption = "挡墙设计底高程dqdy1"
                'dt.Columns（"dqdy2"）.Caption = "挡墙设计顶高程dqdy2"
                'dt.Columns（"pd"）.Caption = "挡墙基底纵坡"
                'dt.Columns（"memo"）.Caption = "备注"

                Dim current_group As Double = 0
                For x = 0 To dtTemplate.Rows.Count - 1
                    If x = 0 Then

                        'dr1 = dtTemplate.Rows(x)
                        'dt.ImportRow(dr1)
                        'dt.Rows(dt.Rows.Count - 1)("L") = DBNull.Value '这三项没有意义了，在dt中后续重新赋值
                        'dt.Rows(dt.Rows.Count - 1)("pd") = DBNull.Value '这三项没有意义了，在dt中后续重新赋值
                        'dt.Rows(dt.Rows.Count - 1)("memo") = "" '这三项没有意义了，在dt中后续重新赋值
                        Dim dr1 = dt.NewRow
                        dr1("x") = dtTemplate.Rows(x)("x")
                        dr1("y1") = dtTemplate.Rows(x)("y1")
                        dr1("group") = dtTemplate.Rows(x)("group")
                        dr1("dqdh") = dtTemplate.Rows(x)("dqdh")
                        'dr1("L") = DBNull.Value
                        dr1("dqdy1") = dtTemplate.Rows(x)("dqdy1")
                        dr1("dqdy2") = dtTemplate.Rows(x)("dqdy2")
                        dt.Rows.Add(dr1)

                        current_group = dtTemplate.Rows(x)("group")
                        'ElseIf x < dtTemplate.Rows.Count - 1 And x > 0 Then
                    ElseIf x < dtTemplate.Rows.Count - 1 And x > 0 Then
                        If dtTemplate.Rows(x)("group") <> current_group Then
                            'Dim dr2 = dt.NewRow

                            ''起点前推算法，解决两段挡墙之间的挡墙的归属，如果下一段挡墙的pfxdqdy1，低于当前挡墙的pfxdqdy1，则将下一段挡墙的pfxdqdy1，前推到当前挡墙的pfxdqdy1，避免过渡段挡墙的基础埋深不满足要求
                            'If dtTemplate.Rows(x)("pfxdqdy1") < dtTemplate.Rows(x - 1)("pfxdqdy1") Then
                            '    dr2 = dtTemplate.Rows(x)
                            '    'dr2("ID") = dtTemplate.Rows(x - 1)("ID") & "起点前推"
                            '    dr2("memo2") = "起点前推"
                            '    'dr2("x") = dtTemplate.Rows(x - 1)("x")
                            '    'dr2("y2") = dtTemplate.Rows(x - 1)("y2")
                            '    'dr2("y1") = dtTemplate.Rows(x - 1)("y1")
                            '    'dr2("h") = dtTemplate.Rows(x - 1)("h")
                            '    'dr2("dq_‌foundation_height") = dtTemplate.Rows(x - 1)("dq_‌foundation_height")

                            '    'dr2("x") = dtTemplate.Rows(x - 1)("x")
                            '    'dr2("y2") = dtTemplate.Rows(x - 1)("y2")
                            '    'dr2("y1") = dtTemplate.Rows(x - 1)("y1")
                            '    'dr2("h") = dtTemplate.Rows(x - 1)("h")
                            '    'dr2("dq_‌foundation_height") = dtTemplate.Rows(x - 1)("dq_‌foundation_height")
                            '    dr2 = dtTemplate.Rows(x - 1)
                            '    dr2("dqdh") = dtTemplate.Rows(x)（"dqdh"）
                            '    dr2("x") = dtTemplate.Rows(x)（"dqdh"）

                            'Else
                            '    dr2 = dtTemplate.Rows(x)
                            'End If

                            'dr2 = dtTemplate.Rows(x)
                            'dt.ImportRow(dr2)
                            'dt.Rows(dt.Rows.Count - 1)("L") = DBNull.Value '这三项没有意义了，在dt中后续重新赋值
                            'dt.Rows(dt.Rows.Count - 1)("pd") = DBNull.Value '这三项没有意义了，在dt中后续重新赋值
                            'dt.Rows(dt.Rows.Count - 1)("memo") = "" '这三项没有意义了，在dt中后续重新赋值
                            '
                            Dim dr1 = dt.NewRow
                            dr1("x") = dtTemplate.Rows(x)("x")
                            dr1("y1") = dtTemplate.Rows(x)("y1")
                            dr1("group") = dtTemplate.Rows(x)("group")
                            dr1("dqdh") = dtTemplate.Rows(x)("dqdh")
                            'dr1("L") = DBNull.Value
                            dr1("dqdy1") = dtTemplate.Rows(x)("dqdy1")
                            dr1("dqdy2") = dtTemplate.Rows(x)("dqdy2")
                            dt.Rows.Add(dr1)
                            current_group = dtTemplate.Rows(x)("group")
                        End If
                        'ElseIf x = dtTemplate.Rows.Count - 1 Then

                    End If
                Next

                '前推算法 暂时取消，假定在计算步长范围内，纵坡变化的导致的基础埋深不足可以忽略
                'For j = 1 To dt.Rows.Count - 1
                '    If dt.Rows(j)("dqdy1") < dt.Rows(j - 1)("dqdy1") Then
                '        Dim row_id As Integer = GetRowIndexByX_Binary(dtTemplate, dt.Rows(j)("x")) - 1
                '        If row_id > 0 Then
                '            dt.Rows(j)("x") = dtTemplate.Rows(row_id)("x")
                '            dt.Rows(j)("memo2") = "起点前推"
                '        Else
                '            MessageBox.Show("起点前推出错!")
                '        End If
                '    End If
                'Next

                ''最小挡墙分段
                'For j = 1 To dt.Rows.Count - 3
                '    If (dt.Rows(j)("x") - dt.Rows(j - 1)("x")) < min_segment_length Then
                '        Dim L_before As Double = dt.Rows(j)("x") - dt.Rows(j - 1)("x")
                '        Dim L_after As Double = dt.Rows(j + 2)("x") - dt.Rows(j + 1)("x")
                '        If L_after >= L_before Then '合并进入前段
                '            dt.Rows(j - 1)("dqdy1") = Math.Min(dt.Rows(j)("dqdy1"), dt.Rows(j - 1)("dqdy1"))
                '            dt.Rows(j - 1)("dqdh") = dt.Rows(j - 1)("dqdy2") - dt.Rows(j - 1)("dqdy1")
                '            dt.Rows(j)("memo2") = "待删除"
                '            'dt.Rows(j)("x") = dt.Rows(j)("x-1")
                '        Else '合并入后段
                '            dt.Rows(j + 1)("x") = dt.Rows(j)("x")
                '            dt.Rows(j + 1)("dqdy1") = Math.Min(dt.Rows(j + 1)("dqdy1"), dt.Rows(j)("dqdy1"))
                '            dt.Rows(j)("memo2") = "待删除"
                '        End If
                '        'Dim row_id As Integer = GetRowIndexByX_Binary(dtTemplate, dt.Rows(j)("x")) - 1
                '        'If row_id > 0 Then
                '        '    dt.Rows(j)("x") = dtTemplate.Rows(row_id)("x")
                '        '    dt.Rows(j)("memo2") = "起点前推"
                '        'Else
                '        '    MessageBox.Show("起点前推出错!")
                '        'End If
                '    End If
                'Next

                'Dim deleteRows As New List(Of DataRow)
                'For j As Integer = 0 To dt.Rows.Count - 1
                '    If dt.Rows(j)("memo2").ToString() = "待删除" Then
                '        deleteRows.Add(dt.Rows(j))
                '    End If
                'Next

                'For Each dr As DataRow In deleteRows
                '    dr.Delete()
                'Next
                'dt.AcceptChanges()



                ''看不懂为啥加#
                'For k = 0 To dt.Rows.Count - 2
                '    Dim min_ele As Double = Double.MinValue
                '    If dt.Rows(k)("dqdy1") > dt.Rows(k + 1)("pfxdqdy1") Then
                '        dt.Rows(k)("ID") = dt.Rows(k)("ID") & "#"
                '        'dt.Rows(k)("dqdy1") = dt.Rows(k + 1)("pfxdqdy1")
                '        'dt.Rows(k)("dqdh") = dt.Rows(k)("dqdy2") - dt.Rows(k)("dqdy1")
                '        min_ele = dt.Rows(k + 1)("pfxdqdy1")
                '    Else
                '        min_ele = dt.Rows(k)("dqdy1")
                '        'dt.Rows(k)("ID") = dt.Rows(k)("ID")
                '    End If
                '    'dt.Rows(k)("pfxdqdy1") = min_ele
                '    Dim h_temp As Double = RoundUpToHalf(dt.Rows(k)("pfxdqdy2") - min_ele)
                '    dt.Rows(k)("dqdy1") = dt.Rows(k)("dqdy2") - h_temp
                '    dt.Rows(k)("dqdh") = dt.Rows(k)("dqdy2") - dt.Rows(k)("dqdy1")
                'Next

                '第四步：再次更新整理dt的L(挡墙长度)和纵坡
                For t = 0 To dt.Rows.Count - 2
                    dt.Rows(t)("L") = dt.Rows(t + 1)("x") - dt.Rows(t)("x")
                    Dim y1_next As Double = dt.Rows(t + 1)("y1")
                    Dim y1 As Double = dt.Rows(t)("y1")
                    dt.Rows(t)("pd") = (y1_next - y1) / (dt.Rows(t + 1)("x") - dt.Rows(t)("x"))
                    If Math.Abs（dt.Rows(t)("pd")） > 0.05 Then
                        dt.Rows(t)("memo") = "陡坡"
                    End If
                    'dt.Rows(t)("name") = t & "#"
                Next
                '单独整理最后一段挡墙的数据
                dt.Rows(dt.Rows.Count - 1)("L") = dtTemplate.Rows(dtTemplate.Rows.Count - 1)("x") - dt.Rows(dt.Rows.Count - 1)("x")
                dt.Rows(dt.Rows.Count - 1)("pd") = (dtTemplate.Rows(dtTemplate.Rows.Count - 1)("y1") - dt.Rows(dt.Rows.Count - 1)("y1")) / dt.Rows(dt.Rows.Count - 1)("L")
                If Math.Abs（dt.Rows(dt.Rows.Count - 1)("pd")） > 0.05 Then
                    dt.Rows(dt.Rows.Count - 1)("memo") = "陡坡"
                End If

                Createdqdxf4（str_filename_temp, dt, datatable_B, datatable_T）
                'Createdqdxf5（str_filename_temp, dtTemplate, datatable_B, datatable_T）

                If ExcelHelper.IsFileLocked(str_filename_temp) = True Then
                    MessageBox.Show($"保存Excel文件时发生未知错误1，文件被占用: {str_filename_temp}")
                    Exit Sub
                Else
                    'ExportDataTableToExcel(dtTemplate, str_filename_temp)
                    ' 定义报告文件夹路径
                    Dim reportFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report")
                    If Not Directory.Exists(reportFolder) Then Directory.CreateDirectory(reportFolder)

                    str_filename_temp = str_filename_temp & $"_{Date.Now:yyyyMMdd_HH_mm_ss}.xlsx"
                    Dim fullPath As String = Path.Combine(reportFolder, str_filename_temp)

                    Dim dt_sum = GroupSumTwoCol(dtTemplate, "dqdh", "L"）
                    Dim dataset_sheet = New DataSet
                    'dataset_sheet.Tables.Add(dtTemplate)
                    dataset_sheet.Tables.Add(dt) '挡墙工程量统计
                    dataset_sheet.Tables.Add(dtTemplate) '挡墙绘图中间基础数据
                    dataset_sheet.Tables.Add(datatable_T) '挡墙顶部设计线数据
                    dataset_sheet.Tables.Add(datatable_B) '挡墙底部设计线数据
                    dataset_sheet.Tables.Add(dt_sum) '挡墙分段汇总数据
                    Dim arr = New ArrayList
                    arr.Add("挡墙工程量统计")
                    arr.Add("挡墙设计中间数据")
                    arr.Add("挡墙顶部地形线")
                    arr.Add("挡墙底部地形线")
                    arr.Add("挡墙长度分组求和")

                    ' 调用导出并检查返回值
                    Dim ok As Boolean = Export_hp_dataset_to_excel(dataset_sheet, fullPath, arr)
                    If Not ok Then
                        ' 导出函数会显示具体异常，额外也弹个提示并退出
                        MessageBox.Show($"导出失败，请检查错误信息。目标路径：{fullPath}", "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If

                    ' 尝试打开刚保存的完整路径文件
                    Try
                        Process.Start(New ProcessStartInfo(fullPath) With {.UseShellExecute = True})
                    Catch ex As Exception
                        MessageBox.Show($"导出成功，但打开文件失败: {ex.GetType().Name}: {ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Try
                End If
            End If
        End If

    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 读取 JSON 并绑定到界面的控件上
        Dim config As DqConfig = DqConfig.LoadConfig()
        'txt_foundation_height.Text = config.dq_foundation_height.ToString()
        txt_step_cal.Text = config.step_cal.ToString()
        'txt_min_dq_length.Text = config.min_dq_length.ToString()
        txt_difference_height_base.Text = config.dq_difference_height_base.ToString()

        ' 同步到模块级变量
        min_dq_height = config.min_dq_height
        step_cal = config.step_cal
        min_segment_length = config.min_segment_length
        dq_difference_height_base = config.dq_difference_height_base
    End Sub

    ' 4 个数值输入框共用的 KeyPress 事件：
    '   只允许 数字(0-9) / 小数点(.) / 退格，其它字符一律拦截
    Private Sub NumericTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles _
        txt_foundation_height.KeyPress,
        txt_step_cal.KeyPress,
        txt_min_dq_length.KeyPress,
        txt_difference_height_base.KeyPress

        ' 数字：通过
        If Char.IsDigit(e.KeyChar) Then Exit Sub

        ' 小数点：每个输入框只允许出现一次
        If e.KeyChar = "."c Then
            Dim tb = DirectCast(sender, TextBox)
            If tb.Text.Contains(".") Then
                e.Handled = True
            End If
            Exit Sub
        End If

        ' 退格键：通过
        If e.KeyChar = ControlChars.Back Then Exit Sub

        ' 其它字符（字母、空格、负号、其它符号等）：全部拦截
        e.Handled = True
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' 保存按钮：将界面参数写回 JSON，并同步到模块级变量
        Try
            ' 先做完整校验，给出具体到字段名的错误提示
            Dim foundationHeight = ParseNonNegative("基础埋深", txt_foundation_height.Text)
            Dim stepCal = ParseNonNegative("插入剖分线的计算步长", txt_step_cal.Text)
            Dim minDqLength = ParseNonNegative("阈值1 两剖分线最小距离", txt_min_dq_length.Text)
            Dim diffHeightBase = ParseNonNegative("阈值2 挡墙基底高差", txt_difference_height_base.Text)

            ' 校验通过，写入配置
            Dim config As New DqConfig()
            'config.dq_foundation_height = foundationHeight
            config.step_cal = stepCal
            'config.min_dq_length = minDqLength
            config.dq_difference_height_base = diffHeightBase

            ' 写入 JSON 文件
            DqConfig.SaveConfig(config)

            ' 同步到模块级变量（保证后续绘图逻辑立即生效）
            'dq_‌foundation_height = config.dq_foundation_height
            step_cal = config.step_cal
            'min_dq_length = config.min_dq_length
            min_segment_length = config.min_segment_length
            dq_difference_height_base = config.dq_difference_height_base

            MessageBox.Show("参数已保存。", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("保存失败：" & ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' 解析一个文本框为 Double；为空/非数值/<0 时抛出 ArgumentException，并附字段名
    Private Function ParseNonNegative(label As String, text As String) As Double
        Dim trimmed = If(text, "").Trim()
        If String.IsNullOrEmpty(trimmed) Then
            Throw New ArgumentException($"【{label}】不能为空")
        End If
        Dim v As Double
        If Not Double.TryParse(trimmed, v) Then
            Throw New ArgumentException($"【{label}】不是合法的数值（输入：'{trimmed}'）")
        End If
        If v < 0 Then
            Throw New ArgumentException($"【{label}】必须大于等于 0（当前值：{v}）")
        End If
        Return v
    End Function
End Class
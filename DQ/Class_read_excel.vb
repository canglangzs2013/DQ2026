Imports System.IO
Imports NPOI.SS.UserModel
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Public Class Class_read_excel

    ''' <summary>
    ''' 读取Excel(xls/xlsx)，按dt_input结构填充；自动识别真实格式，不依赖扩展名
    ''' </summary>
    Public Function DoReadExcelDataTable(ByVal filePath As String, ByVal sheet_id As Integer, ByVal dt_input As DataTable) As DataTable
        If dt_input Is Nothing Then
            Throw New ArgumentNullException(NameOf(dt_input), "dt_input不能为Nothing")
        End If

        Dim dt As DataTable = dt_input.Clone()

        '文件存在性与有效性校验
        If Not File.Exists(filePath) Then
            MessageBox.Show("文件不存在：" & filePath, "读取Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return dt
        End If
        If New FileInfo(filePath).Length < 1024 Then
            MessageBox.Show("文件为空或已损坏（大小 " & New FileInfo(filePath).Length & " 字节）：" & filePath,
                            "读取Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return dt
        End If

        If sheet_id < 0 Then
            Return dt
        End If

        Try
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                '★★★★★ 核心修复：自动识别 OLE2(.xls) / OOXML(.xlsx)，不再看扩展名 ★★★★★
                Dim workbook As IWorkbook = WorkbookFactory.Create(fs)

                Try
                    If sheet_id >= workbook.NumberOfSheets Then
                        Return dt
                    End If

                    Dim sheet As ISheet = workbook.GetSheetAt(sheet_id)

                    '数据起点固定为第2行(0标题,1表头)，不依赖FirstRowNum避免-1
                    Dim startRowIndex As Integer = 2
                    If sheet.LastRowNum < startRowIndex Then
                        Return dt   '空表
                    End If

                    For i As Integer = startRowIndex To sheet.LastRowNum
                        Dim row As IRow = sheet.GetRow(i)
                        If row Is Nothing Then
                            Continue For
                        End If

                        Dim dtrow As DataRow = dt.NewRow()
                        Dim hasData As Boolean = False

                        '列循环直接用模板列数，避免 FirstCellNum=-1
                        For j As Integer = 0 To dt.Columns.Count - 1
                            Dim cell As ICell = row.GetCell(j, MissingCellPolicy.RETURN_NULL_AND_BLANK)
                            If cell Is Nothing Then
                                dtrow(j) = DBNull.Value
                                Continue For
                            End If

                            Dim cellVal As Object = GetCellValue(cell)
                            dtrow(j) = If(cellVal Is Nothing, DBNull.Value, cellVal)

                            If Not IsDBNull(dtrow(j)) Then
                                hasData = True
                            End If
                        Next

                        If hasData Then
                            dt.Rows.Add(dtrow)
                        End If
                    Next
                Finally
                    workbook.Close()
                End Try
            End Using
        Catch ex As IOException
            MessageBox.Show($"文件IO异常：{ex.Message}", "读取Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            MessageBox.Show($"读取Excel出错：{ex.Message}", "读取Excel", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return dt
    End Function

    ''' <summary>
    ''' 通用取单元格值：公式取缓存结果，处理数字/日期/文本/布尔/空
    ''' </summary>
    Private Function GetCellValue(cell As ICell) As Object
        If cell.CellType = CellType.Formula Then
            Select Case cell.CachedFormulaResultType
                Case CellType.String
                    Return cell.StringCellValue
                Case CellType.Numeric
                    Return If(DateUtil.IsCellDateFormatted(cell), CObj(cell.DateCellValue), CObj(cell.NumericCellValue))
                Case CellType.Boolean
                    Return cell.BooleanCellValue
                Case Else
                    Return Nothing
            End Select
        End If

        Select Case cell.CellType
            Case CellType.Blank
                Return Nothing
            Case CellType.String
                Return cell.StringCellValue
            Case CellType.Numeric
                Return If(DateUtil.IsCellDateFormatted(cell), CObj(cell.DateCellValue), CObj(cell.NumericCellValue))
            Case CellType.Boolean
                Return cell.BooleanCellValue
            Case Else
                Return cell.ToString()
        End Select
    End Function

End Class

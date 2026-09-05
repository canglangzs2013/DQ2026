
Imports System.IO
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Module Module_save
    ''' <summary>
    ''' 将 DataTable 导出到指定的 .xlsx 文件。
    ''' </summary>
    ''' <param name="dataTable">要导出的数据表</param>
    ''' <param name="filePath">输出文件的完整路径，例如 "C:\output.xlsx"</param>
    Public Sub ExportDataTableToExcel(dataTable As DataTable, filePath As String)
        ' --- 输入验证 ---
        If dataTable Is Nothing OrElse dataTable.Rows.Count = 0 Then
            Throw New ArgumentException("DataTable 不能为空或没有数据。")
        End If

        If String.IsNullOrWhiteSpace(filePath) Then
            Throw New ArgumentException("文件路径不能为空。")
        End If

        ' --- 确保文件扩展名是 .xlsx ---
        If Not filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) Then
            Throw New ArgumentException("仅支持 .xlsx 格式的文件。")
        End If

        ' --- 创建一个新的 Excel 工作簿 (.xlsx) ---
        Dim workbook As IWorkbook = New XSSFWorkbook()
        Dim sheet As ISheet = workbook.CreateSheet("Sheet1")

        ' --- 创建表头样式 (可选，但推荐) ---
        Dim headerStyle As ICellStyle = workbook.CreateCellStyle()
        Dim headerFont As IFont = workbook.CreateFont()
        headerFont.IsBold = True
        headerFont.FontHeightInPoints = 12
        headerStyle.SetFont(headerFont)
        headerStyle.Alignment = HorizontalAlignment.Center
        headerStyle.VerticalAlignment = VerticalAlignment.Center

        ' --- 1. 写入列标题 (Header Row) ---
        Dim headerRow As IRow = sheet.CreateRow(0)
        For i As Integer = 0 To dataTable.Columns.Count - 1
            Dim cell As ICell = headerRow.CreateCell(i)
            cell.SetCellValue(dataTable.Columns(i).ColumnName)
            cell.CellStyle = headerStyle ' 应用样式
        Next

        ' --- 2. 写入数据行 ---
        Dim rowIndex As Integer = 1
        For Each dataRow As DataRow In dataTable.Rows
            Dim row As IRow = sheet.CreateRow(rowIndex)
            For j As Integer = 0 To dataTable.Columns.Count - 1
                Dim cell As ICell = row.CreateCell(j)
                Dim value As Object = dataRow(j)

                ' --- 根据数据类型设置单元格值，避免格式错误 ---
                If value Is Nothing OrElse value Is DBNull.Value Then
                    cell.SetCellValue("")
                ElseIf TypeOf value Is DateTime Then
                    cell.SetCellValue(CDate(value))
                ElseIf TypeOf value Is Boolean Then
                    cell.SetCellValue(CBool(value))
                ElseIf TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal OrElse TypeOf value Is Integer OrElse TypeOf value Is Long Then
                    cell.SetCellValue(Convert.ToDouble(value))
                Else
                    cell.SetCellValue(value.ToString())
                End If
            Next
            rowIndex += 1
        Next

        ' --- 3. 自动调整列宽 (可选) ---
        For i As Integer = 0 To dataTable.Columns.Count - 1
            sheet.AutoSizeColumn(i)
        Next

        ' --- 4. 保存文件到磁盘 ---
        Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write)
            workbook.Write(fs)
        End Using

        ' --- 5. 释放资源 ---
        workbook.Close()
    End Sub
End Module

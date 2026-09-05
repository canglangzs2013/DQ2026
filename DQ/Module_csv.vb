Imports System.Data
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO

Module Module_csv



    Public Sub SaveDataTableToCsv(dataTable As DataTable, filePath As String)
        If dataTable Is Nothing OrElse dataTable.Rows.Count = 0 Then
            Throw New ArgumentException("DataTable 不能为空或没有数据。")
        End If

        ' 使用 StringBuilder 提高性能
        Dim csvContent As New StringBuilder()

        ' --- 1. 写入表头 ---
        Dim header As New List(Of String)()
        For Each column As DataColumn In dataTable.Columns
            header.Add(EscapeCsvField(column.ColumnName))
        Next
        csvContent.AppendLine(String.Join(",", header))

        ' --- 2. 写入数据行 ---
        For Each row As DataRow In dataTable.Rows
            Dim fields As New List(Of String)()
            For Each column As DataColumn In dataTable.Columns
                fields.Add(EscapeCsvField(row(column).ToString()))
            Next
            csvContent.AppendLine(String.Join(",", fields))
        Next

        ' --- 3. 写入文件 (使用 UTF-8 with BOM 确保 Excel 正确识别中文) ---
        File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 转义 CSV 字段中的特殊字符（如逗号、双引号、换行符）。
    ''' </summary>
    Private Function EscapeCsvField(input As String) As String
        If String.IsNullOrEmpty(input) Then Return ""

        ' 如果字段包含逗号、双引号或换行符，则需要用双引号包围
        If input.Contains(","c) OrElse input.Contains("""") OrElse input.Contains(vbLf) OrElse input.Contains(vbCr) Then
            ' 将字段内的每个双引号替换为两个双引号
            input = input.Replace("""", """""")
            Return $"""{input}"""
        Else
            Return input
        End If
    End Function

    Public Function ReadCsvToDataTable(filePath As String, Optional hasHeader As Boolean = True) As DataTable
        Dim dt As New DataTable()

        Using parser As New TextFieldParser(filePath)
            parser.Delimiters = {","} ' 设置分隔符为逗号
            parser.HasFieldsEnclosedInQuotes = True ' 正确处理 "field, with comma"
            parser.TrimWhiteSpace = True

            Dim isHeaderRow As Boolean = hasHeader

            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()

                ' 跳过空行
                If fields Is Nothing OrElse fields.Length = 0 Then Continue While

                ' 处理表头
                If isHeaderRow Then
                    For Each header In fields
                        ' 处理重复列名
                        Dim columnName As String = If(String.IsNullOrWhiteSpace(header), "Column" & dt.Columns.Count, header.Trim())
                        Dim uniqueName As String = columnName
                        Dim counter As Integer = 1
                        While dt.Columns.Contains(uniqueName)
                            uniqueName = $"{columnName}_{counter}"
                            counter += 1
                        End While
                        dt.Columns.Add(uniqueName)
                    Next
                    isHeaderRow = False
                Else
                    ' 处理数据行
                    ' 确保字段数量与列数一致，避免异常
                    Dim row As DataRow = dt.NewRow()
                    For i As Integer = 0 To Math.Min(fields.Length, dt.Columns.Count) - 1
                        row(i) = If(String.IsNullOrEmpty(fields(i)), DBNull.Value, fields(i))
                    Next
                    dt.Rows.Add(row)
                End If
            End While
        End Using

        Return dt
    End Function


End Module

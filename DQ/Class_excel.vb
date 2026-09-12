'这是关键的一行
Imports System.IO
Imports System.Reflection.Metadata.Ecma335
Imports System.Threading
Imports NPOI.HSSF.UserModel
Imports NPOI.SS.Formula.Functions
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel
'貌似是excel的基本操作，不可排除’
Public Class ExcelHelper
    ' 创建新的Excel工作簿
    Public Function CreateNewWorkbook() As IWorkbook
        Return New XSSFWorkbook()
    End Function

    ' 打开Excel文件
    Public Function OpenWorkbook(filePath As String) As IWorkbook
        Using fileStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
            'MessageBox.Show(filePath)
            Return WorkbookFactory.Create(fileStream)
        End Using
    End Function
    ' 保存Excel文件


    ''' <summary>
    ''' 加载Excel文件到DataTable（自动合并所有非空工作表）
    ''' </summary>
    Public Function LoadExcelToDataTable(filePath As String, datatable_template As DataTable) As DataTable
        Dim datatable_temp As New DataTable()

        Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
            Dim workbook = WorkbookFactory.Create(fs)

            ' 遍历所有工作表
            For sheetIndex = 0 To workbook.NumberOfSheets - 1
                Dim sheet = workbook.GetSheetAt(sheetIndex)
                If Not IsSheetEmpty(sheet) Then
                    '' 首次加载时初始化列结构
                    'If dtResult.Columns.Count = 0 Then
                    '    'InitializeDataTableColumns(dtResult, sheet)
                    'End If

                    If sheetIndex = 0 Then
                        datatable_temp = datatable_template.Clone '2026修改，如果一个工作簿有多个工作表，可以全部读取到一个datatable，修改前只能保存最后一个工作表’
                    End If


                    ' 填充数据行
                    For rowNum = 1 To sheet.LastRowNum ' 跳过表头
                            Dim row = sheet.GetRow(rowNum)
                            If row IsNot Nothing Then
                                Dim dr = datatable_temp.NewRow()
                                For colNum = 0 To datatable_temp.Columns.Count - 1
                                    'If datatable_temp.Columns(colNum).ColumnName = "长度" Or datatable_temp.Columns(colNum).ColumnName = "ratio" Then
                                    '    '如果是模版配置的长度或者放大系数的系数列， 只能取为数值， 否则会报错
                                    '    dr(colNum) = GetCellValue_ONLYNUMBER(row.GetCell(colNum))
                                    'Else
                                    '    dr(colNum) = GetCellValue(row.GetCell(colNum))
                                    'End If

                                    Dim columnType As Type = datatable_temp.Columns(colNum).DataType

                                If columnType Is GetType(String) Then
                                    dr(colNum) = GetCellValue(row.GetCell(colNum))
                                ElseIf columnType Is GetType(Decimal) Then
                                    'Console.WriteLine($"{columnName} 是Double类型")
                                    '如果是模版配置的长度或者放大系数的系数列， 只能取为数值， 否则会报错
                                    dr(colNum) = GetCellValue_ONLYNUMBER(row.GetCell(colNum))
                                    'MessageBox.Show("columnType Is GetType(Decimal)")
                                Else
                                        'Console.WriteLine($"{columnName} 是其他类型: {columnType.Name}")
                                    End If

                                Next
                                datatable_temp.Rows.Add(dr)
                            End If
                        Next
                    End If
            Next
        End Using

        Return datatable_temp
    End Function

    ''' <summary>
    ''' 检查工作表是否为空
    ''' </summary>
    Public Shared Function IsSheetEmpty(sheet As ISheet) As Boolean
        If sheet Is Nothing OrElse sheet.LastRowNum < 0 Then Return True

        ' 检查所有非空单元格
        For rowNum = sheet.FirstRowNum To sheet.LastRowNum
            Dim row = sheet.GetRow(rowNum)
            If row IsNot Nothing Then
                For cellNum = 0 To row.LastCellNum - 1
                    Dim cell = row.GetCell(cellNum)
                    If cell IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cell.ToString()) Then
                        Return False
                    End If
                Next
            End If
        Next
        Return True
    End Function

    '' <summary>
    '' 初始化DataTable列结构（根据第一行表头）
    '' </summary>
    'Public Sub InitializeDataTableColumns(dt As DataTable, sheet As ISheet)
    '    Dim headerRow = sheet.GetRow(sheet.FirstRowNum)
    '    For i = 0 To headerRow.LastCellNum - 1
    '        Dim cell = headerRow.GetCell(i)
    '        dt.Columns.Add(If(cell IsNot Nothing, cell.ToString(), $"Column{i}"))
    '    Next
    'End Sub

    ''' <summary>
    ''' 获取单元格值（处理所有数据类型）
    ''' </summary>
    Public Function GetCellValue(cell As ICell) As Object
        If cell Is Nothing Then Return DBNull.Value

        Select Case cell.CellType
            Case CellType.Numeric
                Return If(DateUtil.IsCellDateFormatted(cell),
                         cell.DateCellValue,
                         cell.NumericCellValue)
            Case CellType.String
                Return cell.StringCellValue

            Case CellType.Boolean
                Return cell.BooleanCellValue

            Case CellType.Formula

                Return cell.NumericCellValue ' 或根据需求返回公式结果
            Case Else
                Return DBNull.Value

        End Select
    End Function

    Public Function GetCellValue_ONLYNUMBER(cell As ICell) As Object
        'NPOI.SS.UserModel.ICell.ICell
        If cell Is Nothing Then Return DBNull.Value

        Select Case cell.CellType
            Case CellType.Numeric
                Return If(DateUtil.IsCellDateFormatted(cell),
                         cell.DateCellValue,
                         cell.NumericCellValue)
            Case CellType.String
                'Return cell.StringCellValue
                'Return 0
                Dim quantity As Decimal = 0
                Decimal.TryParse(cell.ToString(), quantity)
                Return quantity
            Case CellType.Boolean
                'Return cell.BooleanCellValue
                Return 0
            Case CellType.Formula
                'Return 0
                Return cell.NumericCellValue ' 或根据需求返回公式结果
            Case Else
                'Return DBNull.Value
                Return 0
        End Select
    End Function
    ''' <summary>
    ''' 将DataTable保存为Excel文件
    ''' </summary>
    ''' <param name="dt">数据源</param>
    ''' <param name="filePath">文件路径（如：C:\data.xlsx）</param>
    ''' <param name="sheetName">工作表名称（默认Sheet1）</param>
    Public Sub SaveDataTableToExcel(dt As DataTable, filePath As String, Optional sheetName As String = "Sheet1")


        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Throw New ArgumentException("DataTable不能为空")
        End If

        Dim workbook As IWorkbook

        ' 根据扩展名选择工作簿类型
        Select Case Path.GetExtension(filePath).ToLower()
            Case ".xlsx"
                workbook = New XSSFWorkbook()
            Case ".xls"
                workbook = New HSSFWorkbook()
            Case Else
                Throw New ArgumentException("不支持的文件格式，仅支持.xls或.xlsx")
        End Select

        ' 创建工作表
        Dim sheet As ISheet = workbook.CreateSheet(sheetName)

        ' 创建表头行
        Dim headerRow As IRow = sheet.CreateRow(0)
        For i As Integer = 0 To dt.Columns.Count - 1
            headerRow.CreateCell(i).SetCellValue(dt.Columns(i).ColumnName)
        Next

        ' 填充数据行
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim dataRow As IRow = sheet.CreateRow(i + 1)
            For j As Integer = 0 To dt.Columns.Count - 1
                Dim cellValue = If(dt.Rows(i)(j) Is DBNull.Value, String.Empty, dt.Rows(i)(j).ToString())
                dataRow.CreateCell(j).SetCellValue(cellValue)
            Next
        Next

        ' 自动调整列宽
        For i As Integer = 0 To dt.Columns.Count - 1
            sheet.AutoSizeColumn(i)
        Next

        ' 保存文件
        Using fs As New FileStream(filePath, FileMode.Create)
            workbook.Write(fs)
        End Using
    End Sub
    '''' <summary>
    '''' 加载Excel文件到DataTable（自动合并所有非空工作表）
    '''' </summary>
    'Public Function LoadExcelToDataTable(filePath As String) As DataTable
    '    Dim dtResult As New DataTable()

    '    Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
    '        Dim workbook = WorkbookFactory.Create(fs)

    '        ' 遍历所有工作表
    '        For sheetIndex = 0 To workbook.NumberOfSheets - 1
    '            Dim sheet = workbook.GetSheetAt(sheetIndex)
    '            If Not IsSheetEmpty(sheet) Then
    '                ' 首次加载时初始化列结构
    '                If dtResult.Columns.Count = 0 Then
    '                    InitializeDataTableColumns(dtResult, sheet)
    '                End If

    '                ' 填充数据行
    '                For rowNum = 1 To sheet.LastRowNum ' 跳过表头
    '                    Dim row = sheet.GetRow(rowNum)
    '                    If row IsNot Nothing Then
    '                        Dim dr = dtResult.NewRow()
    '                        For colNum = 0 To dtResult.Columns.Count - 1
    '                            dr(colNum) = GetCellValue(row.GetCell(colNum))
    '                        Next
    '                        dtResult.Rows.Add(dr)
    '                    End If
    '                Next
    '            End If
    '        Next
    '    End Using

    '    Return dtResult
    'End Function
    '''' <summary>
    '''' 检查工作表是否为空
    '''' </summary>
    'Private Function IsSheetEmpty(sheet As ISheet) As Boolean
    '    If sheet Is Nothing OrElse sheet.LastRowNum < 0 Then Return True

    '    ' 检查所有非空单元格
    '    For rowNum = sheet.FirstRowNum To sheet.LastRowNum
    '        Dim row = sheet.GetRow(rowNum)
    '        If row IsNot Nothing Then
    '            For cellNum = 0 To row.LastCellNum - 1
    '                Dim cell = row.GetCell(cellNum)
    '                If cell IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cell.ToString()) Then
    '                    Return False
    '                End If
    '            Next
    '        End If
    '    Next
    '    Return True
    'End Function
    ' 获取工作簿中的所有工作表名称
    Public Function GetSheetNames(workbook As IWorkbook) As List(Of String)
        Dim sheetNames As New List(Of String)
        For i As Integer = 0 To workbook.NumberOfSheets - 1
            'Dim sheet = workbook.GetSheetAt(sheetIndex)
            Dim sheet = workbook.GetSheetAt(i)
            If sheet Is Nothing OrElse ExcelHelper.IsSheetEmpty(sheet) Then Continue For
            sheetNames.Add(workbook.GetSheetName(i))
        Next
        Return sheetNames
    End Function

    '' 读取工作表数据（假设第一行是表头，从第二行开始是数据）
    'Public Function ReadSheetData(sheet As ISheet) As List(Of String())
    '    Dim data As New List(Of String())

    '    For rowIndex As Integer = 1 To sheet.LastRowNum
    '        Dim row As IRow = sheet.GetRow(rowIndex)
    '        If row IsNot Nothing Then
    '            Dim rowData As New List(Of String)
    '            For cellIndex As Integer = 0 To row.LastCellNum - 1
    '                Dim cell As ICell = row.GetCell(cellIndex)
    '                rowData.Add(If(cell IsNot Nothing, cell.ToString(), ""))
    '            Next
    '            data.Add(rowData.ToArray())
    '        End If
    '    Next

    '    Return data
    'End Function

    '' 创建工作表并写入数据
    'Public Sub WriteDataToSheet(workbook As IWorkbook, sheetName As String, data As List(Of String()))
    '    Dim sheet As ISheet = workbook.CreateSheet(sheetName)

    '    ' 写入表头（假设固定三列）
    '    Dim headerRow As IRow = sheet.CreateRow(0)
    '    headerRow.CreateCell(0).SetCellValue("项目内容")
    '    headerRow.CreateCell(1).SetCellValue("单位")
    '    headerRow.CreateCell(2).SetCellValue("工程量")

    '    ' 写入数据
    '    For rowIndex As Integer = 0 To data.Count - 1
    '        Dim rowData As String() = data(rowIndex)
    '        Dim dataRow As IRow = sheet.CreateRow(rowIndex + 1) ' +1跳过表头

    '        For colIndex As Integer = 0 To rowData.Length - 1
    '            dataRow.CreateCell(colIndex).SetCellValue(rowData(colIndex))
    '        Next
    '    Next
    'End Sub

    ' 获取项目文件夹中的Excel文件信息（排除模板和汇总文件）
    Public Function GetProjectFilesInfo(folderPath As String, templateFileName As String, resultFileName As String) As Dictionary(Of String, List(Of String))
        Dim filesInfo As New Dictionary(Of String, List(Of String))

        ' 获取所有Excel文件，排除模板和汇总文件
        Dim excelFiles = Directory.EnumerateFiles(folderPath, "*.xlsx").Where(Function(f) Not f.EndsWith(templateFileName) AndAlso Not f.EndsWith(resultFileName))

        For Each file In excelFiles
            Dim fileName = Path.GetFileNameWithoutExtension(file)
            Dim sheetNames As List(Of String)

            ' 打开工作簿获取工作表名称
            Try
                Using fileStream As New FileStream(file, FileMode.Open, FileAccess.Read)
                    Dim workbook = WorkbookFactory.Create(fileStream)
                    sheetNames = GetSheetNames(workbook)
                End Using

                filesInfo.Add(fileName, sheetNames)
            Catch ex As Exception
                ' 忽略无法读取的文件
                Continue For
            End Try
        Next

        Return filesInfo
    End Function





    ' 检查文件是否被占用
    Public Shared Function IsFileLocked(filePath As String) As Boolean

        Dim fileInfo As New FileInfo(filePath)

        If fileInfo.Exists = True Then
            Try
                ' 尝试以独占方式打开文件
                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                    Return False
                End Using
            Catch ex As IOException
                Return True ' 发生IO异常说明文件被占用
            End Try
        Else
            Return False
        End If


    End Function

    Public Sub ExportToExcelWithStyle(dt As DataTable, filePath As String)
        ' 创建工作簿（xlsx格式）
        Dim workbook As IWorkbook = New XSSFWorkbook()
        Dim sheet As ISheet = workbook.CreateSheet("Sheet1")

        ' ===== 1. 定义样式 =====
        ' 表头样式（加粗+居中+背景色）
        Dim headerStyle As ICellStyle = workbook.CreateCellStyle()
        Dim headerFont As IFont = workbook.CreateFont()
        headerFont.IsBold = True
        headerFont.FontHeightInPoints = 12
        headerStyle.SetFont(headerFont)
        headerStyle.Alignment = HorizontalAlignment.Center
        headerStyle.VerticalAlignment = VerticalAlignment.Center
        'headerStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index
        'headerStyle.FillPattern = FillPattern.SolidForeground
        headerStyle.BorderTop = BorderStyle.Thin
        headerStyle.BorderBottom = BorderStyle.Thin
        headerStyle.BorderLeft = BorderStyle.Thin
        headerStyle.BorderRight = BorderStyle.Thin
        ' 内容样式（边框+自动换行）
        Dim contentStyle As ICellStyle = workbook.CreateCellStyle()
        contentStyle.BorderTop = BorderStyle.Thin
        contentStyle.BorderBottom = BorderStyle.Thin
        contentStyle.BorderLeft = BorderStyle.Thin
        contentStyle.BorderRight = BorderStyle.Thin
        contentStyle.WrapText = True ' 自动换行

        ' ===== 2. 写入表头 =====
        Dim headerRow As IRow = sheet.CreateRow(0)
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim cell As ICell = headerRow.CreateCell(i)
            cell.SetCellValue(dt.Columns(i).ColumnName)
            cell.CellStyle = headerStyle
            sheet.SetColumnWidth(i, 20 * 256) ' 初始列宽（20字符）
        Next

        ' ===== 3. 写入数据 =====
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim dataRow As IRow = sheet.CreateRow(i + 1)
            dataRow.HeightInPoints = 20 ' 固定行高

            For j As Integer = 0 To dt.Columns.Count - 1
                Dim cell As ICell = dataRow.CreateCell(j)
                cell.SetCellValue(dt.Rows(i)(j).ToString())
                cell.CellStyle = contentStyle

                ' 动态调整列宽（取最长内容的1.2倍）
                'Dim contentLength As Integer = dt.Rows(i)(j).ToString().Length
                'If contentLength > sheet.GetColumnWidth(j) / 256 Then
                '    sheet.SetColumnWidth(j, CInt(contentLength * 1.2) * 256)
                'End If
            Next
        Next
        sheet.SetColumnWidth(0, 6 * 256)
        sheet.SetColumnWidth(1, 45 * 256)
        sheet.SetColumnWidth(2, 6 * 256)
        sheet.SetColumnWidth(3, 15 * 256)
        'sheet.AutoSizeColumn(0)
        'sheet.AutoSizeColumn(1)
        'sheet.AutoSizeColumn(2)
        'sheet.AutoSizeColumn(3)
        ' ===== 4. 保存文件 =====
        Using fs As New FileStream(filePath, FileMode.Create)
            workbook.Write(fs)
        End Using
    End Sub


End Class

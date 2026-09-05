'Imports Autodesk.AutoCAD
Imports System.Runtime.InteropServices
Imports System.IO
Imports NPOI.SS.UserModel
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel
Module Module_Export
    'Function Export_datatable_to_excel(ByVal datatable_temp As DataTable, ByVal workbookname As String, ByVal sheetname As String) As Boolean
    '    Dim wb As HSSFWorkbook = New HSSFWorkbook()
    '    'Dim sheet1 As HSSFSheet = workbook.CreateSheet("sheet1")
    '    Dim sheet1 As HSSFSheet = wb.CreateSheet(sheetname)
    '    Dim font As HSSFFont = wb.CreateFont()
    '    Dim cellStyle As HSSFCellStyle = wb.CreateCellStyle

    '    '//边框    
    '    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center   '//水平对齐    
    '    cellStyle.VerticalAlignment = VerticalAlignment.Center      '//垂直对齐 
    '    cellStyle.WrapText = True  '//自动换行 


    '    font.FontHeightInPoints = 14
    '    'font.FontName = "黑体"
    '    font.Boldweight = FontBoldWeight.Bold
    '    font.IsBold = True
    '    cellStyle.SetFont(font)


    '    Dim row_tablename As HSSFRow
    '    row_tablename = sheet1.CreateRow(0)
    '    row_tablename.Height = 35 * 20
    '    Dim cell_tablename As HSSFCell
    '    cell_tablename = row_tablename.CreateCell(0)
    '    cell_tablename.SetCellValue(sheetname)
    '    cell_tablename.CellStyle = cellStyle
    '    Dim cellRangeAddress As NPOI.SS.Util.CellRangeAddress = New NPOI.SS.Util.CellRangeAddress(0, 0, 0, datatable_temp.Columns.Count - 1)
    '    sheet1.AddMergedRegion(cellRangeAddress)
    '    cell_tablename.CellStyle = cellStyle

    '    font.FontHeightInPoints = 10
    '    'font.FontName = "宋体"
    '    font.Boldweight = FontBoldWeight.Bold
    '    font.IsBold = True
    '    cellStyle.SetFont(font)

    '    Dim row_header As HSSFRow
    '    row_header = sheet1.CreateRow(1)
    '    row_header.Height = 20 * 20
    '    Dim cell_header As HSSFCell

    '    For j = 0 To datatable_temp.Columns.Count - 1
    '        cell_header = row_header.CreateCell(j)
    '        cell_header.SetCellValue(datatable_temp.Columns(j).Caption)
    '        cell_header.CellStyle = cellStyle
    '    Next


    '    font.FontHeightInPoints = 10
    '    'font.FontName = "宋体"
    '    font.Boldweight = FontBoldWeight.None
    '    font.IsBold = False
    '    cellStyle.SetFont(font)
    '    Dim row As HSSFRow
    '    Dim cell As HSSFCell
    '    Dim rowIndex As Integer = 0
    '    For i = 0 To datatable_temp.Rows.Count - 1
    '        row = sheet1.CreateRow(i + 2)
    '        row.Height = 20 * 20
    '        'Dim colIndex As Integer = 0
    '        For j = 0 To datatable_temp.Columns.Count - 1
    '            cell = row.CreateCell(j)
    '            If String.IsNullOrEmpty(datatable_temp.Rows(i)(j).ToString) = False Then
    '                cell.SetCellValue(datatable_temp.Rows(i)(j))
    '                'Else

    '                '    cell.SetCellValue("djl")
    '            End If
    '            cell.CellStyle = cellStyle
    '        Next
    '        'While colIndex <= rowIndex
    '        '    cell = row.CreateCell(colIndex)
    '        '    cell.SetCellValue([String].Format("{0}*{1}={2}", rowIndex + 1, colIndex + 1, (rowIndex + 1) * (colIndex + 1)))
    '        '    System.Math.Max(System.Threading.Interlocked.Increment(colIndex), colIndex - 1)
    '        'End While
    '        'System.Math.Max(System.Threading.Interlocked.Increment(rowIndex), rowIndex - 1)
    '    Next

    '    'Dim file As New FileStream(Path.Combine(System.Windows.Forms.Application.StartupPath, workbookname & ".xls"), FileMode.Create)
    '    Dim file As New FileStream(Path.Combine("d:\", workbookname & ".xls"), FileMode.OpenOrCreate)
    '    wb.Write(file)

    '    file.Close()
    '    Return True
    'End Function




    Function Export_hp_dataset_to_excel(ByVal dataset_temp As DataSet, ByVal workbookname As String, ByVal sheetname As ArrayList) As Boolean
        '保存滑坡数据dataset到excel
        Dim wb As HSSFWorkbook = New HSSFWorkbook()
        'Dim sheet1 As HSSFSheet = workbook.CreateSheet("sheet1")

        Dim font As HSSFFont = wb.CreateFont()
        Dim font_title As HSSFFont = wb.CreateFont()
        Dim cellStyle As HSSFCellStyle = wb.CreateCellStyle
        Dim cellStyle_title As HSSFCellStyle = wb.CreateCellStyle
        '//边框    
        cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center   '//水平对齐    
        cellStyle.VerticalAlignment = VerticalAlignment.Center      '//垂直对齐 
        cellStyle.WrapText = True  '//自动换行 

        font.FontHeightInPoints = 10
        font.FontName = "宋体"
        'font.Boldweight = FontBoldWeight.None
        'font.IsBold = True
        font.IsBold = False

        cellStyle.SetFont(font)

        '//边框    
        cellStyle_title.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle_title.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle_title.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle_title.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin
        cellStyle_title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center   '//水平对齐    
        cellStyle_title.VerticalAlignment = VerticalAlignment.Center      '//垂直对齐 
        cellStyle_title.WrapText = True  '//自动换行 

        font_title.FontHeightInPoints = 14
        font_title.FontName = "宋体"
        'font_title.Boldweight = FontBoldWeight.Bold
        'font_title.IsBold = True

        cellStyle_title.SetFont(font_title)


        

        For ss = 0 To dataset_temp.Tables.Count - 1

            Dim datatable_temp As DataTable = dataset_temp.Tables(ss).Copy

            'Dim K_temp As Double
            'If String.IsNullOrEmpty(datatable_temp.Rows(datatable_temp.Rows.Count - 1)("Fs_current").ToString) = True Then
            '    K_temp = 1.2
            'Else
            '    K_temp = datatable_temp.Rows(datatable_temp.Rows.Count - 1)("Fs_current")
            'End If










            Dim sheet1 As HSSFSheet = wb.CreateSheet(sheetname(ss))
            'sheet1.SetColumnWidth(3, 30 * 256)
            Dim row_tablename As HSSFRow = sheet1.CreateRow(0)

            row_tablename.Height = 35 * 20
            Dim cell_tablename As HSSFCell
            cell_tablename = row_tablename.CreateCell(0)
            'cell_tablename.SetCellValue(sheetname(ss) & "  稳定性及剩余下滑力计算表")
            cell_tablename.SetCellValue(sheetname(ss))

            'cell_tablename.SetCellValue(sheetname(ss) & "  K=" & arr_output.Item(ss))
            'cell_tablename.SetCellValue(sheetname(ss) & "  K=" & K_temp)
            'cell_tablename.SetCellValue(sheetname(ss) & "  稳定性及剩余下滑力计算表")
            'MessageBox.Show(sheetname(ss) & "  稳定性及剩余下滑力计算表")
            cell_tablename.CellStyle = cellStyle_title
            For cn = 0 To datatable_temp.Columns.Count - 1 '设定标题行单元格格式
                'For cn = 0 To datatable_temp.Columns.Count - 7 '设定标题行单元格格式
                'For cn = 0 To datatable_temp.Columns.Count - 6 '设定标题行单元格格式
                'Dim cell_co As HSSFCell
                'Dim row111 As IRow = NPOI.HSSF.Util.HSSFCellUtil.GetRow(0, sheet1)
                'cell_co = NPOI.HSSF.Util.HSSFCellUtil.GetCell(row111, cn)
                'Dim row111 As IRow = NPOI.HSSF.Util.HSSFCellUtil.GetRow(0, sheet1)
                'cell_co = NPOI.HSSF.Util.HSSFCellUtil.GetCell(row111, cn)
                'Dim row111 As IRow = NPOI.HSSF.Util.HSSFCellUtil.GetRow(0, sheet1)
                'cell_co = NPOI.HSSF.Util.HSSFCellUtil.GetCell(row111, cn)
                ' --- 正确获取或创建行 ---
                Dim row111 As IRow = sheet1.GetRow(0)
                If row111 Is Nothing Then
                    row111 = sheet1.CreateRow(0)
                End If

                ' --- 正确获取或创建单元格 ---
                Dim cell_co As ICell = row111.GetCell(cn)
                If cell_co Is Nothing Then
                    cell_co = row111.CreateCell(cn)
                End If
                cell_co.CellStyle = cellStyle_title
            Next

            'Dim cellRangeAddress As NPOI.SS.Util.CellRangeAddress = New NPOI.SS.Util.CellRangeAddress(0, 0, 0, datatable_temp.Columns.Count - 1)
            'Dim cellRangeAddress As NPOI.SS.Util.CellRangeAddress = New NPOI.SS.Util.CellRangeAddress(0, 0, 0, datatable_temp.Columns.Count - 6) '标题单元格合并
            Dim cellRangeAddress As NPOI.SS.Util.CellRangeAddress = New NPOI.SS.Util.CellRangeAddress(0, 0, 0, datatable_temp.Columns.Count - 1) '标题单元格合并
            sheet1.AddMergedRegion(cellRangeAddress)


            sheet1.PrintSetup.Landscape = True '横向打印
            sheet1.PrintSetup.PaperSize = 9 'A4
            sheet1.HorizontallyCenter = True '水平居中打印
            'sheet1.PrintSetup.
            'font.FontHeightInPoints = 10
            'font.FontName = "宋体"
            'font.Boldweight = FontBoldWeight.Bold
            'font.IsBold = True
            'cellStyle.SetFont(font)

            '　　　　//横向打印
            'sheet.PrintSetup.Landscape = true;
            '//纸张大小
            'sheet.PrintSetup.PaperSize = (int)PaperSize.A4_Small;
            '//缩放比：100% 不缩放
            'sheet.PrintSetup.Scale = 100;
            '//不缩放到一页
            'sheet.FitToPage = false;
            '//居中对齐
            'sheet.HorizontallyCenter = true;
            'sheet.VerticallyCenter = true;
            '//设置打印边距，数值为打印设置里的边距设置(厘米)/3
            '这里设置的是窄边距
            sheet1.SetMargin(MarginType.RightMargin, 0.64 / 3)
            sheet1.SetMargin(MarginType.TopMargin, 1.91 / 3)
            sheet1.SetMargin(MarginType.LeftMargin, 0.64 / 3)
            sheet1.SetMargin(MarginType.BottomMargin, 1.91 / 3)
            '打印时将所有列调整为一页，而非将所有行调整调整为一页
            sheet1.PrintSetup.FitWidth = 1
            sheet1.PrintSetup.FitHeight = 0
            'sheet1.SetMargin(MarginType.RightMargin, (double)1.6/3);
            'sheet1.SetMargin(MarginType.TopMargin, (double)0.8/3);
            'sheet1.SetMargin(MarginType.LeftMargin, (double)1.6 / 3);
            'sheet1.SetMargin(MarginType.BottomMargin, (double)0.8 / 3);
            '//设置打印区域
            'workbook.SetPrintArea(1, 0, 5, 0, rowIndex - 1);
            '//设置重复出现的行（表头）
            '//sheet.RepeatingRows = new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 5);

            Dim row_header As HSSFRow
            row_header = sheet1.CreateRow(1)
            'row_header.Height = 20 * 20
            row_header.Height = 55 * 20
            Dim cell_header As HSSFCell

            For j = 0 To datatable_temp.Columns.Count - 1
                cell_header = row_header.CreateCell(j)
                cell_header.SetCellValue(datatable_temp.Columns(j).Caption)
                cell_header.CellStyle = cellStyle
            Next

            'Dim cellRangeAddress2 As NPOI.SS.Util.CellRangeAddress = New NPOI.SS.Util.CellRangeAddress(0, 1, 0, datatable_temp.Columns.Count - 4)
            Dim cellRangeAddress2 As NPOI.SS.Util.CellRangeAddress = New NPOI.SS.Util.CellRangeAddress(0, 1, 0, datatable_temp.Columns.Count - 1)
            '参数:         起始行号, 终止行号, 起始列号, 终止列号
            sheet1.RepeatingRows = cellRangeAddress2



            'font.FontHeightInPoints = 10
            'font.FontName = "宋体"
            'font.Boldweight = FontBoldWeight.None
            'font.IsBold = False
            'cellStyle.SetFont(font)

            Dim row As HSSFRow
            Dim cell As HSSFCell
            Dim rowIndex As Integer = 0
            For i = 0 To datatable_temp.Rows.Count - 1
                row = sheet1.CreateRow(i + 2)
                row.Height = 20 * 20
                'Dim colIndex As Integer = 0
                For j = 0 To datatable_temp.Columns.Count - 1
                    cell = row.CreateCell(j)
                    If String.IsNullOrEmpty(datatable_temp.Rows(i)(j).ToString) = False Then
                        cell.SetCellValue(datatable_temp.Rows(i)(j))
                        'Else

                        '    cell.SetCellValue("djl")
                    End If
                    cell.CellStyle = cellStyle
                Next
        
            Next
            '设置打印区域

            'wb.SetPrintArea(ss, 0, datatable_temp.Columns.Count - 7, 0, datatable_temp.Rows.Count - 1 + 2)
        Next

        'Dim file As New FileStream(Path.Combine(System.Windows.Forms.Application.StartupPath, workbookname & ".xls"), FileMode.Create)
        'Dim file As New FileStream(Path.Combine("d:\", workbookname & ".xls"), FileMode.OpenOrCreate)
        Try
            Dim file As New FileStream(workbookname, FileMode.OpenOrCreate)
            wb.Write(file)

            file.Close()
            Return True
        Catch
            MessageBox.Show("文件" & workbookname & "被占用,保存文件失败！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try


    End Function

    'Function Export_dataset_to_excel_in_onepage(ByVal dataset_temp As DataSet, ByVal workbookname As String, ByVal sheetname As ArrayList) As Boolean
    '    Dim wb As HSSFWorkbook = New HSSFWorkbook()
    '    'Dim sheet1 As HSSFSheet = workbook.CreateSheet("sheet1")

    '    Dim font As HSSFFont = wb.CreateFont()
    '    Dim cellStyle As HSSFCellStyle = wb.CreateCellStyle

    '    '//边框    
    '    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin
    '    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center   '//水平对齐    
    '    cellStyle.VerticalAlignment = VerticalAlignment.Center      '//垂直对齐 
    '    cellStyle.WrapText = True  '//自动换行 


    '    font.FontHeightInPoints = 14
    '    'font.FontName = "黑体"
    '    font.Boldweight = FontBoldWeight.Bold
    '    font.IsBold = True
    '    cellStyle.SetFont(font)


    '    Dim sheet1 As HSSFSheet = wb.CreateSheet("计算书")
    '    Dim count_row As Int32 = 0
    '    For ss = 0 To dataset_temp.Tables.Count - 1

    '        Dim datatable_temp As DataTable = dataset_temp.Tables(ss).Copy

    '        Dim row_tablename As HSSFRow
    '        row_tablename = sheet1.CreateRow(count_row + 0)
    '        row_tablename.Height = 35 * 20
    '        Dim cell_tablename As HSSFCell
    '        cell_tablename = row_tablename.CreateCell(0)
    '        cell_tablename.SetCellValue(sheetname(ss))
    '        cell_tablename.CellStyle = cellStyle
    '        Dim cellRangeAddress As NPOI.SS.Util.CellRangeAddress = New NPOI.SS.Util.CellRangeAddress(count_row, count_row, 0, datatable_temp.Columns.Count - 1)
    '        sheet1.AddMergedRegion(cellRangeAddress)
    '        cell_tablename.CellStyle = cellStyle

    '        font.FontHeightInPoints = 10
    '        'font.FontName = "宋体"
    '        font.Boldweight = FontBoldWeight.Bold
    '        font.IsBold = True
    '        cellStyle.SetFont(font)

    '        Dim row_header As HSSFRow
    '        row_header = sheet1.CreateRow(1)
    '        row_header.Height = 20 * 20
    '        Dim cell_header As HSSFCell

    '        For j = 0 To datatable_temp.Columns.Count - 1
    '            cell_header = row_header.CreateCell(j)
    '            cell_header.SetCellValue(datatable_temp.Columns(j).Caption)
    '            cell_header.CellStyle = cellStyle
    '        Next


    '        font.FontHeightInPoints = 10
    '        'font.FontName = "宋体"
    '        font.Boldweight = FontBoldWeight.None
    '        font.IsBold = False
    '        cellStyle.SetFont(font)
    '        Dim row As HSSFRow
    '        Dim cell As HSSFCell
    '        Dim rowIndex As Integer = 0
    '        count_row = count_row + 1
    '        For i = 0 To datatable_temp.Rows.Count - 1
    '            row = sheet1.CreateRow(count_row + i + 2)
    '            row.Height = 20 * 20
    '            'Dim colIndex As Integer = 0
    '            For j = 0 To datatable_temp.Columns.Count - 1
    '                cell = row.CreateCell(j)
    '                If String.IsNullOrEmpty(datatable_temp.Rows(i)(j).ToString) = False Then
    '                    cell.SetCellValue(datatable_temp.Rows(i)(j))
    '                    'Else

    '                    '    cell.SetCellValue("djl")
    '                End If
    '                cell.CellStyle = cellStyle
    '            Next
    '            'While colIndex <= rowIndex
    '            '    cell = row.CreateCell(colIndex)
    '            '    cell.SetCellValue([String].Format("{0}*{1}={2}", rowIndex + 1, colIndex + 1, (rowIndex + 1) * (colIndex + 1)))
    '            '    System.Math.Max(System.Threading.Interlocked.Increment(colIndex), colIndex - 1)
    '            'End While
    '            'System.Math.Max(System.Threading.Interlocked.Increment(rowIndex), rowIndex - 1)
    '        Next

    '    Next

    '    'Dim file As New FileStream(Path.Combine(System.Windows.Forms.Application.StartupPath, workbookname & ".xls"), FileMode.Create)
    '    Dim file As New FileStream(Path.Combine("d:\", workbookname & ".xls"), FileMode.OpenOrCreate)
    '    wb.Write(file)

    '    file.Close()
    '    Return True
    'End Function
End Module

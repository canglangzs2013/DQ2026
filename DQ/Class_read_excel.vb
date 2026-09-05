Imports System.IO
Imports NPOI.SS.UserModel
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel
Public Class Class_read_excel
    '/// <summary>  
    '/// 读取Excel保存为datatable  
    '/// </summary>  
    '/// <param name="filePath">Excel文件路径</param>  
    '/// <param name="startRow">第几行开始读取</param>  
    '/// <returns></returns>  
    Public Function DoReadExcelDataTable(ByVal filePath As String, ByVal sheet_id As Integer, ByVal dt_input As DataTable) As DataTable
        'Try
        'Dim dt As DataTable = New DataTable()
        Dim dt As DataTable = dt_input.Clone

            'If (!File.Exists(filePath)) Then
            '{  
            '    return dt;  
            '}  

            If File.Exists(filePath) = False Then
                Return dt
            End If

            Dim workbook As HSSFWorkbook = Nothing


            Dim sheet As HSSFSheet = Nothing


            Dim fs As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)


            workbook = New HSSFWorkbook(fs)


            sheet = workbook.GetSheetAt(sheet_id)





            Dim rowCount As Integer = sheet.LastRowNum


            'for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            'For i = sheet.FirstRowNum + 2 To sheet.LastRowNum
            For i = sheet.FirstRowNum + 2 To sheet.LastRowNum

                Dim row As HSSFRow = sheet.GetRow(i)
                Dim dtrow As DataRow = dt.NewRow()

                Dim cellCount As Integer = row.LastCellNum
                For j = row.FirstCellNum To cellCount - 1
                    'for (int j = row.FirstCellNum; j < cellCount; j++)  
                    'If IsNothing(row.GetCell(j)) = False Then
                    'if (row.GetCell(j) != null)  
                    'dtrow[j] = row.GetCell(j).ToString();

                    If String.IsNullOrEmpty(row.GetCell(j).ToString) = False Then
                        dtrow(j) = row.GetCell(j).ToString()
                    End If


                Next

                dt.Rows.Add(dtrow)
            Next
            sheet = Nothing


            workbook = Nothing


            Return dt
        'Catch
        '    Return Nothing
        'End Try
    End Function


    Public Function get_sheet_number(ByVal filePath As String) As Integer
        'Dim dt As DataTable = New DataTable()
        'Dim dt As DataTable = dt_input.Clone

        'If (!File.Exists(filePath)) Then
        '{  
        '    return dt;  
        '}  

        If File.Exists(filePath) = False Then
            Return 0
        End If

       

        Try
            Dim workbook As HSSFWorkbook = Nothing
            Dim sheet As HSSFSheet = Nothing
            Dim fs As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)
            workbook = New HSSFWorkbook(fs)

            Return workbook.NumberOfSheets

            sheet = Nothing
            workbook = Nothing
        Catch
            MessageBox.Show("文件" & filePath & "可能格式不正确或被占用，打开失败！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return 0
        End Try




        'Dim row1 As HSSFRow = sheet.GetRow(startRow)


        'Dim cellCount As Integer = row1.LastCellNum


        '//此处是读取列名的，如果不需要列名则注释此代码  
        'for (int i = row1.FirstCellNum; i < row1.LastCellNum; i++)  
        '{  
        '    DataColumn columItem = new DataColumn(row1.GetCell(i).StringCellValue);  
        '    dt.Columns.Add(columItem);  
        '}  



        



    End Function


    Public Function DoReadExcelDataTable_cell(ByVal filePath As String, ByVal sheet_id As Integer, ByVal dt_input As DataTable) As String
        Try
            'Dim dt As DataTable = New DataTable()
            Dim dt As DataTable = dt_input.Clone

            'If (!File.Exists(filePath)) Then
            '{  
            '    return dt;  
            '}  

            If File.Exists(filePath) = False Then
                Return "K=1"
            End If

            Dim workbook As HSSFWorkbook = Nothing


            Dim sheet As HSSFSheet = Nothing


            Dim fs As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)


            workbook = New HSSFWorkbook(fs)


            sheet = workbook.GetSheetAt(sheet_id)





            Dim rowCount As Integer = sheet.LastRowNum


            Dim k As String
            Dim row As HSSFRow = sheet.GetRow(0)

            If String.IsNullOrEmpty(row.GetCell(0).ToString) = False Then
                k = row.GetCell(0).ToString()
            Else
                Return "K=1"
            End If


            Return k


            sheet = Nothing


            workbook = Nothing



        Catch
            MessageBox.Show("DoReadExcelDataTable_cell错误！")
            Return Nothing
        End Try
    End Function


End Class

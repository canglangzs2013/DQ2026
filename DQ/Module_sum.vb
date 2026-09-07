Imports NPOI.SS.Formula.Functions
Imports System.Linq
Imports System.Data

Module Module_sum


    ''' <summary>
    ''' 通用分组汇总函数
    ''' </summary>
    ''' <param name="dtSource">源DataTable</param>
    ''' <param name="groupColName">分组列名（列1，例如：墙高）</param>
    ''' <param name="sumColName">求和列名（列2，例如：混凝土量）</param>
    ''' <returns>新DataTable，包含2列：分组列、求和列</returns>
    Public Function GroupSumTwoCol(dtSource As DataTable, groupColName As String, sumColName As String) As DataTable
        ' 空校验
        If dtSource Is Nothing OrElse dtSource.Rows.Count = 0 Then
            Dim dtEmpty As New DataTable()
            dtEmpty.Columns.Add(groupColName, dtSource.Columns(groupColName).DataType)
            dtEmpty.Columns.Add(sumColName, dtSource.Columns(sumColName).DataType)
            Return dtEmpty
        End If

        ' 新建结果表，只有【分组列】和【求和列】
        Dim dtResult As New DataTable()
        dtResult.Columns.Add(groupColName, dtSource.Columns(groupColName).DataType)
        dtResult.Columns.Add(sumColName, dtSource.Columns(sumColName).DataType)

        ' LINQ分组，按groupColName分组，对sumColName求和
        Dim groups = dtSource.AsEnumerable().GroupBy(Function(r) r(groupColName))

        For Each g In groups
            Dim dr As DataRow = dtResult.NewRow()
            dr(groupColName) = g.Key

            ' Decimal高精度求和，DBNull当做0
            Dim total As Decimal = g.Sum(Function(r) If(IsDBNull(r(sumColName)), 0D, Convert.ToDecimal(r(sumColName))))
            dr(sumColName) = total

            dtResult.Rows.Add(dr)
        Next

        Return dtResult
    End Function
End Module

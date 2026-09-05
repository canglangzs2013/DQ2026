Imports NPOI.SS.Formula.Functions

Module Module_sum
    Public Function SumByA(dt As DataTable) As DataTable

        ' 新表结构
        Dim result As New DataTable()
        result.Columns.Add("dh", GetType(Double))
        result.Columns.Add("L", GetType(Double))
        result.Columns("dh").Caption = "挡墙高度dh"
        result.Columns("L").Caption = "挡墙长度L"
        ' 按 A 分组并求和
        Dim query =
        From row As DataRow In dt.Rows
        Group row By key = CDbl(row("dh")) Into Group
        Order By key Ascending
        Select New With {
            .A = key,
            .B_Sum = Group.Sum(Function(r) CDbl(r("L")))
        }


        ' 填充新表
        For Each item In query
            result.Rows.Add(item.A, item.B_Sum)
        Next

        Return result
    End Function
End Module

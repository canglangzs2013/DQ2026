Module Module_del
    Public myexcelHelper As New ExcelHelper()

    Public min_dq_height As Decimal = 2 '最小挡墙深度
    Public step_cal As Decimal = 0.5 '插入剖分线的计算步长（亦即剖分线步长）
    'Public dq_‌foundation_height As Double = 0.5 '计算步长
    Public min_threshold As Decimal = 1 '，阈值,为按步长 step_cal插入剖分线，插入时不能距离已有线太近 min_dq_length
    'Public dq_‌foundation_height As Double = 0.5 '计算步长
    Public min_segment_length As Decimal = 2

    Public dq_difference_height_base As Decimal = 1 '挡墙基底高差的阈值

    ' 先定义结构体（放模块/窗体通用区）
    Structure MatParam
        Dim foundation_height As Decimal   '密度
        Dim name As String   '弹性模量
    End Structure
    '在模块级别定义（和 Structure 放在一起即可）
    Public dq_Map As New Dictionary(Of Decimal, MatParam) From {
    {2D, New MatParam With {.foundation_height = 1, .name = "不挂网"}},
    {3D, New MatParam With {.foundation_height = 1, .name = "不挂网"}},
    {4.5D, New MatParam With {.foundation_height = 1.5, .name = "Φ6@150×150"}},
    {5.5D, New MatParam With {.foundation_height = 1.5, .name = "Φ6@200×200"}},
    {6.5D, New MatParam With {.foundation_height = 1.5, .name = "Φ8@150×150"}},
    {7.5D, New MatParam With {.foundation_height = 1.5, .name = "Φ8@200×200"}},
    {8.5D, New MatParam With {.foundation_height = 1.5, .name = "Φ8@200×200"}},
    {9.5D, New MatParam With {.foundation_height = 2, .name = "Φ8@200×200"}},
    {10.5D, New MatParam With {.foundation_height = 2, .name = "Φ8@200×200"}},
    {11D, New MatParam With {.foundation_height = 2, .name = "Φ8@200×200"}},
    {11.5D, New MatParam With {.foundation_height = 2, .name = "Φ8@200×200"}},
    {12.5D, New MatParam With {.foundation_height = 2, .name = "Φ8@200×200"}},
    {13.5D, New MatParam With {.foundation_height = 2, .name = "Φ8@200×200"}},
    {14D, New MatParam With {.foundation_height = 2, .name = "Φ8@200×200"}}
}

    ''' <summary>
    ''' 输入h，在dq_Map中找到第一个 Key>h 且满足 h+foundation_height<=Key 的Key
    ''' </summary>
    ''' <param name="h">输入值（Decimal类型）</param>
    ''' <returns>第一个满足条件的Key；若无满足条件则返回最大Key</returns>
    Public Function GetFirstCeilingKey(h As Decimal) As Decimal
        For Each kvp In dq_Map  ' Dictionary 插入顺序即升序，无需再 OrderBy
            If kvp.Key <= h Then Continue For

            ' 核心判断：Decimal 类型绝对精确，不需要任何容差（如 1E-9）
            ' 注意：需要将 foundation_height 也转换为 Decimal 参与计算
            If h + CDec(kvp.Value.foundation_height) <= kvp.Key Then
                Return kvp.Key
            End If
        Next

        ' 没有任何 Key>h 满足条件，兜底返回最大 Key
        Return dq_Map.Keys.Max()
    End Function
    Public Function GetRowIndexByX_Binary(dt As DataTable, targetX As Decimal) As Integer
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return -1
        End If
        Dim left As Integer = 0
        Dim right As Integer = dt.Rows.Count - 1
        While left <= right
            Dim mid = (left + right) \ 2
            Dim r As DataRow = dt.Rows(mid)
            If r.IsNull("x") Then
                ' 存在空值，直接跳过（x无重复递增，正常不会进这里）
                right = mid - 1
                Continue While
            End If
            Dim xVal As Decimal = Convert.ToDecimal(r("x"))
            If Decimal.Abs(xVal - targetX) < 0.000000001D Then
                ' 找到
                Return mid
            ElseIf xVal < targetX Then
                left = mid + 1
            Else
                right = mid - 1
            End If
        End While
        ' 未找到
        Return -1
    End Function

    'Public Function GetRowIndexByX_Binary(dt As DataTable, targetX As Double) As Integer
    '    If dt Is Nothing OrElse dt.Rows.Count = 0 Then
    '        Return -1
    '    End If

    '    Dim left As Integer = 0
    '    Dim right As Integer = dt.Rows.Count - 1

    '    While left <= right
    '        Dim mid = (left + right) \ 2
    '        Dim r As DataRow = dt.Rows(mid)

    '        If r.IsNull("x") Then
    '            ' 存在空值，直接跳过（你说x无重复递增，正常不会进这里）
    '            right = mid - 1
    '            Continue While
    '        End If

    '        Dim xVal As Double = Convert.ToDouble(r("x"))
    '        If Math.Abs(xVal - targetX) < 0.000000001 Then
    '            ' 找到
    '            Return mid
    '        ElseIf xVal < targetX Then
    '            left = mid + 1
    '        Else
    '            right = mid - 1
    '        End If
    '    End While

    '    ' 未找到
    '    Return -1
    'End Function

End Module

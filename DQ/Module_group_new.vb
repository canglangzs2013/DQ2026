Module Module_group_new
    ''' <summary>
    ''' 组内pfxdqdy1极差阈值顺序贪心分组；ID含"*"强制新开一组；
    ''' 组内统一记录 min/max 到 min_y1_group / max_y1_group
    ''' </summary>
    Function GroupByPfxdqdy1RangeFixed(ByVal dtTemplate As DataTable, Optional threshold As Double = 1) As DataTable
        If dtTemplate Is Nothing OrElse dtTemplate.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim dtOut As DataTable = dtTemplate.Clone()
        If Not dtOut.Columns.Contains("group") Then dtOut.Columns.Add("group", GetType(Integer))
        If Not dtOut.Columns.Contains("min_pfxdqdy1_group") Then dtOut.Columns.Add("min_pfxdqdy1_group", GetType(Double))
        If Not dtOut.Columns.Contains("min_y1_group") Then dtOut.Columns.Add("min_y1_group", GetType(Double))
        If Not dtOut.Columns.Contains("max_y1_group") Then dtOut.Columns.Add("max_y1_group", GetType(Double))

        For Each r As DataRow In dtTemplate.Rows
            dtOut.ImportRow(r)
        Next

        '===== 阶段1：仅划分group编号 =====
        Dim groupId As Integer = 0
        Dim curGroupMin As Double
        Dim curGroupMax As Double

        Dim r0 As DataRow = dtOut.Rows(0)
        Dim v0 As Double = CDbl(r0("pfxdqdy1"))
        curGroupMin = v0
        curGroupMax = v0
        r0("group") = groupId

        For i As Integer = 1 To dtOut.Rows.Count - 1
            Dim rowCur As DataRow = dtOut.Rows(i)
            Dim valCur As Double = CDbl(rowCur("pfxdqdy1"))

            Dim tempMin As Double = Math.Min(curGroupMin, valCur)
            Dim tempMax As Double = Math.Max(curGroupMax, valCur)

            'ID含"*"强制新开一组
            Dim mustSplit As Boolean = rowCur("ID").ToString().Contains("*")

            If mustSplit OrElse (tempMax - tempMin) >= threshold Then
                groupId += 1
                curGroupMin = valCur
                curGroupMax = valCur
            Else
                curGroupMin = tempMin
                curGroupMax = tempMax
            End If
            rowCur("group") = groupId
        Next

        '===== 阶段2：按group聚合最小值与最大值 =====
        Dim dictGroupMin As New Dictionary(Of Integer, Double)
        Dim dictGroupMax As New Dictionary(Of Integer, Double)
        Dim distinctGroups = dtOut.AsEnumerable().Select(Function(r) CInt(r("group"))).Distinct().ToList()

        For Each gNo As Integer In distinctGroups
            Dim subRows = dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
            dictGroupMin(gNo) = subRows.Min(Function(r) CDbl(r("pfxdqdy1")))
            dictGroupMax(gNo) = subRows.Max(Function(r) CDbl(r("pfxdqdy1")))
        Next

        '===== 阶段3：回写统一最小值/最大值 + ID追加"min" =====
        For Each row As DataRow In dtOut.Rows
            Dim g As Integer = CInt(row("group"))
            row("min_pfxdqdy1_group") = dictGroupMin(g)


            If CDbl(row("pfxdqdy1")) = dictGroupMin(g) Then
                Dim id As String = row("ID").ToString()
                If Not id.EndsWith("min") Then
                    row("ID") = id & "min"
                    'row("dqdh_pre") = row("dqdy2") - row("min_pfxdqdy1_group")
                    row("min_y1_group") = dictGroupMin(g)      '【新增】组内pfxdqdy1最小值
                    row("max_y1_group") = dictGroupMax(g)      '【新增】组内pfxdqdy1最大值
                End If
            End If
        Next

        '===== 阶段5：计算并回写 dqdh / dqdy1 =====
        For Each gNo As Integer In distinctGroups
            Dim firstMinRow As DataRow = dtOut.AsEnumerable() _
            .Where(Function(r) CInt(r("group")) = gNo AndAlso r("ID").ToString().EndsWith("min")) _
            .FirstOrDefault()

            If firstMinRow IsNot Nothing Then
                Dim dqdy2Val As Double
                If dtOut.Columns.Contains("dqdy2") AndAlso Not IsDBNull(firstMinRow("dqdy2")) Then
                    dqdy2Val = CDbl(firstMinRow("dqdy2"))
                Else
                    dqdy2Val = CDbl(firstMinRow("pfxdqdy2"))
                End If

                Dim minG As Double = CDbl(firstMinRow("min_pfxdqdy1_group"))
                'Dim finalVal As Double = Math.Ceiling(dqdy2Val - minG)
                Dim finalVal As Double = dqdy2Val - minG '因为分组前pfxdqdy1已经考虑了向上取整，所以这里不再需要 ceiling，直接用原始差值即可
                'Dim finalVal As Double = dqdy2Val - minG
                For Each row As DataRow In dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
                    row("dqdh") = finalVal
                    row("dqdy1") = row("dqdy2") - row("dqdh")
                Next
            End If
        Next

        Return dtOut
    End Function

    '''' <summary>
    '''' 与 GroupByPfxdqdy1RangeFixed 类似，但分组基准列改为 pfxdqdh：
    ''''   1. 分组前对 pfxdqdh 向上取整为整数，保证不同挡墙高度(1m/2m/3m)绝不混在同一组
    ''''   2. 比较运算符用 ">=" 而不是 ">"，避免 ceiling 差恰为 1 的临界塌陷
    ''''   3. 输出列名加 dh_ 前缀以示区别（与原函数并存不冲突）
    '''' 适用场景：希望"挡墙高度 1m 和 2m 和 3m 必须分到不同组"的工程量统计
    '''' </summary>
    'Function GroupByPfxdqdhCeiling(ByVal dtTemplate As DataTable, Optional threshold As Double = 1.0) As DataTable
    '    If dtTemplate Is Nothing OrElse dtTemplate.Rows.Count = 0 Then
    '        Return Nothing
    '    End If

    '    Dim dtOut As DataTable = dtTemplate.Clone()
    '    If Not dtOut.Columns.Contains("group") Then dtOut.Columns.Add("group", GetType(Integer))
    '    If Not dtOut.Columns.Contains("min_pfxdqdh_group") Then dtOut.Columns.Add("min_pfxdqdh_group", GetType(Double))
    '    If Not dtOut.Columns.Contains("min_dh_group") Then dtOut.Columns.Add("min_dh_group", GetType(Double))
    '    If Not dtOut.Columns.Contains("max_dh_group") Then dtOut.Columns.Add("max_dh_group", GetType(Double))

    '    For Each r As DataRow In dtTemplate.Rows
    '        dtOut.ImportRow(r)
    '    Next

    '    '===== 阶段1：仅划分group编号 =====
    '    ' 分组基准为 Math.Ceiling(pfxdqdh)，保证整数差>=1时必开新组
    '    Dim groupId As Integer = 0
    '    Dim curGroupMin As Double
    '    Dim curGroupMax As Double

    '    Dim r0 As DataRow = dtOut.Rows(0)
    '    Dim v0 As Double = CDbl(r0("pfxdqdh"))
    '    curGroupMin = v0
    '    curGroupMax = v0
    '    r0("group") = groupId

    '    For i As Integer = 1 To dtOut.Rows.Count - 1
    '        Dim rowCur As DataRow = dtOut.Rows(i)
    '        ' 关键：在分组前对当前值向上取整（用 ceiling 后的整数比较）
    '        Dim valCur As Double = CDbl(rowCur("pfxdqdh"))

    '        Dim tempMin As Double = Math.Min(curGroupMin, valCur)
    '        Dim tempMax As Double = Math.Max(curGroupMax, valCur)

    '        'ID含"*"强制新开一组
    '        Dim mustSplit As Boolean = rowCur("ID").ToString().Contains("*")

    '        ' 关键：用 ">=" 而不是 ">"
    '        '   整数 ceiling 差=1 时，">1" 不拆，">=1" 拆（保证不同高度绝不混组）
    '        If mustSplit OrElse (tempMax - tempMin) >= threshold Then
    '            groupId += 1
    '            curGroupMin = valCur
    '            curGroupMax = valCur
    '        Else
    '            curGroupMin = tempMin
    '            curGroupMax = tempMax
    '        End If
    '        rowCur("group") = groupId
    '    Next

    '    '===== 阶段2：按group聚合最小值与最大值（用原始 pfxdqdh 数值） =====
    '    Dim dictGroupMin As New Dictionary(Of Integer, Double)
    '    Dim dictGroupMax As New Dictionary(Of Integer, Double)
    '    Dim distinctGroups = dtOut.AsEnumerable().Select(Function(r) CInt(r("group"))).Distinct().ToList()

    '    For Each gNo As Integer In distinctGroups
    '        Dim subRows = dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
    '        ' 用原始 pfxdqdh 聚合 min/max（保留真实数值，方便后续读出"该组挡墙高度最小值"）
    '        dictGroupMin(gNo) = subRows.Min(Function(r) CDbl(r("pfxdqdh")))
    '        dictGroupMax(gNo) = subRows.Max(Function(r) CDbl(r("pfxdqdh")))
    '    Next

    '    '===== 阶段3：回写统一最小值/最大值 + ID追加"min" =====
    '    For Each row As DataRow In dtOut.Rows
    '        Dim g As Integer = CInt(row("group"))
    '        row("min_pfxdqdh_group") = dictGroupMin(g)

    '        If CDbl(row("pfxdqdh")) = dictGroupMin(g) Then
    '            Dim id As String = row("ID").ToString()
    '            If Not id.EndsWith("min") Then
    '                row("ID") = id & "min"
    '                row("dqdh_pre") = row("dqdy2") - row("min_pfxdqdh_group")
    '                row("min_dh_group") = dictGroupMin(g)      '【新增】组内 pfxdqdh 最小值
    '                row("max_dh_group") = dictGroupMax(g)      '【新增】组内 pfxdqdh 最大值
    '            End If
    '        End If
    '    Next

    '    '===== 阶段4：计算并回写 dqdh / dqdy1 =====
    '    For Each gNo As Integer In distinctGroups
    '        Dim firstMinRow As DataRow = dtOut.AsEnumerable() _
    '        .Where(Function(r) CInt(r("group")) = gNo AndAlso r("ID").ToString().EndsWith("min")) _
    '        .FirstOrDefault()

    '        If firstMinRow IsNot Nothing Then
    '            Dim dqdy2Val As Double
    '            If dtOut.Columns.Contains("dqdy2") AndAlso Not IsDBNull(firstMinRow("dqdy2")) Then
    '                dqdy2Val = CDbl(firstMinRow("dqdy2"))
    '            Else
    '                dqdy2Val = CDbl(firstMinRow("pfxdqdy2"))
    '            End If

    '            Dim minG As Double = CDbl(firstMinRow("min_pfxdqdh_group"))
    '            Dim finalVal As Double = dqdy2Val - minG
    '            For Each row As DataRow In dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
    '                row("dqdh") = finalVal
    '                row("dqdy1") = row("dqdy2") - row("dqdh")
    '            Next
    '        End If
    '    Next

    '    Return dtOut
    'End Function
End Module

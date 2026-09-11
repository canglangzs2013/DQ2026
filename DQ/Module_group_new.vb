Imports System.Data

Module Module_group_new

    ''' <summary>
    ''' 改进的按 pfxdqdy1 分组：
    ''' 1) 先按阈值 threshold 做顺序贪心分组（ID 含 "*" 强制新组）
    ''' 2) 在每次准备结束当前组时，若当前组宽度 < minSegmentLength，则继续向后吸收下一行（临时忽略高度阈值），
    '''    直到组宽 >= minSegmentLength 或遇到受保护行（ID 包含 "*"）或到数据末尾。
    ''' 3) 吸收完成后计算每组的 min_pfxdqdy1_group、dqdy1、dqdh，并把 group 压缩为 0,1,2...
    ''' </summary>
    Public Function GroupByPfxdqdy1RangeFixed(ByVal dtTemplate As DataTable, Optional threshold As Double = 1, Optional minSegmentLength As Double = 2) As DataTable
        If dtTemplate Is Nothing OrElse dtTemplate.Rows.Count = 0 Then
            Return Nothing
        End If
        'MessageBox.Show(minSegmentLength)
        ' 获得默认最小段长
        If minSegmentLength <= 0 Then
            Try
                minSegmentLength = Module_del.min_segment_length
            Catch
                minSegmentLength = 0
            End Try
        End If
        Dim dtOut As DataTable = dtTemplate.Clone()
        If Not dtOut.Columns.Contains("group") Then dtOut.Columns.Add("group", GetType(Integer))
        If Not dtOut.Columns.Contains("min_pfxdqdy1_group") Then dtOut.Columns.Add("min_pfxdqdy1_group", GetType(Double))
        If Not dtOut.Columns.Contains("min_y1_group") Then dtOut.Columns.Add("min_y1_group", GetType(Double))
        If Not dtOut.Columns.Contains("max_y1_group") Then dtOut.Columns.Add("max_y1_group", GetType(Double))
        For Each r As DataRow In dtTemplate.Rows
            dtOut.ImportRow(r)
        Next
        ' 辅助：安全取 x 与 pfxdqdy1
        Dim SafeDouble = Function(obj As Object) As Double
                             If obj Is Nothing OrElse IsDBNull(obj) Then Return Double.NaN
                             Dim d As Double
                             If Double.TryParse(obj.ToString(), d) Then Return d
                             Return Double.NaN
                         End Function
        Dim rowCount As Integer = dtOut.Rows.Count
        ' ===== 阶段1：顺序贪心分组 + 向后吸收保证最小宽度（*强制分割优先级高于最小段长） =====
        Dim groupId As Integer = 0
        Dim startIdx As Integer = 0
        Dim curMin As Double = SafeDouble(dtOut.Rows(0)("pfxdqdy1"))
        Dim curMax As Double = curMin
        dtOut.Rows(0)("group") = groupId
        Dim i As Integer = 1
        While i < rowCount
            Dim valCur As Double = SafeDouble(dtOut.Rows(i)("pfxdqdy1"))
            Dim mustSplit As Boolean = False
            If Not IsDBNull(dtOut.Rows(i)("ID")) Then
                If dtOut.Rows(i)("ID").ToString().Contains("*") Then mustSplit = True
            End If

            Dim tempMin As Double = Math.Min(curMin, valCur)
            Dim tempMax As Double = Math.Max(curMax, valCur)
            Dim shouldSplitByHeight As Boolean = (tempMax - tempMin) >= threshold

            If mustSplit OrElse shouldSplitByHeight Then
                If mustSplit Then
                    'ID带*强制分割，跳过最小段长逻辑，直接新建分组，允许段不足minSegmentLength
                    groupId += 1
                    startIdx = i
                    curMin = valCur
                    curMax = valCur
                    dtOut.Rows(i)("group") = groupId
                    i += 1
                Else
                    '高度差触发拆分，执行向后吸收补齐最小段长
                    Dim xStart = SafeDouble(dtOut.Rows(startIdx)("x"))
                    Dim xEnd = SafeDouble(dtOut.Rows(i - 1)("x"))
                    Dim width As Double = If(Double.IsNaN(xStart) OrElse Double.IsNaN(xEnd), 0.0, xEnd - xStart)

                    If width < minSegmentLength Then
                        Dim canAbsorb As Boolean = True
                        Dim j As Integer = i
                        While j < rowCount AndAlso width < minSegmentLength AndAlso canAbsorb
                            '向后吸收不能跨过带*强制分割行
                            If Not IsDBNull(dtOut.Rows(j)("ID")) AndAlso dtOut.Rows(j)("ID").ToString().Contains("*") Then
                                canAbsorb = False
                                Exit While
                            End If
                            curMin = Math.Min(curMin, SafeDouble(dtOut.Rows(j)("pfxdqdy1")))
                            curMax = Math.Max(curMax, SafeDouble(dtOut.Rows(j)("pfxdqdy1")))
                            xEnd = SafeDouble(dtOut.Rows(j)("x"))
                            width = If(Double.IsNaN(xStart) OrElse Double.IsNaN(xEnd), 0.0, xEnd - xStart)

                            If width >= minSegmentLength Then
                                Exit While
                            End If
                            j += 1
                        End While

                        If width >= minSegmentLength Then
                            For k As Integer = i To j
                                dtOut.Rows(k)("group") = groupId
                            Next
                            i = j + 1
                            Continue While
                        End If
                        '吸收失败：fall‑through往下执行拆组，接受短段
                    End If
                    '执行高度触发拆组
                    groupId += 1
                    startIdx = i
                    curMin = valCur
                    curMax = valCur
                    dtOut.Rows(i)("group") = groupId
                    i += 1
                End If
            Else
                '不拆组：把当前行加入当前组
                dtOut.Rows(i)("group") = groupId
                curMin = tempMin
                curMax = tempMax
                i += 1
            End If
        End While
        ' ===== 阶段2：按 group 聚合最小/最大 pfx 并回写 dqdy1/dqdh =====
        Dim distinctGroups = dtOut.AsEnumerable().Select(Function(r) CInt(r("group"))).Distinct().OrderBy(Function(x) x).ToList()
        Dim dictGroupMin As New Dictionary(Of Integer, Double)
        Dim dictGroupMax As New Dictionary(Of Integer, Double)
        For Each gNo As Integer In distinctGroups
            Dim subRows = dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
            If subRows.Any() Then
                dictGroupMin(gNo) = subRows.Min(Function(r) CDbl(r("pfxdqdy1")))
                dictGroupMax(gNo) = subRows.Max(Function(r) CDbl(r("pfxdqdy1")))
            End If
        Next
        For Each row As DataRow In dtOut.Rows
            Dim g As Integer = CInt(row("group"))
            If dictGroupMin.ContainsKey(g) Then
                row("min_pfxdqdy1_group") = dictGroupMin(g)
                row("min_y1_group") = dictGroupMin(g)
                row("max_y1_group") = dictGroupMax(g)
                If CDbl(row("pfxdqdy1")) = dictGroupMin(g) Then
                    Dim id As String = If(IsDBNull(row("ID")), String.Empty, row("ID").ToString())
                    If Not id.EndsWith("min") Then
                        row("ID") = id & "min"
                    End If
                End If
            End If
        Next
        ' 统一组内 dqdh / dqdy1：用组的 min_pfxdqdy1_group 与行的 dqdy2 计算 dqdh（一组统一高度）
        For Each gNo As Integer In distinctGroups
            Dim firstMinRow As DataRow = dtOut.AsEnumerable() _
            .Where(Function(r) CInt(r("group")) = gNo AndAlso Not IsDBNull(r("min_pfxdqdy1_group"))) _
            .FirstOrDefault()
            If firstMinRow IsNot Nothing Then
                Dim dqdy2Val As Double
                If dtOut.Columns.Contains("dqdy2") AndAlso Not IsDBNull(firstMinRow("dqdy2")) Then
                    dqdy2Val = CDbl(firstMinRow("dqdy2"))
                Else
                    dqdy2Val = CDbl(firstMinRow("pfxdqdy2"))
                End If
                Dim minG As Double = CDbl(firstMinRow("min_pfxdqdy1_group"))
                Dim finalVal As Double = dqdy2Val - minG
                'Dim finalVal As Double = GetFirstCeilingKey(dqdy2Val - minG)'不可犯错
                For Each row As DataRow In dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
                    row("dqdh") = finalVal
                    row("dqdy1") = row("dqdy2") - row("dqdh")
                    row("dq_‌foundation_height") = dq_Map(finalVal).foundation_height
                    row("h_plus_f") = dq_Map(finalVal).foundation_height + row("h")
                Next
            End If
        Next
        ' 最后把 group 编号压缩为 0,1,2...
        Dim map As New Dictionary(Of Integer, Integer)
        Dim nextId As Integer = 0
        For idx As Integer = 0 To dtOut.Rows.Count - 1
            Dim g = CInt(dtOut.Rows(idx)("group"))
            If Not map.ContainsKey(g) Then
                map.Add(g, nextId)
                nextId += 1
            End If
            dtOut.Rows(idx)("group") = map(g)
        Next
        Return dtOut
    End Function

    ' 新增：合并相邻且完全相同的组（按 dqdh、dqdy1、dqdy2 判断）
    ''' <summary>
    ''' 把 dt 中相邻的两个组合并，当且仅当这两个组的 dqdh、dqdy1、dqdy2 三个值都相等（允许浮点微小误差）。
    ''' 不修改原分组函数；就地修改并返回同一个 DataTable 引用。
    ''' 参数：
    '''   dt - 已经分组并按 x 升序的 DataTable（必须包含列 "group","dqdh","dqdy1","dqdy2"）
    '''   tolerance - 比较浮点相等时的容差，默认为 1e-9（可设为 0 强制精确相等）
    ''' </summary>
    Public Function MergeAdjacentEqualGroups(ByVal dt As DataTable, Optional ByVal tolerance As Double = 0.000000001, Optional ByVal renumberGroups As Boolean = True) As DataTable
        ' 仅修改此函数：当且仅当相邻两行的 dqdh、dqdy1、dqdy2 在容差内都相等，则删除第二行（后一组）
        If dt Is Nothing OrElse dt.Rows.Count <= 1 Then Return dt

        ' 必要列检查
        If Not dt.Columns.Contains("dqdh") OrElse Not dt.Columns.Contains("dqdy1") OrElse Not dt.Columns.Contains("dqdy2") Then
            Return dt
        End If

        ' 安全取 double（NaN 表示不可比较）
        Dim SafeDouble = Function(obj As Object) As Double
                             If obj Is Nothing OrElse IsDBNull(obj) Then Return Double.NaN
                             Dim d As Double
                             If Double.TryParse(obj.ToString(), d) Then Return d
                             Return Double.NaN
                         End Function

        Dim i As Integer = 0
        While i <= dt.Rows.Count - 2
            Dim r1 As DataRow = dt.Rows(i)
            Dim r2 As DataRow = dt.Rows(i + 1)

            Dim a1 = SafeDouble(r1("dqdh"))
            Dim b1 = SafeDouble(r1("dqdy1"))
            Dim c1 = SafeDouble(r1("dqdy2"))

            Dim a2 = SafeDouble(r2("dqdh"))
            Dim b2 = SafeDouble(r2("dqdy1"))
            Dim c2 = SafeDouble(r2("dqdy2"))

            ' 若任一值为 NaN 则跳过（不合并）
            If Double.IsNaN(a1) Or Double.IsNaN(b1) Or Double.IsNaN(c1) Or Double.IsNaN(a2) Or Double.IsNaN(b2) Or Double.IsNaN(c2) Then
                i += 1
                Continue While
            End If

            Dim eqA = Math.Abs(a1 - a2) <= tolerance
            Dim eqB = Math.Abs(b1 - b2) <= tolerance
            Dim eqC = Math.Abs(c1 - c2) <= tolerance

            If eqA AndAlso eqB AndAlso eqC Then
                ' 删除第二组（后一行）
                dt.Rows.RemoveAt(i + 1)
                ' 不增加 i，下一轮比较仍与当前 i 的新后继比较
            Else
                i += 1
            End If
        End While

        ' 可选：重编号 group 为 0,1,2,...
        If renumberGroups AndAlso dt.Columns.Contains("group") Then
            Dim map As New Dictionary(Of Object, Integer)
            Dim nextId As Integer = 0
            For idx As Integer = 0 To dt.Rows.Count - 1
                Dim g = dt.Rows(idx)("group")
                If Not map.ContainsKey(g) Then
                    map.Add(g, nextId)
                    nextId += 1
                End If
                dt.Rows(idx)("group") = map(g)
            Next
        End If

        Return dt
    End Function

End Module
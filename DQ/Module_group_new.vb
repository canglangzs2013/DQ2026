Imports System.Data
Module Module_group_new
    ''' <summary>
    ''' 改进的按 pfxdqdy1 分组：全部变量改为Decimal，逻辑完全不变，最小修改
    ''' </summary>
    Public Function GroupByPfxdqdy1RangeFixed(ByVal dtTemplate As DataTable, Optional threshold As Decimal = 1D, Optional minSegmentLength As Decimal = 2D) As DataTable
        If dtTemplate Is Nothing OrElse dtTemplate.Rows.Count = 0 Then
            Return Nothing
        End If
        ' 获得默认最小段长
        If minSegmentLength <= 0D Then
            Try
                minSegmentLength = CDec(Module_del.min_segment_length)
            Catch
                minSegmentLength = 2D
            End Try
        End If
        Dim dtOut As DataTable = dtTemplate.Clone()
        If Not dtOut.Columns.Contains("group") Then dtOut.Columns.Add("group", GetType(Integer))
        If Not dtOut.Columns.Contains("min_pfxdqdy1_group") Then dtOut.Columns.Add("min_pfxdqdy1_group", GetType(Decimal))
        If Not dtOut.Columns.Contains("min_y1_group") Then dtOut.Columns.Add("min_y1_group", GetType(Decimal))
        If Not dtOut.Columns.Contains("max_y1_group") Then dtOut.Columns.Add("max_y1_group", GetType(Decimal))
        For Each r As DataRow In dtTemplate.Rows
            dtOut.ImportRow(r)
        Next
        '【最小改动】替换SafeDouble → SafeDecimal，返回可空Decimal
        Dim SafeDecimal = Function(obj As Object) As Decimal?
                              If obj Is Nothing OrElse IsDBNull(obj) Then Return Nothing
                              Dim d As Decimal
                              If Decimal.TryParse(obj.ToString(), d) Then Return d
                              Return Nothing
                          End Function
        Dim rowCount As Integer = dtOut.Rows.Count
        ' ===== 阶段1：顺序贪心分组 + 向后吸收保证最小宽度 =====
        Dim groupId As Integer = 0
        Dim startIdx As Integer = 0
        Dim curMin As Decimal? = SafeDecimal(dtOut.Rows(0)("pfxdqdy1"))
        Dim curMax As Decimal? = curMin
        dtOut.Rows(0)("group") = groupId
        Dim i As Integer = 1
        While i < rowCount
            Dim valCur As Decimal? = SafeDecimal(dtOut.Rows(i)("pfxdqdy1"))
            Dim mustSplit As Boolean = False
            If Not IsDBNull(dtOut.Rows(i)("ID")) Then
                If dtOut.Rows(i)("ID").ToString().Contains("*") Then mustSplit = True
            End If

            '遇到空值直接分割，和原逻辑行为对齐
            If Not curMin.HasValue OrElse Not curMax.HasValue OrElse Not valCur.HasValue Then
                groupId += 1
                startIdx = i
                curMin = valCur
                curMax = valCur
                dtOut.Rows(i)("group") = groupId
                i += 1
                Continue While
            End If

            Dim tempMin As Decimal = Decimal.Min(curMin.Value, valCur.Value)
            Dim tempMax As Decimal = Decimal.Max(curMax.Value, valCur.Value)
            Dim shouldSplitByHeight As Boolean = (tempMax - tempMin) >= threshold

            If mustSplit OrElse shouldSplitByHeight Then
                If mustSplit Then
                    'ID带*强制分割
                    groupId += 1
                    startIdx = i
                    curMin = valCur
                    curMax = valCur
                    dtOut.Rows(i)("group") = groupId
                    i += 1
                Else
                    '高度差触发拆分，执行向后吸收补齐最小段长
                    Dim xStart As Decimal? = SafeDecimal(dtOut.Rows(startIdx)("x"))
                    Dim xEnd As Decimal? = SafeDecimal(dtOut.Rows(i - 1)("x"))
                    Dim width As Decimal = 0D
                    If xStart.HasValue AndAlso xEnd.HasValue Then
                        width = xEnd.Value - xStart.Value
                    End If
                    If width < minSegmentLength Then
                        Dim canAbsorb As Boolean = True
                        Dim j As Integer = i
                        While j < rowCount AndAlso width < minSegmentLength AndAlso canAbsorb
                            If Not IsDBNull(dtOut.Rows(j)("ID")) AndAlso dtOut.Rows(j)("ID").ToString().Contains("*") Then
                                canAbsorb = False
                                Exit While
                            End If
                            Dim jVal As Decimal? = SafeDecimal(dtOut.Rows(j)("pfxdqdy1"))
                            If jVal.HasValue Then
                                curMin = Decimal.Min(curMin.Value, jVal.Value)
                                curMax = Decimal.Max(curMax.Value, jVal.Value)
                            End If
                            xEnd = SafeDecimal(dtOut.Rows(j)("x"))
                            If xStart.HasValue AndAlso xEnd.HasValue Then
                                width = xEnd.Value - xStart.Value
                            End If
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
                dtOut.Rows(i)("group") = groupId
                curMin = tempMin
                curMax = tempMax
                i += 1
            End If
        End While
        ' ===== 阶段2：按 group 聚合最小/最大 pfx 并回写 =====
        Dim distinctGroups = dtOut.AsEnumerable().Select(Function(r) CInt(r("group"))).Distinct().OrderBy(Function(x) x).ToList()
        Dim dictGroupMin As New Dictionary(Of Integer, Decimal)
        Dim dictGroupMax As New Dictionary(Of Integer, Decimal)
        For Each gNo As Integer In distinctGroups
            Dim subRows = dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
            If subRows.Any() Then
                Dim vals = subRows.Select(Function(r) SafeDecimal(r("pfxdqdy1"))).Where(Function(v) v.HasValue).Select(Function(v) v.Value).ToList()
                If vals.Count > 0 Then
                    dictGroupMin(gNo) = vals.Min()
                    dictGroupMax(gNo) = vals.Max()
                End If
            End If
        Next
        For Each row As DataRow In dtOut.Rows
            Dim g As Integer = CInt(row("group"))
            If dictGroupMin.ContainsKey(g) Then
                row("min_pfxdqdy1_group") = dictGroupMin(g)
                row("min_y1_group") = dictGroupMin(g)
                row("max_y1_group") = dictGroupMax(g)
                Dim pfxVal As Decimal? = SafeDecimal(row("pfxdqdy1"))
                If pfxVal.HasValue AndAlso pfxVal.Value = dictGroupMin(g) Then
                    Dim id As String = If(IsDBNull(row("ID")), String.Empty, row("ID").ToString())
                    If Not id.EndsWith("min") Then
                        row("ID") = id & "min"
                    End If
                End If
            End If
        Next
        ' 统一组内 dqdh / dqdy1
        For Each gNo As Integer In distinctGroups
            Dim firstMinRow As DataRow = dtOut.AsEnumerable() _
            .Where(Function(r) CInt(r("group")) = gNo AndAlso Not IsDBNull(r("min_pfxdqdy1_group"))) _
            .FirstOrDefault()
            If firstMinRow IsNot Nothing Then
                Dim dqdy2Val As Decimal
                If dtOut.Columns.Contains("dqdy2") AndAlso Not IsDBNull(firstMinRow("dqdy2")) Then
                    dqdy2Val = CDec(firstMinRow("dqdy2"))
                Else
                    dqdy2Val = CDec(firstMinRow("pfxdqdy2"))
                End If
                Dim minG As Decimal = CDec(firstMinRow("min_pfxdqdy1_group"))
                Dim finalVal As Decimal = dqdy2Val - minG

                For Each row As DataRow In dtOut.AsEnumerable().Where(Function(r) CInt(r("group")) = gNo)
                    row("dqdh") = finalVal
                    row("dqdy1") = CDec(row("dqdy2")) - CDec(row("dqdh"))
                    Dim roundVal = Decimal.Round(finalVal, 1)
                    row("dq_‌foundation_height") = dq_Map(roundVal).foundation_height
                    row("h_plus_f") = dq_Map(roundVal).foundation_height + CDec(row("h"))
                Next
            End If
        Next
        ' 压缩group编号
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

    Public Function MergeAdjacentEqualGroups(ByVal dt As DataTable, Optional ByVal tolerance As Decimal = 0.000000001D, Optional ByVal renumberGroups As Boolean = True) As DataTable
        If dt Is Nothing OrElse dt.Rows.Count <= 1 Then Return dt
        If Not dt.Columns.Contains("dqdh") OrElse Not dt.Columns.Contains("dqdy1") OrElse Not dt.Columns.Contains("dqdy2") Then
            Return dt
        End If
        Dim SafeDecimal = Function(obj As Object) As Decimal?
                              If obj Is Nothing OrElse IsDBNull(obj) Then Return Nothing
                              Dim d As Decimal
                              If Decimal.TryParse(obj.ToString(), d) Then Return d
                              Return Nothing
                          End Function
        Dim i As Integer = 0
        While i <= dt.Rows.Count - 2
            Dim r1 As DataRow = dt.Rows(i)
            Dim r2 As DataRow = dt.Rows(i + 1)
            Dim a1 = SafeDecimal(r1("dqdh"))
            Dim b1 = SafeDecimal(r1("dqdy1"))
            Dim c1 = SafeDecimal(r1("dqdy2"))
            Dim a2 = SafeDecimal(r2("dqdh"))
            Dim b2 = SafeDecimal(r2("dqdy1"))
            Dim c2 = SafeDecimal(r2("dqdy2"))

            If Not (a1.HasValue AndAlso b1.HasValue AndAlso c1.HasValue AndAlso a2.HasValue AndAlso b2.HasValue AndAlso c2.HasValue) Then
                i += 1
                Continue While
            End If

            Dim eqA = Decimal.Abs(a1.Value - a2.Value) <= tolerance
            Dim eqB = Decimal.Abs(b1.Value - b2.Value) <= tolerance
            Dim eqC = Decimal.Abs(c1.Value - c2.Value) <= tolerance
            If eqA AndAlso eqB AndAlso eqC Then
                dt.Rows.RemoveAt(i + 1)
            Else
                i += 1
            End If
        End While
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

    '修复原代码全角标点，最小改动，全部使用Decimal
    Public Function organize_data_pd_L(ByVal dt As DataTable, ByVal dtTemplate As DataTable) As DataTable
        For t As Integer = 0 To dt.Rows.Count - 2
            Dim x_t As Decimal = CDec(dt.Rows(t)("x"))
            Dim x_t1 As Decimal = CDec(dt.Rows(t + 1)("x"))
            dt.Rows(t)("L") = x_t1 - x_t
            Dim y1_next As Decimal = CDec(dt.Rows(t + 1)("y1"))
            Dim y1 As Decimal = CDec(dt.Rows(t)("y1"))
            dt.Rows(t)("pd") = (y1_next - y1) / (x_t1 - x_t)
            If Decimal.Abs(CDec(dt.Rows(t)("pd"))) > 0.05D Then
                dt.Rows(t)("memo") = "陡纵坡"
            End If
        Next
        '最后一行
        Dim xDtLast As Decimal = CDec(dt.Rows(dt.Rows.Count - 1)("x"))
        Dim xTplLast As Decimal = CDec(dtTemplate.Rows(dtTemplate.Rows.Count - 1)("x"))
        Dim y1DtLast As Decimal = CDec(dt.Rows(dt.Rows.Count - 1)("y1"))
        Dim y1TplLast As Decimal = CDec(dtTemplate.Rows(dtTemplate.Rows.Count - 1)("y1"))

        dt.Rows(dt.Rows.Count - 1)("L") = xTplLast - xDtLast
        Dim LL As Decimal = CDec(dt.Rows(dt.Rows.Count - 1)("L"))
        dt.Rows(dt.Rows.Count - 1)("pd") = (y1TplLast - y1DtLast) / LL
        If Decimal.Abs(CDec(dt.Rows(dt.Rows.Count - 1)("pd"))) > 0.05D Then
            dt.Rows(dt.Rows.Count - 1)("memo") = "陡纵坡"
        End If
        Return dt
    End Function
End Module

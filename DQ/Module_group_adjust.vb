Imports System.Data
Imports System.Diagnostics

Module Module_group_adjust

    ''' <summary>
    ''' 在初次按高度分组完成后，调整过窄的组（合并到左右邻组之一）。
    ''' 评分（越小越好）： score = beta*mergedMaxAbsPd + gamma * widthPenalty
    ''' widthPenalty = max(0, (minSegmentLength - mergedWidth) / minSegmentLength)
    ''' 参数：
    '''   dt                - 已按 x 升序并包含列 "group","x" 的表（会就地修改 group、dqdy1、dqdh 等）
    '''   minSegmentLength  - 最小分段长度（米）
    '''   beta              - 坡率（pd）权重，默认1.0
    '''   gamma             - 宽度不足惩罚权重，默认10.0
    '''   debug             - 若为 True，会输出决策日志（Trace.WriteLine）
    ''' </summary>
    Public Sub AdjustSmallGroups(dt As DataTable, minSegmentLength As Double, Optional beta As Double = 1.0, Optional gamma As Double = 10.0, Optional debug As Boolean = False)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return
        If minSegmentLength <= 0 Then Return
        If Not dt.Columns.Contains("group") OrElse Not dt.Columns.Contains("x") Then Return

        ' 安全读取辅助
        Dim SafeDouble = Function(obj As Object) As Double
                             If obj Is Nothing OrElse IsDBNull(obj) Then Return Double.NaN
                             Dim d As Double
                             If Double.TryParse(obj.ToString(), d) Then Return d
                             Return Double.NaN
                         End Function

        ' 构建组区间列表 (groupId, startIdx, endIdx)
        Dim BuildGroups =
            Function() As List(Of (grp As Double, startIdx As Integer, endIdx As Integer))
                Dim list As New List(Of (Double, Integer, Integer))
                Dim curG As Double = Convert.ToDouble(dt.Rows(0)("group"))
                Dim s As Integer = 0
                For i As Integer = 1 To dt.Rows.Count - 1
                    Dim g = Convert.ToDouble(dt.Rows(i)("group"))
                    If g <> curG Then
                        list.Add((curG, s, i - 1))
                        curG = g
                        s = i
                    End If
                Next
                list.Add((curG, s, dt.Rows.Count - 1))
                Return list
            End Function

        ' 宽度与 pd 计算
        Dim WidthOf = Function(startIdx As Integer, endIdx As Integer) As Double
                          Dim x1 = SafeDouble(dt.Rows(startIdx)("x"))
                          Dim x2 = SafeDouble(dt.Rows(endIdx)("x"))
                          If Double.IsNaN(x1) OrElse Double.IsNaN(x2) Then Return 0.0
                          Return x2 - x1
                      End Function

        Dim MaxAbsPd = Function(startIdx As Integer, endIdx As Integer) As Double
                           Dim maxPd As Double = 0.0
                           If Not dt.Columns.Contains("pd") Then Return 0.0
                           For t As Integer = startIdx To Math.Max(startIdx, endIdx - 1)
                               Dim v = SafeDouble(dt.Rows(t)("pd"))
                               If Not Double.IsNaN(v) Then
                                   If Math.Abs(v) > maxPd Then maxPd = Math.Abs(v)
                               End If
                           Next
                           Return maxPd
                       End Function

        ' 主循环：直到无变化
        Dim changed As Boolean = True
        While changed
            changed = False
            Dim groups = BuildGroups()
            If debug Then Trace.WriteLine($"AdjustSmallGroups: groups={groups.Count}")

            For gi As Integer = 0 To groups.Count - 1
                Dim grp = groups(gi)
                Dim w = WidthOf(grp.startIdx, grp.endIdx)
                If debug Then Trace.WriteLine($" Group {grp.grp} idx[{grp.startIdx},{grp.endIdx}] width={w:0.###}")

                If w >= minSegmentLength Then Continue For ' 足够宽

                ' 保护含星号的控制点
                Dim hasStar As Boolean = False
                For r = grp.startIdx To grp.endIdx
                    If Not IsDBNull(dt.Rows(r)("ID")) AndAlso dt.Rows(r)("ID").ToString().Contains("*") Then
                        hasStar = True : Exit For
                    End If
                Next
                If hasStar Then
                    If debug Then Trace.WriteLine($"  skip group {grp.grp} (has *)")
                    Continue For
                End If

                ' 邻组是否存在
                Dim leftExists = (gi - 1) >= 0
                Dim rightExists = (gi + 1) < groups.Count
                If Not leftExists AndAlso Not rightExists Then Continue For

                ' 评估合并到左/右的评分（越小越好）
                Dim bestSide As Integer = -1
                Dim bestScore As Double = Double.MaxValue

                If leftExists Then
                    Dim left = groups(gi - 1)
                    Dim mergedStart = left.startIdx
                    Dim mergedEnd = grp.endIdx
                    Dim mergedWidth = WidthOf(mergedStart, mergedEnd)
                    Dim mergedMaxPd = MaxAbsPd(mergedStart, mergedEnd)
                    Dim widthPenalty = Math.Max(0.0, (minSegmentLength - mergedWidth) / minSegmentLength)
                    Dim score = beta * mergedMaxPd + gamma * widthPenalty
                    If debug Then Trace.WriteLine($"  left merge: width={mergedWidth:0.###} maxPd={mergedMaxPd:0.###} score={score:0.###}")
                    If score < bestScore Then bestScore = score : bestSide = 0
                End If

                If rightExists Then
                    Dim right = groups(gi + 1)
                    Dim mergedStart = grp.startIdx
                    Dim mergedEnd = right.endIdx
                    Dim mergedWidth = WidthOf(mergedStart, mergedEnd)
                    Dim mergedMaxPd = MaxAbsPd(mergedStart, mergedEnd)
                    Dim widthPenalty = Math.Max(0.0, (minSegmentLength - mergedWidth) / minSegmentLength)
                    Dim score = beta * mergedMaxPd + gamma * widthPenalty
                    If debug Then Trace.WriteLine($"  right merge: width={mergedWidth:0.###} maxPd={mergedMaxPd:0.###} score={score:0.###}")
                    If score < bestScore Then bestScore = score : bestSide = 1
                End If

                ' 如果最优评分仍是不可接受（非常大），跳过
                If bestSide = -1 OrElse bestScore > 100000000.0 Then
                    If debug Then Trace.WriteLine($"  skip merge group {grp.grp} (no viable side) score={bestScore}")
                    Continue For
                End If

                ' 执行合并：把 grp 的所有行 group id 改为邻组 id
                Dim targetGroupId As Double = If(bestSide = 0, groups(gi - 1).grp, groups(gi + 1).grp)
                For r As Integer = grp.startIdx To grp.endIdx
                    dt.Rows(r)("group") = targetGroupId
                Next

                If debug Then Trace.WriteLine($"  merged group {grp.grp} -> {targetGroupId} (bestSide={bestSide} score={bestScore:0.###})")
                changed = True
                Exit For ' 重建 groups 后再继续
            Next
        End While

        ' 合并完成后：对每组写回最小 pfxdqdy1（dqdy1）并更新 dqdh（如果存在 dqdy2）
        Dim groupMin As New Dictionary(Of Double, Double)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim g = Convert.ToDouble(dt.Rows(i)("group"))
            If dt.Columns.Contains("pfxdqdy1") AndAlso Not IsDBNull(dt.Rows(i)("pfxdqdy1")) Then
                Dim v = Convert.ToDouble(dt.Rows(i)("pfxdqdy1"))
                If groupMin.ContainsKey(g) Then
                    If v < groupMin(g) Then groupMin(g) = v
                Else
                    groupMin.Add(g, v)
                End If
            End If
        Next
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim g = Convert.ToDouble(dt.Rows(i)("group"))
            If groupMin.ContainsKey(g) Then
                dt.Rows(i)("dqdy1") = groupMin(g)
                If dt.Columns.Contains("dqdy2") AndAlso Not IsDBNull(dt.Rows(i)("dqdy2")) Then
                    dt.Rows(i)("dqdh") = Convert.ToDouble(dt.Rows(i)("dqdy2")) - Convert.ToDouble(dt.Rows(i)("dqdy1"))
                End If
            End If
        Next

        ' 重新编号 group 从 0 开始连续
        Dim map As New Dictionary(Of Double, Integer)
        Dim nextId As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim g = Convert.ToDouble(dt.Rows(i)("group"))
            If Not map.ContainsKey(g) Then
                map.Add(g, nextId)
                nextId += 1
            End If
            dt.Rows(i)("group") = map(g)
        Next
    End Sub

End Module

Imports System.Data

    Module GroupHelper

    Public Function GroupByThreshold_pmdy1_dy1(dt As DataTable, threshold As Double) As DataTable
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return dt

        If Not dt.Columns.Contains("group") Then dt.Columns.Add("group", GetType(Integer))
        If Not dt.Columns.Contains("dy1") Then dt.Columns.Add("dy1", GetType(Double))

        Dim groupIndex As Integer = 0
        Dim groupMin As Double = Double.MaxValue
        Dim groupMax As Double = Double.MinValue
        Dim groupStartIdx As Integer = 0

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim val As Double = Convert.ToDouble(dt.Rows(i)("pmdy1"))
            Dim isStar As Boolean = dt.Rows(i)("ID").ToString().Trim().Contains("*")

            ' ===== 强制新组 =====
            If isStar AndAlso i > 0 Then
                WriteGroup(dt, groupStartIdx, i - 1, groupMin)
                groupIndex += 1
                groupStartIdx = i
                groupMin = val
                groupMax = val
                dt.Rows(i)("group") = groupIndex
                Continue For
            End If

            ' ===== 初始化第一组 =====
            If i = 0 Then
                groupMin = val
                groupMax = val
                groupStartIdx = 0
                dt.Rows(i)("group") = groupIndex
                Continue For
            End If

            ' ===== ✅ 关键：先算“假设加入后的差值” =====
            Dim tempMin = If(val < groupMin, val, groupMin)
            Dim tempMax = If(val > groupMax, val, groupMax)

            If Math.Abs(tempMax - tempMin) > threshold Then
                ' 当前行不属于这一组
                WriteGroup(dt, groupStartIdx, i - 1, groupMin)
                groupIndex += 1
                groupStartIdx = i
                groupMin = val
                groupMax = val
                dt.Rows(i)("group") = groupIndex
            Else
                ' 属于当前组
                If val < groupMin Then groupMin = val
                If val > groupMax Then groupMax = val
                dt.Rows(i)("group") = groupIndex
            End If
        Next

        ' ===== 最后一组 =====
        WriteGroup(dt, groupStartIdx, dt.Rows.Count - 1, groupMin)

        Return dt
    End Function
    Private Sub WriteGroup(dt As DataTable, startIdx As Integer, endIdx As Integer, minVal As Double)
        For i As Integer = startIdx To endIdx
            dt.Rows(i)("dy1") = minVal
        Next
    End Sub

    'Private Sub WriteGroup(dt As DataTable, startIdx As Integer, endIdx As Integer, minVal As Double)
    '    For i As Integer = startIdx To endIdx
    '        dt.Rows(i)("dy1") = minVal
    '    Next
    'End Sub

    'Private Sub WriteGroup(dt As DataTable, endIdx As Integer, groupIndex As Integer, minVal As Double)
    '    For i As Integer = 0 To endIdx
    '        If dt.Rows(i)("group") Is DBNull.Value OrElse Convert.ToInt32(dt.Rows(i)("group")) = groupIndex Then
    '            dt.Rows(i)("dy1") = minVal
    '        End If

    '        dt.Rows(i)("dh") = dt.Rows(i)("dy2") - dt.Rows(i)("dy1")
    '    Next
    'End Sub
    'If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return dt

    'If Not dt.Columns.Contains("group") Then dt.Columns.Add("group", GetType(Integer))
    'If Not dt.Columns.Contains("dy1") Then dt.Columns.Add("dy1", GetType(Double))
    'Dim groupMin As Double = Double.MaxValue
    'Dim groupMax As Double = Double.MinValue
    'Dim groupIndex As Integer = 0
    'For i = 0 To dt.Rows.Count - 1
    '    If i = 0 Then
    '        groupMin = dt.Rows(i)("pmdy1")
    '        groupMax = dt.Rows(i)("pmdy1")
    '        dt.Rows(i)("group") = groupIndex
    '    Else
    '        If dt.Rows(i)("ID").ToString().Trim().Contains("*") Then
    '            groupIndex = groupIndex + 1
    '            groupMin = dt.Rows(i)("pmdy1")
    '            groupMax = dt.Rows(i)("pmdy1")
    '            dt.Rows(i)("group") = groupIndex
    '            Continue For

    '        End If

    '        'If dt.Rows(i)("pmdy1") >= groupMin And dt.Rows(i)("pmdy1") <= groupMax Then
    '        '    dt.Rows(i)("group") = groupIndex

    '        'ElseIf dt.Rows(i)("pmdy1") < groupMin Then
    '        '    If Math.Abs(dt.Rows(i)("pmdy1") - groupMax) < threshold Then
    '        '        groupMin = dt.Rows(i)("pmdy1")
    '        '        dt.Rows(i)("group") = groupIndex
    '        '    Else
    '        '        groupIndex = groupIndex + 1
    '        '        groupMin = dt.Rows(i)("pmdy1")
    '        '        groupMax = dt.Rows(i)("pmdy1")
    '        '    End If
    '        'ElseIf dt.Rows(i)("pmdy1") > groupMax Then

    '        '    If Math.Abs(dt.Rows(i)("pmdy1") - groupMin) < threshold Then
    '        '        groupMax = dt.Rows(i)("pmdy1")
    '        '        dt.Rows(i)("group") = groupIndex
    '        '    Else
    '        '        groupIndex = groupIndex + 1
    '        '        groupMin = dt.Rows(i)("pmdy1")
    '        '        groupMax = dt.Rows(i)("pmdy1")
    '        '    End If
    '        'End If
    '        ' 判断当前值是否在 [groupMin - threshold, groupMax + threshold] 范围内
    '        If dt.Rows(i)("pmdy1") >= groupMin - threshold AndAlso
    '           dt.Rows(i)("pmdy1") <= groupMax + threshold Then
    '            ' 属于当前组
    '            dt.Rows(i)("group") = groupIndex
    '            ' 更新组内的最大/最小值
    '            If dt.Rows(i)("pmdy1") < groupMin Then groupMin = dt.Rows(i)("pmdy1")
    '            If dt.Rows(i)("pmdy1") > groupMax Then groupMax = dt.Rows(i)("pmdy1")
    '        Else
    '            ' 超出范围，创建新组
    '            groupIndex += 1
    '            groupMin = dt.Rows(i)("pmdy1")
    '            groupMax = dt.Rows(i)("pmdy1")
    '            dt.Rows(i)("group") = groupIndex
    '        End If
    '        'ElseIf Math.Abs(dt.Rows(i)("pmdy1") - groupMin) > threshold Then
    '        '    groupMin = dt.Rows(i)("pmdy1")
    '        '    groupIndex = groupIndex + 1
    '        '    dt.Rows(i)("group") = groupIndex
    '        'End If

    '    End If

    'Next


    'dt = UpdateDy1ByGroupMin(dt)
    'Return dt
    'Dim groupIndex As Integer = 0
    'Dim groupStartIdx As Integer = 0

    '' ✅ 当前组的最值（只包含已确定属于本组的数据）
    'Dim groupMin As Double = Double.MaxValue
    'Dim groupMax As Double = Double.MinValue

    'For i As Integer = 0 To dt.Rows.Count - 1
    '    Dim val As Double = Convert.ToDouble(dt.Rows(i)("pmdy1"))
    '    'Dim isStar As Boolean = dt.Rows(i)("id").ToString().Trim() = "*"
    '    Dim isStar As Boolean = dt.Rows(i)("id").ToString().Trim().Contains("*")
    '    ' ===== 1. 强制新组（id="*") =====
    '    If isStar AndAlso i > groupStartIdx Then
    '        WriteGroup(dt, groupStartIdx, i - 1, groupIndex, groupMin, groupMax)
    '        groupIndex += 1
    '        groupStartIdx = i
    '        groupMin = val
    '        groupMax = val
    '        Continue For
    '    End If

    '    ' ===== 2. 初始化第一组 =====
    '    If i = groupStartIdx Then
    '        groupMin = val
    '        groupMax = val
    '        Continue For
    '    End If

    '    ' ===== 3. 关键：用“旧最值”判断是否超限 =====
    '    If Math.Abs(groupMax - groupMin) > threshold Then
    '        ' ✅ 当前行不属于这一组
    '        WriteGroup(dt, groupStartIdx, i - 1, groupIndex, groupMin, groupMax)

    '        groupIndex += 1
    '        groupStartIdx = i
    '        groupMin = val
    '        groupMax = val
    '    Else
    '        ' ✅ 只有没超限，才加入当前组
    '        If val < groupMin Then groupMin = val
    '        If val > groupMax Then groupMax = val
    '    End If
    'Next

    '' ===== 4. 最后一组（绝不会再错）=====
    'If groupStartIdx <= dt.Rows.Count - 1 Then
    '    WriteGroup(dt, groupStartIdx, dt.Rows.Count - 1, groupIndex, groupMin, groupMax)
    'End If

    'Return dt
    'End Function

    Public Function UpdateDy1ByGroupMin(dt As DataTable) As DataTable
        'If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return

        ' 确保列存在
        If Not dt.Columns.Contains("group") Then Throw New Exception("缺少列 group")
        If Not dt.Columns.Contains("dy1") Then dt.Columns.Add("dy1", GetType(Double))
        If Not dt.Columns.Contains("pmdy1") Then Throw New Exception("缺少列 pmdy1")

        ' 用 Dictionary 存：group → pmdy1 最小值
        Dim groupMinDict As New Dictionary(Of Object, Double)

        ' 第一遍：计算每个 group 的 pmdy1 最小值
        For Each row As DataRow In dt.Rows
            Dim g As Object = row("group")
            Dim val As Double = Convert.ToDouble(row("pmdy1"))

            If groupMinDict.ContainsKey(g) Then
                If val < groupMinDict(g) Then
                    groupMinDict(g) = val
                End If
            Else
                groupMinDict(g) = val
            End If
        Next

        ' 第二遍：把 dy1 设置为该 group 的最小值
        For Each row As DataRow In dt.Rows
            Dim g As Object = row("group")
            row("dy1") = groupMinDict(g)
        Next


        Return dt
    End Function

    'Private Sub WriteGroup(dt As DataTable, startIdx As Integer, endIdx As Integer,
    '                   groupIndex As Integer, minVal As Double, maxVal As Double)

    '    For i As Integer = startIdx To endIdx
    '        dt.Rows(i)("group") = groupIndex
    '        dt.Rows(i)("dy1") = minVal
    '    Next
    'End Sub

    'Private Sub WriteGroup(dt As DataTable, startIdx As Integer, endIdx As Integer,
    '                   groupIndex As Integer, minVal As Double, maxVal As Double)

    '    For i As Integer = startIdx To endIdx
    '        dt.Rows(i)("group") = groupIndex
    '        dt.Rows(i)("dy1") = minVal
    '    Next
    'End Sub

    Private Sub WriteGroup(dt As DataTable, startIdx As Integer, endIdx As Integer,
                       groupIndex As Integer, minVal As Double, maxVal As Double)

        For i As Integer = startIdx To endIdx
            dt.Rows(i)("group") = groupIndex
            dt.Rows(i)("dy1") = minVal

            dt.Rows(i)("dh") = dt.Rows(i)("dy2") - dt.Rows(i)("dy1")
        Next
    End Sub



    Private Sub WriteGroup_pmdy1_dy1(dt As DataTable, startIdx As Integer, endIdx As Integer,
                               groupIndex As Integer, minVal As Double, maxVal As Double)

        For i As Integer = startIdx To endIdx
            dt.Rows(i)("group") = groupIndex
            dt.Rows(i)("dy1") = minVal

            dt.Rows(i)("dh") = dt.Rows(i)("dy2") - dt.Rows(i)("dy1")
        Next
    End Sub


    Public Function GroupByThreshold_pmdy2_dy2(dt As DataTable, threshold As Double) As DataTable
        '貌似是按pmdy2来分组，并修改dy2
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return dt
        End If

        ' 确保输出列存在
        If Not dt.Columns.Contains("group") Then dt.Columns.Add("group", GetType(Integer))
        If Not dt.Columns.Contains("dy2") Then dt.Columns.Add("dy2", GetType(Double))

        Dim groupIndex As Integer = 0
        Dim groupStartIdx As Integer = 0
        Dim groupMin As Double = Double.MaxValue
        Dim groupMax As Double = Double.MinValue

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim row As DataRow = dt.Rows(i)
            Dim val As Double = Convert.ToDouble(row("pmdy2"))
            Dim isStar As Boolean = row("id").ToString().Trim() = "*"

            ' 遇到 * 强制新组
            If isStar AndAlso i > groupStartIdx Then
                WriteGroup_pmdy2_dy2(dt, groupStartIdx, i - 1, groupIndex, groupMin, groupMax)
                groupIndex += 1
                groupStartIdx = i
                groupMin = val
                groupMax = val
                Continue For
            End If

            ' 初始化第一组
            If i = groupStartIdx Then
                groupMin = val
                groupMax = val
                Continue For
            End If

            ' 更新最值
            If val < groupMin Then groupMin = val
            If val > groupMax Then groupMax = val

            ' 超过阈值 → 结束当前组
            If Math.Abs(groupMax - groupMin) > threshold Then
                WriteGroup_pmdy2_dy2(dt, groupStartIdx, i - 1, groupIndex, groupMin, groupMax)
                groupIndex += 1
                groupStartIdx = i
                groupMin = val
                groupMax = val
            End If
        Next

        ' 最后一组
        If groupStartIdx <= dt.Rows.Count - 1 Then
            WriteGroup_pmdy2_dy2(dt, groupStartIdx, dt.Rows.Count - 1, groupIndex, groupMin, groupMax)
        End If

        Return dt
    End Function

    Private Sub WriteGroup_pmdy2_dy2(dt As DataTable, startIdx As Integer, endIdx As Integer,
                               groupIndex As Integer, minVal As Double, maxVal As Double)

        For i As Integer = startIdx To endIdx
            dt.Rows(i)("group") = groupIndex
            'dt.Rows(i)("dy2") = minVal
            dt.Rows(i)("dy2") = maxVal

            dt.Rows(i)("dh") = dt.Rows(i)("dy2") - dt.Rows(i)("dy1")
        Next
    End Sub
End Module


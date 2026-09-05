Imports System.Data
Module Module_group
    ''' <summary>
    ''' 对 DataTable 进行分组处理
    ''' </summary>
    ''' <param name="dt">包含数据的 DataTable</param>
    ''' <param name="threshold">差值阈值</param>
    ''' <summary>
    ''' 分组函数：增加了对特殊 ID 的处理
    ''' </summary>
    ''' <param name="dt">数据表</param>
    ''' <param name="threshold">阈值</param>
    ''' <param name="specialId">特殊ID值，例如 -1，代表需要强制分组的行</param>
    Public Function GroupByThreshold3333333333333333(dt As DataTable, threshold As Double)
        ' 1. 确保列存在
        EnsureColumnExists(dt, "group", GetType(Integer))
        EnsureColumnExists(dt, "pmdy1", GetType(Double))

        ' 2. 初始化变量
        Dim currentGroupID As Integer = 1
        Dim currentGroupMin As Double = 0
        Dim currentGroupMax As Double = 0
        Dim isFirstRow As Boolean = True
        Dim hasCurrentGroupRows As Boolean = False ' 标记当前组是否已有数据

        ' 3. 逐行遍历
        For Each row As DataRow In dt.Rows
            ' 获取值
            If IsDBNull(row("pmdy1")) Then Continue For
            Dim currentValue As Double = Convert.ToDouble(row("pmdy1"))

            ' 获取 ID
            Dim currentId As String = ""
            If Not IsDBNull(row("id")) Then
                currentId = Convert.ToString(row("id"))
            End If

            ' --- 核心逻辑判断 ---

            ' 情况 A: 当前行是特殊行 (例如 id = -1)
            If currentId = "*" Then
                ' 如果当前组已经有数据，必须先结束当前组，开启新组
                ' 这样特殊行就会成为新组的第一行（即处于组的头部）
                If hasCurrentGroupRows Then
                    currentGroupID += 1
                    ' 重置状态
                    currentGroupMin = currentValue
                    currentGroupMax = currentValue
                    hasCurrentGroupRows = True ' 新组现在包含这一行了
                Else
                    ' 如果当前组是空的（比如刚初始化，或者刚分完组），直接加入
                    currentGroupMin = currentValue
                    currentGroupMax = currentValue
                    hasCurrentGroupRows = True
                End If

                ' 情况 B: 普通行
            Else
                If isFirstRow Then
                    ' 第一行数据初始化
                    currentGroupMin = currentValue
                    currentGroupMax = currentValue
                    isFirstRow = False
                    hasCurrentGroupRows = True
                Else
                    ' 普通阈值判断逻辑
                    Dim potentialMin As Double = Math.Min(currentGroupMin, currentValue)
                    Dim potentialMax As Double = Math.Max(currentGroupMax, currentValue)

                    If Math.Abs(potentialMax - potentialMin) <= threshold Then
                        ' 符合阈值，加入当前组
                        currentGroupMin = potentialMin
                        currentGroupMax = potentialMax
                        hasCurrentGroupRows = True
                    Else
                        ' 超出阈值，开启新组
                        currentGroupID += 1
                        currentGroupMin = currentValue
                        currentGroupMax = currentValue
                        hasCurrentGroupRows = True
                    End If
                End If
            End If

            ' --- 写回数据 ---
            row("group") = currentGroupID
            row("dy1") = currentGroupMin
        Next

        dt.AcceptChanges()
        Return dt
    End Function

    ' 辅助方法保持不变
    Private Sub EnsureColumnExists(dt As DataTable, columnName As String, dataType As Type)
        If Not dt.Columns.Contains(columnName) Then
            dt.Columns.Add(columnName, dataType)
        End If
    End Sub
End Module

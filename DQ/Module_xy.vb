Module Module_xy
    ''' <summary>
    ''' 在单调递增的多段线 DataTable 中，根据给定的 X 坐标，计算对应的 Y 坐标。
    ''' </summary>
    ''' <param name="dt">包含多段线顶点的 DataTable，列名为 "X", "Y"，且 X 必须单调递增。</param>
    ''' <param name="targetX">需要查询的 X 坐标。</param>
    ''' <returns>计算得到的 Y 坐标。如果 X 超出范围，返回边界值或抛出异常。</returns>
    Public Function GetYFromX(ByVal dt As DataTable, ByVal targetX As Double) As Double
        ' 1. 边界检查：确保 DataTable 至少有 2 个点
        If dt.Rows.Count < 2 Then
            Throw New ArgumentException("DataTable 必须至少包含 2 个顶点。")
        End If

        ' 2. 获取 X 的范围
        Dim firstX As Double = CDbl(dt.Rows(0)("X"))
        Dim lastX As Double = CDbl(dt.Rows(dt.Rows.Count - 1)("X"))

        ' 3. 处理超出范围的情况
        ' 如果 X 小于第一个点，取第一个点的 Y (外推或限制)
        If targetX <= firstX Then
            Return CDbl(dt.Rows(0)("Y"))
        End If

        ' 如果 X 大于最后一个点，取最后一个点的 Y
        If targetX >= lastX Then
            Return CDbl(dt.Rows(dt.Rows.Count - 1)("Y"))
        End If

        ' 4. 寻找 targetX 所在的线段 (因为 X 递增，可以用二分查找优化，这里用简单的顺序查找演示)
        ' 由于数据量通常不大，顺序查找更直观
        For i As Integer = 0 To dt.Rows.Count - 2
            Dim x1 As Double = CDbl(dt.Rows(i)("X"))
            Dim y1 As Double = CDbl(dt.Rows(i)("Y"))
            Dim x2 As Double = CDbl(dt.Rows(i + 1)("X"))
            Dim y2 As Double = CDbl(dt.Rows(i + 1)("Y"))

            ' 检查 targetX 是否在当前线段 [x1, x2] 之间
            ' 因为 X 是递增的，所以 x1 <= x2
            If targetX >= x1 AndAlso targetX <= x2 Then
                ' 5. 线性插值计算 Y
                ' 公式: Y = Y1 + (TargetX - X1) * (Y2 - Y1) / (X2 - X1)
                ' 加入防除零判断
                If Math.Abs(x2 - x1) < 0.000001 Then
                    Return y1 ' 避免除以零（垂直线）
                End If

                Dim ratio As Double = (targetX - x1) / (x2 - x1)
                Dim resultY As Double = y1 + ratio * (y2 - y1)
                Return resultY
            End If
        Next

        ' 理论上不会走到这里，因为前面已经做了边界检查
        ' 为了防止编译器报错（未返回值），这里返回最后一个点的 Y
        Return CDbl(dt.Rows(dt.Rows.Count - 1)("Y"))
    End Function
End Module

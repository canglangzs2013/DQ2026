Module Module_step
    ''' <summary>
    ''' 根据坡度计算步长，使用线性插值。
    ''' 当坡度为0.4时，步长为2m；当坡度为0.1时，步长为5m。
    ''' 步长被限制在2m到5m之间。
    ''' </summary>
    ''' <param name="slope">输入的坡度值</param>
    ''' <returns>计算出的步长（米）</returns>
    Public Function CalculateStepSize(slope As Double) As Double
        ' 定义已知点
        Const SLOPE_MIN As Double = 0.05 ' 坡度最小值
        Const SLOPE_MAX As Double = 0.4 ' 坡度最大值
        Const STEP_MAX As Double = 5.0  ' 步长最大值 (对应坡度最小值)
        Const STEP_MIN As Double = 2.0  ' 步长最小值 (对应坡度最大值)

        ' 执行线性插值
        ' 公式: y = y1 + (x - x1) * (y2 - y1) / (x2 - x1)
        ' 其中: x1=0.4, y1=2.0, x2=0.1, y2=5.0
        Dim stepSize As Double = STEP_MIN + (slope - SLOPE_MAX) * (STEP_MAX - STEP_MIN) / (SLOPE_MIN - SLOPE_MAX)

        ' 将结果限制在2m到5m之间
        'stepSize = Math.Ceiling(stepSize)
        stepSize = RoundUpToHalf(stepSize)

        If stepSize > STEP_MAX Then
            stepSize = STEP_MAX
        ElseIf stepSize < STEP_MIN Then
            stepSize = STEP_MIN
        End If



        Return stepSize
    End Function


    ''' <summary>
    ''' 将输入的数值按0.5的倍数向上取整
    ''' 核心思路：将原数放大2倍，用Math.Ceiling()向上取整，再缩小2倍
    ''' </summary>
    ''' <param name="value">输入的Double类型数值</param>
    ''' <returns>按0.5倍数向上取整后的结果</returns>
    Public Function RoundUpToHalf(value As Double) As Double
        ' 将原数放大2倍
        Dim scaledValue As Double = value * 2.0

        ' 使用Math.Ceiling向上取整
        Dim ceilingValue As Double = Math.Ceiling(scaledValue)

        ' 缩小2倍得到最终结果
        Dim result As Double = ceilingValue / 2.0

        Return result
    End Function
End Module

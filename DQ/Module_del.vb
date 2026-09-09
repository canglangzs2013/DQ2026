Module Module_del
    Public myexcelHelper As New ExcelHelper()

    Public dq_‌foundation_height As Double = 0.5 '基础埋深
    Public step_cal As Double = 0.5 '插入剖分线的计算步长（亦即剖分线步长）
    'Public dq_‌foundation_height As Double = 0.5 '计算步长
    Public min_dq_length As Double = 1 '，阈值,为按步长 step_cal插入剖分线，插入时不能距离已有线太近 min_dq_length
    'Public dq_‌foundation_height As Double = 0.5 '计算步长
    Public dq_difference_height_base As Double = 1 '挡墙基底高差的阈值
End Module

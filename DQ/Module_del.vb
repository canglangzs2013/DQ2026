Module Module_del
    Public myexcelHelper As New ExcelHelper()
    'Public dtTemplate As System.Data.DataTable = New System.Data.DataTable '注意这是一个仅仅存储滑坡前后缘点的datable。但是其结构和存储整个滑坡坐标的datatable相同，也可以使用其结构
    Public plus_dq_height As Double = 0.2
    'Public datatable_B As System.Data.DataTable = New System.Data.DataTable '注意这是一个仅仅存储滑坡前后缘点的datable。但是其结构和存储整个滑坡坐标的datatable相同，也可以使用其结构
    'Public datatable_T As System.Data.DataTable = New System.Data.DataTable '注意这是一个仅仅存储滑坡前后缘点的datable。但是其结构和存储整个滑坡坐标的datatable相同，也可以使用其结构
    Public step_cal As Double = 0.5 '计算步长
    'Public plus_dq_height As Double = 0.5 '计算步长
    Public min_dq_length As Double = 2 '计算步长
End Module

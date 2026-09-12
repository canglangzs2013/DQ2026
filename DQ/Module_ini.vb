Module Module_ini
    ''' <summary>
    ''' 创建挡墙剖面分组DataTable模板
    ''' </summary>
    ''' <returns>DataTable模板</returns>
    Function ini_dtTemplate() As DataTable
        Dim dtTemplate = New DataTable("WallProfile")

        dtTemplate.Columns.Add("ID", GetType(String))
        dtTemplate.Columns.Add("memo2", GetType(String))
        dtTemplate.Columns.Add("x", GetType(Decimal))
        dtTemplate.Columns.Add("y1", GetType(Decimal)) '底
        dtTemplate.Columns.Add("y2", GetType(Decimal)) '顶
        dtTemplate.Columns.Add("pfxdqdy2", GetType(Decimal)) '顶
        dtTemplate.Columns.Add("h", GetType(Decimal)) '高差
        dtTemplate.Columns.Add("pfx_dq_foundation_height", GetType(Decimal)) '剖分线基础埋深
        dtTemplate.Columns.Add("pfx_h_plus_f", GetType(Decimal)) '设计挡墙高度.刚开始是单个剖面的，后来是分组的
        dtTemplate.Columns.Add("pfxdqdh", GetType(Decimal)) '单个剖面的挡墙高度
        dtTemplate.Columns.Add("pfxdqdy1", GetType(Decimal)) '底
        dtTemplate.Columns.Add("dqdy2", GetType(Decimal)) '顶
        dtTemplate.Columns.Add("h_plus_f", GetType(Decimal)) '设计挡墙高度.刚开始是单个剖面的，后来是分组的
        dtTemplate.Columns.Add("dqdh", GetType(Decimal)) '设计挡墙高度.刚开始是单个剖面的，后来是分组的
        dtTemplate.Columns.Add("dqdy1", GetType(Decimal)) '底
        dtTemplate.Columns.Add("dq_foundation_height", GetType(Decimal)) '剖分线基础埋深
        dtTemplate.Columns.Add("min_pfxdqdy1_group", GetType(Decimal)) '同一个分组的最小pfxdqdy1
        dtTemplate.Columns.Add("group", GetType(Decimal)) '分组
        dtTemplate.Columns.Add("max_y1_group", GetType(Decimal)) '分组
        dtTemplate.Columns.Add("min_y1_group", GetType(Decimal)) '分组
        dtTemplate.Columns.Add("memo", GetType(String))
        dtTemplate.Columns.Add("L", GetType(Decimal)) '挡墙分段长度
        dtTemplate.Columns.Add("pd", GetType(Decimal)) '挡墙纵坡坡度

        ' 设置列显示Caption
        dtTemplate.Columns("y1").Caption = "地形底高程y1"
        dtTemplate.Columns("y2").Caption = "地形顶高程y2"
        dtTemplate.Columns("h").Caption = "地形高差h"
        dtTemplate.Columns("dq_foundation_height").Caption = "挡墙基础埋深"
        dtTemplate.Columns("pfx_dq_foundation_height").Caption = "剖分线挡墙基础埋深"
        dtTemplate.Columns("dqdh").Caption = "挡墙高度dqdh最终值"
        dtTemplate.Columns("L").Caption = "挡墙长度L"
        dtTemplate.Columns("dqdy1").Caption = "挡墙设计底高程dqdy1"
        dtTemplate.Columns("dqdy2").Caption = "挡墙设计顶高程dqdy2"
        dtTemplate.Columns("min_pfxdqdy1_group").Caption = "分组内剖分线最低值min_pfxdqdy1_group"
        dtTemplate.Columns("pfx_h_plus_f").Caption = "高差+基础埋深"
        dtTemplate.Columns("h_plus_f").Caption = "高差+基础埋深"
        dtTemplate.Columns("group").Caption = "挡墙分组group"
        dtTemplate.Columns("pd").Caption = "挡墙基底纵坡"
        dtTemplate.Columns("memo").Caption = "备注"

        Return dtTemplate
    End Function
    ''' <summary>
    ''' 创建挡墙分组汇总DataTable模板（带列Caption）
    ''' </summary>
    ''' <returns>DataTable</returns>
    Function Ini_dt() As DataTable
        Dim dt = New DataTable("WallGroup")

        dt.Columns.Add("group", GetType(Decimal)) '分组
        dt.Columns.Add("dqdh", GetType(Decimal)) '挡墙高度
        dt.Columns.Add("L", GetType(Decimal)) '分段长度
        dt.Columns.Add("x", GetType(Decimal)) 'X坐标
        dt.Columns.Add("y1", GetType(Decimal)) '地形底高程
        dt.Columns.Add("dqdy1", GetType(Decimal)) '挡墙设计底高程
        dt.Columns.Add("dqdy2", GetType(Decimal)) '挡墙设计顶高程
        dt.Columns.Add("pd", GetType(Decimal)) '基底纵坡
        dt.Columns.Add("dq_foundation_height", GetType(Decimal)) '基础埋深
        dt.Columns.Add("memo", GetType(String)) '备注
        dt.Columns.Add("memo2", GetType(String)) '备注2

        '设置列显示Caption
        dt.Columns("group").Caption = "挡墙分组号"
        dt.Columns("dqdh").Caption = "挡墙高度dqdh"
        dt.Columns("L").Caption = "挡墙分段长度L"
        dt.Columns("x").Caption = "X坐标"
        dt.Columns("y1").Caption = "地形底高程y1"
        dt.Columns("dqdy1").Caption = "挡墙设计底高程dqdy1"
        dt.Columns("dqdy2").Caption = "挡墙设计顶高程dqdy2"
        dt.Columns("pd").Caption = "基底纵坡pd"
        dt.Columns("dq_foundation_height").Caption = "基础埋深"
        dt.Columns("memo").Caption = "备注"
        dt.Columns("memo2").Caption = "备注2"

        Return dt
    End Function
    Function Ini_datatable_T() As DataTable
        Dim datatable_T = New DataTable '顶线
        datatable_T.Columns.Add("ID", GetType(String))
        datatable_T.Columns.Add("x", GetType(Decimal))
        datatable_T.Columns.Add("y", GetType(Decimal)) '顶


        Return datatable_T
    End Function
    Function Ini_datatable_B() As DataTable


        Dim datatable_B = New DataTable '底线
        datatable_B.Columns.Add("ID", GetType(String))
        datatable_B.Columns.Add("x", GetType(Decimal))
        datatable_B.Columns.Add("y", GetType(Decimal)) '顶
        Return datatable_B
    End Function
End Module

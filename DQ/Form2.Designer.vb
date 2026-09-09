<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form2))
        lbl_title = New Label()
        lbl_foundation_height = New Label()
        txt_foundation_height = New TextBox()
        lbl_step_cal = New Label()
        txt_step_cal = New TextBox()
        lbl_min_dq_length = New Label()
        txt_min_dq_length = New TextBox()
        lbl_difference_height_base = New Label()
        txt_difference_height_base = New TextBox()
        Button2 = New Button()
        Button1 = New Button()
        SuspendLayout()
        ' 
        ' lbl_title
        ' 
        lbl_title.Font = New Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(134))
        lbl_title.Location = New Point(15, 10)
        lbl_title.Name = "lbl_title"
        lbl_title.Size = New Size(450, 28)
        lbl_title.TabIndex = 0
        lbl_title.Text = "挡墙参数配置（修改后请点击保存）"
        lbl_title.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lbl_foundation_height
        ' 
        lbl_foundation_height.AutoSize = True
        lbl_foundation_height.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_foundation_height.Location = New Point(15, 58)
        lbl_foundation_height.Name = "lbl_foundation_height"
        lbl_foundation_height.Size = New Size(79, 20)
        lbl_foundation_height.TabIndex = 1
        lbl_foundation_height.Text = "基础埋深："
        lbl_foundation_height.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_foundation_height
        ' 
        txt_foundation_height.Font = New Font("Microsoft YaHei UI", 10F)
        txt_foundation_height.Location = New Point(225, 55)
        txt_foundation_height.Name = "txt_foundation_height"
        txt_foundation_height.Size = New Size(240, 24)
        txt_foundation_height.TabIndex = 2
        ' 
        ' lbl_step_cal
        ' 
        lbl_step_cal.AutoSize = True
        lbl_step_cal.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_step_cal.Location = New Point(15, 98)
        lbl_step_cal.Name = "lbl_step_cal"
        lbl_step_cal.Size = New Size(163, 20)
        lbl_step_cal.TabIndex = 3
        lbl_step_cal.Text = "插入剖分线的计算步长："
        lbl_step_cal.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_step_cal
        ' 
        txt_step_cal.Font = New Font("Microsoft YaHei UI", 10F)
        txt_step_cal.Location = New Point(225, 95)
        txt_step_cal.Name = "txt_step_cal"
        txt_step_cal.Size = New Size(240, 24)
        txt_step_cal.TabIndex = 4
        ' 
        ' lbl_min_dq_length
        ' 
        lbl_min_dq_length.AutoSize = True
        lbl_min_dq_length.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_min_dq_length.Location = New Point(15, 138)
        lbl_min_dq_length.Name = "lbl_min_dq_length"
        lbl_min_dq_length.Size = New Size(175, 20)
        lbl_min_dq_length.TabIndex = 5
        lbl_min_dq_length.Text = "阈值1 两剖分线最小距离："
        lbl_min_dq_length.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_min_dq_length
        ' 
        txt_min_dq_length.Font = New Font("Microsoft YaHei UI", 10F)
        txt_min_dq_length.Location = New Point(225, 135)
        txt_min_dq_length.Name = "txt_min_dq_length"
        txt_min_dq_length.Size = New Size(240, 24)
        txt_min_dq_length.TabIndex = 6
        ' 
        ' lbl_difference_height_base
        ' 
        lbl_difference_height_base.AutoSize = True
        lbl_difference_height_base.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_difference_height_base.Location = New Point(15, 178)
        lbl_difference_height_base.Name = "lbl_difference_height_base"
        lbl_difference_height_base.Size = New Size(151, 20)
        lbl_difference_height_base.TabIndex = 7
        lbl_difference_height_base.Text = "阈值2  挡墙基底高差："
        lbl_difference_height_base.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_difference_height_base
        ' 
        txt_difference_height_base.Font = New Font("Microsoft YaHei UI", 10F)
        txt_difference_height_base.Location = New Point(225, 175)
        txt_difference_height_base.Name = "txt_difference_height_base"
        txt_difference_height_base.Size = New Size(240, 24)
        txt_difference_height_base.TabIndex = 8
        ' 
        ' Button2
        ' 
        Button2.Font = New Font("Microsoft YaHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point, CByte(134))
        Button2.Location = New Point(15, 225)
        Button2.Margin = New Padding(2)
        Button2.Name = "Button2"
        Button2.Size = New Size(200, 40)
        Button2.TabIndex = 9
        Button2.Text = "保存参数配置"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Font = New Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(134))
        Button1.Image = My.Resources.Resources.Justicon_Free_Simple_Line_Folder_Check_File_Folder_File_Document_Document_Check_48
        Button1.Location = New Point(15, 280)
        Button1.Margin = New Padding(2)
        Button1.Name = "Button1"
        Button1.Size = New Size(450, 60)
        Button1.TabIndex = 10
        Button1.Text = "打开挡墙数据文件绘图"
        Button1.TextImageRelation = TextImageRelation.ImageBeforeText
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Form2
        ' 
        AutoScaleDimensions = New SizeF(7F, 17F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(480, 355)
        Controls.Add(Button1)
        Controls.Add(Button2)
        Controls.Add(txt_difference_height_base)
        Controls.Add(lbl_difference_height_base)
        Controls.Add(txt_min_dq_length)
        Controls.Add(lbl_min_dq_length)
        Controls.Add(txt_step_cal)
        Controls.Add(lbl_step_cal)
        Controls.Add(txt_foundation_height)
        Controls.Add(lbl_foundation_height)
        Controls.Add(lbl_title)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2)
        MaximizeBox = False
        Name = "Form2"
        StartPosition = FormStartPosition.CenterScreen
        Text = "挡墙立面图绘图"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lbl_title As Label
    Friend WithEvents lbl_foundation_height As Label
    Friend WithEvents txt_foundation_height As TextBox
    Friend WithEvents lbl_step_cal As Label
    Friend WithEvents txt_step_cal As TextBox
    Friend WithEvents lbl_min_dq_length As Label
    Friend WithEvents txt_min_dq_length As TextBox
    Friend WithEvents lbl_difference_height_base As Label
    Friend WithEvents txt_difference_height_base As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents Button1 As Button
End Class
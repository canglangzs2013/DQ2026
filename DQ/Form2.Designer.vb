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
        txt_min_dq_height = New TextBox()
        lbl_step_cal = New Label()
        txt_step_cal = New TextBox()
        lbl_min_dq_length = New Label()
        txt_min_threshold = New TextBox()
        lbl_difference_height_base = New Label()
        txt_dq_difference_height_base = New TextBox()
        Button2 = New Button()
        Button1 = New Button()
        txt_min_segment_length = New TextBox()
        Label1 = New Label()
        Button3 = New Button()
        SuspendLayout()
        ' 
        ' lbl_title
        ' 
        lbl_title.Font = New Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(134))
        lbl_title.Location = New Point(19, 12)
        lbl_title.Margin = New Padding(4, 0, 4, 0)
        lbl_title.Name = "lbl_title"
        lbl_title.Size = New Size(579, 33)
        lbl_title.TabIndex = 0
        lbl_title.Text = "挡墙参数配置（修改后请点击保存）"
        lbl_title.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lbl_foundation_height
        ' 
        lbl_foundation_height.AutoSize = True
        lbl_foundation_height.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_foundation_height.Location = New Point(19, 68)
        lbl_foundation_height.Margin = New Padding(4, 0, 4, 0)
        lbl_foundation_height.Name = "lbl_foundation_height"
        lbl_foundation_height.Size = New Size(129, 23)
        lbl_foundation_height.TabIndex = 1
        lbl_foundation_height.Text = "最小挡墙高度："
        lbl_foundation_height.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_min_dq_height
        ' 
        txt_min_dq_height.Font = New Font("Microsoft YaHei UI", 10F)
        txt_min_dq_height.Location = New Point(289, 65)
        txt_min_dq_height.Margin = New Padding(4)
        txt_min_dq_height.Name = "txt_min_dq_height"
        txt_min_dq_height.Size = New Size(307, 29)
        txt_min_dq_height.TabIndex = 2
        ' 
        ' lbl_step_cal
        ' 
        lbl_step_cal.AutoSize = True
        lbl_step_cal.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_step_cal.Location = New Point(19, 115)
        lbl_step_cal.Margin = New Padding(4, 0, 4, 0)
        lbl_step_cal.Name = "lbl_step_cal"
        lbl_step_cal.Size = New Size(197, 23)
        lbl_step_cal.TabIndex = 3
        lbl_step_cal.Text = "插入剖分线的计算步长："
        lbl_step_cal.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_step_cal
        ' 
        txt_step_cal.Font = New Font("Microsoft YaHei UI", 10F)
        txt_step_cal.Location = New Point(289, 112)
        txt_step_cal.Margin = New Padding(4)
        txt_step_cal.Name = "txt_step_cal"
        txt_step_cal.Size = New Size(307, 29)
        txt_step_cal.TabIndex = 4
        ' 
        ' lbl_min_dq_length
        ' 
        lbl_min_dq_length.AutoSize = True
        lbl_min_dq_length.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_min_dq_length.Location = New Point(19, 162)
        lbl_min_dq_length.Margin = New Padding(4, 0, 4, 0)
        lbl_min_dq_length.Name = "lbl_min_dq_length"
        lbl_min_dq_length.Size = New Size(212, 23)
        lbl_min_dq_length.TabIndex = 5
        lbl_min_dq_length.Text = "阈值1 两剖分线最小距离："
        lbl_min_dq_length.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_min_threshold
        ' 
        txt_min_threshold.Font = New Font("Microsoft YaHei UI", 10F)
        txt_min_threshold.Location = New Point(289, 159)
        txt_min_threshold.Margin = New Padding(4)
        txt_min_threshold.Name = "txt_min_threshold"
        txt_min_threshold.Size = New Size(307, 29)
        txt_min_threshold.TabIndex = 6
        ' 
        ' lbl_difference_height_base
        ' 
        lbl_difference_height_base.AutoSize = True
        lbl_difference_height_base.Font = New Font("Microsoft YaHei UI", 10F)
        lbl_difference_height_base.Location = New Point(19, 209)
        lbl_difference_height_base.Margin = New Padding(4, 0, 4, 0)
        lbl_difference_height_base.Name = "lbl_difference_height_base"
        lbl_difference_height_base.Size = New Size(183, 23)
        lbl_difference_height_base.TabIndex = 7
        lbl_difference_height_base.Text = "阈值2  挡墙基底高差："
        lbl_difference_height_base.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txt_dq_difference_height_base
        ' 
        txt_dq_difference_height_base.Font = New Font("Microsoft YaHei UI", 10F)
        txt_dq_difference_height_base.Location = New Point(289, 206)
        txt_dq_difference_height_base.Margin = New Padding(4)
        txt_dq_difference_height_base.Name = "txt_dq_difference_height_base"
        txt_dq_difference_height_base.Size = New Size(307, 29)
        txt_dq_difference_height_base.TabIndex = 8
        ' 
        ' Button2
        ' 
        Button2.Font = New Font("Microsoft YaHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point, CByte(134))
        Button2.Location = New Point(19, 290)
        Button2.Margin = New Padding(3, 2, 3, 2)
        Button2.Name = "Button2"
        Button2.Size = New Size(257, 45)
        Button2.TabIndex = 9
        Button2.Text = "保存参数配置"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Font = New Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(134))
        Button1.Image = My.Resources.Resources.Justicon_Free_Simple_Line_Folder_Check_File_Folder_File_Document_Document_Check_48
        Button1.Location = New Point(19, 348)
        Button1.Margin = New Padding(3, 2, 3, 2)
        Button1.Name = "Button1"
        Button1.Size = New Size(579, 65)
        Button1.TabIndex = 10
        Button1.Text = "打开挡墙的墙顶墙底数据文件绘图"
        Button1.TextImageRelation = TextImageRelation.ImageBeforeText
        Button1.UseVisualStyleBackColor = True
        ' 
        ' txt_min_segment_length
        ' 
        txt_min_segment_length.Font = New Font("Microsoft YaHei UI", 10F)
        txt_min_segment_length.Location = New Point(289, 253)
        txt_min_segment_length.Margin = New Padding(4)
        txt_min_segment_length.Name = "txt_min_segment_length"
        txt_min_segment_length.Size = New Size(307, 29)
        txt_min_segment_length.TabIndex = 12
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Microsoft YaHei UI", 10F)
        Label1.Location = New Point(19, 256)
        Label1.Margin = New Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(163, 23)
        Label1.TabIndex = 11
        Label1.Text = "挡墙最小分段长度："
        Label1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' Button3
        ' 
        Button3.Font = New Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(134))
        Button3.Image = My.Resources.Resources.Justicon_Free_Simple_Line_Folder_Check_File_Folder_File_Document_Document_Check_48
        Button3.Location = New Point(17, 423)
        Button3.Margin = New Padding(3, 2, 3, 2)
        Button3.Name = "Button3"
        Button3.Size = New Size(579, 65)
        Button3.TabIndex = 13
        Button3.Text = "打开人工修改的挡墙数据文件绘图"
        Button3.TextImageRelation = TextImageRelation.ImageBeforeText
        Button3.UseVisualStyleBackColor = True
        ' 
        ' Form2
        ' 
        AutoScaleDimensions = New SizeF(9F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(617, 567)
        Controls.Add(Button3)
        Controls.Add(txt_min_segment_length)
        Controls.Add(Label1)
        Controls.Add(Button1)
        Controls.Add(Button2)
        Controls.Add(txt_dq_difference_height_base)
        Controls.Add(lbl_difference_height_base)
        Controls.Add(txt_min_threshold)
        Controls.Add(lbl_min_dq_length)
        Controls.Add(txt_step_cal)
        Controls.Add(lbl_step_cal)
        Controls.Add(txt_min_dq_height)
        Controls.Add(lbl_foundation_height)
        Controls.Add(lbl_title)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(3, 2, 3, 2)
        MaximizeBox = False
        Name = "Form2"
        StartPosition = FormStartPosition.CenterScreen
        Text = "挡墙立面图绘图"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lbl_title As Label
    Friend WithEvents lbl_foundation_height As Label
    Friend WithEvents txt_min_dq_height As TextBox
    Friend WithEvents lbl_step_cal As Label
    Friend WithEvents txt_step_cal As TextBox
    Friend WithEvents lbl_min_dq_length As Label
    Friend WithEvents txt_min_threshold As TextBox
    Friend WithEvents lbl_difference_height_base As Label
    Friend WithEvents txt_dq_difference_height_base As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents txt_min_segment_length As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Button3 As Button
End Class
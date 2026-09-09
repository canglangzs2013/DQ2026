Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json

Public Class DqConfig
    ' 基础埋深
    Public Property dq_foundation_height As Double = 0.5
    ' 插入剖分线的计算步长（亦即剖分线步长）
    Public Property step_cal As Double = 0.5
    ' 插入时不能距离已有线太近的阈值
    Public Property min_dq_length As Double = 1
    ' 挡墙基底高差的阈值
    Public Property dq_difference_height_base As Double = 1

    ' 将 JSON 文件保存在 WinForms 程序的同级目录下
    Private Shared ConfigPath As String = Path.Combine(Application.StartupPath, "DqConfig.json")

    ' 1. 读取配置
    Public Shared Function LoadConfig() As DqConfig
        If File.Exists(ConfigPath) Then
            Try
                Dim json As String = File.ReadAllText(ConfigPath)
                Return JsonConvert.DeserializeObject(Of DqConfig)(json)
            Catch ex As Exception
                ' 如果文件损坏，返回默认配置
                Return New DqConfig()
            End Try
        Else
            ' 如果文件不存在，创建默认配置并保存
            Dim defaultConfig As New DqConfig()
            SaveConfig(defaultConfig)
            Return defaultConfig
        End If
    End Function

    ' 2. 保存配置
    Public Shared Sub SaveConfig(config As DqConfig)
        Try
            ' Formatting.Indented 会让 JSON 文件排版好看，方便人工查看
            Dim json As String = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented)
            File.WriteAllText(ConfigPath, json)
        Catch ex As Exception
            MessageBox.Show("保存配置文件失败：" & ex.Message)
        End Try
    End Sub
End Class

Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json
Imports System.Windows.Forms

Public Class DqConfig
    ' 最终权威字段（与 UI / Module_del 保持一致）
    Public Property min_dq_height As Double = 2             ' 最小挡墙高度
    Public Property step_cal As Double = 0.5                ' 插入剖分线步长
    Public Property min_threshold As Double = 1             ' 阈值1插入剖分线时距离已有剖分线不小于
    Public Property dq_difference_height_base As Double = 1 ' 阈值2挡墙基底高差阈值
    Public Property min_segment_length As Double = 2        ' 最小分段长度

    ' 配置文件路径（程序目录）
    Private Shared ReadOnly ConfigPath As String = Path.Combine(Application.StartupPath, "DqConfig.json")

    ' 读取配置（兼容旧字段）
    Public Shared Function LoadConfig() As DqConfig
        If File.Exists(ConfigPath) Then
            Try
                Dim json As String = File.ReadAllText(ConfigPath)
                ' 尝试反序列化为 DqConfig（简洁）
                Dim cfg As DqConfig = JsonConvert.DeserializeObject(Of DqConfig)(json)
                If cfg Is Nothing Then
                    Return New DqConfig()
                End If
                ' 兼容处理：有时候老配置使用其他字段名，做最小兼容补充
                Dim j = JsonConvert.DeserializeObject(Of Newtonsoft.Json.Linq.JObject)(json)
                If j IsNot Nothing Then
                    If j("min_dq_height") IsNot Nothing AndAlso Double.TryParse(j("min_dq_height").ToString(), Nothing) Then
                        cfg.min_dq_height = CDbl(j("min_dq_height"))
                    End If
                    If j("min_threshold") IsNot Nothing AndAlso Double.TryParse(j("min_threshold").ToString(), Nothing) Then
                        cfg.min_threshold = CDbl(j("min_threshold"))
                    End If
                    ' 其它老字段同理（这里只保留常见兼容）
                End If
                Return cfg
            Catch ex As Exception
                ' 损坏或解析失败 -> 备份并返回默认
                Try
                    File.Copy(ConfigPath, ConfigPath & ".broken", True)
                Catch
                End Try
                Return New DqConfig()
            End Try
        Else
            ' 不存在则写入默认配置
            Dim cfg As New DqConfig()
            SaveConfig(cfg)
            Return cfg
        End If
    End Function

    ' 保存配置
    Public Shared Sub SaveConfig(cfg As DqConfig)
        Try
            Dim json As String = JsonConvert.SerializeObject(cfg, Newtonsoft.Json.Formatting.Indented)
            File.WriteAllText(ConfigPath, json)
        Catch ex As Exception
            MessageBox.Show("保存配置文件失败: " & ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class

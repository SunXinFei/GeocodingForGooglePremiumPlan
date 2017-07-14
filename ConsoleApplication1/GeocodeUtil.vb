Class GeocodeUtil
    Public Shared Function JsonSerialize(Of T)(ByVal obj As T) As String
        Dim javaScriptSerializer As System.Web.Script.Serialization.JavaScriptSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
        javaScriptSerializer.MaxJsonLength = Int32.MaxValue
        Return javaScriptSerializer.Serialize(obj)
    End Function

    Public Shared Function JsonDeserialize(Of T)(ByVal str As String) As T
        Dim javaScriptSerializer As System.Web.Script.Serialization.JavaScriptSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
        javaScriptSerializer.MaxJsonLength = Int32.MaxValue
        Return javaScriptSerializer.Deserialize(Of T)(str)
    End Function
End Class

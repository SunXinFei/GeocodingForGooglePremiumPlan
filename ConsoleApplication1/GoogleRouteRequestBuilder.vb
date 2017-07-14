Imports System.Text
Imports System.Security.Cryptography
Class BC_Location
    Public Latitude As String
    Public Longitude As String
    Public Sub New(ByVal _latitude As String, _
                   ByVal _longitude As String)

        Latitude = _latitude
        Longitude = _longitude
    End Sub
End Class
Class GoogleRouteRequestBuilder
    Implements IDisposable

    Private BasicQueryUri As String
    Private SpeedLimitQueryUri As String
    Private WayPoints As List(Of BC_Location)
    Private Optimizer As Boolean
    Private DisposedValue As Boolean
    Private Units As String
    Private Key As String
    Private ClientID As String
    Public Sub New(ByVal _Key As String, ByVal _ClientID As String)
        WayPoints = New List(Of BC_Location)
        BasicQueryUri = "https://maps.googleapis.com/maps/api/directions/"
        SpeedLimitQueryUri = "https://roads.googleapis.com/v1/speedLimits?"
        Key = _Key
        ClientID = _ClientID
        Optimizer = False
        Units = "metric"
    End Sub

    Public Sub InitWayPoints(ByVal _wayPoints As List(Of BC_Location))
        Me.WayPoints = _wayPoints
    End Sub

    Public Sub SetOptimizer(ByVal optimizer As Boolean)
        Me.Optimizer = optimizer
    End Sub

    Public Function GetQueryUri() As String
        Dim WayPointIndex As Integer
        Dim resultB As New StringBuilder

        WayPointIndex = 0
        resultB.Append(Me.BasicQueryUri)
        resultB.Append("xml?")

        If Me.WayPoints.Count < 2 Or Me.WayPoints.Count > 25 Then
            Throw New ArgumentOutOfRangeException("WayPoints.Count", String.Format("{0}", WayPoints.Count))
        End If

        resultB.AppendFormat("origin={0},{1}", WayPoints(0).Latitude, WayPoints(0).Longitude)
        resultB.AppendFormat("&destination={0},{1}", WayPoints(WayPoints.Count - 1).Latitude, WayPoints(WayPoints.Count - 1).Longitude)
        resultB.Append("&waypoints=")

        If Me.Optimizer Then
            resultB.Append("optimize:true|")
        End If

        For i As Integer = 1 To Me.WayPoints.Count - 2
            resultB.AppendFormat("{0},{1}", WayPoints(i).Latitude, WayPoints(i).Longitude)
            If i < WayPoints.Count - 2 Then
                resultB.AppendFormat("{0}", "|")
            End If
        Next

        resultB.AppendFormat("&units={0}", Me.Units)
        resultB.AppendFormat("&client={0}", ClientID)
        Return Me.GoogleSignedUrl(resultB.ToString(), Key)
    End Function

    Public Function GetSpeedLimitQueryUri() As String
        Dim WayPointIndex As Integer
        Dim resultB As New StringBuilder

        WayPointIndex = 0
        resultB.Append(Me.SpeedLimitQueryUri)

        If Me.WayPoints.Count < 2 Or Me.WayPoints.Count > 25 Then
            Throw New ArgumentOutOfRangeException("WayPoints.Count", String.Format("{0}", WayPoints.Count))
        End If

        resultB.Append("path=")

        For i As Integer = 1 To Me.WayPoints.Count - 2
            resultB.AppendFormat("{0},{1}", WayPoints(i).Latitude, WayPoints(i).Longitude)
            If i < WayPoints.Count - 2 Then
                resultB.AppendFormat("{0}", "|")
            End If
        Next

        resultB.AppendFormat("&units={0}", "MPH")
        resultB.AppendFormat("&client={0}", ClientID)
        Return Me.GoogleSignedUrl(resultB.ToString(), Key)
    End Function

    Public Function GoogleSignedUrl(ByVal url As String, ByVal key As String) As String
        Dim encoding As ASCIIEncoding = New ASCIIEncoding()
        'converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
        Dim usablePrivateKey As String = key.Replace("-", "+").Replace("_", "/")
        Dim privateKeyBytes() As Byte = Convert.FromBase64String(usablePrivateKey)

        Dim uri As Uri = New Uri(url)
        Dim encodedPathAndQueryBytes() As Byte = encoding.GetBytes(uri.LocalPath + uri.Query)

        ' compute the hash
        Dim algorithm As HMACSHA1 = New HMACSHA1(privateKeyBytes)
        Dim hash() As Byte = algorithm.ComputeHash(encodedPathAndQueryBytes)

        'convert the bytes to string and make url-safe by replacing '+' and '/' characters
        Dim signature As String = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_")

        'Add the signature to the existing URI.
        Return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature
    End Function

    Protected Overridable Sub Dispose(ByVal Disposing As Boolean)
        If Not Me.DisposedValue Then
            If Disposing Then
                WayPoints = Nothing
            End If
        End If

        Me.DisposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

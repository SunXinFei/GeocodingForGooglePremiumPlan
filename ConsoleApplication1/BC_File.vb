
Class BC_File
    Public Shared Function Http(ByVal Url As String, ByVal PostData As String, ByVal TimeOutSeconds As Integer) As String
        Return BC_File.Http(Url, PostData, TimeOutSeconds, Nothing)
    End Function

    Public Shared Function Http(ByVal Url As String, ByVal PostData As String, ByVal TimeOutSeconds As Integer, ByVal Certificate As System.Security.Cryptography.X509Certificates.X509Certificate2) As String
        Dim ResponseText As String = ""

        Using myHttpWebResponse As Net.HttpWebResponse = BC_File.HttpResponse(Url, PostData, TimeOutSeconds, Nothing, Certificate)
            If Not myHttpWebResponse Is Nothing Then
                Dim ResponseStream As IO.Stream = Nothing

                Try
                    ResponseStream = myHttpWebResponse.GetResponseStream()
                Catch myException As Exception
                    Throw New Exception(myException.Message, myException)
                End Try

                Using myStreamReader As New IO.StreamReader(ResponseStream)
                    ResponseText = myStreamReader.ReadToEnd()
                    Call myStreamReader.Close()
                End Using

                Call ResponseStream.Close()
                ResponseStream = Nothing

                Call myHttpWebResponse.Close()
            End If
        End Using

        Return ResponseText
    End Function
    Public Shared Function HttpResponse(ByVal Url As String, ByVal PostData As String, ByVal TimeOutSeconds As Integer) As Net.HttpWebResponse
        Return BC_File.HttpResponse(Url, PostData, TimeOutSeconds, Nothing, Nothing)
    End Function

    Public Shared Function HttpResponse(ByVal Url As String, ByVal PostData As String, ByVal TimeOutSeconds As Integer, ByVal CookieContainer As Net.CookieContainer, ByVal Certificate As System.Security.Cryptography.X509Certificates.X509Certificate2) As Net.HttpWebResponse
        Dim myHttpWebRequest As Net.HttpWebRequest = CType(Net.HttpWebRequest.Create(Url), Net.HttpWebRequest)
        Dim timeOutMilliseconds As Integer = TimeOutSeconds * 1000

        myHttpWebRequest.UserAgent = "UserAgent"
        myHttpWebRequest.AllowAutoRedirect = False
        myHttpWebRequest.Proxy = Nothing
        myHttpWebRequest.KeepAlive = True
        myHttpWebRequest.Timeout = timeOutMilliseconds
        'Default value is 5 minutes. (5 * 60 * 1000)
        If timeOutMilliseconds > 300000 Then
            myHttpWebRequest.ReadWriteTimeout = timeOutMilliseconds
        End If

        If Not CookieContainer Is Nothing Then
            myHttpWebRequest.CookieContainer = CookieContainer
        End If

        If Not Certificate Is Nothing Then
            Call myHttpWebRequest.ClientCertificates.Add(Certificate)
        End If

        If PostData <> "" Then
            myHttpWebRequest.Method = "POST"
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded"

            myHttpWebRequest.ContentLength = System.Text.Encoding.UTF8.GetByteCount(PostData)

            Using myStreamWriter As IO.Stream = myHttpWebRequest.GetRequestStream()
                Dim PostDataBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(PostData)
                Call myStreamWriter.Write(PostDataBytes, 0, PostDataBytes.Length)
                Call myStreamWriter.Close()
            End Using
        End If

        Dim myHttpWebResponse As Net.HttpWebResponse = Nothing

        Try
            myHttpWebResponse = CType(myHttpWebRequest.GetResponse(), Net.HttpWebResponse)
        Catch Retry1 As Exception
            Try
                myHttpWebResponse = CType(myHttpWebRequest.GetResponse(), Net.HttpWebResponse)
            Catch Retry2 As Exception
                myHttpWebResponse = CType(myHttpWebRequest.GetResponse(), Net.HttpWebResponse)
            End Try
        End Try

        If Not IsNothing(myHttpWebResponse) Then
            Select Case myHttpWebResponse.StatusCode
                Case Net.HttpStatusCode.Redirect, Net.HttpStatusCode.RedirectKeepVerb, Net.HttpStatusCode.RedirectMethod
                    Return BC_File.HttpResponse(myHttpWebResponse.Headers("Location"), PostData, TimeOutSeconds, CookieContainer, Certificate)
                    Exit Select

                Case Else
                    Return myHttpWebResponse
                    Exit Select
            End Select
        End If

        Return Nothing
    End Function
End Class

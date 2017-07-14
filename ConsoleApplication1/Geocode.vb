Imports System.IO

Module Geocode


    Public Function GoogleGeocode(ByVal tmpCustomer As LocationEntity) As GoogleGeocodeEntity
        Dim encodeAddress As String = System.Web.HttpUtility.UrlEncode(tmpCustomer.Address & " " & tmpCustomer.City & " " & tmpCustomer.State & " " & tmpCustomer.PostalCode)
        Dim httpURL As String = "https://maps.googleapis.com/maps/api/geocode/json?address=" & encodeAddress & "&client=" & "Google maps api client ID"
        Dim googleGeocodeResult As New GoogleGeocodeEntity
        Dim key As String = "Google Map Crypto Key"
        Dim GoogleRouteRequestBuilder As New GoogleRouteRequestBuilder(key, "Google Map Api Client ID")
        Dim requestURL As String = GoogleRouteRequestBuilder.GoogleSignedUrl(httpURL, key)
        Try
            Dim responseFromServer As String = BC_File.Http(requestURL, "", 50)
            googleGeocodeResult = GeocodeUtil.JsonDeserialize(Of GoogleGeocodeEntity)(responseFromServer)
            Return googleGeocodeResult
        Catch myException As Exception
            googleGeocodeResult.status = "UNKNOWN_STATUS"
            Return googleGeocodeResult
        End Try
    End Function

    Sub Main()
        Dim location As New LocationEntity
        location.Address = ""
        location.City = ""
        location.State = ""
        location.PostalCode = ""
        Dim result As GoogleGeocodeEntity = GoogleGeocode(location)
    End Sub

End Module

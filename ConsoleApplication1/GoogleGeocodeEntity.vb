Class GoogleGeocodeEntity
    Public Property results As List(Of GoogleGeocodeEntityTwo)
    Public Property status As String
End Class

Class GoogleGeocodeEntityTwo
    Public Property geometry As GoogleGeocodeEntityThree
End Class

Class GoogleGeocodeEntityThree
    Public Property location As GoogleGeocodeEntityFour
    Public Property location_type As String
End Class

Class GoogleGeocodeEntityFour
    Public Property lat As Decimal
    Public Property lng As Decimal
End Class
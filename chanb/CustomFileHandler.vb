Public Interface CustomFileHandler

    ''' <summary>
    ''' Called when a user upload a file.
    ''' </summary>
    ''' <param name="f"></param>
    ''' <returns></returns>
    ''' <remarks>If no thumbnail is required for the file, please return a CFHThumbData class with thumbRequired = False.</remarks>
    Function GetHTTPFileThumb(ByVal f As IO.Stream) As CFHThumbData

    ''' <summary>
    ''' Called when ChanB need to display a file.
    ''' </summary>
    ''' <param name="wpi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetFileHTML(ByVal wpi As WPostImage) As String


    ''' <summary>
    ''' Called when ChanB need to display a file when JavaScript is disabled.
    ''' </summary>
    ''' <param name="wpi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetFileHTML_NoScript(ByVal wpi As WPostImage) As String

    ''' <summary>
    ''' Called by ChanB JS extension when updating a thread.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetJSHandler() As String


    Function Is_FileSupported(ByVal file_extension As String) As Boolean

    ''' <summary>
    ''' Needs to return an array containing all the supported files extension.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Get_Supported_Files() As String()

End Interface

Public Class CFHThumbData
    Public thumbRequired As Boolean = True
    Public thumbData As Drawing.Image
    Public thumbExtension As String
    Public thumbDimensions As String
    Public mimeType As String
End Class
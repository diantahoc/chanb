Public Class HTMLParameters
    Public isModerator As Boolean 'Indicate if a mod is logged in
    Public isAdmin As Boolean 'Indicate if the admin is logged in
    Public CredPowers As String ' Power string. Required for mods.
    Public CredMenu As String ' The generated admin/mod HTML menu. Currently it is stored in the http session

    Public replyButton As Boolean ' Indicate either to show a [Reply] button in OP post.
    Public isTrailPost As Boolean ' Indicate if this post appear on the main page, so we can trim to long comment.
    Public isCurrentThread As Boolean = True ' Indicate if this thread is archived or not.
End Class

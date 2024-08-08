Imports System.Drawing.Text
Imports System.IO
Imports System.Media

''' <summary>
''' This module is used only in the current WinForms project, containing most of
''' the requisites for this game program.
''' 
''' Note: The measurement of each sprite used in the game is 15 by 15 pixels,
'''       creating an "invisible grid" for the game.
''' </summary>
Friend Module SnakeGameRequisites
    Friend ReadOnly random As New Random
    Friend ReadOnly Property Commodore64T As FontFamily
        Get
            Dim fontCollection As New PrivateFontCollection
            fontCollection.AddFontFile("assets/commodore-64t.ttf")
            Return fontCollection.Families(0)
        End Get
    End Property

    Friend Enum Directions As Byte : Up : Down : Left : Right : End Enum

    Private Function CreateImageByName(imageName As String) As Image
        Return Image.FromFile($"assets/{imageName}.png")
    End Function

    Private Function CreateAudioByName(audioName As String) As SoundPlayer
        Return New SoundPlayer($"assets/{audioName}.wav")
    End Function

    Friend ReadOnly snakeHeadAnimation1 As New List(Of Image) From {
        CreateImageByName("snake_head_left1"), CreateImageByName("snake_head_right1"),
        CreateImageByName("snake_head_up1"), CreateImageByName("snake_head_down1")
    }
    Friend ReadOnly snakeHeadAnimation2 As New List(Of Image) From {
        CreateImageByName("snake_head_left2"), CreateImageByName("snake_head_right2"),
        CreateImageByName("snake_head_up2"), CreateImageByName("snake_head_down2")
    }

    Friend ReadOnly imgSnakeSegment As Image = CreateImageByName("snake_body")
    Friend ReadOnly imgRedApple As Image = CreateImageByName("red_delicious")
    Friend ReadOnly imgGoldenApple As Image = CreateImageByName("golden_delicious")

    Friend ReadOnly sndMainTheme As SoundPlayer = CreateAudioByName("main_theme")
    Friend ReadOnly sndRedApple As SoundPlayer = CreateAudioByName("red_apple")
    Friend ReadOnly sndGoldenApple As SoundPlayer = CreateAudioByName("golden_apple")
    Friend ReadOnly sndDeathSound As SoundPlayer = CreateAudioByName("death_sound")

    Friend ReadOnly Property StartingPositions As List(Of Rectangle)
        Get
            Dim initialProfile As New List(Of Rectangle)
            For xPos As Integer = 150 To 75 Step -15
                initialProfile.Add(New Rectangle(xPos, 75, 15, 15))
            Next xPos
            Return initialProfile
        End Get
    End Property

    Friend Function GridRandi(num As Integer) As Integer
        ' This function returns a random integer that fits in with the invisible grid.
        Return CInt(random.Next(0, num) / 15) * 15
    End Function
End Module

#Disable Warning IDE1006
''' <summary>
''' The main part of this program, with dozens of elements set up on its "Load" event.
''' 
''' A majority of magic numbers in the program are derived from "15" and "25", which have
''' something to do with the sizes and layouts of quite a few components.
''' 
''' Note: All colors in the program are carefully coded, so modifying them isn't good.
''' </summary>
Public Class frmMain
    Friend snakeDirection As Directions
    Private imgSnakeHead As Image
    Private playerScore As New Integer
    Private bonusCountdown As Integer = random.Next(5, 10)

    Friend Property PlayingGameWithSoundEffect As New Boolean
    Friend Property ActiveSnakeHeadAnim As List(Of Image) = snakeHeadAnimation1
    Friend Property RedApplePosition As Point
    Friend Property GoldenApplePosition As New Point(15, 310)
    Friend Property SnakeSegments As List(Of Rectangle) = StartingPositions

    Private WithEvents tmrFrameUpdate As New Timer
    Private lblCurrentScore As New Label
    Private lblHighestScore As New Label
    Private lblStartGamePrompt As New Label
    Private lblGameTitle As New Label

    Shared Sub New()
        Dim assetPath As String = Path.GetFullPath("../../../assets")
        Dim targetPath As String = Path.GetFullPath("assets")
        If Directory.Exists(targetPath) Then Exit Sub
        Directory.CreateDirectory(targetPath)
        Dim assets As String() = Directory.GetFiles(assetPath)
        For Each asset As String In assets
            Dim fileName As String = Path.GetFileName(asset)
            File.Copy(asset, Path.Combine(targetPath, fileName), True)
        Next asset
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Text = "Green Delicious -- The Snake Game"
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        Controls.Add(lblCurrentScore)
        With lblCurrentScore
            .Text = "CURRENT SCORE:"
            .TextAlign = ContentAlignment.MiddleLeft
            .Location = New Point(225, 300)
            .Font = New Font(Commodore64T, 7.5F, FontStyle.Regular, GraphicsUnit.Point)
            .ForeColor = Color.FromArgb(255, 255, 204, 128)
            .BackColor = Color.Black
            .Size = New Size(225, 25)
        End With
        Controls.Add(lblHighestScore)
        With lblHighestScore
            .Text = "HIGHEST SCORE:"
            .TextAlign = ContentAlignment.MiddleLeft
            .Location = New Point(0, 300)
            .Font = New Font(Commodore64T, 7.5F, FontStyle.Regular, GraphicsUnit.Point)
            .ForeColor = Color.FromArgb(255, 255, 224, 130)
            .BackColor = Color.Black
            .Size = New Size(225, 25)
        End With
        Controls.Add(lblStartGamePrompt)
        With lblStartGamePrompt
            .Text = "PUSH ""ENTER"" TO START WITH BACKGROUND MUSIC

PUSH ""SPACE"" TO START WITH SOUND EFFECTS"
            .TextAlign = ContentAlignment.MiddleCenter
            .Location = New Point(0, 225)
            .Font = New Font(Commodore64T, 7.5F, FontStyle.Regular, GraphicsUnit.Point)
            .ForeColor = Color.Black
            .Size = New Size(450, 50)
        End With
        Controls.Add(lblGameTitle)
        With lblGameTitle
            .Text = "GREEN DELICIOUS
---------------"  ' Add enough hyphens below the text to form an underscore.
            .TextAlign = ContentAlignment.TopCenter
            .Location = New Point(175, 75)
            .Font = New Font(Commodore64T, 7.5F, FontStyle.Regular, GraphicsUnit.Point)
            .ForeColor = Color.ForestGreen
            .Size = New Size(200, 25)
        End With

        ClientSize = New Size(450, 325)
        BackColor = Color.FromArgb(255, 128, 222, 234)
        imgSnakeHead = ActiveSnakeHeadAnim(1)
        snakeDirection = Directions.Right
        tmrFrameUpdate.Interval = 100
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If {Directions.Left, Directions.Right}.Contains(snakeDirection) Then
                    snakeDirection = Directions.Up
                    imgSnakeHead = ActiveSnakeHeadAnim(2)
                End If
            Case Keys.Down
                If {Directions.Left, Directions.Right}.Contains(snakeDirection) Then
                    snakeDirection = Directions.Down
                    imgSnakeHead = ActiveSnakeHeadAnim(3)
                End If
            Case Keys.Left
                If {Directions.Up, Directions.Down}.Contains(snakeDirection) Then
                    snakeDirection = Directions.Left
                    imgSnakeHead = ActiveSnakeHeadAnim(0)
                End If
            Case Keys.Right
                If {Directions.Up, Directions.Down}.Contains(snakeDirection) Then
                    snakeDirection = Directions.Right
                    imgSnakeHead = ActiveSnakeHeadAnim(1)
                End If
            Case Keys.Enter
                If Not tmrFrameUpdate.Enabled Then
                    lblStartGamePrompt.Visible = False
                    InitializeGame()
                    sndMainTheme.PlayLooping()
                    PlayingGameWithSoundEffect = False
                End If
            Case Keys.Space
                If Not tmrFrameUpdate.Enabled Then
                    lblStartGamePrompt.Visible = False
                    InitializeGame()
                    PlayingGameWithSoundEffect = True
                End If
        End Select
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim gameScreen As Graphics = e.Graphics

        For i As Integer = 0 To SnakeSegments.Count - 1 Step 1
            Dim segment As Rectangle = SnakeSegments(i)
            Dim segmentImage As Image = If(i = 0, imgSnakeHead, imgSnakeSegment)
            gameScreen.DrawImage(segmentImage, segment.Location)
        Next i

        gameScreen.DrawImage(imgRedApple, RedApplePosition)
        gameScreen.DrawImage(imgGoldenApple, GoldenApplePosition)

        MyBase.OnPaint(e)
    End Sub

    Private Sub MoveSnake()
        Static usingFirstSet As Boolean = True
        ActiveSnakeHeadAnim = If(usingFirstSet, snakeHeadAnimation1, snakeHeadAnimation2)
        usingFirstSet = Not usingFirstSet

        Dim snakeHead As Rectangle = SnakeSegments(0)
        Dim updatedPos As Rectangle

        Select Case snakeDirection
            Case Directions.Up
                updatedPos = New Rectangle(snakeHead.Left, snakeHead.Top - 15, 15, 15)
            Case Directions.Down
                updatedPos = New Rectangle(snakeHead.Left, snakeHead.Top + 15, 15, 15)
            Case Directions.Left
                updatedPos = New Rectangle(snakeHead.Left - 15, snakeHead.Top, 15, 15)
            Case Directions.Right
                updatedPos = New Rectangle(snakeHead.Left + 15, snakeHead.Top, 15, 15)
        End Select

        SnakeSegments.Insert(0, updatedPos)
        SnakeSegments.RemoveAt(SnakeSegments.Count - 1)
    End Sub

    Private Sub CheckCollision()
        Static isGoldenAppleOnStage As New Boolean

        Dim redApple As New Rectangle(RedApplePosition, New Size(15, 15))
        Dim goldenApple As New Rectangle(GoldenApplePosition, New Size(15, 15))

        ' The golden apple will be hidden if the player eats the red one first.
        If isGoldenAppleOnStage AndAlso SnakeSegments(0).IntersectsWith(redApple) Then
            bonusCountdown = random.Next(5, 10)
            GoldenApplePosition = New Point(15, 315)
            isGoldenAppleOnStage = False
        End If

        ' At normal times the player scores 1 point by consuming a red apple.
        If SnakeSegments(0).IntersectsWith(redApple) Then
            If PlayingGameWithSoundEffect Then sndRedApple.Play()
            SnakeSegments.Add(New Rectangle(0, 310, 15, 15))
            ProduceRedApple()
            playerScore += 1
            If Not isGoldenAppleOnStage Then bonusCountdown -= 1
            GoldenApplePosition = New Point(15, 315)
        End If

        ' The following code explains the mechanism of the golden apple.
        If bonusCountdown <= 0 AndAlso Not isGoldenAppleOnStage Then
            ProduceGoldenApple()
            isGoldenAppleOnStage = True
        End If

        If SnakeSegments(0).IntersectsWith(goldenApple) Then
            If PlayingGameWithSoundEffect Then sndGoldenApple.Play()
            SnakeSegments.Add(New Rectangle(0, 310, 15, 15))
            playerScore += 2
            isGoldenAppleOnStage = False
            GoldenApplePosition = New Point(15, 315)
            bonusCountdown = random.Next(5, 10)
        End If

        ' If the player bites himself or hit the wall, the game ends.
        Dim GameOver = Sub()
                           tmrFrameUpdate.Stop()
                           lblStartGamePrompt.Visible = True
                           sndDeathSound.Play()
                       End Sub

        For i As Integer = 1 To SnakeSegments.Count - 1 Step 1
            If SnakeSegments(i).IntersectsWith(SnakeSegments(0)) Then
                GameOver() : Exit Sub
            End If
        Next i

        Dim boundaries As Rectangle = ClientRectangle
        boundaries.Height -= 25
        If Not boundaries.Contains(SnakeSegments(0)) Then GameOver()
    End Sub

    Private Sub ProduceRedApple()
        Dim xMaxBound As Integer = ClientSize.Width - 15
        Dim yMaxBound As Integer = ClientSize.Height - 40

        RedApplePosition = New Point(GridRandi(xMaxBound), GridRandi(yMaxBound))
        For Each segment As Rectangle In SnakeSegments
            Do Until RedApplePosition <> segment.Location
                RedApplePosition = New Point(GridRandi(xMaxBound), GridRandi(yMaxBound))
            Loop
        Next segment
    End Sub

    Private Sub ProduceGoldenApple()
        Dim xMaxBound As Integer = ClientSize.Width - 15
        Dim yMaxBound As Integer = ClientSize.Height - 40

        Do
            GoldenApplePosition = New Point(GridRandi(xMaxBound), GridRandi(yMaxBound))
        Loop Until GoldenApplePosition <> RedApplePosition

        For Each segment As Rectangle In SnakeSegments
            Do Until GoldenApplePosition <> segment.Location
                GoldenApplePosition = New Point(GridRandi(xMaxBound), GridRandi(yMaxBound))
            Loop
        Next segment
    End Sub

    Private Sub InitializeGame()
        If Controls.Contains(lblGameTitle) Then Controls.Remove(lblGameTitle)
        tmrFrameUpdate.Start()
        SnakeSegments = StartingPositions
        GoldenApplePosition = New Point(15, 315)
        bonusCountdown = random.Next(5, 10)
        playerScore = New Integer
        snakeDirection = Directions.Right
        imgSnakeHead = ActiveSnakeHeadAnim(1)
        ProduceRedApple()
    End Sub

    Private Sub OnFrameUpdate(sender As Object, e As EventArgs) Handles tmrFrameUpdate.Tick
        Static highestScore As New Integer
        MoveSnake() : CheckCollision() : Refresh()
        lblCurrentScore.Text = $"CURRENT SCORE: {playerScore}"

        If playerScore > highestScore Then highestScore = playerScore
        lblHighestScore.Text = $"HIGHEST SCORE: {highestScore}"
    End Sub
End Class
#Enable Warning IDE1006
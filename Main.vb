Option Explicit On
Public Class Main
    Dim mouseX As Integer
    Dim mouseY As Integer
    Dim xAdv As Integer
    Dim yAdv As Integer
    Dim comResponse As String
    Dim byteOut(8) As Byte

    Dim buttonA As Byte
    Dim buttonB As Byte
    Dim LX As Byte
    Dim LY As Byte
    Dim RX As Byte
    Dim RY As Byte
    Dim padD As Byte

    'Dim a As String
    'Dim file As System.IO.StreamWriter

    Dim lastKeyDown As Byte

    Dim comThread As New System.Threading.Thread(AddressOf byteSender)

    Private Sub Main_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        mouseX = Cursor.Position.X
        mouseY = Cursor.Position.Y
    End Sub

    Private Sub Main_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        Timer1.Enabled = False
        'file = My.Computer.FileSystem.OpenTextFileWriter("c:\WebCam\test.txt", False)
        LX = 128
        LY = 128
        padD = 8
    End Sub

    Sub Main_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        Select Case e.Button
            'button ZR
            Case MouseButtons.Left
                buttonA = buttonA + 128
                'button R
            Case MouseButtons.Right
                buttonA = buttonA + 32
        End Select
    End Sub

    Sub Main_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp
        Select Case e.Button
            Case MouseButtons.Left
                buttonA = buttonA - 128
            Case MouseButtons.Right
                buttonA = buttonA - 32
        End Select
    End Sub

    Private Sub Main_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
        If lastKeyDown <> e.KeyCode Then
            lastKeyDown = e.KeyCode
            Select Case e.KeyCode
                'locks/unlocks mouse
                Case Keys.D0
                    If Timer1.Enabled = False Then
                        Timer1.Enabled = True
                        SerialPort1.Open()
                        comThread.Start()

                    Else
                        Timer1.Enabled = False
                        comThread.Abort()
                        comThread = New System.Threading.Thread(AddressOf byteSender)
                        SerialPort1.Close()
                        'file.WriteLine(a)
                    End If
                    'direction movement, left stick
                Case Keys.W
                    LY = 0
                Case Keys.A
                    LX = 0
                Case Keys.S
                    LY = 255
                Case Keys.D
                    LX = 255

                    'button B
                Case Keys.Space
                    buttonA = buttonA + 2
                    'button Y
                Case Keys.Enter
                    buttonA = buttonA + 1
                    'button X
                Case Keys.Tab
                    e.SuppressKeyPress = True
                    buttonA = buttonA + 8
                    'button L
                Case Keys.D3
                    buttonA = buttonA + 16
                    'button A
                Case Keys.E
                    buttonA = buttonA + 4
                    'button ZL
                Case Keys.ShiftKey
                    If buttonA And 64 Then
                        buttonA = buttonA - 64
                    Else
                        buttonA = buttonA + 64
                    End If





                    'button - 
                Case Keys.H
                    buttonB = buttonB + 1
                    'button +
                Case Keys.L
                    buttonB = buttonB + 2
                    'L click
                Case Keys.J
                    buttonB = buttonB + 4
                    'button Rclick
                Case Keys.R
                    buttonB = buttonB + 8
                    'home
                Case Keys.B
                    buttonB = buttonB + 16
                    'capture
                Case Keys.D1
                    buttonB = buttonB + 32

                Case Keys.Up
                    padD = 0
                Case Keys.Down
                    padD = 4
                Case Keys.Left
                    padD = 6
                Case Keys.Right
                    padD = 2
            End Select
        End If
    End Sub

    Private Sub Main_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyUp
        Select Case e.KeyCode
            'direction movement, left stick
            Case Keys.W
                LY = 128
            Case Keys.A
                If LX = 0 Then
                    LX = 128
                End If
            Case Keys.S
                LY = 128
            Case Keys.D
                If LX = 255 Then
                    LX = 128
                End If

                'button B
            Case Keys.Space
                buttonA = buttonA - 2
                'button Y
            Case Keys.Enter
                buttonA = buttonA - 1
                'button X
            Case Keys.Tab
                e.SuppressKeyPress = True
                buttonA = buttonA - 8
                'button L
            Case Keys.D3
                buttonA = buttonA - 16
                'button R
            Case Keys.E
                buttonA = buttonA - 4
                'button ZL
                'Case Keys.ShiftKey
                'buttonA = buttonA - 64





                'button - 
            Case Keys.H
                buttonB = buttonB - 1
                'button +
            Case Keys.L
                buttonB = buttonB - 2
                'L click
            Case Keys.J
                buttonB = buttonB - 4
                'button Rclick
            Case Keys.R
                buttonB = buttonB - 8
                'home
            Case Keys.B
                buttonB = buttonB - 16
                'capture
            Case Keys.D1
                buttonB = buttonB - 32

            Case Keys.Up
                padD = 8
            Case Keys.Down
                padD = 8
            Case Keys.Left
                padD = 8
            Case Keys.Right
                padD = 8

        End Select
        If lastKeyDown = e.KeyCode Then
            'The last key has been released so forget it.
            lastKeyDown = 0
        End If
    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        xAdv = 128 - (Screen.PrimaryScreen.Bounds.Width / 2 - mouseX) * 3
        yAdv = 128 - (Screen.PrimaryScreen.Bounds.Height / 2 - mouseY) * 3.5
        RX = setLimit(xAdv)
        RY = setLimit(yAdv)
        If RY < 128 And RY > 108 Then
            RY = 108
        End If
        If RY > 128 And RY < 148 Then
            RY = 148
        End If
        Cursor.Position = New Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2)
        Label1.Text = LX
        Label2.Text = LY
        Label3.Text = RX
        Label4.Text = RY
        Label5.Text = buttonA
        Label6.Text = buttonB

        byteOut(0) = &H53
        byteOut(1) = buttonA
        byteOut(2) = buttonB
        byteOut(3) = RX
        byteOut(4) = RY
        byteOut(5) = LX
        byteOut(6) = LY
        byteOut(7) = padD
        byteOut(8) = 0

    End Sub


    Sub byteSender()
        Do
            'Label7.Text = ""
            'comResponse = SerialPort1.ReadExisting
            'Label7.Text = comResponse
            'a = a & comResponse
            SerialPort1.Write(byteOut, 0, 9)
        Loop
    End Sub

    Function setLimit(ByVal valu As Integer)
        If valu > 255 Then
            setLimit = 255
        ElseIf valu < 0 Then
            setLimit = 0
        ElseIf valu = 83 Then
            setLimit = 84
        Else
            setLimit = valu
        End If
    End Function

End Class
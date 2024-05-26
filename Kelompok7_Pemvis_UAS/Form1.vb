Imports System.Data.SqlClient
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class Form1
    Private isAddNew As Boolean
    Private connectionString As String = "Server=YOUR_SERVER;Database=uaspemvis;Uid=YOUR_USERNAME;Pwd=YOUR_PASSWORD;"
    Private dataTable As DataTable

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dataTable = New DataTable()
        dataTable.Columns.Add("NIM", GetType(String))
        dataTable.Columns.Add("Nama", GetType(String))
        dataTable.Columns.Add("JenisKelamin", GetType(String))
        dataTable.Columns.Add("ProgramStudi", GetType(String))
        dataTable.Columns.Add("PernahMengulang", GetType(Boolean))
        dataTable.Columns.Add("IPK", GetType(Double))
        DataGridView1.DataSource = dataTable
    End Sub

    Private Sub LoadIpkPerJurusan()
        Using connection As New MySqlConnection(connectionString)
            connection.Open()
            Dim query As String = "SELECT Nama, ProgramStudi, IPK FROM tabel_data ORDER BY IPK DESC"
            Using command As New MySqlCommand(query, connection)
                Using reader As MySqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim nama As String = reader("Nama").ToString()
                        Dim programStudi As String = reader("ProgramStudi").ToString()
                        Dim ipk As Double = Convert.ToDouble(reader("IPK"))
                        Dim listItem As String = $"Nama: {nama}, Program Studi: {programStudi}, IPK: {ipk:F2}"
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub btnInfoIPK_Click(sender As Object, e As EventArgs) Handles btnInfoIPK.Click
        LoadIpkPerJurusan()
    End Sub

    Private Sub txtNIM_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtNIM.KeyPress
        If Not Char.IsDigit(e.KeyChar) And e.KeyChar <> ControlChars.Back Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtNama_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtNama.KeyPress
        If Not Char.IsLetter(e.KeyChar) And e.KeyChar <> ControlChars.Back And e.KeyChar <> " " Then
            e.Handled = True
        End If
    End Sub

    Private Sub btnUnggah_Click(sender As Object, e As EventArgs) Handles btnUnggah.Click
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "Gambar|*.jpg;*.png;*.jpeg"

        If openFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            pbFoto.Image = Image.FromFile(openFileDialog.FileName)
        End If
    End Sub

    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        Me.Close()
    End Sub

    Private Sub btnInput_Click(sender As Object, e As EventArgs) Handles btnInput.Click
        PanelInput.Visible = True
        PanelGrafik.Visible = False
        PanelInformasi.Visible = False
        PanelIjazahdankelulusan.Visible = False
    End Sub

    'Private Sub btnGrafik_Click(sender As Object, e As EventArgs) Handles btnGrafik.Click
    '    PanelInput.Visible = False
    '    PanelGrafik.Visible = True
    '    PanelInformasi.Visible = False
    '    PanelIjazahdankelulusan.Visible = False
    '    LoadChart()
    'End Sub

    Private Sub btnInformasi_Click(sender As Object, e As EventArgs) Handles btnInformasi.Click
        PanelInput.Visible = False
        PanelGrafik.Visible = False
        PanelInformasi.Visible = True
        PanelIjazahdankelulusan.Visible = False
        LoadIpkPerJurusan()
    End Sub

    Private Sub btnIjazah_Click(sender As Object, e As EventArgs) Handles btnIjazah.Click
        PanelInput.Visible = False
        PanelGrafik.Visible = False
        PanelInformasi.Visible = False
        PanelIjazahdankelulusan.Visible = True
    End Sub

    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        isAddNew = True
        If String.IsNullOrWhiteSpace(txtNIM.Text) Then
            MessageBox.Show("Silakan masukkan NIM.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If String.IsNullOrWhiteSpace(txtNama.Text) Then
            MessageBox.Show("Silakan masukkan Nama Lengkap.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim jenisKelamin As String = ""
        If rbLaki.Checked Then
            jenisKelamin = "Laki-Laki"
        ElseIf rbPerempuan.Checked Then
            jenisKelamin = "Perempuan"
        Else
            MessageBox.Show("Silakan pilih Jenis Kelamin.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If cbProdi.SelectedIndex = -1 Then
            MessageBox.Show("Silakan pilih Program Studi.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim pernahMengulang As Boolean
        If rbYa.Checked Then
            pernahMengulang = True
        ElseIf rbTidak.Checked Then
            pernahMengulang = False
        Else
            MessageBox.Show("Silakan pilih apakah pernah mengulang atau tidak.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If String.IsNullOrWhiteSpace(txtIpk.Text) Then
            MessageBox.Show("Silakan masukkan IPK.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim nim As String = txtNIM.Text
        Dim nama As String = txtNama.Text
        Dim prodi As String = cbProdi.SelectedItem.ToString()
        Dim ipk As Double = Double.Parse(txtIpk.Text)
        Dim row As DataRow = dataTable.NewRow()
        row("NIM") = nim
        row("Nama") = nama
        row("JenisKelamin") = jenisKelamin
        row("ProgramStudi") = prodi
        row("PernahMengulang") = pernahMengulang
        row("IPK") = ipk
        dataTable.Rows.Add(row)
        SaveToDatabase(row)
        ClearInputFields()
    End Sub

    Private Sub ClearInputFields()
        txtNIM.Clear()
        txtNama.Clear()
        TextBox1.Clear()
        cbProdi.SelectedIndex = -1
        rbLaki.Checked = False
        rbPerempuan.Checked = False
        rbYa.Checked = False
        rbTidak.Checked = False
        txtIpk.Clear()
        pbFoto.Image = Nothing
    End Sub

    Private Sub SaveToDatabase(row As DataRow)
        Using connection As New MySqlConnection(connectionString)
            connection.Open()
            Dim query As String = "INSERT INTO tabel_data (nim, nama, jenis_kelamin, prodi, pernah_mengulang, ipk, gambar) VALUES (@nim, @nama, @jenis_kelamin, @prodi, @pernah_mengulang, @ipk, @gambar)"
            Using command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@nim", row("NIM"))
                command.Parameters.AddWithValue("@nama", row("Nama"))
                command.Parameters.AddWithValue("@jenis_kelamin", row("JenisKelamin"))
                command.Parameters.AddWithValue("@prodi", row("ProgramStudi"))
                command.Parameters.AddWithValue("@pernah_mengulang", row("PernahMengulang"))
                command.Parameters.AddWithValue("@ipk", row("IPK"))
                If pbFoto.Image IsNot Nothing Then
                    command.Parameters.AddWithValue("@gambar", ImageToByteArray(pbFoto.Image))
                Else
                    command.Parameters.AddWithValue("@gambar", DBNull.Value)
                End If

                command.ExecuteNonQuery()
                MessageBox.Show("Data berhasil ditambahkan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
        End Using
    End Sub


    Private Function ImageToByteArray(image As Image) As Byte()
        Using ms As New MemoryStream()
            image.Save(ms, image.RawFormat)
            Return ms.ToArray()
        End Using
    End Function

    Private Sub btnPensil_Click(sender As Object, e As EventArgs) Handles btnPensil.Click
        isAddNew = False
        If String.IsNullOrWhiteSpace(txtNIM.Text) Then
            MessageBox.Show("Silakan masukkan NIM.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim nim As String = txtNIM.Text.Trim()
        Using connection As New MySqlConnection(connectionString)
            connection.Open()
            Dim query As String = "SELECT * FROM tabel_data WHERE nim = @nim"
            Using command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@nim", nim)
                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Dim nama As String = reader("nama").ToString()
                        Dim jenisKelamin As String = reader("jenis_kelamin").ToString()
                        Dim prodi As String = reader("prodi").ToString()
                        Dim pernahMengulang As String = reader("pernah_mengulang").ToString()
                        Dim ipk As Double = Convert.ToDouble(reader("ipk"))
                        txtNama.Text = nama
                        If jenisKelamin = "Laki-laki" Then
                            rbLaki.Checked = True
                        ElseIf jenisKelamin = "Perempuan" Then
                            rbPerempuan.Checked = True
                        End If
                        cbProdi.SelectedItem = prodi
                        If pernahMengulang = "ya" Then
                            rbYa.Checked = True
                        ElseIf pernahMengulang = "tidak" Then
                            rbTidak.Checked = True
                        End If
                        txtIpk.Text = ipk.ToString()
                    Else
                        MessageBox.Show("Data tidak ditemukan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            koneksi()
            If isAddNew Then
                For Each row As DataRow In dataTable.Rows
                    If String.IsNullOrEmpty(row("NIM").ToString()) Then
                        tambahdata(row)
                    End If
                Next
                MessageBox.Show("Data baru berhasil ditambahkan ke dalam database.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                For Each row As DataRow In dataTable.Rows
                    If Not String.IsNullOrEmpty(row("NIM").ToString()) Then
                        ubahdata(row)
                    End If
                Next
                MessageBox.Show("Perubahan berhasil disimpan ke dalam database.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Finally
            If CONN.State = ConnectionState.Open Then
                CONN.Close()
            End If
        End Try
    End Sub

    Private Sub ubahdata(row As DataRow)
        STR = "UPDATE tabel_data SET nama = @nama, jenis_kelamin = @jenis_kelamin, prodi = @prodi, pernah_mengulang = @pernah_mengulang, ipk = @ipk, gambar = @gambar WHERE nim = @nim"
        CMD = New MySqlCommand(STR, CONN)
        CMD.Parameters.AddWithValue("@nim", row("NIM"))
        CMD.Parameters.AddWithValue("@nama", row("Nama"))
        CMD.Parameters.AddWithValue("@jenis_kelamin", row("JenisKelamin"))
        CMD.Parameters.AddWithValue("@prodi", row("ProgramStudi"))
        CMD.Parameters.AddWithValue("@pernah_mengulang", row("PernahMengulang"))
        CMD.Parameters.AddWithValue("@ipk", row("IPK"))
        If pbFoto.Image IsNot Nothing Then
            CMD.Parameters.AddWithValue("@gambar", ImageToByteArray(pbFoto.Image))
        Else
            CMD.Parameters.AddWithValue("@gambar", DBNull.Value)
        End If
        CMD.ExecuteNonQuery()
    End Sub

    Private Sub tambahdata(row As DataRow)
        STR = "INSERT INTO tabel_data (nim, nama, jenis_kelamin, prodi, pernah_mengulang, ipk, gambar) VALUES (@nim, @nama, @jenis_kelamin, @prodi, @pernah_mengulang, @ipk, @gambar)"
        CMD = New MySqlCommand(STR, CONN)
        CMD.Parameters.AddWithValue("@nim", row("NIM"))
        CMD.Parameters.AddWithValue("@nama", row("Nama"))
        CMD.Parameters.AddWithValue("@jenis_kelamin", row("JenisKelamin"))
        CMD.Parameters.AddWithValue("@prodi", row("ProgramStudi"))
        CMD.Parameters.AddWithValue("@pernah_mengulang", row("PernahMengulang"))
        CMD.Parameters.AddWithValue("@ipk", row("IPK"))
        If pbFoto.Image IsNot Nothing Then
            CMD.Parameters.AddWithValue("@gambar", ImageToByteArray(pbFoto.Image))
        Else
            CMD.Parameters.AddWithValue("@gambar", DBNull.Value)
        End If
        CMD.ExecuteNonQuery()
    End Sub


    'Private Sub LoadChart()
    '    Chart1.Series.Clear()
    '    Chart1.Series.Add("Jumlah Mahasiswa")
    '    Chart1.Series("Jumlah Mahasiswa").ChartType = SeriesChartType.Column

    '    Dim dataByProdi As New Dictionary(Of String, Integer)()

    '    Using connection As New MySqlConnection(connectionString)
    '        connection.Open()
    '        Dim query As String = "SELECT prodi, COUNT(*) AS jumlah FROM tabel_data GROUP BY prodi"
    '        Using command As New MySqlCommand(query, connection)
    '            Using reader As MySqlDataReader = command.ExecuteReader()
    '                While reader.Read()
    '                    Dim prodi As String = reader("prodi").ToString()
    '                    Dim jumlah As Integer = Convert.ToInt32(reader("jumlah"))
    '                    dataByProdi(prodi) = jumlah
    '                End While
    '            End Using
    '        End Using
    '    End Using

    '    For Each kvp As KeyValuePair(Of String, Integer) In dataByProdi
    '        Chart1.Series("Jumlah Mahasiswa").Points.AddXY(kvp.Key, kvp.Value)
    '    Next
    'End Sub

End Class

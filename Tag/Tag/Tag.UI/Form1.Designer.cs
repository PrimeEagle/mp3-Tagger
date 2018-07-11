namespace Tag.UI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FolderPath = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.BrowseFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.LookupButton = new System.Windows.Forms.Button();
            this.UseAlbumFolder = new System.Windows.Forms.CheckBox();
            this.SelectAll = new System.Windows.Forms.CheckBox();
            this.SaveChanges = new System.Windows.Forms.Button();
            this.GridView = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Filename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrigTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrigArtist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Artist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrigAlbum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Album = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Publisher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Year = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Genre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Styles = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Moods = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Themes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Instruments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lyrics = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UniqueID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Classical = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            this.SuspendLayout();
            // 
            // FolderPath
            // 
            this.FolderPath.Location = new System.Drawing.Point(12, 12);
            this.FolderPath.Name = "FolderPath";
            this.FolderPath.Size = new System.Drawing.Size(240, 20);
            this.FolderPath.TabIndex = 0;
            this.FolderPath.TextChanged += new System.EventHandler(this.FolderPath_TextChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(467, 47);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(163, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(258, 10);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(31, 23);
            this.BrowseButton.TabIndex = 2;
            this.BrowseButton.Text = "...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // LookupButton
            // 
            this.LookupButton.Enabled = false;
            this.LookupButton.Location = new System.Drawing.Point(308, 9);
            this.LookupButton.Name = "LookupButton";
            this.LookupButton.Size = new System.Drawing.Size(120, 23);
            this.LookupButton.TabIndex = 4;
            this.LookupButton.Text = "Lookup AMG Data";
            this.LookupButton.UseVisualStyleBackColor = true;
            this.LookupButton.Click += new System.EventHandler(this.LookupButton_Click);
            // 
            // UseAlbumFolder
            // 
            this.UseAlbumFolder.AutoSize = true;
            this.UseAlbumFolder.Location = new System.Drawing.Point(467, 12);
            this.UseAlbumFolder.Name = "UseAlbumFolder";
            this.UseAlbumFolder.Size = new System.Drawing.Size(163, 17);
            this.UseAlbumFolder.TabIndex = 6;
            this.UseAlbumFolder.Text = "Use Parent Folder as Album?";
            this.UseAlbumFolder.UseVisualStyleBackColor = true;
            // 
            // SelectAll
            // 
            this.SelectAll.AutoSize = true;
            this.SelectAll.Checked = true;
            this.SelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectAll.Location = new System.Drawing.Point(46, 66);
            this.SelectAll.Name = "SelectAll";
            this.SelectAll.Size = new System.Drawing.Size(70, 17);
            this.SelectAll.TabIndex = 7;
            this.SelectAll.Text = "Select All";
            this.SelectAll.UseVisualStyleBackColor = true;
            this.SelectAll.CheckedChanged += new System.EventHandler(this.SelectAll_CheckedChanged);
            // 
            // SaveChanges
            // 
            this.SaveChanges.Location = new System.Drawing.Point(308, 47);
            this.SaveChanges.Name = "SaveChanges";
            this.SaveChanges.Size = new System.Drawing.Size(120, 23);
            this.SaveChanges.TabIndex = 8;
            this.SaveChanges.Text = "Save Changes";
            this.SaveChanges.UseVisualStyleBackColor = true;
            this.SaveChanges.Click += new System.EventHandler(this.SaveChanges_Click);
            // 
            // GridView
            // 
            this.GridView.AllowUserToAddRows = false;
            this.GridView.AllowUserToDeleteRows = false;
            this.GridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.Filename,
            this.OrigTitle,
            this.Title,
            this.OrigArtist,
            this.Artist,
            this.OrigAlbum,
            this.Album,
            this.Publisher,
            this.TrackNo,
            this.Year,
            this.Genre,
            this.Styles,
            this.Moods,
            this.Themes,
            this.Instruments,
            this.Lyrics,
            this.UniqueID});
            this.GridView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.GridView.Location = new System.Drawing.Point(0, 101);
            this.GridView.Name = "GridView";
            this.GridView.Size = new System.Drawing.Size(1068, 375);
            this.GridView.TabIndex = 9;

            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 43;
            // 
            // Filename
            // 
            this.Filename.HeaderText = "Filename";
            this.Filename.Name = "Filename";
            this.Filename.ReadOnly = true;
            this.Filename.Width = 74;
            // 
            // OrigTitle
            // 
            this.OrigTitle.HeaderText = "OrigTitle";
            this.OrigTitle.Name = "OrigTitle";
            this.OrigTitle.ReadOnly = true;
            this.OrigTitle.Width = 71;
            // 
            // Title
            // 
            this.Title.HeaderText = "Title";
            this.Title.Name = "Title";
            this.Title.Width = 52;
            // 
            // OrigArtist
            // 
            this.OrigArtist.HeaderText = "OrigArtist";
            this.OrigArtist.Name = "OrigArtist";
            this.OrigArtist.ReadOnly = true;
            this.OrigArtist.Width = 74;
            // 
            // Artist
            // 
            this.Artist.HeaderText = "Artist";
            this.Artist.Name = "Artist";
            this.Artist.Width = 55;
            // 
            // OrigAlbum
            // 
            this.OrigAlbum.HeaderText = "OrigAlbum";
            this.OrigAlbum.Name = "OrigAlbum";
            this.OrigAlbum.ReadOnly = true;
            this.OrigAlbum.Width = 80;
            // 
            // Album
            // 
            this.Album.HeaderText = "Album";
            this.Album.Name = "Album";
            this.Album.Width = 61;
            // 
            // Publisher
            // 
            this.Publisher.HeaderText = "Publisher";
            this.Publisher.Name = "Publisher";
            this.Publisher.Width = 75;
            // 
            // TrackNo
            // 
            this.TrackNo.HeaderText = "TrackNo";
            this.TrackNo.Name = "TrackNo";
            this.TrackNo.Width = 74;
            // 
            // Year
            // 
            this.Year.HeaderText = "Year";
            this.Year.Name = "Year";
            this.Year.Width = 54;
            // 
            // Genre
            // 
            this.Genre.HeaderText = "Genre";
            this.Genre.Name = "Genre";
            this.Genre.Width = 61;
            // 
            // Styles
            // 
            this.Styles.HeaderText = "Styles";
            this.Styles.Name = "Styles";
            this.Styles.Width = 60;
            // 
            // Moods
            // 
            this.Moods.HeaderText = "Moods";
            this.Moods.Name = "Moods";
            this.Moods.Width = 64;
            // 
            // Themes
            // 
            this.Themes.HeaderText = "Themes";
            this.Themes.Name = "Themes";
            this.Themes.Width = 70;
            // 
            // Instruments
            // 
            this.Instruments.HeaderText = "Instruments";
            this.Instruments.Name = "Instruments";
            this.Instruments.Width = 86;
            // 
            // Lyrics
            // 
            this.Lyrics.HeaderText = "Lyrics";
            this.Lyrics.Name = "Lyrics";
            this.Lyrics.Width = 59;
            // 
            // UniqueID
            // 
            this.UniqueID.HeaderText = "UniqueID";
            this.UniqueID.Name = "UniqueID";
            this.UniqueID.ReadOnly = true;
            this.UniqueID.Visible = false;
            this.UniqueID.Width = 77;
            // 
            // Classical
            // 
            this.Classical.AutoSize = true;
            this.Classical.Location = new System.Drawing.Point(683, 10);
            this.Classical.Name = "Classical";
            this.Classical.Size = new System.Drawing.Size(155, 17);
            this.Classical.TabIndex = 10;
            this.Classical.Text = "Lookup as Classical Work?";
            this.Classical.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 476);
            this.Controls.Add(this.Classical);
            this.Controls.Add(this.GridView);
            this.Controls.Add(this.SaveChanges);
            this.Controls.Add(this.SelectAll);
            this.Controls.Add(this.UseAlbumFolder);
            this.Controls.Add(this.LookupButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.FolderPath);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FolderPath;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.FolderBrowserDialog BrowseFolder;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button LookupButton;
        private System.Windows.Forms.CheckBox UseAlbumFolder;
        private System.Windows.Forms.CheckBox SelectAll;
        private System.Windows.Forms.Button SaveChanges;
        private System.Windows.Forms.DataGridView GridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filename;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrigTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrigArtist;
        private System.Windows.Forms.DataGridViewTextBoxColumn Artist;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrigAlbum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Album;
        private System.Windows.Forms.DataGridViewTextBoxColumn Publisher;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrackNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Year;
        private System.Windows.Forms.DataGridViewTextBoxColumn Genre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Styles;
        private System.Windows.Forms.DataGridViewTextBoxColumn Moods;
        private System.Windows.Forms.DataGridViewTextBoxColumn Themes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Instruments;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lyrics;
        private System.Windows.Forms.DataGridViewTextBoxColumn UniqueID;
        private System.Windows.Forms.CheckBox Classical;
    }
}


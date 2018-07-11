using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tag.AMG;
using Tag.LyricsDotCom;
using Tag.LyricsHead;
using Tag.mp3;
using Tag.Utility;

namespace Tag.UI
{
    public partial class Form1 : Form
    {
        List<MP3File> mp3Files = new List<MP3File>();

        public Form1()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            BrowseFolder.ShowDialog();
            FolderPath.Text = BrowseFolder.SelectedPath;
        }

        private void FolderPath_TextChanged(object sender, EventArgs e)
        {
            if (FolderPath.Text.Length > 0)
            {
                LookupButton.Enabled = true;
            }
            else
            {
                LookupButton.Enabled = false;
            }
        }

        private void LookupButton_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;

            if (Classical.Checked)
            {
                InitializeClassicalGrid();
                ThreadStart process = new ThreadStart(ProcessClassicalFiles);
                Thread processThread = new Thread(process);
                processThread.Start();
            }
            else
            {
                InitializeGrid();
                ThreadStart process = new ThreadStart(ProcessFiles);
                Thread processThread = new Thread(process);
                processThread.Start();
            }
        }

        private void ProcessFiles()
        {
            AmgConnection amgConn = new AmgConnection();
            bool useParentFolderAsAlbum = UseAlbumFolder.Checked;
            List<MP3File> mp3Files = BuildFileList();
            this.initializeProgressBar(0, mp3Files.Count * 9);

            foreach (MP3File mp3File in BuildFileList())
            {
                mp3File.Classical = false;
                string genre = string.Empty;
                StringBuilder styles = new StringBuilder();
                StringBuilder moods = new StringBuilder();
                StringBuilder themes = new StringBuilder();
                StringBuilder instruments = new StringBuilder();
                Song currentSong = null;
                Album currentAlbum = null;
                Artist currentArtist = null;
                string origArtist = string.Empty;
                string origTitle = string.Empty;
                string origAlbum = string.Empty;

                mp3File.InitializeID3Tags();
                origAlbum = mp3File.ID3.Album;
                origArtist = mp3File.ID3.Artist;
                origTitle = mp3File.ID3.SongTitle;

                string parentFolder = GetParentFolder(mp3File.FilePath);
                
                incrementProgressBar(1);
                currentArtist = amgConn.GetArtist(mp3File.ID3.Artist);
                if (currentArtist != null)
                {
                    incrementProgressBar(1);
                    currentSong = amgConn.GetSong(mp3File.ID3.SongTitle, currentArtist.AMGAllSongsUrl);

                    if (currentSong != null)
                    {
                        incrementProgressBar(1);
                        currentAlbum = amgConn.GetAlbum(currentSong.AMGUrl, currentArtist.AMGOverviewUrl, useParentFolderAsAlbum, parentFolder);

                        if (currentAlbum != null)
                        {
                            incrementProgressBar(1);
                            currentSong.AMGAlbumId = currentAlbum.AMGId;
                            currentSong.AMGArtistId = currentArtist.AMGId;
                            
                            incrementProgressBar(1);
                            
                            LyricsDotComConnection lyricsDCConn = new LyricsDotComConnection();
                            string lyrics = lyricsDCConn.GetLyrics(currentArtist.Name, currentAlbum.Name, currentSong.Title);
                            if (lyrics != null)
                            {
                                currentSong.Lyrics = lyrics;
                            }
                            else
                            {
                                LyricsHeadConnection lyricsLHConn = new LyricsHeadConnection();
                                lyrics = lyricsLHConn.GetLyrics(currentArtist.Name, currentSong.Title);
                                if (lyrics != null)
                                {
                                    currentSong.Lyrics = lyrics;
                                }
                            }
                        }

                        incrementProgressBar(1);
                        for (int t = 0; t < currentAlbum.Tracks.Count; t++)
                        {
                            if (Management.NamesMatch(currentAlbum.Tracks[t].Title, currentSong.Title))
                            {
                                currentSong.AlbumTrackNum = t + 1;
                                currentSong.Performer = currentAlbum.Tracks[t].Performer;
                                currentSong.Composer = currentAlbum.Tracks[t].Composer;
                                break;
                            }
                        }

                        incrementProgressBar(1);
                        if (currentAlbum.Compilation)
                        {
                            currentAlbum.AlbumArtist = string.Empty;
                        }
                        else
                        {
                            currentAlbum.AlbumArtist = currentArtist.Name;
                            currentSong.Performer = currentArtist.Name;
                        }
                    }
                }

                if (currentArtist == null)
                {
                    currentArtist = new Artist();
                    incrementProgressBar(6);
                }

                if (currentAlbum == null)
                {
                    currentAlbum = new Album();
                    incrementProgressBar(5);
                }

                if (currentSong == null)
                {
                    currentSong = new Song();
                    incrementProgressBar(4);
                }


                incrementProgressBar(1);
                mp3File.LoadArtistData(currentArtist);
                mp3File.LoadSongData(currentSong);
                mp3File.LoadAlbumData(currentAlbum);
                mp3File.PopulateID3();

                foreach (string style in mp3File.ID3.Styles)
                {
                    styles.Append(style + @";");
                }
                if (styles.Length > 0)
                    styles.Remove(styles.Length - 1, 1);


                foreach (string mood in mp3File.ID3.Moods)
                {
                    moods.Append(mood + @";");
                }
                if (moods.Length > 0)
                    moods.Remove(moods.Length - 1, 1);


                foreach (string theme in mp3File.ID3.Themes)
                {
                    themes.Append(theme + @";");
                }
                if (themes.Length > 0)
                    themes.Remove(themes.Length - 1, 1);


                foreach (string instrument in mp3File.ID3.Instruments)
                {
                    instruments.Append(instrument + @";");
                }
                if (instruments.Length > 0)
                    instruments.Remove(instruments.Length - 1, 1);

                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(GridView, new object[] {true, mp3File.FileName, origTitle, mp3File.ID3.SongTitle, origArtist, mp3File.ID3.Artist,
                        origAlbum, mp3File.ID3.Album, mp3File.ID3.Publisher, mp3File.ID3.TrackNumber, mp3File.ID3.Year, mp3File.ID3.Genre,
                        styles.ToString(), moods.ToString(), themes.ToString(), instruments.ToString(), mp3File.ID3.Lyrics, mp3File.UniqueID} );

                dr.Cells[GridView.Columns["Filename"].Index].Style.BackColor = Color.LightGray;
                dr.Cells[GridView.Columns["OrigTitle"].Index].Style.BackColor = Color.LightGray;
                dr.Cells[GridView.Columns["OrigArtist"].Index].Style.BackColor = Color.LightGray;
                dr.Cells[GridView.Columns["OrigAlbum"].Index].Style.BackColor = Color.LightGray;

                if (mp3File.AMGArtist.Name == string.Empty)
                {
                    dr.Cells[GridView.Columns["Artist"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Instruments"].Index].Style.BackColor = Color.Yellow;
                }

                if (mp3File.AMGAlbum.Name == string.Empty)
                {
                    dr.Cells[GridView.Columns["Album"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["TrackNo"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Year"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Themes"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Publisher"].Index].Style.BackColor = Color.Yellow;
                }

                if (mp3File.AMGSong.Title == string.Empty)
                {
                    dr.Cells[GridView.Columns["Title"].Index].Style.BackColor = Color.Yellow;
                }

                if (mp3File.AMGSong.Title == string.Empty && mp3File.AMGArtist.Name == string.Empty)
                {
                    dr.Cells[GridView.Columns["Lyrics"].Index].Style.BackColor = Color.Yellow;
                }

                if (mp3File.AMGArtist.Name == string.Empty && mp3File.AMGAlbum.Name == string.Empty)
                {
                    dr.Cells[GridView.Columns["Genre"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Styles"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Moods"].Index].Style.BackColor = Color.Yellow;
                }

                incrementProgressBar(1);
                addGridViewRow(dr);
            }

            MessageBox.Show("Done.");
        }

        private void ProcessClassicalFiles()
        {
            AmgConnection amgConn = new AmgConnection();
            bool useParentFolderAsAlbum = UseAlbumFolder.Checked;
            List<MP3File> mp3Files = BuildFileList();

            this.initializeProgressBar(0, mp3Files.Count * 4);

            foreach (MP3File mp3File in mp3Files)
            {
                mp3File.Classical = true;
                string genre = string.Empty;
                StringBuilder styles = new StringBuilder();
                StringBuilder moods = new StringBuilder();
                StringBuilder themes = new StringBuilder();
                StringBuilder instruments = new StringBuilder();
                ClassicalSong currentSong = null;
                ClassicalArtist currentArtist = null;
                string origArtist = string.Empty;
                string origTitle = string.Empty;
                string origAlbum = string.Empty;
                int origTrackNo;
                int origNumTracks;

                mp3File.InitializeID3Tags();
                origAlbum = mp3File.ID3.Album;
                origArtist = mp3File.ID3.Artist;
                origTitle = mp3File.ID3.SongTitle;
                origTrackNo = mp3File.ID3.TrackNumber;
                origNumTracks = mp3File.ID3.TrackCount;

                string parentFolder = GetParentFolder(mp3File.FilePath);

                incrementProgressBar(1);
                currentArtist = amgConn.GetClassicalArtist(mp3File.ID3.Artist);

                if (currentArtist.Name.Contains("Bach"))
                {
                    int aa = 1;
                }
                if (currentArtist != null)
                {
                    incrementProgressBar(1);
                    currentSong = amgConn.GetClassicalSong(mp3File.ID3.SongTitle, currentArtist.AMGAllSongsUrl);
                    currentSong.Composer = currentArtist.Name;
                }

                if (currentArtist == null)
                {
                    currentArtist = new ClassicalArtist();
                    incrementProgressBar(1);
                }

                if (currentSong == null)
                {
                    currentSong = new ClassicalSong();
                }

                if (currentSong.AlbumTrackNum == 0)
                {
                    currentSong.AlbumTrackNum = origTrackNo;
                }

                Album currentAlbum = new Album();
                currentAlbum.Name = origAlbum;

                incrementProgressBar(1);
                mp3File.LoadArtistData(currentArtist);
                mp3File.LoadSongData(currentSong);
                mp3File.LoadAlbumData(currentAlbum);
                mp3File.PopulateID3();

                StringBuilder workTypes = new StringBuilder();
                foreach (string work in currentSong.WorkTypes)
                {
                    workTypes.Append(work + @";");
                }
                if (workTypes.Length > 0)
                    workTypes.Remove(workTypes.Length - 1, 1);

                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(GridView, new object[] {true, mp3File.FileName, origTitle, mp3File.ID3.SongTitle, origArtist, mp3File.ID3.Artist,
                        origAlbum, mp3File.ID3.Album, currentSong.Movement, currentSong.MovementName, origTrackNo, mp3File.ID3.TrackNumber, 
                        mp3File.ID3.Year, currentArtist.Country, mp3File.ID3.Genre, workTypes.ToString(), currentSong.Description, mp3File.UniqueID});

                dr.Cells[GridView.Columns["Filename"].Index].Style.BackColor = Color.LightGray;
                dr.Cells[GridView.Columns["OrigTitle"].Index].Style.BackColor = Color.LightGray;
                dr.Cells[GridView.Columns["OrigArtist"].Index].Style.BackColor = Color.LightGray;
                dr.Cells[GridView.Columns["OrigAlbum"].Index].Style.BackColor = Color.LightGray;

                if (mp3File.AMGClassicalArtist.Name == string.Empty)
                {
                    dr.Cells[GridView.Columns["Artist"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Country"].Index].Style.BackColor = Color.Yellow;
                }

                if (mp3File.AMGClassicalSong.Title == string.Empty)
                {
                    dr.Cells[GridView.Columns["Title"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Genre"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["WorkTypes"].Index].Style.BackColor = Color.Yellow;
                    dr.Cells[GridView.Columns["Description"].Index].Style.BackColor = Color.Yellow;
                }

                incrementProgressBar(1);
                addGridViewRow(dr);
            }

            MessageBox.Show("Done.");
        }

        private void InitializeGrid()
        {
            GridView.AutoGenerateColumns = false;
            GridView.Columns.Clear();

            DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "Select";
            checkCol.HeaderText = "Select";
            checkCol.ReadOnly = false;
            GridView.Columns.Add(checkCol);

            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "Filename";
            col.HeaderText = "Filename";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "OrigTitle";
            col.HeaderText = "OrigTitle";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Title";
            col.HeaderText = "Title";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "OrigArtist";
            col.HeaderText = "OrigArtist";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Artist";
            col.HeaderText = "Artist";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "OrigAlbum";
            col.HeaderText = "OrigAlbum";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Album";
            col.HeaderText = "Album";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Publisher";
            col.HeaderText = "Publisher";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "TrackNo";
            col.HeaderText = "TrackNo";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Year";
            col.HeaderText = "Year";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Genre";
            col.HeaderText = "Genre";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Styles";
            col.HeaderText = "Styles";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Moods";
            col.HeaderText = "Moods";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Themes";
            col.HeaderText = "Themes";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Instruments";
            col.HeaderText = "Instruments";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Lyrics";
            col.HeaderText = "Lyrics";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "UniqueID";
            col.HeaderText = "UniqueID";
            col.ReadOnly = true;
            col.Visible = false;
            GridView.Columns.Add(col);
        }

        private void InitializeClassicalGrid()
        {
            GridView.AutoGenerateColumns = false;
            GridView.Columns.Clear();

            DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "Select";
            checkCol.HeaderText = "Select";
            checkCol.ReadOnly = false;
            GridView.Columns.Add(checkCol);

            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "Filename";
            col.HeaderText = "Filename";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "OrigTitle";
            col.HeaderText = "OrigTitle";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Title";
            col.HeaderText = "Title";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "OrigArtist";
            col.HeaderText = "OrigArtist";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Artist";
            col.HeaderText = "Artist";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "OrigAlbum";
            col.HeaderText = "OrigAlbum";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Album";
            col.HeaderText = "Album";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Movement";
            col.HeaderText = "Movement";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "MovementName";
            col.HeaderText = "MovementName";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "OrigTrackNo";
            col.HeaderText = "OrigTrackNo";
            col.ReadOnly = true;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "TrackNo";
            col.HeaderText = "TrackNo";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Year";
            col.HeaderText = "Year";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Country";
            col.HeaderText = "Country";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Genre";
            col.HeaderText = "Genre";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "WorkTypes";
            col.HeaderText = "WorkTypes";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "Description";
            col.HeaderText = "Description";
            col.ReadOnly = false;
            GridView.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.Name = "UniqueID";
            col.HeaderText = "UniqueID";
            col.ReadOnly = true;
            col.Visible = false;
            GridView.Columns.Add(col);
        }

        delegate void initializeProgressBarDelegate(int min, int max);
        
        private void initializeProgressBar(int min, int max)
        {
            if (progressBar1.InvokeRequired)
            {
                // this is worker thread
                initializeProgressBarDelegate del = new initializeProgressBarDelegate(initializeProgressBar);
                progressBar1.Invoke(del, new object[] { min, max }); 
            } else {     
                // this is UI thread     
                progressBar1.Minimum = min;
                progressBar1.Maximum = max;
            }
        }

        delegate void incrementProgressBarDelegate(int amount);

        private void incrementProgressBar(int amount)
        {
            if (progressBar1.InvokeRequired)
            {
                // this is worker thread
                incrementProgressBarDelegate del = new incrementProgressBarDelegate(incrementProgressBar);
                progressBar1.Invoke(del, new object[] { amount });
            }
            else
            {
                // this is UI thread     
                progressBar1.Value = progressBar1.Value + amount;
            }
        }

        delegate void addGridViewRowDelegate(DataGridViewRow dr);

        private void addGridViewRow(DataGridViewRow dr)
        {
            if (GridView.InvokeRequired)
            {
                // this is worker thread
                addGridViewRowDelegate del = new addGridViewRowDelegate(addGridViewRow);
                GridView.Invoke(del, new object[] { dr });
            }
            else
            {
                // this is UI thread     
                GridView.Rows.Add(dr);
            }
        }

        private void SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool check = SelectAll.Checked;

            foreach (DataGridViewRow r in GridView.Rows)
            {
                r.Cells[0].Value = check;
            }
        }

        private void SaveChanges_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in GridView.Rows)
            {
                if (Convert.ToBoolean(r.Cells[GridView.Columns["Select"].Index].Value) == true)
                {
                    MP3File file = GetFileByID(new Guid(r.Cells[GridView.Columns["UniqueID"].Index].Value.ToString()));
                    if (file != null)
                    {
                        file.ID3.Album = r.Cells["Album"].Value.ToString();
                        file.ID3.Artist = r.Cells["Artist"].Value.ToString();
                        file.ID3.SongTitle = r.Cells["Title"].Value.ToString();
                        file.ID3.TrackNumber = Convert.ToInt32(r.Cells["TrackNo"].Value);
                        file.ID3.Year = Convert.ToInt32(r.Cells["Year"].Value.ToString());
                        file.ID3.Lyrics = r.Cells["Lyrics"].Value.ToString();
                        file.ID3.Publisher = r.Cells["Publisher"].Value.ToString();

                        file.Save();
                    }
                }
            }
        }

        private MP3File GetFileByID(Guid id)
        {
            MP3File file = mp3Files.Find(
                delegate(MP3File mp3) 
                {
                    return (id == mp3.UniqueID); 
                }
            );
            
            return file;
        }

        private List<MP3File> BuildFileList()
        {
            List<MP3File> mp3Files = new List<MP3File>();
            List<string> files = new List<string>();

            FileManagement.GetFiles(FolderPath.Text, true, files);

            foreach (string file in files)
            {
                if (file.ToUpper().EndsWith(".MP3"))
                {
                    MP3File mp3 = new MP3File(file);
                    mp3Files.Add(mp3);
                }
            }

            return mp3Files;
        }

        private string GetParentFolder(string path)
        {
            string parentFolder = System.IO.Path.GetDirectoryName(path);
            int lastIdx = parentFolder.LastIndexOf(@"\");
            parentFolder = parentFolder.Substring(lastIdx + 1);

            return parentFolder;
        }

    }
}

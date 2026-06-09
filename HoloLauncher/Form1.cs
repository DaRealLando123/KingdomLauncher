using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KingdomLauncher {
    public partial class Form1: Form {

        string launcherFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KingdomLauncher");
        string versionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KingdomLauncher", "HoloDemo1");

        public Form1() {
            InitializeComponent();
            label1.BackColor = Color.FromArgb(150, 0, 0, 0);
            //pictureBox1.Location = new Point(ClientSize.Width / 2 - pictureBox1.Width / 2, pictureBox1.Location.Y);
            PositionButton();
            FetchVersions();
        }

        /*
        1. Get game ISO from user
        2. Copy to temp directory
        3. Download patch zip file
        4. Extract patch zip file
        5. Download English patch file
        6. Download mod launcher .exe
        7. Launch mod launcher .exe and automatically patch the ISO 
        8. Download PCSX2 zip
        9. Extract PCSX2 zip
        10. Delete temp file
         */
        async private void btn_InstallPlay_Click(object sender, EventArgs e) {
            if (btn_InstallPlay.Text == "Play") {

                if (!DetectValidBIOS()) {

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "BIOS files (*.bin)|*.bin";
                    ofd.Title = "Select the Playstation 2 scph39001 BIOS file you wish to use.";

                    var result = MessageBox.Show("A legally obtained Playstation 2 scph39001 BIOS is needed to launch the game. Would you like to select one now?\n\nThe BIOS will be added to " + Path.Combine(versionFolder, "PCSX2", "bios"), "A Playstation 2 scph39001 BIOS is required.", MessageBoxButtons.YesNo);
                    Debug.WriteLine(result);

                    if (result == DialogResult.Yes)
                    {
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            await Task.Run(() =>
                            {
                                File.Copy(ofd.FileName, Path.Combine(versionFolder, "PCSX2", "BIOS", Path.GetFileName(ofd.FileName)), true);
                            });
                        }
                    }
                } else {

                    var psi = new ProcessStartInfo {
                        FileName = Path.Combine(versionFolder, "PCSX2", "pcsx2.exe"),
                        Arguments = "-portable -batch KH2FM.NEW.ISO",
                        WorkingDirectory = Path.Combine(versionFolder),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true
                    };
                    var process = Process.Start(psi) ?? throw new InvalidOperationException();

                    Close();

                }

            } else {
                await InstallProcess();
            }
        }

        private async Task FetchVersions() {
            box_version.Items.Clear();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("KingdomLauncher");

            try {
                string jsonResponse = await client.GetStringAsync("https://api.github.com/repos/DaRealLando123/DaysFMMirror/releases");
                JsonDocument docs = JsonDocument.Parse(jsonResponse);

                foreach (JsonElement release in docs.RootElement.EnumerateArray()) {
                    if (release.TryGetProperty("tag_name", out JsonElement tagElement)) {
                        string version = tagElement.GetString();

                        box_version.Items.Add(version);
                    }
                }
                box_version.SelectedIndex = 0;
                Debug.WriteLine(box_version.SelectedItem.ToString());
                versionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KingdomLauncher", box_version.SelectedItem.ToString());
            } catch (Exception ex) {
                string[] folders = Directory.GetDirectories(launcherFolder);

                foreach (string folderPath in folders) {
                    string folderName = Path.GetFileName(folderPath);

                    box_version.Items.Add(folderName);
                }

                if (box_version.Items.Count <= 0) {
                    MessageBox.Show("Couldn't load the online version list.\nIt could be that your internet or GitHub is down.\n\nSince you have nothing installed, the launcher cannot run offline. Try reconnecting your wifi or retrying after a few minutes.\n\nAfter you install a version, you can run the launcher offline.", "Error loading version list", MessageBoxButtons.OK);
                    this.Close();
                } else {
                    MessageBox.Show("Couldn't load the online version list.\nIt could be that your internet or GitHub is down.\n\nOnly currently installed versions will be shown.", "Error loading version list", MessageBoxButtons.OK);
                    box_version.SelectedIndex = 0;
                }
            }
            DetectValidInstall();
        }

        private void DetectValidInstall() {
            if (File.Exists(Path.Combine(versionFolder, "KH2FM.NEW.ISO")) && Directory.Exists(Path.Combine(versionFolder, "PCSX2"))) {
                btn_InstallPlay.Text = "Play";
            } else {
                btn_InstallPlay.Text = "Install";
            }
            label1.Visible = false;
            progressBar1.Visible = false;
            btn_InstallPlay.Visible = true;
            box_version.Visible = true;

            if (Directory.Exists(Path.Combine(versionFolder))) {
                btn_del.Visible = true;
                btn_dir.Visible = true;
            } else {
                btn_del.Visible = false;
                btn_dir.Visible = false;
            }
        }

        private bool DetectValidBIOS() {
            return Directory.EnumerateFiles(Path.Combine(versionFolder, "PCSX2", "bios"),"*.bin").Any();
        }

        private async Task InstallProcess() {

            var newIsoPath = Path.Combine(versionFolder, "KH2FM.NEW.ISO");
            if (File.Exists(newIsoPath)) {
                File.Delete(newIsoPath);
            }

            label1.Visible = true;
            progressBar1.Visible = true;
            btn_InstallPlay.Visible = false;
            box_version.Visible = false;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "ISO files (*.iso)|*.iso";
            ofd.Title = "Select the Kingdom Hearts II - Final Mix+ (Japan) game ISO file you wish to use";

            label1.Text = "Choosing ISO...";

            var result = MessageBox.Show("Kingdom Hearts II - Final Mix+ (Japan) is required.\nA legally obtained ISO is needed to continue.", "ISO Selection", MessageBoxButtons.OKCancel);
            Debug.WriteLine(result);
            if (result != DialogResult.OK) {
                DetectValidInstall();
                return;
            }

            if (ofd.ShowDialog() != DialogResult.OK) {
                DetectValidInstall();
                return;
            }

            CreateDirectories();

            label1.Text = "Copying ISO... (this is slow)";

            progressBar1.Style = ProgressBarStyle.Marquee;

            await Task.Run(() => {
                File.Copy(ofd.FileName, Path.Combine(versionFolder, Path.GetFileName("KH2FM.ISO")), true);
            });

            progressBar1.Style = ProgressBarStyle.Continuous;

            var progress = new Progress<int>(value => {
                progressBar1.Value = value;
            });

            label1.Text = "Downloading (1/4) DaysFM...";

            await DownloadFromURL("https://github.com/DaRealLando123/DaysFMMirror/releases/download/"+ box_version.SelectedItem.ToString() + "/mod.7z", Path.Combine(versionFolder, "mod.7z"), progress);

            Task extractTask1 = Task.Run(() =>
            {
                var archive = SharpCompress.Archives.ArchiveFactory.Open(Path.Combine(versionFolder, "mod.7z"));

                archive.WriteToDirectory(
                    versionFolder,
                    new SharpCompress.Common.ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    }
                );
            });


            label1.Text = "Downloading (2/4) PCSX2...";

            await DownloadFromURL("https://github.com/DaRealLando123/DaysFMMirror/releases/download/" + box_version.SelectedItem.ToString() + "/PCSX2.7z", Path.Combine(versionFolder, "PCSX2.7z"), progress);

            Task extractTask2 = Task.Run(() =>
            {
                var archive = SharpCompress.Archives.ArchiveFactory.Open(Path.Combine(versionFolder, "PCSX2.7z"));

                Directory.CreateDirectory(Path.Combine(versionFolder, "PCSX2"));

                archive.WriteToDirectory(
                    Path.Combine(versionFolder, "PCSX2"),
                    new SharpCompress.Common.ExtractionOptions {
                        ExtractFullPath = true,
                        Overwrite = true
                    }
                );
            });


            label1.Text = "Downloading (3/4) Toolkit...";

            await DownloadFromURL("https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/KH2FM.Toolkit.exe", Path.Combine(versionFolder, "KH2FM.Toolkit.exe"), progress);

            label1.Text = "Downloading (4/4) English patch...";

            await DownloadFromURL("https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/English.Patch.kh2patch", Path.Combine(versionFolder, "English.Patch.kh2patch"), progress);

            label1.Text = "Extracting (1/2) PCSX2... (This is slow!)";

            progressBar1.Style = ProgressBarStyle.Marquee;

            await extractTask2;

            // HERE

            label1.Text = "Extracting (2/2) DaysFM... (This is slow!)";

            await extractTask1;

            progressBar1.Style = ProgressBarStyle.Marquee;

            label1.Text = "Patching... (This is slow!)";

            var psi = new ProcessStartInfo {
                FileName = Path.Combine(versionFolder, "KH2FM.Toolkit.exe"),
                Arguments = "-batch English.Patch.kh2patch mod.kh2patch KH2FM.NEW.ISO",
                WorkingDirectory = versionFolder,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            var process = Process.Start(psi) ?? throw new InvalidOperationException();

            var stdoutTask = Task.Run(async () =>
            {
                while (await process.StandardOutput.ReadLineAsync() != null) { }
            });
            var stderrTask = Task.Run(async () =>
            {
                while (await process.StandardError.ReadLineAsync() != null) { }
            });

            process.WaitForExit();
            await Task.WhenAll(stdoutTask, stderrTask);

            Debug.WriteLine("Process finished with code " + process.ExitCode);

            process.Dispose();

            File.Delete(Path.Combine(versionFolder, "English.Patch.kh2patch"));
            File.Delete(Path.Combine(versionFolder, "PCSX2.7z"));
            File.Delete(Path.Combine(versionFolder, "KH2FM.ISO"));
            File.Delete(Path.Combine(versionFolder, "mod.7z"));
            File.Delete(Path.Combine(versionFolder, "mod.kh2patch"));

            progressBar1.Style = ProgressBarStyle.Continuous;

            label1.Text = "Done Installing!";

            progressBar1.Value = 0;

            DetectValidInstall();

        }

        private void CreateDirectories() {
            Directory.CreateDirectory(Path.Combine(versionFolder));
        }

        private async Task DownloadFromURL(string url, string destination, IProgress<int> progress) {

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)) {
                response.EnsureSuccessStatusCode();

                long totalBytes = response.Content.Headers.ContentLength ?? -1;
                long downloadedBytes = 0;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        if (totalBytes > 0 && progress != null) {
                            int percent = (int)(downloadedBytes * 100 / totalBytes);
                            progress.Report(percent);
                        }
                    }
                }
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e) {
            PositionButton();
        }

        private void PositionButton() {
           /*btn_InstallPlay.Location = new Point(
                ClientSize.Width / 2 - btn_InstallPlay.Size.Width / 2,
                ClientSize.Height / 2 - btn_InstallPlay.Size.Height / 2
            );
            btn_Uninstall.Location = new Point(
                ClientSize.Width / 2 - btn_Uninstall.Size.Width / 2,
                btn_InstallPlay.Location.Y + btn_InstallPlay.Size.Height
            );*/
        }

        private void btn_del_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to uninstall DaysFM?","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No) return;
            Directory.Delete(versionFolder, true);
            DetectValidInstall();

        }

        private void btn_dir_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", versionFolder);
        }

        private void btn_help_Click(object sender, EventArgs e) {
            MessageBox.Show("Created by DaRealLando123 with help from zpitolava22350\r\n358 / 2 Days Final Mix created by O’Shinobi ツ\n\nOpen-sourced on GitHub:\nhttps://github.com/DaRealLando123/KingdomLauncher\n\nLatest Update:\n+ Added Version Selector\nChanged file directories to function with multiple game versions. Old versions will require a reinstall!\n+ Improved Launcher UI\nReplaced old textures with higher fidelity ones and and revised layout of elements.\n+ Lowered Install File Size Footprint\nSwapped to 7zip as the compressor.", "KingdomLauncher v2.1", MessageBoxButtons.OK);
        }

        private void box_version_Changed(object sender, EventArgs e) {
            versionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KingdomLauncher", box_version.SelectedItem.ToString());
            DetectValidInstall();
        }
    }
}

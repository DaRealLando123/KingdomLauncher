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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoloLauncher {
    public partial class Form1: Form {

        string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public Form1() {
            InitializeComponent();
            label1.BackColor = Color.FromArgb(150, 0, 0, 0);
            //pictureBox1.Location = new Point(ClientSize.Width / 2 - pictureBox1.Width / 2, pictureBox1.Location.Y);
            PositionButton();
            DetectValidInstall();
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

                var psi = new ProcessStartInfo {
                    FileName = Path.Combine(docFolder, "KingdomLauncher", "PCSX2", "pcsx2.exe"),
                    Arguments = "KH2FM.NEW.ISO --nogui",
                    WorkingDirectory = Path.Combine(docFolder, "KingdomLauncher"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                };
                var process = Process.Start(psi) ?? throw new InvalidOperationException();

                Close();

            } else {
                await InstallProcess();
            }

        }

        private void DetectValidInstall() {
            if (File.Exists(Path.Combine(docFolder, "KingdomLauncher", "KH2FM.NEW.ISO")) && Directory.Exists(Path.Combine(docFolder, "KingdomLauncher", "PCSX2"))) {
                btn_InstallPlay.Text = "Play";
            } else {
                btn_InstallPlay.Text = "Install";
            }
            label1.Visible = false;
            progressBar1.Visible = false;
            btn_InstallPlay.Enabled = true;

            if (Directory.Exists(Path.Combine(docFolder, "KingdomLauncher"))) {
                btn_Uninstall.Enabled = true;
                btn_dir.Visible = true;
            } else {
                btn_Uninstall.Enabled = false;
                btn_dir.Visible = false;
            }
        }

        async private Task InstallProcess() {

            var newIsoPath = Path.Combine(docFolder, "KingdomLauncher", "KH2FM.NEW.ISO");
            if (File.Exists(newIsoPath)) {
                File.Delete(newIsoPath);
            }

            label1.Visible = true;
            progressBar1.Visible = true;
            btn_InstallPlay.Enabled = false;
            btn_InstallPlay.Text = "Installing...";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "ISO files (*.iso)|*.iso";
            ofd.Title = "Select the base game ISO file";

            label1.Text = "Choosing ISO...";

            var result = MessageBox.Show("Kingdom Hearts II - Final Mix+ (Japan) is required.\nA *legally* obtained ISO is needed to continue.", "ISO Selection", MessageBoxButtons.OKCancel);
            Debug.WriteLine(result);
            if (result != DialogResult.OK) {
                DetectValidInstall();
                return;
            }

            if (ofd.ShowDialog() != DialogResult.OK) {
                DetectValidInstall();
                return;
            }

            btn_Uninstall.Enabled = false;

            CreateDirectories();

            string tempFolder = Path.Combine(docFolder, "KingdomLauncher");

            label1.Text = "Copying ISO...";

            await Task.Run(() => {
                File.Copy(ofd.FileName, Path.Combine(tempFolder, Path.GetFileName("KH2FM.ISO")), true);
            });

            var progress = new Progress<int>(value => {
                progressBar1.Value = value;
            });

            //label1.Text = "Downloading mod...";

            //await DownloadFromURL("https://github.com/DaRealLando123/DaysFM/releases/download/Alpha/v0.034.Alpha.7z", Path.Combine(tempFolder, "Mod.zip"), progress);

            //Task extractTask1 = Task.Run(() => {
            //    ZipFile.ExtractToDirectory("Mod.zip", "Mod", true);
            //});

            label1.Text = "Downloading (2/4) PCSX2 Emulator...";

            await DownloadFromURL("https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/PCSX2.1.6.0.zip", Path.Combine(tempFolder, "PCSX2.zip"), progress);

            Task extractTask2 = Task.Run(() =>
            {
                string zipPath = Path.Combine(tempFolder, "PCSX2.zip");
                string extractPath = Path.Combine(docFolder, "KingdomLauncher", "PCSX2");

                // Delete the folder if it exists to emulate 'overwrite'
                if (Directory.Exists(extractPath)) {
                    Directory.Delete(extractPath, true);
                }
                
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            });

            label1.Text = "Downloading (3/4) Toolkit...";

            await DownloadFromURL("https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/KH2FM.Toolkit.exe", Path.Combine(tempFolder, "KH2FM.Toolkit.exe"), progress);

            label1.Text = "Downloading (4/4) English patch...";

            await DownloadFromURL("https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/English.Patch.kh2patch", Path.Combine(tempFolder, "English.Patch.kh2patch"), progress);

            label1.Text = "Still extracting files...";

            //await extractTask1;
            await extractTask2;

            label1.Text = "Patching... (this is slow/laggy)";

            var psi = new ProcessStartInfo {
                FileName = Path.Combine(tempFolder, "KH2FM.Toolkit.exe"),
                Arguments = "-batch English.Patch.kh2patch ModAlpha.kh2patch KH2FM.NEW.ISO",
                WorkingDirectory = tempFolder,
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

            File.Delete(Path.Combine(tempFolder, "English.Patch.kh2patch"));
            File.Delete(Path.Combine(tempFolder, "PCSX2.zip"));
            File.Delete(Path.Combine(tempFolder, "KH2FM.ISO"));
            //File.Delete(Path.Combine(tempFolder, "ModAlpha.kh2patch"));

            label1.Text = "Done Installing!";

            progressBar1.Value = 0;

            DetectValidInstall();

        }

        private void CreateDirectories() {
            Directory.CreateDirectory(Path.Combine(docFolder, "KingdomLauncher"));
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

        private void btn_Uninstall_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to uninstall DaysFM?","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No) return;
            Directory.Delete(Path.Combine(docFolder, "KingdomLauncher"), true);
            DetectValidInstall();

        }

        private void btn_dir_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(docFolder, "KingdomLauncher"));
        }
    }
}

using RidgeRacer3DTableTool.Models;
using RidgeRacer3DTableTool.RR3D_Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RidgeRacer3DTableTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (bgWorkerPack.IsBusy || bgWorkerUnpack.IsBusy)
            {
                return;
            }

            string[] paths = Environment.GetCommandLineArgs();
            // not set files
            if (paths.Count() == 1)
            {
                MessageBox.Show("Please set TBL or CSV file.");
                return;
            }

            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            for (int i = 1; i < 2; i++)
            {
                ArgsDoWorker argments = new ArgsDoWorker();
                argments.filePath = paths[i];
                argments.fileDirectory = Path.GetDirectoryName(paths[i]);

                string fileName = Path.GetFileName(paths[i]);
                argments.fileName = fileName.Split('.')[0];

                string extension = Path.GetExtension(paths[i]).ToLower();

                // Unpack
                if (".tbl".Equals(extension))
                {
                    bgWorkerUnpack.WorkerReportsProgress = true;
                    bgWorkerUnpack.RunWorkerAsync(argments);

                // Pack
                }
                else if (".csv".Equals(extension))
                {
                    bgWorkerPack.WorkerReportsProgress = true;
                    bgWorkerPack.RunWorkerAsync(argments);

                }

            }

        }

        private void bgWorkerUnpack_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            ArgsDoWorker argments = e.Argument as ArgsDoWorker;
            string fileName = argments.fileName; // file name without extention
            string filePath = argments.filePath; // path of tbl file
            string fileDirectory = argments.fileDirectory;

            int i = 0;
            UserStateProgressChanged argsProgressChanged = new UserStateProgressChanged();
            Encoding enc = Encoding.UTF8;

            try
            {
                // header file
                using (FileStream headerFileStream = new FileStream($@"{fileDirectory}\{fileName}_header.bin", FileMode.Create, FileAccess.Write))
                // tbl file
                using (FileStream tblFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                // csv file
                using (StreamWriter csvStreamWriter = new StreamWriter($@"{fileDirectory}\{fileName}.csv"))
                {
                    Header header = new Header();
                    header.Unpack(tblFileStream, headerFileStream);
                    argsProgressChanged.maximum = header.count;

                    csvStreamWriter.WriteLine("index, id, unk04h, unk0Ah, text");
                    tblFileStream.Seek(header.dataPtr, SeekOrigin.Begin);
                    for (i = 0; i < header.count; i++)
                    {
                        StringElement strElem = new StringElement();
                        strElem.Unpack(tblFileStream);

                        string row = String.Format("{0:X8}, {1}, {2:X8}, {3:X4}, {4}", i, strElem.id, strElem.unk14h, strElem.unk1Ah, strElem.text);
                        csvStreamWriter.WriteLine(row);

                        bgWorker.ReportProgress(i+1, argsProgressChanged); // update progress var
                    }
                }
            }
            catch
            {
                ErrorInfo errorInfo = new ErrorInfo();
                errorInfo.result = true;
                errorInfo.message = "Faild: Unpack.";
                bgWorker.ReportProgress(i, argsProgressChanged);
                return;
            }

            MessageBox.Show("Success: Unpack Complete.");

        }

        private void bgWorkerUnpack_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            UserStateProgressChanged userState = e.UserState as UserStateProgressChanged;
            progressBar1.Maximum = userState.maximum;

            progressBar1.Value = e.ProgressPercentage;
            label1.Text = $@"{e.ProgressPercentage.ToString()}/{ progressBar1.Maximum.ToString()}";
        }

        private void bgWorkerUnpack_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "Done.";

            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorkerPack_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            ArgsDoWorker argments = e.Argument as ArgsDoWorker;
            string fileName = argments.fileName; // file name without extention
            string filePath = argments.filePath; // path of tbl file
            string fileDirectory = argments.fileDirectory;

            int i = 0;
            UserStateProgressChanged argsProgressChanged = new UserStateProgressChanged();

            try
            {
                string destPath = $@"{fileDirectory}\build";
                Directory.CreateDirectory(destPath);

                int rowCount = 0;
                using (StreamReader csvStreamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    while (!csvStreamReader.EndOfStream)
                    {
                        csvStreamReader.ReadLine();
                        rowCount++;
                    }
                }

                // header file
                using (FileStream headerFileStream = new FileStream($@"{fileDirectory}\{fileName}_header.bin", FileMode.Open, FileAccess.Read))
                // tbl file
                using (FileStream tblFileStream = new FileStream($@"{destPath}\{fileName}.tbl", FileMode.Create, FileAccess.Write))
                // csv file
                using (StreamReader csvStreamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    csvStreamReader.ReadLine(); // read header line

                    Header header = new Header();
                    header.Pack(tblFileStream, headerFileStream);
                    argsProgressChanged.maximum = rowCount-1;

                    string row = "";
                    string[] splited;

                    while (!csvStreamReader.EndOfStream)
                    {
                        row = csvStreamReader.ReadLine();
                        splited = row.Split(',');

                        StringElement strElem = new StringElement();
                        strElem.id = splited[1].Trim();
                        int.TryParse(splited[2].Trim(), NumberStyles.HexNumber, null, out strElem.unk14h);
                        short.TryParse(splited[3].Trim(), NumberStyles.HexNumber, null, out strElem.unk1Ah);
                        strElem.text = splited[4].Trim();
                        strElem.Pack(tblFileStream);

                        bgWorker.ReportProgress(i + 1, argsProgressChanged); // update progress var
                        i++;
                    }

                    // padding
                    byte[] bytes = { (byte)0x00 };
                    while (tblFileStream.Position % 0x10 > 1)
                    {
                        tblFileStream.Write(bytes, 0x00, 0x01);
                    }
                }
            }
            catch
            {
                ErrorInfo errorInfo = new ErrorInfo();
                errorInfo.result = true;
                errorInfo.message = "Faild: Pack.";
                bgWorker.ReportProgress(i, argsProgressChanged);
                return;
            }

            MessageBox.Show("Success: Pack Complete.");

        }

        private void bgWorkerPack_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            UserStateProgressChanged userState = e.UserState as UserStateProgressChanged;
            progressBar1.Maximum = userState.maximum;

            progressBar1.Value = e.ProgressPercentage;
            label1.Text = $@"{e.ProgressPercentage.ToString()}/{ progressBar1.Maximum.ToString()}";
        }

        private void bgWorkerPack_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "Done.";

            Close();
        }

    }
}

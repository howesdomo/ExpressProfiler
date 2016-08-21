using System;
using System.Windows.Forms;

namespace ExpressProfiler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.IO.Directory.CreateDirectory(Program.DirectoryPath);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(new MainForm());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).Message , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string FileName = "ExpressProfilerDB.xml";

        /// <summary>
        /// 目录
        /// </summary>
        public static string DirectoryPath =
            System.IO.Path.Combine(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    , "HoweSoftware"
                ),
                "ExpressProfiler"
            )
            ;

        /// <summary>
        /// 文件路径
        /// </summary>
        public static string FullName =
            System.IO.Path.Combine(
                  Program.DirectoryPath
                , Program.FileName
            );



    }
}

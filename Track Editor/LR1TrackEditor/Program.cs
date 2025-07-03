namespace LR1TrackEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal static class Program
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 8;

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!Settings.Default.ShowConsole)
            {
                ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 0);
            }
            Console.Title = "Console window";
            bool flag = (args.Length > 0) && (args[0] == "-fullscreen");
            GameView game = new GameView();
            try
            {
                FormEditor form = new FormEditor(game);
                game.setSurface(form.getPCTHandle(), form.getPCTSize(), false);
                form.Show();
                if (flag)
                {
                    game.setOriginalSurface();
                    game.IsMouseVisible = false;
                    game.mouselock = true;
                    game.fullscreen = true;
                }
                game.Run();
            }
            finally
            {
                if (game is object)
                {
                    game.Dispose();
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}


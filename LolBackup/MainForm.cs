using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml; 
using System.Threading;
using System.IO;
using vcFramework.Assemblies;
using vcFramework.Xml;

namespace LolBackup
{
    public partial class MainForm : Form
    {
        #region FIELDS

        private BackupSteerer _steerer;

        private readonly StateHolder _stateholder;
        
        private DateTime _lastProcessTime;
        
        private System.Timers.Timer _shouldBackupStartTimer;

        private Thread _workerThread;
        
        private readonly string _processesFilePath = Application.StartupPath + @"\processes.cfg";
        
        private bool _busy;

        /// <summary>
        /// Holds messages from background thread, to be written to UI in main thread.
        /// </summary>
        private string _messageToAdd = string.Empty;

        /// <summary>
        /// Temporary store for crossthread progressbar update
        /// </summary>
        private int _progress;

        /// <summary>
        /// Temporary store for crossthread progressbar update
        /// </summary>
        private int _progressTotal;

        /// <summary>
        /// Holds contents of backup processes xml file.
        /// </summary>
        XmlDocument _jobs;

        #endregion

        #region CTORS

        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // try to load app state from xml document
            string appstate = Application.StartupPath + @"\appstate.cfg";
            
            try
            {
                _stateholder = new StateHolder(appstate);
            }
            catch
            {
                // if fail to open, app state is probably corrupt. delete, and reload
                if (File.Exists(appstate))
                    File.Delete(appstate);

                try
                {
                    _stateholder = new StateHolder(appstate);
                }
                catch(Exception ex)
                {
                    string message = string.Format("LolBackup is shutting down - critical error trying to access appstate file : {0}", ex);
                    MessageBox.Show(message);
                    Environment.Exit(0);
                }
            }


        }

        #endregion

        #region METHODS

        /// <summary>
        /// If app is not in ready state, starts a timer that periodically checks if next scheduled backup time has arrived. If 
        /// in ready state, stops ready state. 
        /// </summary>
        private void ToggleReadyState()
        {
            if (_steerer == null)
            {
                _steerer = new BackupSteerer(_jobs, StatusUpdate, ProgressBarUpdate, WriteToConsole);
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                _shouldBackupStartTimer = new System.Timers.Timer(10000);
                _shouldBackupStartTimer.Elapsed += this.OnTimerElapsed;
                _shouldBackupStartTimer.Start();

            }
            else 
            {
                if (_shouldBackupStartTimer != null)
                    _shouldBackupStartTimer.Stop();

                _steerer = null;
                btnStart.Enabled = true; ;
                btnStop.Enabled = false;
           
            }
        }

        /// <summary>
        /// Writes time since last backup to UI.
        /// </summary>
        private void ShowLastBackupTime()
        { 
            TimeSpan lastbackup = DateTime.Now - _lastProcessTime;
            if (lastbackup.TotalDays >= 2)
                lblLastBackupTime.Text = Math.Round(lastbackup.TotalDays) + " days ago.";
            else if (lastbackup.TotalHours > 24)
                lblLastBackupTime.Text = Math.Round(lastbackup.TotalDays) + " day ago.";
            else if (lastbackup.TotalMinutes > 240)
                lblLastBackupTime.Text = Math.Round(lastbackup.TotalHours) + " hours ago.";
            else if (lastbackup.TotalMinutes > 60)
                lblLastBackupTime.Text = Math.Round(lastbackup.TotalHours) + " hour ago.";
            else if (lastbackup.TotalSeconds > 60)
                lblLastBackupTime.Text = Math.Round(lastbackup.TotalMinutes) + " minutes ago.";
            else
                lblLastBackupTime.Text = Math.Round(lastbackup.TotalSeconds) + " seconds ago.";
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// First UI load. Use this to set up application's state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFormLoad(object sender, EventArgs e)
        {
            if (!File.Exists(_processesFilePath))
            {
                this.WriteToConsole("Couldn't find a processes.cfg file, so it created one for you. Be sure to update this with your own backup processes.");
                AssemblyAccessor assemblyAccessor = new AssemblyAccessor(this.GetType());
                string fileContent = assemblyAccessor.GetStringDocument(this.GetType().Namespace + ".Processes.cfg");
                File.WriteAllText(Path.Combine(Application.StartupPath, "Processes.cfg"), fileContent);
            }

            _jobs = new XmlDocument();
            _jobs.Load(_processesFilePath);
            scanStatus.Text = string.Empty;

            if (_stateholder.Contains("autoStartProcessing"))
                cbProcessOnStartup.Checked = (bool)_stateholder.Retrieve("autoStartProcessing");

            if (_stateholder.Contains("formSize"))
                this.Size = (Size)_stateholder.Retrieve("formSize");

            if (_stateholder.Contains("formLocation"))
                this.Location = (Point)_stateholder.Retrieve("formLocation");

            if (_stateholder.Contains("backupInterval"))
                backupInterval.Value = (decimal)_stateholder.Retrieve("backupInterval");

            if (_stateholder.Contains("lastProcessTime"))
                _lastProcessTime = (DateTime)_stateholder.Retrieve("lastProcessTime");

            if (this.Location.X < 0 || this.Location.Y < 0)
                this.Location = new Point(0, 0);
                
            if (cbProcessOnStartup.Checked)
                this.ToggleReadyState();

            this.ShowLastBackupTime();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConsoleSetWidth(object sender, EventArgs e)
        {
            // forces column in console listview to fill entire listview
            console.Columns[0].Width = -2;
        }

        /// <summary>
        /// Puts app in "ready to backup" state. App will start periodically checking if a backup should be done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartClick(object sender, EventArgs e)
        {
            this.ToggleReadyState();
        }

        /// <summary>
        /// Takes app out of "ready to backup" state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopClick(object sender, EventArgs e)
        {
            this.ToggleReadyState();
        }

        /// <summary>
        /// Invoked when main app window resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            this.ConsoleSetWidth(null, null);

            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        /// <summary>
        /// Invoked when app icon is clicked in system tray. Unhides app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Invoked when "restore" is clicked on system tray context menu. Unhides app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Invoked when "shutdown" is clicked on system tray context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Invoked when the system tray context menu is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void systemTrayContextMenuOpening(object sender, CancelEventArgs e)
        {
            restoreToolStripMenuItem.Enabled = this.WindowState != FormWindowState.Normal;

        }

        /// <summary>
        /// Invoked when app shuts down. Cleans up and writes app state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_workerThread != null && _workerThread.ThreadState == ThreadState.Running)
            {
                try 
                {
                    _workerThread.Abort();
                }
                catch(ThreadAbortException ex)
                {
                    // trap this
                }
            }

            _stateholder.Add("autoStartProcessing", cbProcessOnStartup.Checked);
            _stateholder.Add("formSize", this.Size);
            _stateholder.Add("formLocation", this.Location);
            _stateholder.Add("backupInterval", backupInterval.Value);
            _stateholder.Add("lastProcessTime", _lastProcessTime);
            _stateholder.Save();

        }

        /// <summary>
        /// Writes a message to the console. This method is crossthread-friendly; it can be passed via a delegate 
        /// to a background thread where it can be called directly.
        /// </summary>
        /// <param name="message"></param>
        public void WriteToConsole(string message)
        {
            _messageToAdd = message;
            console.Invoke(new UiInvoke(WriteToConsoleThreadSafe));
        }

        /// <summary>
        /// Writes a message to the console. This method is crossthread-friendly; it can be passed via a delegate 
        /// to a background thread where it can be called directly.
        /// </summary>
        /// <param name="message"></param>
        public void StatusUpdate(string message)
        {
            _messageToAdd = message;
            scanStatus.Invoke(new UiInvoke(StatusUpdateThreadSafe));
        }

        /// <summary>
        /// Updates progress bar. This methos is crossthread-friendly.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="progressTotal"></param>
        public void ProgressBarUpdate(int progress, int progressTotal)
        {
            _progress = progress;
            _progressTotal = progressTotal;
            backupProgressBar.Invoke(new UiInvoke(ProgressBarUpdateThreadSafe));
        }

        /// <summary>
        /// 
        /// </summary>
        private void StatusUpdateThreadSafe()
        {
            scanStatus.Text = _messageToAdd;
            backupProgressBar.Value = 0; // when status message is written, bar should be empty
        }

        /// <summary>
        /// To be invoked by WriteToConsole in main app thread.
        /// </summary>
        private void WriteToConsoleThreadSafe()
        {
            ListViewItem row = new ListViewItem(new[] { string.Format("{0}      {1}", DateTime.Now.ToShortTimeString(), _messageToAdd) });
            console.Items.Insert(0, row);
            if (console.Items.Count > 100)
                console.Items.RemoveAt(console.Items.Count - 1);
        }

        /// <summary>
        /// To be invoked by 
        /// </summary>
        private void ProgressBarUpdateThreadSafe()
        {
            // prevent overflow
            if (_progress > _progressTotal)
                _progress = _progressTotal;

            backupProgressBar.Value = _progress;
            backupProgressBar.Maximum = _progressTotal;
            scanStatus.Text = "Processing ..."; // when bar updates, status label must be blank
        }

        /// <summary>
        /// Invoked when the "backup now" buttons is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackupNowClick(object sender, EventArgs e)
        {
            this.StartBackup();
        }

        /// <summary>
        /// Invoked periodically by the timer which checks if a backup should be done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new UiInvoke(this.CheckIfBackupShouldStart));

        }

        #endregion

        #region WORK Threads

        /// <summary>
        /// Checks to see if enough time has passed since. This logic is not placed in the timer elapsed event handler
        /// because the UI is updated here regardless of whether a backup is started or not, and UI updates cant be done
        /// from a timer handler.
        /// </summary>
        private void CheckIfBackupShouldStart()
        {
            this.ShowLastBackupTime();

            // check if enough time has elapsed since last backup
            if ((long)Math.Round((DateTime.Now - _lastProcessTime).TotalDays) <= backupInterval.Value)
                return;

            this.StartBackup();
        }
        
        /// <summary>
        /// Starts a backup process. Can be called directly by clicking the "backup now" button, or is called by a regular check to see if 
        /// scheduled backup interval has elapsed.
        /// </summary>
        private void StartBackup() 
        {
            if (_busy)
                return;
            _busy = true;

            this.WriteToConsole("Starting back up work");
            btnBackupNow.Enabled = false;

            _workerThread = new Thread(this.StartBackupWorkThread) 
            {
                Name = "Work", IsBackground = true
            };

            _workerThread.Start();
        }

        /// <summary>
        /// This method must be run as a background thread.
        /// </summary>
        private void StartBackupWorkThread()
        {
            if (_steerer != null)
                _steerer.Process();

            // backup is done - leave background thread and update UI.
            this.Invoke(new UiInvoke(CleanupAfterBackup));
        }

        /// <summary>
        /// Invoked by the background backup thread as it exits.
        /// </summary>
        private void CleanupAfterBackup()
        {
            _lastProcessTime = DateTime.Now;
            this.ShowLastBackupTime();
            btnBackupNow.Enabled = true;

            this.WriteToConsole("Done backing up.");
            _busy = false;
        }


        #endregion

    }
}

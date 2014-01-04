//////////////////////////////////////////////////////////////////////////////////////
// Author				: Shukri Adams												//
// Contact				: shukri@cauterized.com										//
// Compiler requirement : .Net 3.5													//
//																					//
// LolBackup : A really simple backup utility                                       //
// Copyright (C)																	//
//																					//
// This program is free software; you can redistribute it and/or modify it under	//
// the terms of the GNU General Public License as published by the Free Software	//
// Foundation; either version 2 of the License, or (at your option) any later		//
// version.																			//
//																					//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY	//
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A	//
// PARTICULAR PURPOSE. See the GNU General Public License for more details.			//
//																					//
// You should have received a copy of the GNU General Public License along with		//
// this program; if not, write to the Free Software Foundation, Inc., 59 Temple		//
// Place, Suite 330, Boston, MA 02111-1307 USA										//
//////////////////////////////////////////////////////////////////////////////////////
namespace LolBackup
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.trayNotificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.systemTrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbProcessOnStartup = new System.Windows.Forms.CheckBox();
            this.backupInterval = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBackupNow = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblLastBackupTime = new System.Windows.Forms.Label();
            this.console = new ListViewFast();
            this.column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.systemTrayContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backupInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(12, 41);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStopClick);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStartClick);
            // 
            // trayNotificationIcon
            // 
            this.trayNotificationIcon.ContextMenuStrip = this.systemTrayContextMenu;
            this.trayNotificationIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayNotificationIcon.Icon")));
            this.trayNotificationIcon.Text = "SimpleBackup";
            this.trayNotificationIcon.Visible = true;
            this.trayNotificationIcon.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // systemTrayContextMenu
            // 
            this.systemTrayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem,
            this.shutdownToolStripMenuItem});
            this.systemTrayContextMenu.Name = "contextMenuStrip1";
            this.systemTrayContextMenu.Size = new System.Drawing.Size(153, 70);
            this.systemTrayContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.systemTrayContextMenuOpening);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // shutdownToolStripMenuItem
            // 
            this.shutdownToolStripMenuItem.Name = "shutdownToolStripMenuItem";
            this.shutdownToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.shutdownToolStripMenuItem.Text = "Shutdown";
            this.shutdownToolStripMenuItem.Click += new System.EventHandler(this.shutdownToolStripMenuItem_Click);
            // 
            // cbProcessOnStartup
            // 
            this.cbProcessOnStartup.AutoSize = true;
            this.cbProcessOnStartup.Checked = true;
            this.cbProcessOnStartup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbProcessOnStartup.Location = new System.Drawing.Point(12, 70);
            this.cbProcessOnStartup.Name = "cbProcessOnStartup";
            this.cbProcessOnStartup.Size = new System.Drawing.Size(68, 17);
            this.cbProcessOnStartup.TabIndex = 6;
            this.cbProcessOnStartup.Text = "Autostart";
            this.cbProcessOnStartup.UseVisualStyleBackColor = true;
            // 
            // backupInterval
            // 
            this.backupInterval.Location = new System.Drawing.Point(74, 94);
            this.backupInterval.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.backupInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.backupInterval.Name = "backupInterval";
            this.backupInterval.Size = new System.Drawing.Size(35, 20);
            this.backupInterval.TabIndex = 7;
            this.backupInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Run every";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "days";
            // 
            // btnBackupNow
            // 
            this.btnBackupNow.Location = new System.Drawing.Point(12, 173);
            this.btnBackupNow.Name = "btnBackupNow";
            this.btnBackupNow.Size = new System.Drawing.Size(75, 23);
            this.btnBackupNow.TabIndex = 10;
            this.btnBackupNow.Text = "Backup now";
            this.btnBackupNow.UseVisualStyleBackColor = true;
            this.btnBackupNow.Click += new System.EventHandler(this.btnBackupNowClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Last backup : ";
            // 
            // lblLastBackupTime
            // 
            this.lblLastBackupTime.AutoSize = true;
            this.lblLastBackupTime.Location = new System.Drawing.Point(12, 139);
            this.lblLastBackupTime.Name = "lblLastBackupTime";
            this.lblLastBackupTime.Size = new System.Drawing.Size(13, 13);
            this.lblLastBackupTime.TabIndex = 12;
            this.lblLastBackupTime.Text = "..";
            // 
            // console
            // 
            this.console.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.console.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.console.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.column1});
            this.console.GridLines = true;
            this.console.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.console.Location = new System.Drawing.Point(150, 12);
            this.console.Name = "console";
            this.console.Scrollable = false;
            this.console.Size = new System.Drawing.Size(455, 390);
            this.console.TabIndex = 14;
            this.console.UseCompatibleStateImageBehavior = false;
            this.console.View = System.Windows.Forms.View.Details;
            // 
            // column1
            // 
            this.column1.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 414);
            this.Controls.Add(this.console);
            this.Controls.Add(this.cbProcessOnStartup);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLastBackupTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBackupNow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.backupInterval);
            this.Name = "MainForm";
            this.Text = "Lolbackup - A simple backup tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.systemTrayContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.backupInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.NotifyIcon trayNotificationIcon;
        private System.Windows.Forms.ContextMenuStrip systemTrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shutdownToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbProcessOnStartup;
        private System.Windows.Forms.NumericUpDown backupInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBackupNow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLastBackupTime;
        private ListViewFast console;
        private System.Windows.Forms.ColumnHeader column1;
    }
}


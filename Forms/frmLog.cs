﻿using System;
using System.Windows.Forms;

namespace Syn3Updater.Forms
{
    public partial class FrmLog : Form
    {
        public FrmLog()
        {
            InitializeComponent();
        }

        // ReSharper disable once NotAccessedField.Local
        private FrmMain _frmMain;
        public void frmLog_Load(object sender, EventArgs e)
        {
            _frmMain = new FrmMain();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtLog.Text = FrmMain.Logoutput;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }
    }
}
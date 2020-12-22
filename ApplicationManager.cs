﻿//using DiscordRPC;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using Syn3Updater.Helper;
using Syn3Updater.Model;
using Syn3Updater.UI;
using Syn3Updater.UI.Tabs;
using Settings = Syn3Updater.Properties.Settings;

namespace Syn3Updater
{
    public class ApplicationManager
    {
        public string DownloadLocation,
            DriveName,
            DrivePartitionType,
            DriveFileSystem,
            DriveNumber,
            DriveLetter,
            SelectedMapVersion,
            SelectedRelease,
            SelectedRegion,
            InstallMode;

        public ObservableCollection<HomeViewModel.Ivsu> Ivsus = new ObservableCollection<HomeViewModel.Ivsu>();

        public MainWindow MainWindow;
        public bool SkipCheck, DownloadOnly, SkipFormat, IsDownloading;
        public static ApplicationManager Instance { get; } = new ApplicationManager();

        public void FireLanguageChangedEvent()
        {
            LanguageChangedEvent?.Invoke(this, new EventArgs());
        }

        public void FireDownloadsTabEvent()
        {
            ShowDownloadsTab?.Invoke(this, new EventArgs());
        }

        public void FireHomeTabEvent()
        {
            ShowHomeTab?.Invoke(this, new EventArgs());
        }

        public void FireSettingsTabEvent()
        {
            ShowSettingsTab?.Invoke(this, new EventArgs());
        }

        public event EventHandler LanguageChangedEvent;

        public event EventHandler ShowDownloadsTab;

        public event EventHandler ShowHomeTab;

        public event EventHandler ShowSettingsTab;

        #region Constructors

        private ApplicationManager()
        {
        }

        public static readonly SimpleLogger Logger = new SimpleLogger();

        #endregion

        #region Methods

        public void Initialize()
        {
            Logger.Debug("[App] Syn3 Updater is Starting");

            // ReSharper disable once IdentifierTypo
            // ReSharper disable once UnusedVariable
            List<LanguageModel> langs = LanguageManager.Languages;
            CultureInfo ci = CultureInfo.InstalledUICulture;
            if (Settings.Default.Lang == "")
            {
                Logger.Debug(
                    $"[Settings]  Language is not set, inferring language from system culture. Lang={ci.TwoLetterISOLanguageName}");
                Settings.Default.Lang = ci.TwoLetterISOLanguageName;
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Lang);

            //client = new DiscordRpcClient("");
            //client.Initialize();

            if (string.IsNullOrWhiteSpace(Settings.Default.DownloadLocation))
            {
                Logger.Debug(
                    $"[Settings] Download location is not set, defaulting to {KnownFolders.GetPath(KnownFolder.Downloads)}\\Syn3Updater\\");
                Settings.Default.DownloadLocation =
                    $@"{KnownFolders.GetPath(KnownFolder.Downloads)}\Syn3Updater\";
            }

            DownloadLocation = Settings.Default.DownloadLocation;

            if (!Directory.Exists(DownloadLocation) && DownloadLocation != "")
            {
                Logger.Debug("[App] Download location does not exist");
                Directory.CreateDirectory(DownloadLocation);
            }

            foreach (string arg in Environment.GetCommandLineArgs())
                switch (arg)
                {
                    case "/updated":
                        Logger.Debug("[App] /updated detected, upgrading settings");
                        Settings.Default.Upgrade();
                        Settings.Default.Save();
                        break;
                    case "/debug":
                        Logger.Debug("[App] /debug flag detected, skipping all verification steps");
                        SkipCheck = true;
                        break;
                }

            if (MainWindow == null) MainWindow = new MainWindow();
            if (!MainWindow.IsVisible) MainWindow.Show();

            if (MainWindow.WindowState == WindowState.Minimized) MainWindow.WindowState = WindowState.Normal;
        }

        public void RestartApp()
        {
            Logger.Debug("[App] Syn3 Updater is restarting.");
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void Exit()
        {
            Logger.Debug("[Settings] Saving settings before shutdown");
            Settings.Default.Save();
            Logger.Debug("[App] Syn3 Updater is shutting down");
            File.WriteAllText("log.txt", JsonConvert.SerializeObject(Logger.Log));
            Application.Current.Shutdown();
        }

        #endregion
    }
}
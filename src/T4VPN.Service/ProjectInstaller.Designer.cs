﻿/*
 * Copyright (c) 2020 Time4VPS
 *
 * This file is part of T4VPN.
 *
 * T4VPN is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * T4VPN is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with T4VPN.  If not, see <https://www.gnu.org/licenses/>.
 */

namespace T4VPN.Service
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.t4vpnServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.t4vpnServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // t4vpnServiceProcessInstaller
            // 
            this.t4vpnServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.t4vpnServiceProcessInstaller.Password = null;
            this.t4vpnServiceProcessInstaller.Username = null;
            // 
            // t4vpnServiceInstaller
            // 
            this.t4vpnServiceInstaller.DisplayName = "Time4VPS VPN Service";
            this.t4vpnServiceInstaller.ServiceName = "Time4VPS VPN Service";
            this.t4vpnServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.t4vpnServiceProcessInstaller,
            this.t4vpnServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller t4vpnServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller t4vpnServiceInstaller;
    }
}
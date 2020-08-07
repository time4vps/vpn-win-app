/*
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using T4VPN.Core.Auth;
using T4VPN.Core.Settings;

namespace T4VPN.Core.Language
{
    public class Language : ILoggedInAware
    {
        private readonly IAppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly string _translationsFolder;

        private const string ResourceFile = "T4VPN.resources.dll";
        private const string DefaultLanguage = "en";        

        public Language(IAppSettings appSettings, ILogger logger, string translationsFolder)
        {
            _appSettings = appSettings;
            _logger = logger;
            _translationsFolder = translationsFolder;
        }

        public string GetStartupLanguage()
        {
            return string.IsNullOrEmpty(_appSettings.Language) ? GetCurrentLanguage() : _appSettings.Language;
        }

        public List<string> GetAll()
        {
            try
            {
                return InternalGetAll();
            }
            catch (Exception e) when (e.IsFileAccessException())
            {
                _logger.Error(e);
                return new List<string> {DefaultLanguage};
            }
        }

        public void OnUserLoggedIn()
        {
            if (string.IsNullOrEmpty(_appSettings.Language))
            {
                _appSettings.Language = GetCurrentLanguage();
            }
        }

        private List<string> InternalGetAll()
        {
            var langs = new List<string> {DefaultLanguage};

            var files = Directory.GetFiles(_translationsFolder, ResourceFile, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var dirInfo = new DirectoryInfo(file);
                if (dirInfo.Parent != null)
                {
                    langs.Add(dirInfo.Parent.Name);
                }
            }

            return langs;
        }

        private string GetCurrentLanguage()
        {
            var osLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            return GetAll().Contains(osLanguage) ? osLanguage : DefaultLanguage;
        }
    }
}

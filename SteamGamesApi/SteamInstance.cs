using Gameloop.Vdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDFParser;
using VDFParser.Models;

namespace SteamGamesApi
{
    public class SteamInstance(string steamroot)
    {
        public string SteamRoot { get; init; } = steamroot;

        public SteamInstance() : this(FindRoot()) { }

        public string SteamExe { get => Path.Combine(SteamRoot, "steam.exe"); }

        public static string FindRoot()
        {
            string reg = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";
            if (!Environment.Is64BitOperatingSystem)
                reg = @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";
            string? steamRoot = Registry.GetValue(reg, "InstallPath", null)?.ToString();

            if (string.IsNullOrWhiteSpace(steamRoot) || !Directory.Exists(steamRoot))
                steamRoot = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Steam");

            return steamRoot;
        }

        public LibraryFoldersVDF? FindLibraries()
        {
            string steamAppsDir = Path.Combine(SteamRoot, "steamapps");

            string libraryConfigFile = Path.Combine(steamAppsDir, "libraryfolders.vdf");
            if (!File.Exists(libraryConfigFile))
                return null;

            return LibraryFoldersVDF.FromPath(libraryConfigFile);
        }

        public static int? GetCurrentUser()
        {
            string reg = @"HKEY_CURRENT_USER\Software\Valve\Steam\ActiveProcess";
            return Registry.GetValue(reg, "ActiveUser", null) as int?;
        }

        public string? GetUserDir(int? user = null)
        {
            user ??= GetCurrentUser();
            if (user is null)
                return null;
            var path = Path.Combine(SteamRoot, "userdata", $"{user:d}");
            if (path is null || !Directory.Exists(path))
                return null;
            return path;
        }

        public LocalConfigVDF? GetUserLocalConfig(int? user = null)
        {
            var userDir = GetUserDir(user);
            if (userDir is null) return null;
            var localConfigPath = Path.Combine(userDir, "config", "localconfig.vdf");

            try
            {
                return LocalConfigVDF.FromPath(localConfigPath);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public VDFEntry[]? GetUserShortcuts(int? user = null)
        {
            var userDir = GetUserDir(user);
            if (userDir is null) return null;
            var shortcutsFile = Path.Combine(userDir, "config", "shortcuts.vdf");

            try
            {
                return VDFParser.VDFParser.Parse(shortcutsFile);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
[assembly: AssemblyTitle("Riff Racer")]
[assembly: AssemblyDescription("Riff Racer")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Riff Racer")]
[assembly: AssemblyCopyright("Copyright © SmilerRyan 2024")]
[assembly: AssemblyTrademark("SmilerRyan")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("17a5b934-883c-4216-adf6-8b01bc3638e1")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
class Program {
    static string GenerateRandomString(int length) {
        Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        StringBuilder stringBuilder = new StringBuilder(length);
        for (int i = 0; i < length; i++) { stringBuilder.Append(chars[random.Next(chars.Length)]); }
        return stringBuilder.ToString();
    }
    static void WebServerThread() {
        HttpListener listener = new HttpListener();
        string prefix = "http://127.0.0.1:5144/";
        listener.Prefixes.Add(prefix);
        listener.Start();
        Console.WriteLine("Listening for requests on " + prefix);
        while (continueListening) {
            HttpListenerContext context = listener.GetContext();
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;
			string url = request.RawUrl;
			string jsonResponse = "{}";
			if (url.Contains("/events/AppOpened")) {
				jsonResponse = "{}";
			} else if (url.Contains("/files")) {
				jsonResponse = "{\"url\":\"http://127.0.0.1/file.bin\",\"name\":\"file.bin\"}";
			} else if (url.Contains("/classes/Configuration")) {
				jsonResponse = "{\"results\":[{\"objectId\":\"ITz5K79wcQ\",\"updatedAt\":\"2015-10-06T15:37:10.761Z\",\"RegionGroup\":\"Default\",\"createdAt\":\"2015-08-27T16:48:23.993Z\",\"OnTrackCoinValue\":10,\"TrackConversionCost\":300,\"ShareScoreCoinReward\":200,\"WatchAdCoinReward\":200,\"TrackCreatorCoinReward\":100}]}";
			} else if (url.Contains("/classes/News")) {
				jsonResponse = "{{\"results\":[{{\"objectId\":\"RiffApiLocalNews\",\"updatedAt\":\"2000-01-01T00:00:00.000Z\",\"de\":\"NoNews\",\"createdAt\":\"2000-01-01T00:00:00.000Z\",\"ru\":\"NoNews\",\"en\":\"NoNews\",\"esTitle\":\"NoNews\",\"it\":\"NoNews\",\"fr\":\"NoNews\",\"frTitle\":\"NoNews\",\"itTitle\":\"NoNews\",\"es\":\"NoNews\",\"enTitle\":\"NoNews\",\"Status\":\"Active\",\"ruTitle\":\"NoNews\",\"deTitle\":\"NoNews\",\"Pinned\":true,\"ID\":16}}]}}";
			} else if (url.Contains("/classes/Profile_Steam")) {
				jsonResponse = "{\"results\":[{\"objectId\":\"LocalProfile\",\"SystemID\":\"1\",\"FavouriteCar\":1,\"FavouriteSong\":\"null\",\"FavouriteGenre\":\"pop\",\"DriverLevel\":0,\"Platform\":\"PC\",\"Username\":\"Local\",\"createdAt\":\"2000-01-01T00:00:00.000Z\",\"updatedAt\":\"2000-01-01T00:00:00.000Z\"}]}";
			} else if (url.Contains("/classes/Version")) {
				jsonResponse = "{\"results\":[{\"objectId\":\"BoJMZ1TQJZ\",\"updatedAt\":\"2015-02-10T21:05:53.214Z\",\"Ver\":100,\"createdAt\":\"2015-02-10T20:50:24.164Z\"}]}";
			} else if (url.Contains("/classes/_Installation")) {
				jsonResponse = "{\"opens\":1,\"updatedAt\":\"2000-01-01T00:00:00.000Z\"}";
			} else if (url.Contains("/classes/") & request.HttpMethod == "GET") {
				jsonResponse = "{\"results\":[]}";
			} else if (url.Contains("/classes")) {
				jsonResponse = "{\"objectId\":\"" + GenerateRandomString(10) + "\",\"createdAt\":\"2000-01-01T00:00:00.000Z\"}";
			}
			Console.WriteLine(request.HttpMethod + " " + WebUtility.UrlDecode(url));
			if(!url.Contains("fuckoff")) {
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponse);
				response.ContentType = "application/json";
				response.ContentLength64 = buffer.Length;
				response.OutputStream.Write(buffer, 0, buffer.Length);
			} else {
				continueListening = false;
			}
			response.OutputStream.Close();
        }
    }
    static volatile bool continueListening = true;
    static void Main(string[] args) {
        if (File.Exists("game.exe")) {
            if (!ReplaceServerURL("ParseOctane.dll", "http://127.000000000.0.1:5144")) {
				return;
			}
            Thread serverThread = new Thread(WebServerThread);
            serverThread.Start();
            Process gameProcess = Process.Start("game.exe");
            gameProcess.EnableRaisingEvents = true;
            gameProcess.Exited += (sender, e) => {
                new System.Net.WebClient().DownloadString("http://127.0.0.1:5144/fuckoff");
            };
        } else {
            MessageBox.Show("Game.exe not found", "Riff Racer Server");
            return;
        }
    }
    static bool ReplaceServerURL(string filePath, string newServerURL) {
        if (!File.Exists(filePath)) {
			MessageBox.Show("ParseOctane.dll needs to be patched, but it was not found.", "Riff Racer");
			return false;
		}
		byte[] data = File.ReadAllBytes(filePath);
        data = WriteNewServer(data, "https://parseapi.back4app.com");
        string hash = CalculateSHA256(data);
        if (hash != "d661606db8658adb54d5be18b3e3a3a6f07c8e738f2d78556ece42355f8e62e4") {
			MessageBox.Show("ParseOctane.dll needs to be patched, but it is not the correct file.", "Riff Racer");
			return false;
        }
        data = WriteNewServer(data, newServerURL);
        return SaveByteArray(filePath, data);
    }
    static byte[] WriteNewServer(byte[] data, string serverURL) {
        const int offset = 0x57A4;
        if (serverURL.Length != 29) {
            throw new ArgumentException("Invalid server URL length.");
        }
        if (data.Length != 75776)
        {
            return new byte[1];
        }
        for (int i = 0; i < 29; i++) {
            data[offset + i] = (byte)serverURL[i];
        }
        return data;
    }
    static string CalculateSHA256(byte[] data) {
        using (SHA256 sha256 = SHA256.Create()) {
            byte[] hashBytes = sha256.ComputeHash(data);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes) {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
    static bool SaveByteArray(string filePath, byte[] data) {
        byte[] originalData = File.ReadAllBytes(filePath);
        if (!ByteArraysEqual(originalData, data)) {
			if (File.Exists(filePath + ".bak")) {
				MessageBox.Show("ParseOctane.dll needs to be patched, but you already a backup file. Please remove this file and try again.", "Riff Racer");
				return false;
			} else {
				File.Move(filePath, filePath + ".bak");
				File.WriteAllBytes(filePath, data);
				// MessageBox.Show("ParseOctane.dll has been patched.", "Riff Racer");
			}
		}
		return true;
    }
    static bool ByteArraysEqual(byte[] array1, byte[] array2) {
        if (array1.Length != array2.Length) return false;
        for (int i = 0; i < array1.Length; i++) {
            if (array1[i] != array2[i]) return false;
        }
        return true;
    }
}

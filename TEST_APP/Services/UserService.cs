using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TEST_APP.Services {
    public class UserData {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserImg { get; set; }
    }
    internal class UserService {
        public static class UService {
            public static UserData currentUser;
            private const string ACTIVE_USER_KEY = "active_user_key";

            public static async Task<UserData> GetCurrentUserAsync() {
                if (currentUser == null) {
                    await LoadUserFromStorageAsync();
                }
                return currentUser;
            }

            public static async Task SetCurrentUserAsync(UserData user) {
                SecureStorage.Remove(ACTIVE_USER_KEY);
                currentUser = user;

                string userJson = JsonSerializer.Serialize(user);
                await SecureStorage.SetAsync(ACTIVE_USER_KEY, userJson);
            }

            public static void LogoutUser() {
                currentUser = null;
                SecureStorage.Remove(ACTIVE_USER_KEY);
            }

            private static async Task LoadUserFromStorageAsync() {
                try {
                     string userJson = await SecureStorage.GetAsync(ACTIVE_USER_KEY) ?? "none";
                     if (!string.IsNullOrEmpty(userJson)) {
                         currentUser = JsonSerializer.Deserialize<UserData>(userJson);
                     }
                }
                catch (Exception ex) {
                    Debug.WriteLine($"Error loading user: {ex.Message}");
                }
            }
        }

        public static class UserImgService {
            public static string ToBase64(byte[] data) {
                if (data == null || data.Length == 0)
                    throw new ArgumentException("O array de bytes está vazio.");

                return Convert.ToBase64String(data);
            }

            public static byte[] ToByteList(string base64) {
                if (string.IsNullOrEmpty(base64))
                    throw new ArgumentException("A string Base64 está vazia.");

                return Convert.FromBase64String(base64);
            }

            public static async Task<byte[]> ImgToBytesAsync(FileResult fileResult) {
                if (fileResult == null)
                    throw new ArgumentNullException(nameof(fileResult));

                using (var stream = await fileResult.OpenReadAsync())
                using (var memoryStream = new MemoryStream()) {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            public static string ByteToImage(byte[] data, string extension = ".png") {
                string tempPath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}{extension}");
                File.WriteAllBytes(tempPath, data);
                return tempPath;
            }
        }
    }
}

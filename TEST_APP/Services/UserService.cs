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
        public string UserImgPath { get; set; }
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

            public static async Task<string?> GetUserImgPathAsync(FileResult result) {
                if (result == null)
                    return null;

                try {
                    string destPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);

                    // Try to open stream (FileResult from FilePicker)
                    try {
                        using var sourceStream = await result.OpenReadAsync();
                        using var destStream = File.Create(destPath);
                        await sourceStream.CopyToAsync(destStream);
                    }
                    catch {
                        // fallback: maybe Windows cannot open OpenReadAsync, copy directly from path
                        if (!string.IsNullOrEmpty(result.FullPath) && File.Exists(result.FullPath)) {
                            File.Copy(result.FullPath, destPath, overwrite: true);
                        }
                        else {
                            throw; // nothing we can do
                        }
                    }

                    return destPath;
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Error saving file: {ex.Message}");
                    return null;
                }
            }
        }
    }
}

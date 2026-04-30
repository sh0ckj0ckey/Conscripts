using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Conscripts.Helpers
{
    public static class StorageFilesService
    {
        private static StorageFolder? _dataFolder = null;

        /// <summary>
        /// Get the data folder for the application. If it doesn't exist, create it in the user's Documents folder under "NoMewing/Conscript".
        /// </summary>
        /// <returns>The <see cref="StorageFolder"/> representing the data folder.</returns>
        public static async Task<StorageFolder> GetDataFolderAsync()
        {
            if (_dataFolder is null)
            {
                StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().Documents);
                var noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                _dataFolder = await noMewingFolder.CreateFolderAsync("Conscript", CreationCollisionOption.OpenIfExists);
            }

            return _dataFolder;
        }

        /// <summary>
        /// Asynchronously reads the contents of the specified file and returns it as a string.
        /// </summary>
        /// <remarks>
        /// If the specified file does not exist or an error occurs during reading, 
        /// the method returns an empty string, rather than throwing an exception.
        /// </remarks>
        /// <param name="fileName">The name of the file to read.</param>
        /// <returns>The contents of the file as a string, or an empty string if something goes wrong.</returns>
        public static async Task<string> ReadFileAsync(string fileName)
        {
            string text = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
                }

                var dataFolder = await GetDataFolderAsync();
                var storageFile = await dataFolder.GetFileAsync(fileName);
                text = await FileIO.ReadTextAsync(storageFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }

            return text;
        }

        /// <summary>
        /// Asynchronously writes the specified text content to a file, replacing any existing file with the same name.
        /// </summary>
        /// <param name="fileName">The name of the file to write to. The file will be created or replaced in the application's data folder.</param>
        /// <param name="content">The text content to write to the file. If the file already exists, its contents will be replaced.</param>
        /// <returns><see langword="true"/> if the file was written successfully; otherwise, <see langword="false"/>.</returns>
        public static async Task<bool> WriteFileAsync(string fileName, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
                }

                const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
                const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);

                int retryAttempts = 3;

                var dataFolder = await GetDataFolderAsync();

                while (retryAttempts > 0)
                {
                    StorageFile? storageFile = null;

                    try
                    {
                        retryAttempts--;
                        storageFile = await dataFolder.CreateFileAsync(fileName + ".tmp", CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteTextAsync(storageFile, content);
                        await storageFile.RenameAsync(fileName, NameCollisionOption.ReplaceExisting);
                        return true;
                    }
                    catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) || (ex.HResult == ERROR_SHARING_VIOLATION))
                    {
                        System.Diagnostics.Trace.WriteLine(ex);

                        try
                        {
                            if (storageFile is not null)
                            {
                                await storageFile.DeleteAsync();
                            }
                        }
                        catch (Exception cleanupEx)
                        {
                            System.Diagnostics.Trace.WriteLine(cleanupEx);
                        }

                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex);

                        try
                        {
                            if (storageFile is not null)
                            {
                                await storageFile.DeleteAsync();
                            }
                        }
                        catch (Exception cleanupEx)
                        {
                            System.Diagnostics.Trace.WriteLine(cleanupEx);
                        }

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }

            return false;
        }

        /// <summary>
        /// Asynchronously copies the specified file into the application's data folder.
        /// </summary>
        /// <remarks>
        /// If a file with the same name already exists in the data folder, 
        /// a unique file name will be generated automatically.
        /// If the source file does not exist or an error occurs during copying,
        /// the method returns <see langword="null"/> rather than throwing an exception.
        /// </remarks>
        /// <param name="sourceFilePath">The full path of the source file to copy.</param>
        /// <param name="desiredFileName">The desired file name in the data folder, including its extension.</param>
        /// <returns>The full path of the copied file in the data folder if the operation succeeds; otherwise, <see langword="null"/>.</returns>
        public static async Task<string?> CopyToDataFolderAsync(string sourceFilePath, string desiredFileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException("Source file path cannot be null or empty.", nameof(sourceFilePath));
                }

                if (string.IsNullOrWhiteSpace(desiredFileName))
                {
                    throw new ArgumentException("Desired file name cannot be null or empty.", nameof(desiredFileName));
                }

                if (!System.IO.File.Exists(sourceFilePath))
                {
                    return null;
                }

                StorageFolder dataFolder = await GetDataFolderAsync();

                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(desiredFileName);
                string extension = System.IO.Path.GetExtension(desiredFileName);

                string targetPath = System.IO.Path.Combine(dataFolder.Path, desiredFileName);

                if (System.IO.File.Exists(targetPath))
                {
                    int index = 1;
                    do
                    {
                        string uniqueFileName = $"{fileNameWithoutExtension}_{index}{extension}";
                        targetPath = System.IO.Path.Combine(dataFolder.Path, uniqueFileName);
                        index++;
                    }
                    while (System.IO.File.Exists(targetPath));
                }

                System.IO.File.Copy(sourceFilePath, targetPath, false);

                return targetPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return null;
            }
        }
    }

}

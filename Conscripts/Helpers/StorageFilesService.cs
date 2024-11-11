using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Conscripts.Helpers
{
    public static class StorageFilesService
    {
        // 存储数据的文件夹对象
        private static StorageFolder DataFolder = null;

        /// <summary>
        /// 获取存储数据的文件夹的对象
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> GetDataFolder()
        {
            if (DataFolder == null)
            {
                StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().Documents);
                var noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                DataFolder = await noMewingFolder.CreateFolderAsync("Conscript", CreationCollisionOption.OpenIfExists);
            }
            return DataFolder;
        }

        /// <summary>
        /// 读取本地文件夹根目录的文件，默认从OpenDotaData目录读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string> ReadFileAsync(string fileName, StorageFolder applicationFolder = null)
        {
            string text = string.Empty;
            try
            {
                applicationFolder ??= await GetDataFolder();

                var storageFile = await applicationFolder.GetFileAsync(fileName);

                if (storageFile != null)
                {
                    IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                    using (StreamReader streamReader = new StreamReader(accessStream.AsStreamForRead((int)accessStream.Size)))
                    {
                        text = streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return text;
        }

        /// <summary>
        /// 写入本地文件夹根目录的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<bool> WriteFileAsync(string fileName, string content, StorageFolder applicationFolder = null)
        {
            try
            {
                if (applicationFolder == null)
                    applicationFolder = await GetDataFolder();

                var storageFile = await applicationFolder.CreateFileAsync(fileName + "Tmp", CreationCollisionOption.ReplaceExisting);

                int retryAttempts = 3;
                const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
                const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);

                while (retryAttempts > 0)
                {
                    try
                    {
                        retryAttempts--;
                        await FileIO.WriteTextAsync(storageFile, content);
                        await storageFile.RenameAsync(fileName, NameCollisionOption.ReplaceExisting);
                        return true;
                    }
                    catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) || (ex.HResult == ERROR_SHARING_VIOLATION))
                    {
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return false;
        }
    }

}

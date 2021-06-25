using AzureADPgPass.PgPass;
using DokanNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using FileAccess = DokanNet.FileAccess;

namespace AzureADPgPass
{
    internal class PgPassVfs : IDokanOperations
    {
        public PgPassVfs(string volumeLabel, params PgPassBase[] items)
        {
            if(string.IsNullOrEmpty(volumeLabel))
            {
                volumeLabel = "pgAADPass";
            }
            VolumeLabel = volumeLabel;
            Items = items;
        }

        public string VolumeLabel { get; private set; }
        public PgPassBase[] Items { get; private set; }

        public void Cleanup(string fileName, IDokanFileInfo info)
        {

        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {

        }

        public NtStatus CreateFile(string fileName, FileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, IDokanFileInfo info)
        {
            if (info.IsDirectory && mode == FileMode.CreateNew)
                return DokanResult.AccessDenied;
            return DokanResult.Success;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new List<FileInformation>();

            if (fileName == "\\")
            {
                foreach (var item in Items)
                {
                    var finfo = new FileInformation
                    {
                        FileName = item.FileName,
                        Attributes = FileAttributes.Normal,
                        LastAccessTime = DateTime.Now,
                        LastWriteTime = DateTime.Now,
                        CreationTime = DateTime.Now,
                    };
                    files.Add(finfo);
                }
                return DokanResult.Success;
            }
            else
            {
                return DokanResult.Error;
            }
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new FileInformation[0];
            return DokanResult.NotImplemented;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams = new FileInformation[0];
            return DokanResult.NotImplemented;
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, IDokanFileInfo info)
        {
            freeBytesAvailable = Environment.WorkingSet;
            totalNumberOfBytes = Environment.WorkingSet;
            totalNumberOfFreeBytes = Environment.WorkingSet;
            return DokanResult.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, IDokanFileInfo info)
        {
            fileInfo = new FileInformation { FileName = fileName };
            if (fileName == "\\")
            {
                fileInfo.Attributes = FileAttributes.Directory;
                fileInfo.LastAccessTime = DateTime.Now;
                fileInfo.LastWriteTime = DateTime.Now;
                fileInfo.CreationTime = DateTime.Now;

                return DokanResult.Success;
            }

            var item = Items.FirstOrDefault(i => i.FileName == fileName.TrimStart('\\'));

            if(item != null)
            {
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.LastAccessTime = DateTime.Now;
                fileInfo.LastWriteTime = DateTime.Now;
                fileInfo.CreationTime = DateTime.Now;
                fileInfo.Length = item.FileLength;
                return DokanResult.Success;
            }

            return DokanResult.AccessDenied;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            security = new FileSecurity();

            security.AddAccessRule(new FileSystemAccessRule(
                WindowsIdentity.GetCurrent().Name,
                FileSystemRights.ReadData,
                AccessControlType.Allow
                ));

            return DokanResult.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = VolumeLabel;
            features = FileSystemFeatures.None;
            fileSystemName = string.Empty;
            maximumComponentLength = 256;
            return DokanResult.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Mounted(IDokanFileInfo info)
        {
            Console.WriteLine($"{VolumeLabel} mounted.");
            Console.WriteLine("Press any key to exit ...");
            return DokanResult.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            var item = Items.FirstOrDefault(i => i.FileName == fileName.TrimStart('\\'));

            if (item != null)
            {
                var content = item.GetContent();

                var bytes = Encoding.UTF8.GetBytes(content);
                bytesRead = 0;
                for (long i = 0; i < Math.Min(buffer.Length, bytes.Length - offset); i++)
                {
                    buffer[i] = bytes[offset + i];
                    bytesRead += 1;
                }

                return DokanResult.Success;
            }
            bytesRead = 0;
            return DokanResult.Error;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            Console.WriteLine($"{VolumeLabel} unmounted.");
            return DokanResult.Success;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            bytesWritten = 0;
            return DokanResult.Error;
        }
    }
}

using System.Diagnostics;
using System.Text.Json;
using alexandria.api.Models;
using System.Runtime.InteropServices;


namespace alexandria.api.Services
{
    public interface IFileService
    {
        void CopyFile(string sourcePath, string destinationPath, string filename);
        IEnumerable<DirectoryListing> GetSubDirectories(string path);
        // this method will likely only work on a Linux system
        Task<IEnumerable<USBDevice>> CheckUSBDeviceInformation();
        Task UnmountUSBDevice();
    }
    public class DirectoryListing
    {
        public required string FullPath { get; set; }
        public required string Name { get; set; }
    }
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CopyFile(string sourcePath, string destinationPath, string filename)
        {
            if (Directory.Exists(destinationPath))
            {
                if (File.Exists(sourcePath))
                {
                    var destinationFilePath = Path.Combine(destinationPath, filename);
                    if (!File.Exists(destinationFilePath))
                    {
                        File.Copy(sourcePath, destinationFilePath);
                    }
                }
                else
                {
                    throw new FileNotFoundException($"Source file not found: {sourcePath}");
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"Destination directory not found: {destinationPath}");
            }
        }


        public IEnumerable<DirectoryListing> GetSubDirectories(string path)
        {
            var directories = Directory.GetDirectories(path);
            var directoryList = new List<DirectoryListing>();
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                directoryList.Add(
                    new DirectoryListing
                    {
                        FullPath = directory,
                        Name = directoryInfo.Name
                    });
            }
            return directoryList;
        }

        // this method will likely only work on a Linux system
        public async Task<IEnumerable<USBDevice>> CheckUSBDeviceInformation()
        {
            // Example output from lsblk command:
            // lsblk --output vendor,model,serial,name,path,label,mountpoint,uuid,fsavail,fssize,fsused,fsuse% --include 8 --json
            // {
            //    "blockdevices": [
            //       {"vendor":"Kindle  ", "model":"Internal_Storage", "serial":"G000S109740403H0",
            //           "name":"sdb", "path":"/dev/sdb", "label":"Kindle", "mountpoint":"/media/usb0", 
            //           "uuid":"6578-753B", "fsavail":"27.1G", "fssize":"27.3G", "fsused":"228M", "fsuse%":"1%"}
            //    ]
            // }
            var usbDevices = new List<USBDevice>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {


                // LINUX ALLOCATED DEVICES
                // http://mirrors.mit.edu/kernel/linux/docs/lanana/device-list/devices-2.6.txt
                //   8 block	SCSI disk devices (0-15)
                //   0 = /dev/sda		First SCSI disk whole disk
                //   16 = /dev/sdb		Second SCSI disk whole disk
                //   32 = /dev/sdc		Third SCSI disk whole disk
                //     ...
                // 240 = /dev/sdp		Sixteenth SCSI disk whole disk
                var storageDevices = 8;
                var fieldsToDisplay = "vendor,model,serial,name,path,label,mountpoint,uuid,fsavail,fssize,fsused,fsuse%";
                // lsblk --output vendor,model,serial,name,path,label,mountpoint,uuid,fsavail,fssize,fsused,fsuse% --include 8 --json
                var lsblkOutput = await RunCommand($"lsblk --output {fieldsToDisplay} --include {storageDevices} --json");
                var lsblkJson = JsonDocument.Parse(lsblkOutput);
                var blockDevices = lsblkJson.RootElement.GetProperty("blockdevices");
                foreach (var blockDevice in blockDevices.EnumerateArray())
                {
                    var usbDevice = new USBDevice
                    {
                        Vendor = blockDevice.TryGetProperty("vendor", out var vendor) ? vendor.GetString()?.Trim() : null,
                        Model = blockDevice.TryGetProperty("model", out var model) ? model.GetString()?.Trim() : null,
                        Serial = blockDevice.TryGetProperty("serial", out var serial) ? serial.GetString()?.Trim() : null,
                        Name = blockDevice.TryGetProperty("name", out var name) ? name.GetString()?.Trim() : null,
                        Path = blockDevice.TryGetProperty("path", out var path) ? path.GetString()?.Trim() : null,
                        Label = blockDevice.TryGetProperty("label", out var label) ? label.GetString()?.Trim() : null,
                        Mountpoint = blockDevice.TryGetProperty("mountpoint", out var mountpoint) ? mountpoint.GetString()?.Trim() : null,
                        UUID = blockDevice.TryGetProperty("uuid", out var uuid) ? uuid.GetString()?.Trim() : null,
                        FSavail = blockDevice.TryGetProperty("fsavail", out var fsavail) ? fsavail.GetString()?.Trim() : null,
                        FSsize = blockDevice.TryGetProperty("fssize", out var fssize) ? fssize.GetString()?.Trim() : null,
                        FSused = blockDevice.TryGetProperty("fsused", out var fsused) ? fsused.GetString()?.Trim() : null,
                        FSuse = blockDevice.TryGetProperty("fsuse%", out var fsuse) ? fsuse.GetString()?.Trim() : null
                    };
                    usbDevices.Add(usbDevice);
                }
            }
            return usbDevices;
        }

        private static async Task<string> RunCommand(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return output;
        }

        // send pumount /media/usb0 command to unmount the device
        public async Task UnmountUSBDevice()
        {
            var mountpoint = _configuration["DeviceMountDirectory"];
            await RunCommand($"pumount {mountpoint}");
        }
    }
}
using System;
using System.Security.Cryptography;
using System.Text;

namespace MD5;

public static class CheckSumComputerAsync
{
    public static async Task<byte[]> CreateCheckSum(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException("invalid path: shouldn't be a null value");
        }

        if (Path.HasExtension(path))
        {
            try
            {
                var hashValueFiles = await CreateFileCheckSum(path);
                return hashValueFiles;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
        var result = await ComputeCheckSum(path);
        return result;
    }

    private static async Task<byte[]> CreateFileCheckSum(string filePath)
    {
        var fileInfo = File.ReadAllBytes(filePath);
        var hash = System.Security.Cryptography.MD5.HashData(fileInfo);
        return hash;
    }

    private static async Task<byte[]> ComputeCheckSum(string path)
    {
        var allDirectories = Directory.GetDirectories(path);
        Array.Sort(allDirectories);
        var allFiles = Directory.GetFiles(path);
        Array.Sort(allFiles);

        var hashValues = new List<byte[]>();
        foreach (var file in allFiles)
        {
            try
            {
                var hash = await CreateFileCheckSum(file);
                if (hash != null)
                {
                    hashValues.Add(hash);
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        foreach (var dir in allDirectories)
        {
            var hash = await ComputeCheckSum(dir);
            if (hash != null)
            {
                hashValues.Add(hash);
            }
        }

        var pathBytes = Encoding.ASCII.GetBytes(path);
        var resultLength = pathBytes.Length;
        foreach (var byteArray in hashValues)
        {
            resultLength += byteArray.Length;
        }

        var result = new byte[resultLength];
        var shift = 0;
        Buffer.BlockCopy(pathBytes, 0, result, shift, pathBytes.Length);
        shift += pathBytes.Length;
        for (var i = 0; i < hashValues.Count; ++i)
        {
            Buffer.BlockCopy(hashValues[i], 0, result, shift, hashValues[i].Length);
            shift += hashValues[i].Length;
        }

        return result;
    }
}
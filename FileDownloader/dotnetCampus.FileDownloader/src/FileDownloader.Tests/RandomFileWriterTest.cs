using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace FileDownloader.Tests
{
    [TestClass]
    public class RandomFileWriterTest
    {
        [ContractTestCase]
        public void WriteFile()
        {
            "����ֶ�д���ļ������Զ�ȡ������˳����ļ�".Test(async () =>
            {
                var file = new FileInfo("File.txt");

                if (file.Exists)
                {
                    file.Delete();
                }

                var str = new StringBuilder();

                await using (var fileStream = file.Create())
                await using (var randomFileWriter = new RandomFileWriter(fileStream))
                {
                    const int count = 'z' - 'a' + 1;
                    fileStream.SetLength(count * 100);

                    var list = new List<(int startPoint, byte[] data)>();
                    for (int i = 0; i < 100; i++)
                    {
                        var data = new byte[count];
                        for (int j = 0; j < count; j++)
                        {
                            data[j] = (byte)('a' + j);
                        }

                        list.Add((count * i, data));
                    }

                    foreach (var (startPoint, data) in list)
                    {
                        str.Append(string.Join("", data.Select(temp => (char)temp)));
                    }

                    // ����˳��
                    var random = new Random();

                    for (int i = 0; i < 100; i++)
                    {
                        var a = random.Next(0, list.Count);
                        var b = random.Next(0, list.Count);

                        var t = list[a];
                        list[a] = list[b];
                        list[b] = t;
                    }

                    foreach (var (startPoint, data) in list)
                    {
                        randomFileWriter.WriteAsync(startPoint, data, count);
                    }
                }

                Thread.Sleep(1000);

                var text = await File.ReadAllTextAsync(file.FullName);

                Assert.AreEqual(str.ToString(), text);
            });


            "�������ļ�д�룬����д���������ļ�".Test(async () =>
            {
                var file = new FileInfo("File.txt");

                if (file.Exists)
                {
                    file.Delete();
                }

                var str = new StringBuilder();

                await using (var fileStream = file.Create())
                await using (var randomFileWriter = new RandomFileWriter(fileStream))
                {
                    const int count = 'z' - 'a' + 1;
                    fileStream.SetLength(count * 100);

                    var list = new List<(int startPoint, byte[] data)>();
                    for (int i = 0; i < 100; i++)
                    {
                        var data = new byte[count];
                        for (int j = 0; j < count; j++)
                        {
                            data[j] = (byte)('a' + j);
                        }

                        list.Add((count * i, data));
                    }

                    foreach (var (startPoint, data) in list)
                    {
                        randomFileWriter.WriteAsync(startPoint, data, count);
                        str.Append(string.Join("", data.Select(temp => (char)temp)));
                    }
                }

                Thread.Sleep(1000);

                var text = await File.ReadAllTextAsync(file.FullName);

                Assert.AreEqual(str.ToString(), text);
            });
        }
    }
}
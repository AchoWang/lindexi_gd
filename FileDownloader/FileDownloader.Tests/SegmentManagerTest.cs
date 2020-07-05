using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace FileDownloader.Tests
{
    [TestClass]
    public class SegmentManagerTest
    {
        [ContractTestCase]
        public void GetNewDownloadSegment()
        {
            "��λ�ȡ���᲻�ϷֶΣ����зֶκ��������ļ���С".Test(() =>
            {
                const long fileLength = 1000;
                var segmentManager = new SegmentManager(fileLength);
                var downloadSegmentList = new List<DownloadSegment>();

                for (int i = 0; i < 100; i++)
                {
                    DownloadSegment downloadSegment = segmentManager.GetNewDownloadSegment();
                    downloadSegmentList.Add(downloadSegment);
                }

                var length = downloadSegmentList.Select(temp => temp.RequirementDownloadPoint - temp.StartPoint).Sum();
                Assert.AreEqual(fileLength, length);
            });

            "�ڻ�ȡ�����ε�ʱ�򣬿��Ի�ȡ��һ�κ͵ڶ��ε��м�".Test(() =>
            {
                const long fileLength = 1000;
                var segmentManager = new SegmentManager(fileLength);

                var firstDownloadSegment = segmentManager.GetNewDownloadSegment();
                Assert.AreEqual(fileLength, firstDownloadSegment.RequirementDownloadPoint);

                var secondDownloadSegment = segmentManager.GetNewDownloadSegment();

                var thirdDownloadSegment = segmentManager.GetNewDownloadSegment();
                Assert.AreEqual(250, thirdDownloadSegment.StartPoint);
            });

            "�ڻ�ȡ�ڶ��ε�ʱ�򣬽��޸ĵ�һ����Ҫ���صĳ��ȣ�ͬʱ�ڶ��δ��м俪ʼ".Test(() =>
            {
                const long fileLength = 1000;
                var segmentManager = new SegmentManager(fileLength);

                var firstDownloadSegment = segmentManager.GetNewDownloadSegment();
                Assert.AreEqual(fileLength, firstDownloadSegment.RequirementDownloadPoint);

                var secondDownloadSegment = segmentManager.GetNewDownloadSegment();

                Assert.AreEqual(0, firstDownloadSegment.StartPoint);
                Assert.AreEqual(fileLength / 2 - 1, firstDownloadSegment.RequirementDownloadPoint);

                Assert.AreEqual(fileLength / 2, secondDownloadSegment.StartPoint);
                Assert.AreEqual(fileLength, secondDownloadSegment.RequirementDownloadPoint);
            });

            "��һ���������ݵĳ������ļ��ĳ���".Test(() =>
            {
                const long fileLength = 1000;
                var segmentManager = new SegmentManager(fileLength);

                var downloadSegment = segmentManager.GetNewDownloadSegment();

                Assert.AreEqual(fileLength, downloadSegment.RequirementDownloadPoint);
            });

            "Ĭ�ϵ�һ�����������Ǵ��㿪ʼ".Test(() =>
            {
                const long fileLength = 1000;
                var segmentManager = new SegmentManager(fileLength);
                var downloadSegment = segmentManager.GetNewDownloadSegment();
                Assert.AreEqual(0, downloadSegment.StartPoint);
            });
        }
    }
}

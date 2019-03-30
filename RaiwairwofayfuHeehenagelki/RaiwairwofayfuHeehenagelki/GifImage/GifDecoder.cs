using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace RaiwairwofayfuHeehenagelki.GifImage
{
    /// <summary>
    ///     GIFͼ���ļ��Ľ�����
    /// </summary>
    internal class GifDecoder
    {

        /// <summary>
        ///     ��gifͼ���ļ����н���
        /// </summary>
        /// <param name="gifPath">gif�ļ�·��</param>
        internal static GifImage Decode(string gifPath)
        {
            GifImage gifImage;
            using (FileStream fs = new FileStream(gifPath, FileMode.Open))
            {
                gifImage = new GifImage();
                var graphics = new List<GraphicEx>();
                var frameCount = 0;

                var streamHelper = new GifStream(fs);
                //��ȡ�ļ�ͷ
                gifImage.Header = streamHelper.ReadString(6);
                //��ȡ�߼���Ļ��ʾ��
                gifImage.LogicalScreenDescriptor = streamHelper.GetLCD(fs);
                if (gifImage.LogicalScreenDescriptor.GlobalColorTableFlag)
                {
                    //��ȡȫ����ɫ�б�
                    gifImage.GlobalColorTable =
                        streamHelper.ReadByte(gifImage.LogicalScreenDescriptor.GlobalColorTableSize * 3);
                }

                var nextFlag = streamHelper.Read();
                while (nextFlag != 0)
                {
                    if (nextFlag == GifExtensions.ImageLabel)
                    {
                        ReadImage(streamHelper, fs, gifImage, graphics, frameCount);
                        frameCount++;
                    }
                    else if (nextFlag == GifExtensions.ExtensionIntroducer)
                    {
                        var gcl = streamHelper.Read();
                        switch (gcl)
                        {
                            case GifExtensions.GraphicControlLabel:
                            {
                                var graphicEx = streamHelper.GetGraphicControlExtension(fs);
                                graphics.Add(graphicEx);
                                break;
                            }
                            case GifExtensions.CommentLabel:
                            {
                                var comment = streamHelper.GetCommentEx(fs);
                                gifImage.CommentExtensions.Add(comment);
                                break;
                            }
                            case GifExtensions.ApplicationExtensionLabel:
                            {
                                var applicationEx = streamHelper.GetApplicationEx(fs);
                                gifImage.ApplictionExtensions.Add(applicationEx);
                                break;
                            }
                            case GifExtensions.PlainTextLabel:
                            {
                                var textEx = streamHelper.GetPlainTextEx(fs);
                                gifImage.PlainTextExtensions.Add(textEx);
                                break;
                            }
                        }
                    }
                    else if (nextFlag == GifExtensions.EndIntroducer)
                    {
                        //�����ļ�β
                        break;
                    }

                    nextFlag = streamHelper.Read();
                }
            }

            return gifImage;
        }



        private static void ReadImage(GifStream streamHelper, Stream fs, GifImage gifImage, List<GraphicEx> graphics,
            int frameCount)
        {
            var imgDes = streamHelper.GetImageDescriptor(fs);
            var frame = new GifFrame();
            frame.ImageDescriptor = imgDes;
            frame.LocalColorTable = gifImage.GlobalColorTable;
            if (imgDes.LctFlag)
            {
                frame.LocalColorTable = streamHelper.ReadByte(imgDes.LctSize * 3);
            }

            var lzwDecoder = new LZWDecoder(fs);
            var dataSize = streamHelper.Read();
            frame.ColorDepth = dataSize;
            var pixel = lzwDecoder.DecodeImageData(imgDes.Width, imgDes.Height, dataSize);
            frame.IndexedPixel = pixel;
            var blockSize = streamHelper.Read();
            _ = new DataStruct(blockSize, streamHelper);
            var graphicEx = graphics[frameCount];
            frame.GraphicExtension = graphicEx;
            frame.Image = GetImageFromPixel(pixel, imgDes.Width, imgDes.Height, frame.Palette,
                imgDes.InterlaceFlag);
            gifImage.Frames.Add(frame);
        }

        private static ImageSource GetImageFromPixel(byte[] pixel, int iw,
            int ih, Color[] colorTable, bool interlaceFlag)
        {
            var dest = new int[iw * ih];
            var pointer = 0;
            var tempPointer = pointer;

            var offSet = 0;
            //var i = 0;
            if (interlaceFlag)
            {
                #region ��֯�洢ģʽ

                var i = 0;
                var pass = 0; //��ǰͨ��            
                while (pass < 4)
                {
                    //�ܹ���4��ͨ��
                    if (pass == 1)
                    {
                        pointer = tempPointer;
                        pointer += 4 * iw;
                        offSet += 4 * iw;
                    }
                    else if (pass == 2)
                    {
                        pointer = tempPointer;
                        pointer += 2 * iw;
                        offSet += 2 * iw;
                    }
                    else if (pass == 3)
                    {
                        pointer = tempPointer;
                        pointer += 1 * iw;
                        offSet += 1 * iw;
                    }

                    var rate = 2;
                    if ((pass == 0) | (pass == 1))
                    {
                        rate = 8;
                    }
                    else if (pass == 2)
                    {
                        rate = 4;
                    }

                    while (i < pixel.Length)
                    {
                        dest[pointer] = colorTable[pixel[i++]].ToArgb();
                        pointer++;

                        offSet++;
                        if (i % iw == 0)
                        {
                            pointer += iw * (rate - 1);
                            offSet += iw * (rate - 1);
                            if (offSet >= pixel.Length)
                            {
                                pass++;
                                offSet = 0;
                                break;
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {
                for (var i = 0; i < pixel.Length;)
                {
                    dest[pointer] = colorTable[pixel[i++]].ToArgb();
                    pointer++;
                }
            }

            return BitmapSource.Create(iw, ih, 96, 96, PixelFormats.Bgr32, null, dest, 4 * iw);
        }

    }
}
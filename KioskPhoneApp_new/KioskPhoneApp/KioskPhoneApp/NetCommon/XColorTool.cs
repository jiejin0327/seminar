using KioskPhoneApp;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Rugal.XTool
{
    public static class XColorTool
    {
        public static ImageSource LineGrad_GetImgSource(GdColorInfo CInfo)
        {
            try
            {
                var Stre = LineGrad_GetStream(CInfo);
                var Ret = ImageSource.FromStream(() => Stre);
                return Ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static ImageSource BytesToImgSource(byte[] Bs)
        {
            try
            {
                var St = new MemoryStream(Bs);
                var Ret = ImageSource.FromStream(() => St);
                return Ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static byte[] LineGrad_GetBytes(GdColorInfo CInfo)
        {
            var St = LineGrad_GetStream(CInfo);

            var Ret = new byte[St.Length];
            St.Read(Ret, 0, Convert.ToInt32(St.Length));
            return Ret;
        }
        public static Stream LineGrad_GetStream(GdColorInfo CInfo)
        {
            try
            {
                var Img = LineGrad_GetSkImg(CInfo);
                //using ()
                //{
                if (Img is null)
                    return null;

                var SkData = Img.Encode(SKEncodedImageFormat.Jpeg, 100);
                return SkData.AsStream();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static SKImage LineGrad_GetSkImg(GdColorInfo CInfo)
        {
            try
            {
                var Surface = LineGrad_GetSurface(CInfo);
                //using ()
                //{
                var Ret = Surface.Snapshot();

                return Ret;
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static SKSurface LineGrad_GetSurface(GdColorInfo CInfo)
        {
            try
            {
                var Points = GetPointPath(CInfo);
                var Surface = SKSurface.Create(CInfo.SkInfo);
                var Canvas = Surface.Canvas;

                Canvas.Clear(SKColors.White);

                var Shader = SKShader.CreateLinearGradient(
                    Points[0], Points[1],
                    CInfo.Colors,
                    CInfo.ColorPos,
                    SKShaderTileMode.Clamp);

                var Paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Shader = Shader
                };

                Canvas.DrawPaint(Paint);
                return Surface;
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static SKImageInfo GetImgInfo(int XWidth, int XHeigh)
        {
            return new SKImageInfo(XWidth, XHeigh);
        }
        public static SKPoint[] GetPointPath(GradientPath Path, int W, int H)
        {
            var PLTop = new SKPoint(0, 0); //左上
            var PRTop = new SKPoint(W, 0); //右上
            var PLBot = new SKPoint(0, H); //左下
            var PRBot = new SKPoint(W, H); //右下

            var Dic = new Dictionary<GradientPath, SKPoint[]>
            {
                { GradientPath.ToBottom, new SKPoint[]{ PLTop, PLBot } },
                { GradientPath.ToRight, new SKPoint[]{ PLTop, PRTop } },
                { GradientPath.ToLeft, new SKPoint[]{ PRTop, PLTop } },
                { GradientPath.ToTop, new SKPoint[]{ PLBot, PLTop } },
                { GradientPath.ToBottomRight, new SKPoint[]{ PLTop, PRBot } },
                { GradientPath.ToBottomLeft, new SKPoint[]{ PRTop, PLBot } },
                { GradientPath.ToTopRight, new SKPoint[]{ PLBot, PRTop } },
                { GradientPath.ToTopLeft, new SKPoint[]{ PRBot, PLTop } },
            };
            return Dic[Path];
        }
        public static SKPoint[] GetPointPath(GdColorInfo CInfo)
        {
            var Ret = GetPointPath(CInfo.PointPath, CInfo.Width, CInfo.Heigh);
            return Ret;
        }
        public static SKColor[] GetSkColors(params Color[] Colors)
        {
            var Ret = Colors.Select(Item => Item.ToSKColor()).ToArray();
            return Ret;
        }
        public static Stream EncodeToStream(this SKImage SkImg)
        {
            return SkImg.Encode(SKEncodedImageFormat.Jpeg, 100).AsStream(false);
        }
        public static ImageSource ToImgSource(this SKImage SkImg)
        {
            return ImageSource.FromStream(() => SkImg.EncodeToStream());
        }

        public static void SetBackGroundImage(this ContentPage Page, GdColorInfo CInfo)
        {
            Page.BackgroundImageSource = LineGrad_GetImgSource(CInfo);
        }

    }
    public enum GradientPath
    {
        /// <summary>
        /// 上至下
        /// </summary>
        ToBottom,
        /// <summary>
        /// 下至上
        /// </summary>
        ToTop,
        /// <summary>
        /// 左至右
        /// </summary>
        ToRight,
        /// <summary>
        /// 右至左
        /// </summary>
        ToLeft,
        /// <summary>
        /// 左上至右下
        /// </summary>
        ToBottomRight,
        /// <summary>
        /// 右上至左下
        /// </summary>
        ToBottomLeft,
        /// <summary>
        /// 左下至右上
        /// </summary>
        ToTopRight,
        /// <summary>
        /// 右下至左上
        /// </summary>
        ToTopLeft,
    }
    public class GdColorInfo
    {
        public int Width { get; set; }
        public int Heigh { get; set; }
        public float[] ColorPos { get; set; }
        public GradientPath PointPath { get; set; }
        public SKColor[] Colors { get; set; }
        public SKImageInfo SkInfo { get; }

        private GdColorInfo(int XWidth, int XHeigh, GradientPath Path, string _ColorPos)
        {
            Width = XWidth;
            Heigh = XHeigh;
            PointPath = Path;
            ColorPos = Getfloat(_ColorPos);
            SkInfo = XColorTool.GetImgInfo(XWidth, XHeigh);
        }
        public GdColorInfo(int XWidth, int XHeigh, GradientPath Path, string _ColorPos, params SKColor[] _Colors) : this(XWidth, XHeigh, Path, _ColorPos)
        {
            Colors = _Colors;
        }
        public GdColorInfo(int XWidth, int XHeigh, GradientPath Path, string _ColorPos, params Color[] _Colors) : this(XWidth, XHeigh, Path, _ColorPos)
        {
            Colors = XColorTool.GetSkColors(_Colors);
        }
        public GdColorInfo(int XWidth, int XHeigh, GradientPath Path, string _ColorPos, params string[] _Colors) : this(XWidth, XHeigh, Path, _ColorPos)
        {
            Colors = _Colors.Select(Item => Color.FromHex(Item).ToSKColor()).ToArray();
        }
        private float[] Getfloat(string Pos)
        {
            if (Pos is null || Pos.Length < 1)
                return null;
            var SArr = Pos.Split(',', ' ', ':');
            var Ret = SArr.Select(ItemS => float.Parse(ItemS)).ToArray();

            if (Ret.Count() < 1)
                return null;

            return Ret;
        }
    }

    public class GdColorView : AbsoluteLayout
    {
        public Image BackGroundImage { get; set; }
        public StackLayout StackChildren { get; set; }
        GdColorInfo Info;
        public GdColorInfo ColorInfo
        {
            get
            {
                return Info;
            }
            set
            {
                Info = value;

                var ImgStack = new StackLayout() { };
                SetLayoutBounds(ImgStack, new Rectangle(0, 0, 1, 1));
                SetLayoutFlags(ImgStack, AbsoluteLayoutFlags.All);

                BackGroundImage = new Image()
                {
                    Source = XColorTool.LineGrad_GetImgSource(Info),
                    Aspect = Aspect.AspectFill
                };
                ImgStack.Children.Add(BackGroundImage);
                Children.Insert(0, ImgStack);
            }
        }
        public GdColorView()
        {

        }
    }
}

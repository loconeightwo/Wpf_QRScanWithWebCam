
using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;
using ZXing.Aztec;

namespace QR
{
    static class QRScan
    {
        
        public static string QRDecode(BitmapImage image)
        {
            string result = "";
            BarcodeReader Reader = new BarcodeReader();
            try
            {
                result = Reader.Decode(image.BitmapImage2Bitmap()).ToString().Trim();
            }
            catch (Exception ex)
            {
                result = "";
            }
                return result;
        }

    }
}

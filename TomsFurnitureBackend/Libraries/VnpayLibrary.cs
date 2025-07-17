using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using TomsFurnitureBackend.Common.Models.Vnpay;

namespace TomsFurnitureBackend.Libraries
{
    public class VnpayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        // B??c 1: Nh?n d? li?u tr? v? t? VNPAY và ki?m tra ch? ký
        public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
        {
            // T?o m?i ??i t??ng VnpayLibrary ?? l?u d? li?u tr? v?
            var vnPay = new VnpayLibrary();
            // L?p qua t?t c? các tham s? tr? v?, ch? l?y các tham s? b?t ??u b?ng 'vnp_'
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value);
                }
            }
            // L?y mã ??n hàng t? tham s? tr? v?
            var orderIdStr = vnPay.GetResponseData("vnp_TxnRef");
            var orderId = long.TryParse(orderIdStr, out var oid) ? oid : 0;
            // L?y mã giao d?ch VNPAY
            var vnPayTranIdStr = vnPay.GetResponseData("vnp_TransactionNo");
            var vnPayTranId = long.TryParse(vnPayTranIdStr, out var tid) ? tid : 0;
            // L?y mã ph?n h?i t? VNPAY
            var vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            // L?y mã hash ?? ki?m tra ch? ký
            var vnpSecureHash = collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value;
            // L?y thông tin ??n hàng
            var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");
            // Ki?m tra ch? ký h?p l? hay không
            var checkSignature = vnPay.ValidateSignature(vnpSecureHash, hashSecret);
            if (!checkSignature)
                return new PaymentResponseModel()
                {
                    Success = false
                };
            // Tr? v? k?t qu? thanh toán
            return new PaymentResponseModel()
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = orderInfo,
                OrderId = orderId.ToString(),
                PaymentId = vnPayTranId.ToString(),
                TransactionId = vnPayTranId.ToString(),
                Token = vnpSecureHash,
                VnPayResponseCode = vnpResponseCode
            };
        }

        // B??c 2: L?y ??a ch? IP c?a client th?c hi?n thanh toán
        public string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "127.0.0.1";
        }

        // B??c 3: Thêm d? li?u vào request g?i lên VNPAY
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        // B??c 4: Thêm d? li?u vào response nh?n t? VNPAY
        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        // B??c 5: L?y d? li?u t? response
        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        // B??c 6: T?o URL thanh toán g?i lên VNPAY
        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();

            // Ghép các tham s? thành query string
            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var querystring = data.ToString();

            baseUrl += "?" + querystring;
            var signData = querystring;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            // T?o mã hash ?? xác th?c giao d?ch
            var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnpSecureHash;

            return baseUrl;
        }

        // B??c 7: Ki?m tra ch? ký giao d?ch tr? v? t? VNPAY
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rspRaw = GetResponseData();
            var myChecksum = HmacSha512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        // B??c 8: T?o mã hash HMAC SHA512
        private string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new System.Security.Cryptography.HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        // B??c 9: Ghép d? li?u response thành chu?i ?? xác th?c ch? ký
        private string GetResponseData()
        {
            var data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }

            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }

            foreach (var (key, value) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }

        // Bước 10: So sánh key cho SortedList
        public class VnPayCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                var vnpCompare = CompareInfo.GetCompareInfo("en-US");
                return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
            }
        }
    }
}

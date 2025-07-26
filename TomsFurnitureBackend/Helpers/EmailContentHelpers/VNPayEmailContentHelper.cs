using TomsFurnitureBackend.Models;
using System.Text;

namespace TomsFurnitureBackend.Helpers.EmailContentHelpers
{
    public static class VNPayEmailContentHelper
    {
        public static (string subject, string body) BuildVnPaySuccessEmailContent(Order order)
        {
            string recipientName = "Khách hàng";
            if (order.User != null)
            {
                recipientName = order.User.UserName;
            }
            else if (order.UserGuest != null && !string.IsNullOrWhiteSpace(order.UserGuest.FullName))
            {
                recipientName = order.UserGuest.FullName;
            }
            var subject = $"Thanh toán thành công đơn hàng #{order.Id} qua VNPAY - TomFurniture";

            // Địa chỉ giao hàng
            var address = $"{order.OrderAdd?.Recipient}, {order.OrderAdd?.AddressDetailRecipient}, {order.OrderAdd?.Ward}, {order.OrderAdd?.District}, {order.OrderAdd?.City}";

            // Chi tiết sản phẩm
            var sb = new StringBuilder();
            foreach (var detail in order.OrderDetails)
            {
                var productName = detail.ProVar?.Product?.ProductName ?? "Sản phẩm";
                var color = detail.ProVar?.Color?.ColorName;
                var size = detail.ProVar?.Size?.SizeName;
                var material = detail.ProVar?.Material?.MaterialName;
                var attrs = new List<string>();
                if (!string.IsNullOrWhiteSpace(color)) attrs.Add(color);
                if (!string.IsNullOrWhiteSpace(size)) attrs.Add(size);
                if (!string.IsNullOrWhiteSpace(material)) attrs.Add(material);
                var attrStr = attrs.Count > 0 ? $" ({string.Join(", ", attrs)})" : "";
                sb.Append($"<tr><td style='padding:8px;border:1px solid #ddd;'>{productName}{attrStr}</td><td style='padding:8px;border:1px solid #ddd;text-align:center;'>{detail.Quantity}</td><td style='padding:8px;border:1px solid #ddd;text-align:right;'>{detail.Price:N0}đ</td></tr>");
            }

            var body = $@"<!DOCTYPE html><html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 8px; border: 1px solid #eee;'><h2 style='color: #2e6da4;'>🎉 Thanh toán thành công đơn hàng #{order.Id}</h2><p>Xin chào <strong>{recipientName}</strong>,</p><p>Chúng tôi xác nhận rằng bạn đã <strong>thanh toán thành công</strong> đơn hàng #{order.Id} qua <strong>VNPAY</strong> tại <strong>TomFurniture</strong>.</p><h3 style='border-bottom: 1px solid #ddd; padding-bottom: 4px;'>Thông tin đơn hàng</h3><p>🧾 <strong>Mã đơn hàng:</strong> #{order.Id}<br/>📅 <strong>Ngày đặt hàng:</strong> {order.OrderDate:dd/MM/yyyy}<br/>💳 <strong>Phương thức thanh toán:</strong> VNPAY<br/>🚚 <strong>Địa chỉ giao hàng:</strong> {address}</p><h3 style='border-bottom: 1px solid #ddd; padding-bottom: 4px;'>Chi tiết sản phẩm</h3><table style='width: 100%; border-collapse: collapse;'><thead><tr style='background-color: #f2f2f2;'><th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Sản phẩm</th><th style='padding: 8px; border: 1px solid #ddd; text-align: center;'>Số lượng</th><th style='padding: 8px; border: 1px solid #ddd; text-align: right;'>Giá</th></tr></thead><tbody>{sb}</tbody></table><p style='margin-top: 20px; font-size: 16px;'><strong>➡️ Tổng cộng: <span style='color: #d9534f;'>{order.Total:N0}đ</span></strong></p><p style='background-color: #dff0d8; padding: 10px; border-radius: 4px;'>✅ Đơn hàng của bạn đã được thanh toán thành công qua VNPAY. Chúng tôi sẽ tiến hành xử lý và giao hàng trong thời gian sớm nhất.</p><p>📦 Chúng tôi sẽ tiếp tục cập nhật tình trạng đơn hàng của bạn qua email khi đơn hàng được giao cho đơn vị vận chuyển.<br/>Nếu bạn có bất kỳ thắc mắc nào, đừng ngần ngại liên hệ với chúng tôi.</p><p style='margin-top: 30px;'>Trân trọng,<br/><strong>TomFurniture Team</strong><br/>🌐 <a href='https://tomfurniture.vn' style='color: #337ab7;'>https://tomfurniture.vn</a></p></div></body></html>";
            return (subject, body);
        }
    }
}

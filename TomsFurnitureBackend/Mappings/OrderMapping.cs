using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.OrderVModel;
using static TomsFurnitureBackend.VModels.ProductVModel;
using System.Collections.Generic;
using System;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Common.Contansts;

namespace TomsFurnitureBackend.Mappings
{
    public static class OrderMapping
    {
        public static Order ToEntity(this OrderCreateVModel model)
        {
            // Bước 1: Tính tổng giá trị từ OrderDetails
            decimal subTotal = model.OrderDetails?.Sum(d => d.Quantity * d.Price) ?? 0;

            // Bước 2: Khởi tạo Order
            var order = new Order
            {
                // UserId luôn null khi t?o t? client (khách vãng lai)
                UserId = null,
                OrderAddId = model.OrderAddId,
                PaymentMethodId = model.PaymentMethodId,
                PromotionId = model.PromotionId,
                // Total = model.Total,
                ShippingPrice = model.ShippingPrice,
                Note = model.Note,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                PaymentStatus = model.PaymentStatus ?? PaymentStatus.Unpaid, // M?c ??nh ch?a thanh toán
                OrderDetails = new List<OrderDetail>(),
                UserGuestId = model.UserGuestId,
                IsUserGuest = model.UserGuestId.HasValue // B?t true n?u có UserGuestId
            };
            foreach (var detail in model.OrderDetails)
            {
                order.OrderDetails.Add(detail.ToEntity());
            }

            // Bước 4: Gán giá trị Total (sẽ được cập nhật lại trong Service để áp dụng Promotion)
            order.Total = subTotal;

            return order;
        }

        public static OrderDetail ToEntity(this OrderDetailCreateVModel model)
        {
            return new OrderDetail
            {
                ProVarId = model.ProVarId,
                Quantity = model.Quantity,
                Price = model.Price,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static void UpdateEntity(this OrderDetail entity, OrderDetailCreateVModel model)
        {
            entity.Quantity = model.Quantity;
            entity.Price = model.Price;
            entity.UpdatedDate = DateTime.UtcNow;
            // Có th? b? sung các tr??ng khác n?u c?n
        }

        public static void UpdateEntity(this Order entity, OrderUpdateVModel model)
        {
            entity.OrderStaId = model.OrderStaId;
            entity.IsPaid = model.IsPaid ?? entity.IsPaid;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.Note = model.Note;
            // entity.Total = model.Total;
            entity.ShippingPrice = model.ShippingPrice;
        }

        public static OrderGetVModel ToGetVModel(this Order entity)
        {
            return new OrderGetVModel
            {
                Id = entity.Id,
                UserId = entity.UserId,
                OrderAddId = entity.OrderAddId,
                PaymentMethodId = entity.PaymentMethodId,
                PromotionId = entity.PromotionId,
                Total = entity.Total,
                ShippingPrice = entity.ShippingPrice,
                Note = entity.Note,
                IsActive = entity.IsActive,
                OrderStaId = entity.OrderStaId,
                IsPaid = entity.IsPaid,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                PaymentStatus = entity.PaymentStatus,
                OrderDetails = entity.OrderDetails?.Select(x => x.ToGetVModel()).ToList() ?? new List<OrderDetailGetVModel>()
            };
        }

        public static OrderDetailGetVModel ToGetVModel(this OrderDetail entity)
        {
            return new OrderDetailGetVModel
            {
                Id = entity.Id,
                ProVarId = entity.ProVarId ?? 0,
                Quantity = entity.Quantity,
                Price = entity.Price,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                ProductVariant = entity.ProVar?.ToGetVModel()
            };
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using System;

namespace TomsFurnitureBackend.Mappings
{
    public static class UserGuestMapping
    {
        public static UserGuestGetVModel ToGetVModel(this UserGuest x)
        {
            return new UserGuestGetVModel
            {
                Id = x.Id,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                DetailAddress = x.DetailAddress,
                City = x.City,
                District = x.District,
                Ward = x.Ward,
                CreatedDate = x.CreatedDate,
                IsActive = true // Không có tr??ng IsActive, m?c ??nh true
            };
        }

        public static List<UserGuestGetVModel> ToGetVModelList(this IEnumerable<UserGuest> list)
        {
            return list.Select(x => x.ToGetVModel()).ToList();
        }

        // Mapping cho Create
        public static UserGuest ToEntity(this UserGuestCreateVModel model)
        {
            return new UserGuest
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                DetailAddress = model.DetailAddress,
                City = model.City,
                District = model.District,
                Ward = model.Ward,
                CreatedDate = DateTime.UtcNow
            };
        }

        // Mapping cho Update (c?p nh?t entity t? model)
        //public static void UpdateEntity(this UserGuestUpdateVModel model, UserGuest entity)
        //{
        //    entity.FullName = model.FullName;
        //    entity.PhoneNumber = model.PhoneNumber;
        //    entity.Email = model.Email;
        //    entity.DetailAddress = model.DetailAddress;
        //    entity.City = model.City;
        //    entity.District = model.District;
        //    entity.Ward = model.Ward;
        //}
    }
}

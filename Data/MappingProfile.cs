using AutoMapper;
using demo_invoice_processor.Controllers.Dto;
using demo_invoice_processor.Models;
using System.Reflection;

namespace demo_invoice_processor.Data
{
    public class MappingProfile : Profile
    {
        public class ReceivableProfile : Profile
        {
            public ReceivableProfile()
            {
                //source mapping to destination
                CreateMap<Receivable, ReceivableRecord>().ReverseMap();
                //.ForMember(des => des.UserId, opt => opt.MapFrom(src => src.Id));
                //    //    .ForMember(t => t.ToDoItemModel, s => s.MapFrom(audit => Newtonsoft.Json.JsonConvert.DeserializeObject<ToDoItemModel>(audit.Entity)));

            }
        }
    }
}

using AutoMapper;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;

namespace Comparator.Repositories.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Table, string>()
                .ConvertUsing(send => send.TABLE_NAME ?? string.Empty);   
            
            CreateMap<Procedure, string>()
                .ConvertUsing(send => send.PROCEDURE_NAME ?? string.Empty);

            CreateMap<Constraint, DtoConstraint>()
                .ForMember(dest => dest.ConstraintType, send => send.MapFrom(t => t.constraint_type))
                .ForMember(dest => dest.ConstraintName, send => send.MapFrom(n => n.constraint_name))
                .ForMember(dest => dest.ConstraintKeys, send => send.MapFrom(k => k.constraint_keys))
                .ForMember(dest => dest.OnDelete, send => send.MapFrom(d => d.delete_action))
                .ForMember(dest => dest.OnUpdate, send => send.MapFrom(u => u.update_action));

            CreateMap<SyBaseConstaintsModel, DtoSyBaseConstaintsModel>()
                .ForMember(dest => dest.AllowNull, send => send.MapFrom(a => a.allow_null))
                .ForMember(dest => dest.ConstraintKeys, send => send.MapFrom(c => c.constraint_keys))
                .ForMember(dest => dest.ConstraintName, send => send.MapFrom(n => n.constraint_name))
                .ForMember(dest => dest.ConstraintType, send => send.MapFrom(t => t.constraint_type))
                .ForMember(dest => dest.FieldName, send => send.MapFrom(f => f.fields_name))
                .ForMember(dest => dest.OnDelete, send => send.MapFrom(d => d.on_delete))
                .ForMember(dest => dest.OnUpdate, send => send.MapFrom(u => u.on_update))
                .ForMember(dest => dest.OtherColumns, send => send.MapFrom(o => o.other_columns))
                .ForMember(dest => dest.OtherTable, send => send.MapFrom(y => y.other_table));
        }
    }
}

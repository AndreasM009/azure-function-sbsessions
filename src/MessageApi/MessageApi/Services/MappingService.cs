using AutoMapper;
using MessageApi.DomainObjects;
using MessageApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MessageApi.Services
{
    public class MappingService
    {
        private readonly Lazy<MapperConfiguration> _configuration;
        private readonly Lazy<IMapper> _mapper;

        public MappingService()
        {
            _configuration = new Lazy<MapperConfiguration>(() => CreateConfiguration());
            _mapper = new Lazy<IMapper>(() => CreateMapper());
        }

        private MapperConfiguration CreateConfiguration()
        {
            return new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<DutyMessage, DutyMessageDto>()
                    .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => Guid.Parse(s.PartitionKey)))
                    .ForMember(d => d.Id, opt => opt.MapFrom(s => Guid.Parse(s.RowKey)));

                cfg.CreateMap<DutyMessageDto, DutyMessage>()
                    .ForMember(d => d.PartitionKey, opt => opt.MapFrom(s => s.CustomerId.ToString()))
                    .ForMember(d => d.RowKey, opt => opt.MapFrom(s => s.Id.ToString()));
            });
        }

        private IMapper CreateMapper()
        {
            return _configuration.Value.CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Value.Map<TDestination>(source);
        }

        public List<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return source.Select(s => _mapper.Value.Map<TDestination>(s)).ToList();
        }
    }
}

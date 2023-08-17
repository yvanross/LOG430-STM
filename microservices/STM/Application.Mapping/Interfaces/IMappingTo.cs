namespace Application.Mapping.Interfaces;

public interface IMappingTo<in TDto, out TAggregate> where TDto : class where TAggregate : class
{
    TAggregate MapFrom(TDto dto);
}
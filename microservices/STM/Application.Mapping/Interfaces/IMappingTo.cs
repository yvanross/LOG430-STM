namespace Application.Mapping.Interfaces;

public interface IMappingTo<in TDto, out TAggregate>
{
    TAggregate MapTo(TDto dto);
}
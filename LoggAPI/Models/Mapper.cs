using System;
using System.Collections.Generic;
using System.Linq;

namespace LoggAPI.Models
{
    public abstract class Mapper<DOMAIN, ENTITY>
        where DOMAIN : class
        where ENTITY : Mapper<DOMAIN, ENTITY>, new()
    {
        public static List<ENTITY> ToDto(IEnumerable<DOMAIN> domainObjects)
        {
            List<ENTITY> dtoObjects = new List<ENTITY>();
            foreach (DOMAIN dO in domainObjects)
            {
                dtoObjects.Add(ToDto(dO));
            }
            return dtoObjects;
        }
        public static ENTITY ToDto(DOMAIN domainObject)
        {
            if (domainObject == null) return null;
            return new ENTITY().MapToDto(domainObject);
        }
        public static IEnumerable<DOMAIN> ToDomainObject(IEnumerable<ENTITY> entities)
        {
            return entities.Select(x => ToDomainObject(x));
        }
        public static DOMAIN ToDomainObject(ENTITY entity)
        {
            if (entity == null) return null;
            return entity.MapToDomainObject();
        }
        public abstract DOMAIN MapToDomainObject();
        public abstract ENTITY MapToDto(DOMAIN domainObject);
    }
}

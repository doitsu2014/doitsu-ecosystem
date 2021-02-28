using FileConversion.Core.Interface;
using System.Collections.Generic;
using System.Linq;

namespace FileConversion.Core.Services
{
    public class BeanMapperService : IBeanMapperService
    {
        public IEnumerable<IBeanMapper> _beanMappers { get; set; }
        public BeanMapperService(IEnumerable<IBeanMapper> beanMappers)
        {
            _beanMappers = beanMappers;
        }

        public IBeanMapper GetBeanMapper(string className)
        {
            var beanMapper = _beanMappers.FirstOrDefault(x => x.GetType().ToString().Equals(className));
            return beanMapper;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace HISInterfaceService.Core.DataMapper
{
    public static class AutoMapExtensions
    {
        //
        // 摘要:
        //     Converts an object to another using AutoMapper library. Creates a new object
        //     of TDestination. There must be a mapping between objects before calling this
        //     method.
        //
        // 参数:
        //   source:
        //     Source object
        //
        // 类型参数:
        //   TDestination:
        //     Type of the destination object
        public static TDestination MapTo<TDestination>(this object source)
        {
            if (source == null) throw new ArgumentNullException("参数错误");
            return Mapper.Map<TDestination>(source);
        }
        //
        // 摘要:
        //     Execute a mapping from the source object to the existing destination object There
        //     must be a mapping between objects before calling this method.
        //
        // 参数:
        //   source:
        //     Source object
        //

        // 类型参数:
        //   TSource:
        //     Source type
        //
        //   TDestination:
        //     Destination type

        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> obj)
        {
            if (obj == null) throw new ArgumentNullException("参数错误");
            return Mapper.Map<List<TDestination>>(obj);
        }

    }
}

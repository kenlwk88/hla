using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.Infra.Pagination
{
    public static class Pagination
    {
        public static List<T> ToPagination<T>(this List<T> obj, int? index, int? size) where T : class
        {
            int _index = 0;
            int _size = 0;

            if (index == null || index < 0)
            {
                _index = 0;
            }
            else 
            {
                _index = Convert.ToInt32(index);
            }

            if (size != null && size > 0) 
            {
                _size = Convert.ToInt32(size);
            }

            if (_size > 0) 
            {
                obj = obj.Skip(_index * _size).Take(_size).ToList();
            }
            return obj;
        }
    }
}

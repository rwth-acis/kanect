using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonnectUI.Entities
{
    public class Entity
    {
        private String _Name;
        private Type _Type;
        private Object _Item;

        public Entity(String name, Type type, Object item)
        {
            _Name = name;
            _Type = type;
            _Item = item;
        }

        public string Name { get { return _Name; } set { _Name = value; } }
        public Type Type { get { return _Type; } set { _Type = value; } }
        public Object Item { get { return _Item; } set { _Item = value; } }
    }
}

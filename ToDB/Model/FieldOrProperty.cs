using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class FieldOrProperty
    {
        public FieldOrProperty() { }
        public FieldOrProperty(PropertyInfo p)
        {
            Name = p.Name;
            MemberType = p.PropertyType;
        }

        public FieldOrProperty(FieldInfo f)
        {
            Name = f.Name;
            MemberType = f.FieldType;
        }

        public string Name { get; set; }
        public Type MemberType { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ MemberType.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            FieldOrProperty other = obj as FieldOrProperty;
            if (other == null)
                return false;
            return Name.Equals(other.Name) && MemberType.Equals(other.MemberType);
        }

    }


}

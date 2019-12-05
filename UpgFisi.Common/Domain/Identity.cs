using System;

namespace UpgFisi.Common.Domain
{
    public class Identity : IIdentity
    {
        public Guid Id { get; private set; }

        public Identity()
        {
            Id = Guid.NewGuid();
        }

        public Identity(string id)
        {
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id), "The Id cannot be empty");
            }
            Id = new Guid(id);
        }

        public Identity(Guid guid)
        {
            if (guid == default)
            {
                throw new ArgumentNullException(nameof(guid), "The Id cannot be empty");
            }
            Id = guid;
        }

        public bool Equals(Identity id)
        {
            if (ReferenceEquals(this, id))
            {
                return true;
            }
            if (id is null)
            {
                return false;
            }
            return Id.Equals(id.Id);
        }

        public override bool Equals(object anotherObject)
        {
            return Equals(anotherObject as Identity);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + " [Id=" + Id + "]";
        }
    }
}
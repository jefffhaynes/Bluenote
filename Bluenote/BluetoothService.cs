using System;

namespace Bluenote
{
    public class BluetoothService
    {
        internal BluetoothService(ushort attributeHandle, Guid longUuid, ushort? shortUuid)
        {
            AttributeHandle = attributeHandle;
            LongUuid = longUuid;
            ShortUuid = shortUuid;
        }

        internal ushort AttributeHandle { get; }

        public ushort? ShortUuid { get; }

        public Guid LongUuid { get; }

        public string SpecificationName
        {
            get
            {
                if(ShortUuid == null)
                    return null;

                return GattSpecification.GetSpecificationName(ShortUuid.Value);
            }
        }

        public override string ToString()
        {
            var name = SpecificationName;
            if (name != null)
                return name;

            return LongUuid.ToString();
        }
    }
}

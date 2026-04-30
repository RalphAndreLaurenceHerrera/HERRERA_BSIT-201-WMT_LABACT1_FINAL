using System.Collections.Concurrent;

namespace HERRERA_BSIT_201_WMT_LABACT1_FINAL
{
    internal class ParkedVehicles
    {
        private string _platenumber;
        private string _vehicletype;
        private int _hoursparked;
        private string _assignedslot;
        private int _ratefee;
        private const double _servicecharge = 20.00;

        public string PlateNumber
        {
            get { return _platenumber; }
            set
            {
                _platenumber = value;
            }   
        }

        public string VehicleType
        {
            get { return _vehicletype; }
            set
            {
                _vehicletype = value;
            }
        }

        public int HoursParked
        {
            get { return _hoursparked; }
            set
            {
                _hoursparked = value;
            }
        }

        public string AssignedSlot
        {
            get { return _assignedslot; }
            set
            {
                _assignedslot = value;
            }
        }
    }
}

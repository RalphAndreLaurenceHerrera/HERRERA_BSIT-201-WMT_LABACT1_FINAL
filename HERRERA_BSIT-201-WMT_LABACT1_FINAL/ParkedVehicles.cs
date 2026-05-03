    using System;
    using System.Collections.Concurrent;
    using System.Security.Policy;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    namespace HERRERA_BSIT_201_WMT_LABACT1_FINAL
    {
        internal class ParkedVehicles
        {
            public const double CarRate = 50.0;
            public const double MotorcycleRate = 30.0;
            public const double VanRate = 70.0;
            public const double ServiceCharge = 20.0;
            public const int Overtime = 8;

            private string _plateNumber;
            private string _vehicleType;
            private int _hoursParked;
            private string _assignedSlot;
            private string _discountType;

            public string PlateNumber
            {
                get { return _plateNumber; }
                set
                {
                    if (!Regex.IsMatch(value, @"^[A-Z]{3}-\d{4}$"))
                    {
                        throw new ArgumentException("Invalid plate number format. Please use the format 'ABC-1234'.");
                    }
                    _plateNumber = value;
                }
            }
            public string VehicleType
            {
                get { return _vehicleType; }
                set
                {
                    _vehicleType = value;
                }
            }
            public int HoursParked
            {
                get { return _hoursParked; }
                set
                {
                    if (value <= 0)
                    {
                        throw new ArgumentException("Invalid hour format. Please only put a valid number and is greater than 0.");
                    }
                    _hoursParked = value;
                }
            }
            public string AssignedSlot 
            {
                get { return _assignedSlot; }
                set
                {
                    if (!Regex.IsMatch(value, @"[A-H][1-5]$"))
                    {
                        throw new ArgumentException("Invalid slot format. Please use the format 'A1' to 'H5'.");
                    }
                    _assignedSlot = value;
                }
            }
            public string DiscountType
            {
                get { return _discountType; }
                set
                {
                    _discountType = value;
                }
            }

            public ParkedVehicles(string plateNumber, string vehicleType, int hoursParked, string assignedSlot)
            {
                PlateNumber = plateNumber;
                VehicleType = vehicleType;
                HoursParked = hoursParked;
                AssignedSlot = assignedSlot;
            }

            public double CalculateStandardFee()
            {
                if (VehicleType == "Car")
                {
                    return CarRate * HoursParked;
                }
                else if (VehicleType == "Motorcycle")
                {
                    return MotorcycleRate * HoursParked;
                }
                else if (VehicleType == "Van")
                {
                    return VanRate * HoursParked;
                }
                return 0.0;            
            }
            public double CalculateOvertimeFee()
            {
                if (HoursParked > Overtime)
                {
                    int overtimeHours = HoursParked - Overtime;
                    double overtimeRate = (CalculateStandardFee() / HoursParked) * 0.5;
                    return overtimeRate * overtimeHours;
                }
                return 0.0;
            }
            public double CalculateTotalFee()
            {
                return CalculateStandardFee() + CalculateOvertimeFee() + ServiceCharge;
            }

            public double ComputeDiscount()
            {
                double totalFee = CalculateTotalFee();
                if (DiscountType == "Employee")
                {
                    return totalFee * 0.1;
                }
                else if (DiscountType == "Senior Citizen")
                {
                    return totalFee * 0.2;
                }
                return 0.0;
            }

            public double ComputeNewTotal()
            {
                return CalculateTotalFee() - ComputeDiscount();
            }

            public double ComputeChange(double AmountPay = 0)
            {
                return AmountPay - ComputeNewTotal();
            }

        public string PrintReceipt(double AmountPay)
        {
            string n = Environment.NewLine;

            // Build the first half of the receipt
            string receipt = "==== SMART PARKING RECEIPT ====" + n +
                             "-------------------------------" + n +
                             $"Plate Number:   {PlateNumber}" + n +
                             $"Vehicle Type:   {VehicleType}" + n +
                             $"Assigned Slot:  {AssignedSlot}" + n +
                             $"Hours Parked:   {HoursParked} hrs" + n +
                             "-------------------------------" + n +
                             $"Standard Fee:   P{CalculateStandardFee():F2}" + n +
                             $"Service Charge: P{ServiceCharge:F2}" + n;

            // Conditionally add overtime if it exists
            if (CalculateOvertimeFee() > 0)
            {
                receipt += $"Overtime Fee:   P{CalculateOvertimeFee():F2}" + n;
            }

            // Add subtotal
            receipt += $"Subtotal:       P{CalculateTotalFee():F2}" + n;

            // Conditionally add discount if it exists
            if (ComputeDiscount() > 0)
            {
                receipt += $"Discount ({DiscountType}): -P{ComputeDiscount():F2}" + n;
            }

            // Add the final totals
            receipt += "-------------------------------" + n +
                       $"Amount Payable: P{ComputeNewTotal():F2}" + n +
                       $"Amount Paid:    P{AmountPay:F2}" + n +
                       $"Change:         P{ComputeChange(AmountPay):F2}" + n +
                       "===============================";

            return receipt;
        }
    }
    }

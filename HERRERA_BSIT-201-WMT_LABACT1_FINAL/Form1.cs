using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
    
namespace HERRERA_BSIT_201_WMT_LABACT1_FINAL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Random _rand = new Random();
        private Dictionary<string, ParkedVehicles> parkedVehicles = new Dictionary<string, ParkedVehicles>();

        public void GeneratePlateNumber()
        {
            string letters = "";
            for (int i = 0; i < 3; i++)
                letters += (char)_rand.Next('A', 'Z' + 1);

            int numbers = _rand.Next(1000, 10000);
            txtPlateNumber.Text = $"{letters}-{numbers}";
        }
        public void GenerateVehicleType()
        {
            if (cbVehicleType.Items.Count > 0)
            {
                cbVehicleType.SelectedIndex = _rand.Next(cbVehicleType.Items.Count);
            }
        }
        public void GenerateHoursParked(int i = 1)
        {
            txtHoursParked.Text = _rand.Next(i, 25).ToString();
        }
        public void Generator(object sender, EventArgs e)
        {
            foreach (Button btn in gbParkingStatus.Controls) // Checks if there are other selected buttons and puts them back to the original state.
            {
                if (txtPlateNumber.ReadOnly == false || cbVehicleType.Enabled == true)
                {
                    GeneratePlateNumber();
                    GenerateVehicleType();
                    GenerateHoursParked();
                    return;
                }
                else
                {
                    GenerateHoursParked(int.TryParse(txtHoursParked.Text, out int hours) ? hours : 1);
                    return;
                }
            }
        }
        public void RegisterVehicle(object sender, EventArgs e)
        {
            try
            {
                string PlateNumber = txtPlateNumber.Text.Trim();
                string AssignedSlot = txtAssignedSlot.Text.Trim();

                if (parkedVehicles.Values.Any(ParkedVehicles => ParkedVehicles.PlateNumber == PlateNumber))
                {
                    MessageBox.Show("This plate number is already registered.", "Input Error");
                    return;
                }
                if (cbVehicleType.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a vehicle type.", "Input Error");
                    return;
                }
                if (!int.TryParse(txtHoursParked.Text, out int HoursParked) || HoursParked <= 0)
                {
                    MessageBox.Show("Please enter a valid number of hours.", "Input Error");
                    return;
                }
                if (parkedVehicles.ContainsKey(AssignedSlot))
                {
                    MessageBox.Show("This slot is already taken! Please choose another available slot.", "Slot Selection");
                    return;
                }

                Button assignedButton = gbParkingStatus.Controls["btn" + AssignedSlot] as Button; 
                
                ParkedVehicles pv = new ParkedVehicles(PlateNumber, cbVehicleType.SelectedItem?.ToString(), HoursParked, AssignedSlot);
                parkedVehicles[AssignedSlot] = pv;
                assignedButton.BackColor = Color.Maroon;

                txtPlateNumber.Clear(); txtPlateNumber.ReadOnly = false;
                cbVehicleType.SelectedIndex = -1; cbVehicleType.Enabled = true;
                txtHoursParked.Clear();
                txtAssignedSlot.Clear();
                lblSlot.Text = "N/A";
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Input Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error");
            }
        }
        public void SelectButton(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;
            bool justselected = false;
            string slotID;
            if (txtPlateNumber.ReadOnly == true)
            {
                justselected = true;
            }

            slotID = clickedButton.Name.Replace("btn", ""); // Extract slot ID from button name

            if (clickedButton.BackColor == Color.ForestGreen || clickedButton.BackColor == Color.Maroon)
            {
                foreach (Button btn in gbParkingStatus.Controls) // Checks if there are other selected buttons and puts them back to the original state.
                {
                    if (btn.BackColor == Color.Orange)
                    {
                        btn.BackColor = Color.ForestGreen;
                    }
                    else if (btn.BackColor == Color.Tomato)
                    {
                        btn.BackColor = Color.Maroon;                        
                    }
                }

                if (clickedButton.BackColor == Color.ForestGreen) // Checks if this has no data
                {
                    clickedButton.BackColor = Color.Orange;
                    txtAssignedSlot.Text = slotID;
                    if (justselected)
                    {
                        txtPlateNumber.ReadOnly = true;
                        cbVehicleType.Enabled = false;

                        btnRegisterVehicle.Enabled = false;
                        btnUpdateStatus.Enabled = true;
                    }
                    else
                    {
                        txtPlateNumber.ReadOnly = false;
                        cbVehicleType.Enabled = true;

                        btnRegisterVehicle.Enabled = true;
                        btnUpdateStatus.Enabled = false;
                    }
                    return;
                }

                else if (clickedButton.BackColor == Color.Maroon) // Checks if this has data
                {
                    clickedButton.BackColor = Color.Tomato;
                    if (parkedVehicles.ContainsKey(slotID))
                    {
                        ParkedVehicles pv = parkedVehicles[slotID];
                        
                        txtPlateNumber.Text = pv.PlateNumber;               txtPlateNumber.ReadOnly = true;
                        cbVehicleType.SelectedItem = pv.VehicleType;        cbVehicleType.Enabled = false;
                        txtHoursParked.Text = $"{pv.HoursParked}";
                        txtAssignedSlot.Text = pv.AssignedSlot;

                        btnRegisterVehicle.Enabled = false;
                        btnUpdateStatus.Enabled = true;

                        pv.DiscountType = cbDiscount.Text;
                        lblPlateNumber.Text = pv.PlateNumber;
                        lblVehicleInfo.Text = pv.VehicleType;
                        lblDuration.Text = $"{pv.HoursParked} hrs";
                        lblSlot.Text = pv.AssignedSlot;
                        lblOvertimeFee.Text = $"P{pv.CalculateOvertimeFee():F2}";
                        lblStandardFee.Text = $"P{pv.CalculateStandardFee():F2}";
                        lblServiceCharge.Text = $"P{ParkedVehicles.ServiceCharge:F2}";
                        lblTotal.Text = $"P{pv.CalculateTotalFee():F2}";
                        lblNewTotal.Text = $"P{pv.ComputeNewTotal():F2}";
                        double amountPay = double.TryParse(txtPayAmount.Text.Replace("P", ""), out double result) ? result : 0;
                        lblChange.Text = $"P{pv.ComputeChange(amountPay):F2}";
                    }
                    else
                    {
                        MessageBox.Show($"Slot {slotID} is currently empty.", "Status");
                    }
                    return;
                }
                return;
            }
            else if (clickedButton.BackColor == Color.Orange || clickedButton.BackColor == Color.Tomato)
            {
                if (clickedButton.BackColor == Color.Orange)
                {
                    clickedButton.BackColor = Color.ForestGreen;
                    if (justselected)
                    {
                        txtPlateNumber.Text = "";
                        cbVehicleType.SelectedIndex = -1;
                        txtHoursParked.Text = "";
                    }
                }
                else
                {
                    clickedButton.BackColor = Color.Maroon; 
                    txtPlateNumber.Text = "";
                    cbVehicleType.SelectedIndex = -1;
                    txtHoursParked.Text = "";
                }
                txtPlateNumber.ReadOnly = false;
                cbVehicleType.Enabled = true;
                txtAssignedSlot.Clear();

                btnRegisterVehicle.Enabled = true;
                btnUpdateStatus.Enabled = false;

                lblPlateNumber.Text = "N/A";
                lblVehicleInfo.Text = "N/A";
                lblDuration.Text = "0 hrs";
                lblSlot.Text = "N/A";
                lblOvertimeFee.Text = "P0";
                lblStandardFee.Text = "P0";
                lblServiceCharge.Text = $"P{ParkedVehicles.ServiceCharge:F2}";
                lblTotal.Text = "P0";
                cbDiscount.SelectedIndex = -1;
                lblNewTotal.Text = "P0";

                txtPayAmount.Clear();
                lblChange.Text = "P0";
                return;
            }
        }
        public void UpdateStatus(object sender, EventArgs e)
        {
            try
            {
                string currentSlot = lblSlot.Text.Trim();
                string selectedNewSlot = txtAssignedSlot.Text.Trim().ToUpper();
                
                if (currentSlot == "N/A" || !parkedVehicles.ContainsKey(currentSlot))
                {
                    MessageBox.Show("Please select a parked vehicle to update.", "Selection Error");
                    return;
                }

                ParkedVehicles pv = parkedVehicles[currentSlot];

                if (selectedNewSlot != currentSlot)
                {
                    if (parkedVehicles.ContainsKey(selectedNewSlot))
                    {
                        MessageBox.Show("This slot is already taken! Please choose another available slot.", "Slot Selection");
                        return;
                    }

                    Button currentButton = gbParkingStatus.Controls["btn" + currentSlot] as Button;
                    Button newButton = gbParkingStatus.Controls["btn" + selectedNewSlot] as Button;

                    if (currentButton != null && newButton != null)
                    {
                        parkedVehicles.Remove(currentSlot);
                        parkedVehicles[selectedNewSlot] = pv;

                        pv.AssignedSlot = selectedNewSlot;

                        currentButton.BackColor = Color.ForestGreen;
                        newButton.BackColor = Color.Maroon;

                        currentSlot = selectedNewSlot;
                        lblAssignedSlot.Text = currentSlot;
                    }
                }

                if (int.TryParse(txtHoursParked.Text.Trim(), out int newHours) && newHours >= pv.HoursParked)
                {
                    pv.HoursParked = newHours;
                }
                else
                {
                    MessageBox.Show("Please enter a valid number of hours equal or greater than "+pv.HoursParked+".", "Input Error");
                    return;
                }
                pv.DiscountType = cbDiscount.Text;

                btnRegisterVehicle.Enabled = false;

                lblDuration.Text = $"{pv.HoursParked} hrs";
                lblSlot.Text = pv.AssignedSlot;

                lblStandardFee.Text = $"P{pv.CalculateStandardFee():F2}";
                lblOvertimeFee.Text = $"P{pv.CalculateOvertimeFee():F2}";
                lblServiceCharge.Text = $"P{ParkedVehicles.ServiceCharge:F2}";
                lblTotal.Text = $"P{pv.CalculateTotalFee():F2}";

                lblNewTotal.Text = $"P{pv.ComputeNewTotal():F2}";
                
                double amountPay = double.TryParse(txtPayAmount.Text.Replace("P", ""), out double result) ? result : 0;
                lblChange.Text = $"P{pv.ComputeChange(amountPay):F2}";
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Input Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error");
            }
        }
        public void ClearForm(object sender, EventArgs e)
        {
            cbVehicleType.SelectedIndex = -1;
            cbDiscount.SelectedIndex = -1;
            lblPlateNumber.Text = "N/A";
            lblVehicleInfo.Text = "N/A";
            lblDuration.Text = "0 hrs";
            lblSlot.Text = "N/A";
            lblStandardFee.Text = "P0";
            lblServiceCharge.Text = $"P{ParkedVehicles.ServiceCharge:F2}";
            lblOvertimeFee.Text = "P0";
            lblTotal.Text = "P0";
            lblNewTotal.Text = "P0";
            lblChange.Text = "P0";
            txtPlateNumber.Clear();
            txtHoursParked.Clear();
            txtAssignedSlot.Clear();
            txtPayAmount.Clear();
            txtReceipt.Clear();

            foreach (Button btn in gbParkingStatus.Controls) // Checks if there are other selected buttons and puts them back to the original state.
            {
                if (btn.BackColor == Color.Orange)
                {
                    btn.BackColor = Color.ForestGreen;
                }
                else if (btn.BackColor == Color.Tomato)
                {
                    btn.BackColor = Color.Maroon;
                }
            }
        }

        private void Process_Payment(object sender, EventArgs e)
        {
            string currentSlot = lblSlot.Text.Trim();
            if (currentSlot == "N/A" || !parkedVehicles.ContainsKey(currentSlot))
            {
                return;
            }
            ParkedVehicles pv = parkedVehicles[currentSlot];

            pv.DiscountType = cbDiscount.Text; 
            lblNewTotal.Text = $"P{pv.ComputeNewTotal():F2}";

            double amountPay = double.TryParse(txtPayAmount.Text.Replace("P", ""), out double result) ? result : 0;
            lblChange.Text = $"P{pv.ComputeChange(amountPay):F2}";
        }
        public void PrintReceipt(object sender, EventArgs e)
        {
            try
            {
                // 1. IDENTIFY THE VEHICLE
                string currentSlot = lblSlot.Text;
                if (currentSlot == "N/A" || string.IsNullOrWhiteSpace(currentSlot) || !parkedVehicles.ContainsKey(currentSlot))
                {
                    MessageBox.Show("Please select a parked vehicle to checkout.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ParkedVehicles pv = parkedVehicles[currentSlot];
                double amountPay = 0;
                if (!double.TryParse(txtPayAmount.Text.Replace("P", "").Trim(), out amountPay))
                {
                    MessageBox.Show("Please enter a valid payment amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (amountPay < pv.ComputeNewTotal())
                {
                    MessageBox.Show("Insufficient payment amount! Please enter the correct amount.", "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                txtReceipt.Text = pv.PrintReceipt(amountPay);
                Button slotButton = gbParkingStatus.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btn" + currentSlot);
                if (slotButton != null)
                {
                    slotButton.BackColor = Color.ForestGreen;
                }
                parkedVehicles.Remove(currentSlot);

                double change = pv.ComputeChange(amountPay);
                
                ClearFormWithoutReceipt();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during checkout: " + ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ClearFormWithoutReceipt()
        {
            cbVehicleType.SelectedIndex = -1;
            cbDiscount.SelectedIndex = -1;
            lblPlateNumber.Text = "N/A";
            lblVehicleInfo.Text = "N/A";
            lblDuration.Text = "0 hrs";
            lblSlot.Text = "N/A";
            lblStandardFee.Text = "P0";
            lblServiceCharge.Text = $"P{ParkedVehicles.ServiceCharge:F2}";
            lblOvertimeFee.Text = "P0";
            lblTotal.Text = "P0";
            lblNewTotal.Text = "P0";
            lblChange.Text = "P0";
            txtPlateNumber.Clear();
            txtHoursParked.Clear();
            txtAssignedSlot.Clear();
            txtPayAmount.Clear();
        }
    }
}

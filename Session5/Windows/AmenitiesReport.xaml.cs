using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Session5
{
    /// <summary>
    /// Interaction logic for AmenitiesReport.xaml
    /// </summary>
    public partial class AmenitiesReport : Window
    {

        public List<Tickets> TicketList;

        public List<AmenityReport> AmenitiesReportList { get; set; } = new List<AmenityReport>();

        public AmenitiesReport()
        {
            InitializeComponent();

            FlightIdTextBox.Text = "";

            FromDateTextBox.Text = "01.10.2018";
            ToDateTextBox.Text = "10.10.2018";

        }

        private void MakeReportButton_Click(object sender, RoutedEventArgs e)
        {
            if(FromDateTextBox.Text == string.Empty || ToDateTextBox.Text == string.Empty)
            {
                MessageBox.Show("Необходимо ввести обе даты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime FromDate;
            DateTime ToDate;

            try
            {
                FromDate = DateTime.Parse(FromDateTextBox.Text);
                ToDate = DateTime.Parse(ToDateTextBox.Text);
            }
            catch 
            {
                MessageBox.Show("Некорректный формат даты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
               
            }


            Func<Tickets, bool> FlightNumberFilter = t=> true;
            if(FlightIdTextBox.Text != string.Empty)
            {
                FlightNumberFilter = t => t.Schedules.FlightNumber == FlightIdTextBox.Text;
            }

            var entities = new Session5Entities();
            TicketList = entities.Tickets
                .Where(t => t.Schedules.Date >= FromDate && t.Schedules.Date <= ToDate && t.Confirmed == true)
                .AsEnumerable()
                .Where(t=> FlightNumberFilter(t))
                .ToList();

            

            AmenitiesReportList.Clear();
            foreach (var cabinType in entities.CabinTypes)
            {


                var report = new AmenityReport
                {

                    CabinType = cabinType.Name,
                    ExtraBlanketCount = TicketList.Where(t=> t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at=>at.Amenities.Service).Contains("Extra Blanket")).Count(),
                    NextSeatFreeCount = TicketList.Where(t=> t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at=>at.Amenities.Service).Contains("Next Seat Free")).Count(),
                    TwoSeatsFreeCount = TicketList.Where(t=>t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at=>at.Amenities.Service).Contains("Two Neighboring Seats Free")).Count(),
                    TabletRentalCount = TicketList.Where(t=> t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at=>at.Amenities.Service).Contains("Tablet Rental")).Count(),
                    LaptopRentalCount = TicketList.Where(t=> t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at=>at.Amenities.Service).Contains("Laptop Rental")).Count(),
                    LoungeAccessCount = TicketList.Where(t=> t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at=>at.Amenities.Service).Contains("Lounge Access")).Count(),
                    SoftDrinksCount = TicketList.Where(t=> t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at=>at.Amenities.Service).Contains("Soft Drinks")).Count(),
                    PremiumHeadphonesRentalCount = TicketList.Where(t => t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at => at.Amenities.Service).Contains("Premium Headphones Rental")).Count(),
                    ExtraBagCount = TicketList.Where(t => t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at => at.Amenities.Service).Contains("Extra Bag")).Count(),
                    FastCheckingLaneCount = TicketList.Where(t => t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at => at.Amenities.Service).Contains("Fast Checkin Lane")).Count(),
                    Wifi50mbCount = TicketList.Where(t => t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at => at.Amenities.Service).Contains("Wi-Fi 50 mb")).Count(),
                    Wifi250mbCount = TicketList.Where(t => t.CabinTypes.Name == cabinType.Name && t.AmenitiesTickets.Select(at => at.Amenities.Service).Contains("Wi-Fi 250 mb")).Count(),



                };


                AmenitiesReportList.Add(report);

            }

            AmenitiesDataGrid.ItemsSource = new List<AmenityReport>();
            AmenitiesDataGrid.ItemsSource = AmenitiesReportList;

        }



        private void FlightIdTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            char character = (char)KeyInterop.VirtualKeyFromKey(e.Key);

            if (!char.IsDigit(character) && e.Key != Key.Back)
                e.Handled = true;
           

        }
    }


    public class AmenityReport
    {
        
        public string CabinType { get; set; }

        public int ExtraBlanketCount { get; set; }
        public int NextSeatFreeCount { get; set; }
        public int TwoSeatsFreeCount { get; set; }
        public int TabletRentalCount { get; set; }
        public int LaptopRentalCount { get; set; }
        public int LoungeAccessCount { get; set; }
        public int SoftDrinksCount { get; set; }
        public int PremiumHeadphonesRentalCount { get; set; }
        public int ExtraBagCount { get; set; }
        public int FastCheckingLaneCount { get; set; }
        public int Wifi50mbCount { get; set; }
        public int Wifi250mbCount { get; set; }



    }


}

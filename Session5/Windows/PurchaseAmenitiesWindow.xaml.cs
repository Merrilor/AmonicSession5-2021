using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Session5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PurchaseAmenitiesWindow : Window
    {

        private readonly DateTime CurrentDateTime = new DateTime(2017, 10, 14,0,0,1);


        public double ItemSelectedPrice { get; set; }

        public double TaxesPrice { get; set; }

        public double TotalPayable { get; set; } 

        private Tickets SelectedTicket;


        public PurchaseAmenitiesWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void BookingReferenceSearchButton_Click(object sender, RoutedEventArgs e)
        {
            var entities = new Session5Entities();

            var FlightList = entities.Tickets
                .Where(t => t.BookingReference == BookingReferenceTextBox.Text && t.Confirmed == true)
                .AsEnumerable()
                .Where(t=>(t.Schedules.Date.Add(t.Schedules.Time)) - TimeSpan.FromHours(24) > CurrentDateTime)
                .ToList();

            if(FlightList.Count == 0)
            {
                MessageBox.Show("Не найдено ни одного полета");
                return;
            }

            FlightComboBox.ItemsSource = FlightList;
            FlightComboBox.SelectedIndex = 0;
            
            
        }

        private  void ShowAmenitiesButton_Click(object sender, RoutedEventArgs e)
        {

            if (FlightComboBox.SelectedItem is null)
                return;


            AmenitiesWrapPanel.Children.Clear();

            var entities = new Session5Entities();           

            var amenities = entities.Amenities.ToList();


            var AmenityCheckBoxes = new List<CheckBox>();

            foreach (var item in amenities)
            {

                CheckBox newAmenityCheckBox = new CheckBox();
                newAmenityCheckBox.Content = item.Service.ToString();
                newAmenityCheckBox.Margin = new Thickness(10);

                if (((Tickets)FlightComboBox.SelectedItem).CabinTypes.Amenities.Select(a=>a.Service).Contains(item.Service) || item.Price == 0)
                {
                    newAmenityCheckBox.Content += "(Free)";
                    newAmenityCheckBox.Opacity = 0.5;
                    newAmenityCheckBox.IsChecked = true;
                    newAmenityCheckBox.IsEnabled = false;
                }
                else
                {
                    newAmenityCheckBox.Content += $"(${item.Price:0.})";

                    if(((Tickets)FlightComboBox.SelectedItem).AmenitiesTickets.Select(at=> at.AmenityID).Contains(item.ID))
                    {
                        newAmenityCheckBox.IsChecked = true;
                    }

                    newAmenityCheckBox.Checked += CalculatePay;
                    newAmenityCheckBox.Unchecked += CalculatePay;

                }

                newAmenityCheckBox.Tag = item.Price.ToString();
                AmenityCheckBoxes.Add(newAmenityCheckBox);

            }

            

            AmenityCheckBoxes = AmenityCheckBoxes.OrderBy(cb => cb.Opacity).ToList();
            foreach (var checkBox in AmenityCheckBoxes)
            {
                AmenitiesWrapPanel.Children.Add(checkBox);
            }


            SelectedTicket = (Tickets)FlightComboBox.SelectedItem;


        }

        private void CalculatePay(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.IsChecked == true)
            {
                ItemSelectedPrice += (double)decimal.Parse(checkBox.Tag.ToString());
            }
            else
            {
                ItemSelectedPrice -= (double)decimal.Parse(checkBox.Tag.ToString());
            }
            

            TaxesPrice = (double)ItemSelectedPrice / 100 * 5;

            TotalPayable = ItemSelectedPrice + TaxesPrice;


            SelectedItemsPriceTextBlock.Text = $"Items selected: ${ItemSelectedPrice}";

            TaxesPriceTextBlock.Text = $"Duties and taxes: ${TaxesPrice}";

            TotalPayableTextBlock.Text = $"Total payable: ${TotalPayable}";

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if(SelectedTicket is null)
            {
                return;
            }

            var entities = new Session5Entities();

            var deleteAmenitiesTickets = entities.AmenitiesTickets.Where(at => at.TicketID == SelectedTicket.ID);
            entities.AmenitiesTickets.RemoveRange(deleteAmenitiesTickets);
            entities.SaveChanges();


            foreach (var amenityCheckBox in AmenitiesWrapPanel.Children)
            {
                CheckBox checkBox = amenityCheckBox as CheckBox;
                string AmenityName = checkBox.Content.ToString().Remove(checkBox.Content.ToString().IndexOf('('));
                string AmenityPrice = checkBox.Tag.ToString().Replace(',', '.');

                if (checkBox.Opacity != 1 || checkBox.IsChecked == false )
                {
                    continue;
                }


                AmenitiesTickets newAmenitiesTickets = new AmenitiesTickets
                {

                    AmenityID = entities.Amenities.Where(a => a.Service == AmenityName).Single().ID,
                    TicketID = SelectedTicket.ID,
                    Price = decimal.Parse(checkBox.Tag.ToString())

                };

                entities.AmenitiesTickets.Add(newAmenitiesTickets);

                try
                {
                    entities.SaveChanges();
                    
                }
                catch
                {
                    MessageBox.Show("Ошибка");
                    return;
                }

                

            }

            MessageBox.Show("Данные успешно сохранены");

            

        }

        private void AmenitiesReportButton_Click(object sender, RoutedEventArgs e)
        {
            new AmenitiesReport().ShowDialog();
           
        }
    }
}

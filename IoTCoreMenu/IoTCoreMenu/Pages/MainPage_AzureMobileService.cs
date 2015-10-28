using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;

// To add offline sync support, add the NuGet package Microsoft.WindowsAzure.MobileServices.SQLiteStore
// to your project. Then, uncomment the lines marked // offline sync
// For more information, see: http://aka.ms/addofflinesync
//using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
//using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync

namespace IoTCoreMenu
{

        public class TelemetryAPI
        {
            //Note: To change the table from the Azure Mobile Service (ie Same service and therefore same app key) ..
            // Just change the class name in the file using Refactoring
            private MobileServiceCollection<Telemetry2, Telemetry2> telemetryItems;
            private IMobileServiceTable<Telemetry2> telemetryTable = App.MobileService.GetTable<Telemetry2>();

            // This page needs to update the sensor lists, or at least, update Item Sources fo rlists there
            private static ListTelemetryPage listTelemetryPage;
            public TelemetryAPI()
            {
                Globals.TelemetryApi = this;
            }

            public static void SetListTelemetryPage(ListTelemetryPage ListTelemetryPage)
            {
                listTelemetryPage = ListTelemetryPage;
            }


        #region INSERT
        //Clean up of Sensor names.
        private string Normalise(string strn)
        {
            string strnTemp = "";
            if (strn != "")
            {
                strnTemp = strn.Substring(1).ToLower();
                strnTemp = strn.ToUpper()[0].ToString() + strnTemp;
            }
            // Replace spaces (should be any at this level though)
            strnTemp.Replace(" ", "_");
            return strnTemp;
        }

        public async Task InsertTelemetryItem(Telemetry2 telemetryItem)
        {
            await telemetryTable.InsertAsync(telemetryItem);
            //await SyncAsync(); // offline sync
        }
        public async Task Post(string SensorInput, double SensorValue)
        {
            var telemetryItem = new Telemetry2 { Sensor = Normalise(SensorInput), Value = SensorValue, Complete = false };
            await InsertTelemetryItem(telemetryItem);
        }
        #endregion

        #region GET_RECORD/S

        /// <summary>
        /// Get current entry in table for sensor
        /// </summary>
        /// <param name="sensor">Name of the the sensor</param>
        /// <returns></returns>
        public async Task<Telemetry2> GetTelemetryItem(string sensor)
        {
            var telemetryItem = await telemetryTable
            .Where(
                (s => (s.Complete == false) && (s.Sensor == sensor))
                )
            .ToCollectionAsync();

            if (telemetryItem.Count() != 1)
                return null;
            Telemetry2 item = (Telemetry2)telemetryItem.First();
                return item;
        }
        #endregion

        #region UPDATE
        public async Task<bool> UpdateTelemetryItem(string sensor, double value)
        {
        // This code takes a freshly completed TodoItem and updates the database. When the MobileService 
        // responds, the item is removed from the list 
            Telemetry2 item = await GetTelemetryItem(sensor);
            if (item == null)
                return false;
            item.Value = value;
            await UpdateTelemetryItem(item);
            return true;
        }

        public async Task UpdateTelemetryItem(Telemetry2 item)
        {
            if (item != null)
            {
                await telemetryTable.UpdateAsync(item);
            }
        }

        #endregion

        #region DELETE
        public  async Task<bool> DeleteTelemetryItem(string sensor)
        {
            Telemetry2 item = await GetTelemetryItem(sensor);
            if (item == null)
                return false;
            await DeleteTelemetryItem(item);
            return true;
        }

        public async Task DeleteTelemetryItem(Telemetry2 item)
        {
            await telemetryTable.DeleteAsync(item);
        }

        public async Task ClearTelemetryItems(string sensor)
        {
            var telemetryItems = await telemetryTable
           .Where(
               (s => (s.Sensor == sensor))
               )
           .ToCollectionAsync();
            foreach (Telemetry2 item in telemetryItems)
            {
                await DeleteTelemetryItem(item);
            }
        }
        #endregion

        #region REFRESH_DISPLAY_LIST

        public async Task<double> RefreshTelemetryItemValue(string sensor)
        {
            double value = -1;
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the telemetry2 table.
                telemetryItems = await telemetryTable
                .Where(
                    (s => (s.Complete == false) && (s.Sensor == sensor))
                    )
                .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                //Sort in descending order = Most recent first .. Ww want the latest value of each sensor
                if (telemetryItems != null)
                {
                    if (telemetryItems.Count() != 0)
                        value = (double)telemetryItems.First().Value;
                }
            }
            return value;
        }

        public async Task RefreshTelemetryHistory(string sensor)
        {
            var telemetryItems = await telemetryTable
               .Where(
                   (s => (s.Sensor == sensor))
                   )
               .ToCollectionAsync();

            if (listTelemetryPage != null)
                //Set as displayed list
                listTelemetryPage.ListItems.ItemsSource = telemetryItems.OrderBy(s => s._DateTime).Reverse();

        }

        #endregion

        #region REFRESH_TABLE_SO_ONLY_LAST_ENTRIES_ARE_CURRENT
        public async Task RefreshTelemetryItems()
        {
            MobileServiceInvalidOperationException exception = null;

            try
            {
                // This code refreshes the entries in the list view by querying the telemetry2 table.
                telemetryItems = await telemetryTable
                .Where
                (s => (s.Complete == false))
                .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                //Sort in descending order = Most recent first .. Ww want the latest value of each sensor
                var sortedTelemetryItems = telemetryItems
                    .OrderByDescending(telemetryItem => telemetryItem.Id);

                //Group by sensor name
                var orderGroups =
                    from p in sortedTelemetryItems
                    group p by p.Sensor into g
                    select new { Category = g.Key, Sensors = g };

                //Get only the first item in each group = Most Recent
                List<Telemetry2> telemetryItemsListMostRecentInGroup = new List<Telemetry2>();
                foreach (var key in orderGroups)
                {

                    telemetryItemsListMostRecentInGroup.Add(key.Sensors.First<Telemetry2>());
                    //Mark other vakues as complete. Embedded devices then can just get the "incomplete values".
                    for (int i = 1; i < key.Sensors.Count(); i++)
                    {
                        Telemetry2 t2i = key.Sensors.ElementAt<Telemetry2>(i);
                        t2i.Complete = true;
                        await telemetryTable.UpdateAsync(t2i);
                    }
                }

                if (listTelemetryPage != null)
                    //Set as displayed list
                    listTelemetryPage.ListItems.ItemsSource = telemetryItemsListMostRecentInGroup.OrderBy(s => s.Id).Reverse(); // telemetryItems;
                                                                                                                  //this.ButtonSave.IsEnabled = true;
            }
        }
        public async Task Refresh()
            {
                //await SyncAsync(); // offline sync
                await RefreshTelemetryItems();
            }
        #endregion

    }
}


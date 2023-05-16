using PoliGranColAppIoT.Models;
using PoliGranColAppIoT.Services.IServices;
using PoliGranColAppIoT.Utilities;
using System.ComponentModel;
using System.Dynamic;
using System.Text.Json;
using System.Windows.Input;
using System.Xml.Linq;

namespace PoliGranColAppIoT.ViewModel
{
    public  class HomePageViewModel : INotifyPropertyChanged
    {
        string token;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _idDevice;
        public string IdDevice
        {
            get { return _idDevice; }
            set { _idDevice = value; }
        }

        private string _statusSearch;
        public string StatusSearch
        {
            get { return _statusSearch; }
            set { _statusSearch = value; }
        }

        private List<Devices> _deviceList;

        public List<Devices> DeviceList
        {
            get { return _deviceList; }
            set
            {
                if (_deviceList != value)
                {
                    _deviceList = value;
                    OnPropertyChanged(nameof(DeviceList));
                }
            }
        }

        private Devices _deviceSelected;
        public Devices DeviceSelected
        {
            get {
                return _deviceSelected;
            }
            set
            {
                _deviceSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceSelected)));
            }
        }

        private readonly IRepository<Data[]> _repository;
        public HomePageViewModel(IRepository<Data[]> repository)
        {
            _repository = repository;
            SetToken();
            SetDevices();
        }

        private async void SetDevices()
        {
            DeviceList = new List<Devices>()
            {
                new Devices() { Id = "6cba5e50-de70-11ed-a681-85bbb1891ac6", Name = "Robot Sumo" },
                new Devices() { Id = "5a92f340-eed8-11ed-a9c2-9b8419f24e4a", Name = "Estación Clima" },
                new Devices() { Id = "b3aed900-f133-11ed-a9c2-9b8419f24e4a", Name = "Dispensador Mascotas" }
            };
        }

        private async void SetToken()
        {
            token = await SecureStorage.GetAsync("myToken");
        }

        public ICommand GetAllStatus =>
            new Command(async () =>
            {
                await GetActions();
            });

        public ICommand GetCurrentStatus =>
                new Command(async () =>
                {
                    await GetAction(StatusSearch);
                });

        private async Task GetActions()
        {
            if (DeviceSelected == null)
            {
                MessagingCenter.Send(this, "ShowMessage", "Error, Seleccione un dispositivo!");
                return;
            }

            var url = $"{CT.UrlBaseApi}/plugins/telemetry/DEVICE/{DeviceSelected.Id}/values/attributes/SHARED_SCOPE";

            string token = await SecureStorage.GetAsync("myToken");

            var response = await _repository.Get(url, null, token);
            string resp = response.IsSuccess ? GetJsonBeatifull(response.Data) : response.Message;

            MessagingCenter.Send(this, "ShowMessage", $"{resp}");
        }

        private async Task GetAction(string prmAction = "")
        {
            if (DeviceSelected == null)
            {
                MessagingCenter.Send(this, "ShowMessage", "Error, Seleccione un dispositivo!");
                return;
            }

            var url = $"{CT.UrlBaseApi}/plugins/telemetry/DEVICE/{DeviceSelected.Id}/values/attributes/SHARED_SCOPE";

            string token = await SecureStorage.GetAsync("myToken");

            var response = await _repository.Get(url, prmAction, token);
            string resp = response.IsSuccess ? GetJsonBeatifull(response.Data) : response.Message;

            MessagingCenter.Send(this, "ShowMessage", resp);
        }

        private string GetJsonBeatifull(Data[] objData)
        {
            string response = string.Empty;

            foreach (var data in objData)
            {
                response += $"{data.key} : {data.value}\n";
            }

            return response;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

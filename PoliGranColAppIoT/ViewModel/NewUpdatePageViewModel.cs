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
    public  class NewUpdatePageViewModel : INotifyPropertyChanged
    {
        string token;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _idDevice;
        public string IdDevice
        {
            get { return _idDevice; }
            set { _idDevice = value; }
        }

        private string _newStatusName;
        public string NewStatusName
        {
            get { return _newStatusName; }
            set { _newStatusName = value; }
        }

        private string _newStatus;
        public string NewStatus
        {
            get { return _newStatus; }
            set { _newStatus = value; }
        }

        private string _updateStatus;
        public string UpdateStatus
        {
            get { return _updateStatus; }
            set { _updateStatus = value; }
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
                SetAttributes(_deviceSelected);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceSelected)));
            }
        }

        private List<Attributes> _attributesList;

        public List<Attributes> AttributesList
        {
            get { return _attributesList; }
            set
            {
                if (_attributesList != value)
                {
                    _attributesList = value;
                    OnPropertyChanged(nameof(AttributesList));
                }
            }
        }

        private Attributes _attributeSelected;
        public Attributes AttributeSelected
        {
            get
            {
                return _attributeSelected;
            }
            set
            {
                _attributeSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AttributeSelected)));
            }
        }

        private readonly IRepository<Data[]> _repository;
        public NewUpdatePageViewModel(IRepository<Data[]> repository)
        {
            _repository = repository;
            SetToken();
            SetDevices();
        }

        private async void SetAttributes(Devices idDevice)
        {
            var url = $"{CT.UrlBaseApi}/plugins/telemetry/DEVICE/{idDevice.Id}/values/attributes/SHARED_SCOPE";
            var response = await _repository.Get(url, null, await SecureStorage.GetAsync("myToken"));

            if (response.IsSuccess)
            {
                var listTemp = new List<Attributes>();

                foreach (var item in response.Data)
                {
                    var tempObj = new Attributes()
                    {
                        Key = item.key,
                        Value = item.value,
                        LastUpdateTs = item.lastUpdateTs
                    };

                    listTemp.Add(tempObj);
                }

                AttributesList = listTemp;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AttributesList)));
            }
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

        public ICommand InsertActionOne =>
            new Command(async () =>
            {
                await UpdateAction(UpdateStatus);
            });

        public ICommand InsertAction =>
            new Command(async () =>
            {
                await NewAction(NewStatusName, NewStatus);
            });

        private async Task UpdateAction(string prmAction)
        {
            if (DeviceSelected == null || AttributeSelected == null || string.IsNullOrEmpty(prmAction))
            {
                MessagingCenter.Send(this, "ShowMessage", "Error, Seleccione un dispositivo, atributo y valor");
                return;
            }

            var url = $"{CT.UrlBaseApi}/plugins/telemetry/{DeviceSelected.Id}/SHARED_SCOPE";

            string token = await SecureStorage.GetAsync("myToken");

            dynamic tempObject = new ExpandoObject();
            Dictionary<string, object> myAttributes = new Dictionary<string, object>();
            myAttributes[AttributeSelected.Key] = prmAction;
            tempObject = myAttributes;

            string jsonData = JsonSerializer.Serialize((object)tempObject);

            var reponse = await _repository.Update(url, jsonData, token);

            string msgResponse = reponse ? $"Exito! Se actualizo el atributo {AttributeSelected.Key} con valor: {prmAction} exitosamente" : $"Error! el servicio genero error al actualizar el campo {prmAction}";

            MessagingCenter.Send(this, "ShowMessage", msgResponse);
        }

        private async Task NewAction(string prmNameAction, string prmAction)
        {
            if (DeviceSelected == null || string.IsNullOrEmpty(prmAction))
            {
                MessagingCenter.Send(this, "ShowMessage", "Error, Seleccione un dispositivo y valor");
                return;
            }

            var url = $"{CT.UrlBaseApi}/plugins/telemetry/{DeviceSelected.Id}/SHARED_SCOPE";

            string token = await SecureStorage.GetAsync("myToken");

            dynamic tempObject = new ExpandoObject();
            Dictionary<string, object> myAttributes = new Dictionary<string, object>();
            myAttributes[prmNameAction] = prmAction;
            tempObject = myAttributes;

            string jsonData = JsonSerializer.Serialize((object)tempObject);

            var reponse = await _repository.Update(url, jsonData, token);

            string msgResponse = reponse ? $"Exito! Se inserto el atributo {prmNameAction} con valor: {prmAction} exitosamente" : $"Error! el servicio genero error al agregar el campo {prmAction}";

            MessagingCenter.Send(this, "ShowMessage", msgResponse);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace People.ViewModel
{
    public class PersonViewModel : ObservableObject, IQueryAttributable
    {
        private readonly PersonRepository _personRepository;
        private Models.Person _person;
        private string _statusMessage;

        public ICommand GuardarCommand { get; }
        public ICommand CargarPeopleCommand { get; }
        public ICommand EliminarPeopleCommand { get; }

        public ObservableCollection<Models.Person> PeopleList { get; private set; }

        public Models.Person Person
        {
            get { return _person; }
            set
            {
                if (_person != value)
                {
                    _person = value;
                    OnPropertyChanged(nameof(Person));
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name
        {
            get { return _person.Name; }
            set
            {
                if (_person.Name != value)
                {
                    _person.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int Id
        {
            get { return _person.Id; }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public PersonViewModel()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "people.db3");
            _personRepository = new PersonRepository(dbPath);

            _person = new Models.Person();
            PeopleList = new ObservableCollection<Models.Person>();

            GuardarCommand = new AsyncRelayCommand(Save);
            CargarPeopleCommand = new AsyncRelayCommand(LoadPeople);
            EliminarPeopleCommand = new AsyncRelayCommand<Models.Person>(Eliminar);
        }

        private async Task Save()
        {
            try
            {
                if (string.IsNullOrEmpty(_person.Name))
                {
                    throw new Exception("El nombre no puede estar vacío.");
                }

                _personRepository.AddNewPerson(_person.Name);
                StatusMessage = $"El nombre: {_person.Name} ha sido guardado.";

                await Shell.Current.GoToAsync($"..?saved={_person.Name}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al guardar la persona: {ex.Message}";
            }
        }

        private async Task Eliminar(Models.Person personaAEliminar)
        {
            try
            {
                if (personaAEliminar == null)
                {
                    throw new Exception("Persona no válida.");
                }

                _personRepository.EliminarPersona(personaAEliminar.Name);
                PeopleList.Remove(personaAEliminar);
                StatusMessage = $"Se eliminó a {personaAEliminar.Name}.";

                await Shell.Current.DisplayAlert("Eliminar", $"Emilio Guerrero acaba de eliminar su registro", "Aceptar");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al eliminar a la persona: {ex.Message}";
            }
        }

        private async Task LoadPeople()
        {
            try
            {
                var people = _personRepository.GetAllPeople();
                PeopleList.Clear();
                foreach (var person in people)
                {
                    PeopleList.Add(person);
                }

                StatusMessage = $"Se cargaron {PeopleList.Count} personas.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al obtener personas: {ex.Message}";
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("person") && query["person"] is Models.Person person)
            {
                Person = person;
            }
            else if (query.ContainsKey("deleted"))
            {
                string nombre = query["deleted"].ToString();
                Models.Person matchedPerson = PeopleList.FirstOrDefault(p => p.Name == nombre);

                if (matchedPerson != null)
                    PeopleList.Remove(matchedPerson);
            }
        }
    }
}

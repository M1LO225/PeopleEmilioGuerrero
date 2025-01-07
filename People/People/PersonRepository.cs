using SQLite;
using People.Models;

namespace People;

public class PersonRepository
{
    string _dbPath;

    public string StatusMessage { get; set; }

    private SQLiteConnection conn;

    private void Init()
    {
        if (conn != null) 
            return;

        conn = new SQLiteConnection(_dbPath);
        conn.CreateTable<Person>();
    }

    public PersonRepository(string dbPath)
    {
        _dbPath = dbPath;                        
    }

    public void AddNewPerson(string name) 
    {            
        int result = 0;
        try
        {
            Init();


            if (string.IsNullOrEmpty(name))
                throw new Exception("Valid name required");
            result = conn.Insert(new Person { Name = name });

            

            StatusMessage = string.Format("{0} Se ha añadido (Name: {1})", result, name);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", name, ex.Message);
        }

    }

    public List<Person> GetAllPeople()
    {
        
        try
        {
            Init ();
            return conn.Table<Person>().ToList();
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Person>();
    }
    public void EliminarPersona(string name)
    {
        int result = 0;
        try
        {
            Init();

            if (string.IsNullOrEmpty(name))
                throw new Exception("Se requiere un nombre válido");

            var person = conn.Table<Person>().FirstOrDefault(p => p.Name == name);

            if (person == null)
                throw new Exception($"No se encontró una persona con el nombre '{name}'");

            result = conn.Delete(person);

            StatusMessage = string.Format("{0} registro(s) eliminado(s) (Nombre: {1})", result, name);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Error al eliminar '{0}'. Detalles: {1}", name, ex.Message);
        }
    }

}

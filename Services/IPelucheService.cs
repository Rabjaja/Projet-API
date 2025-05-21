public interface IPelucheService
{
    List<Peluche> GetAll();
    Peluche? GetById(int id);
    void Add(Peluche peluche);
    void Update(Peluche peluche);
    bool Delete(int id);
}

namespace Service.Models;

public class FundaRealEstateAgentsOffers
{
    public List<FundaObject> Objects { get; set; }
    public Paging Paging { get; set; }
    public int? TotaalAantalObjecten { get; set; }
}

public class FundaObject
{
    public int? MakelaarId { get; set; }
    public string? MakelaarNaam { get; set; }
}

public class Paging
{
    public int? AantalPaginas { get; set; }
    public int? HuidigePagina { get; set; }
}
namespace Util.ProffApiClasses;


/* Må lage en class basert på hva data som skal hentes inn fra proff. basert på JSON schema. Se på proffjson schema for referanse. */
public class CompanyReturn
{
    public string companyType { get; set; }
    public string companyTypeName { get; set; }
    public string estimatedTurnover { get; set; }
    public bool hasSecurity { get; set; }
    public string[] previousNames { get; set; }
    public string proffListingId { get; set; }

}